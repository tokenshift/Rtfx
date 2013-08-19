using System;
using System.IO;
using System.Linq;
using System.Text;
using Harmful;

namespace Rtfx {
    /// <summary>
    /// Utility class to consume specific tokens/entities from an RTF file.
    /// </summary>
    internal static class RtfParser {
        /// <summary>
        /// Parses a control word from the input stream without consuming it.
        /// </summary>
        /// <param name="buffer">
        /// The input stream that the control word will be read from.
        /// </param>
        /// <param name="offset">
        /// The position in the input stream to start parsing.
        /// </param>
        /// <param name="word">
        /// The parsed control word.
        /// </param>
        /// <param name="consumed">
        /// The number of byes parsed.
        /// </param>
        /// <remarks>
        /// A control word consists of a backslash followed by up to 32 ASCII
        /// letters ([a-zA-Z]), followed by a delimiter. The delimiter can be
        /// any of the following:
        /// * Space: consumed with the control word.
        /// * Numeric digit or '-': start of a numeric parameter for the control word.
        /// The numeric parameter can be up to 10 numeric digits (not including the
        /// minus sign), followed by any non-numeric character.
        /// * Any character other than a letter or digit: not consumed as part of
        /// the control word.
        /// </remarks>
        public static bool ParseControlWord(InputBuffer buffer, int offset, out ControlWord word, out int consumed) {
            var startOffset = offset;

            if (buffer.At(offset) != '\\') {
                throw new ParseException("Expected control word");
            }
            ++offset;

            if (buffer.At(offset) == '*') {
                // Handle starred control word (e.g. "\*\foo").
                ++offset;
                if (ParseControlWord(buffer, offset, out word, out consumed)) {
                    word = new ControlWord(word.Text, true, word.Parameter);
                    consumed += offset;
                    return true;
                }
                else {
                    word = null;
                    consumed = 0;
                    return false;
                }
            }

            int nameLength;
            for (nameLength = 0; nameLength < 32 && IsAsciiLetter(buffer.At(nameLength + offset)); ++nameLength) {}
            if (nameLength == 0 || nameLength > 32) {
                word = null;
                consumed = 0;
                return false;
            }

            var name = Encoding.UTF8.GetString(buffer.Span(offset, nameLength), 0, nameLength);
            int? param = null;

            offset += nameLength;

            var delim = buffer.At(offset);
            if (IsNumericDigit(delim) || delim == '-') {
                // Parse the numeric parameter.
                if (delim == '-') {
                    ++offset;
                }

                int paramLength;
                for (paramLength = 0;
                    paramLength < 10 &&
                    IsNumericDigit(buffer.At(paramLength + offset));
                    ++paramLength) {}

                if (paramLength == 0) {
                    throw new ParseException("Missing numeric parameter.");
                }

                param = int.Parse(Encoding.UTF8.GetString(buffer.Span(offset, paramLength), 0, paramLength));
                if (delim == '-') {
                    param *= -1;
                }

                offset += paramLength;
            }

            delim = buffer.At(offset);
            if (delim == ' ') {
                // Consume the delimiter.
                ++offset;
            }
            // Otherwise, leave the delimiter where it is.

            word = new ControlWord(name, false, param);
            consumed = offset - startOffset;

            return true;
        }

        /// <summary>
        /// Reads binary data from the input stream.
        /// </summary>
        /// <param name="buffer">
        /// The input stream that the data will be read from.
        /// </param>
        /// <param name="length">
        /// The amount of data to be read.
        /// </param>
        public static Binary ReadBinary(InputBuffer buffer, int length) {
            if (length < 0) {
                throw new ArgumentOutOfRangeException("length");
            }

            var data = new Binary(buffer.Consume(length));

            if (data.Data.Length != length) {
                throw new ParseException(string.Format(@"\bin{0} specified, got {1} bytes of data.", length,
                    data.Data.Length));
            }

            return data;
        }

        /// <summary>
        /// Reads a control word from the input stream.
        /// </summary>
        /// <param name="buffer">
        /// The input stream that the control word will be read from.
        /// </param>
        /// <remarks>
        /// A control word consists of a backslash followed by up to 32 ASCII
        /// letters ([a-zA-Z]), followed by a delimiter. The delimiter can be
        /// any of the following:
        /// * Space: consumed with the control word.
        /// * Numeric digit or '-': start of a numeric parameter for the control word.
        /// The numeric parameter can be up to 10 numeric digits (not including the
        /// minus sign), followed by any non-numeric character.
        /// * Any character other than a letter or digit: not consumed as part of
        /// the control word.
        /// </remarks>
        public static ControlWord ReadControlWord(InputBuffer buffer) {
            ControlWord word;
            int consumed;

            if (ParseControlWord(buffer, 0, out word, out consumed)) {
                buffer.Consume(consumed);
                return word;
            }
            else {
                throw new ParseException("Failed to parse control word.");
            }
        }

        /// <summary>
        /// Reads a group end ('}') from the input stream.
        /// </summary>
        public static GroupEnd ReadGroupEnd(InputBuffer buffer) {
            if (buffer.At(0) != '}') {
                throw new ParseException("Expected Group End ('}')");
            }

            buffer.Discard(1);

            return new GroupEnd();
        }

        /// <summary>
        /// Reads a group start ('{') from the input stream.
        /// </summary>
        public static GroupStart ReadGroupStart(InputBuffer buffer) {
            if (buffer.At(0) != '{') {
                throw new ParseException("Expected Group Start ('{')");
            }

            buffer.Discard(1);

            return new GroupStart();
        }

        /// <summary>
        /// Reads the next token/entity from the input stream.
        /// </summary>
        /// <returns>Null if there is nothing left to read.</returns>
        public static ReadEvent ReadNext(InputBuffer buffer) {
            while (true) {
                var signal = buffer.At(0);
                switch (signal) {
                    case null:
                        // Reached end of input.
                        return null;
                    case (byte) '{':
                        // Start of new group.
                        return ReadGroupStart(buffer);
                    case (byte) '}':
                        // End of current group.
                        return ReadGroupEnd(buffer);
                    case (byte) '\\':
                        // Control word or symbol.
                        var next = buffer.At(1);
                        if (IsAsciiLetter(next) || next == '*') {
                            ControlWord word;
                            int consumed;

                            if (ParseControlWord(buffer, 0, out word, out consumed)) {
                                if (word.Text == "bin" && word.Parameter.HasValue) {
                                    // Read binary data from the stream.
                                    buffer.Discard(consumed);
                                    return ReadBinary(buffer, word.Parameter.Value);
                                }
                                else if (word.Text == "u" && word.Parameter.HasValue) {
                                    // Read unicode characters (control word \u####) as text.
                                    return ReadSpan(buffer);
                                }
                                else {
                                    // Return the control word as-is.
                                    buffer.Discard(consumed);
                                    return word;
                                }
                            }
                            else {
                                throw new ParseException("Failed to parse control word.");
                            }
                        }
                        else {
                            // Control symbols are handled as spans.
                            return ReadSpan(buffer);
                        }
                    case (byte) '\n':
                    case (byte) '\r':
                        // CR/LF should be ignored by RTF readers.
                        buffer.Discard(1);
                        break;
                    case (byte) '\0':
                        // EOF indicator.
                        return null;
                    default:
                        // Anything else is plain text.
                        return ReadSpan(buffer);
                }
            }
        }

        /// <summary>
        /// Reads plain text from the input stream.
        /// </summary>
        public static Span ReadSpan(InputBuffer buffer) {
            var span = new StringBuilder();

            while (true) {
                var text = buffer.ConsumeUntil(b => _escapeChars.Contains(b));
                span.Append(Encoding.UTF8.GetString(text, 0, text.Length));

                byte? next;
                if (buffer.At(0) == '\\' && (next = buffer.At(1)).HasValue) {
                    ControlWord word;
                    int consumed;

                    if (ParseControlWord(buffer, 0, out word, out consumed)) {
                        if (word.Text == "u" && word.Parameter.HasValue && word.Parameter > 0) {
                            // Handle unicode characters.
                            span.Append(Utf16.FromCode((uint) word.Parameter));
                            buffer.Discard(consumed + 1);
                        }
                        else {
                            break;
                        }
                    }
                    else if (!IsAsciiLetter(next)) {
                        // Handle control symbols.
                        // TODO: Translate control symbol to special character, e.g. \~ => nbsp.
                        span.Append((char) next.Value);
                        buffer.Discard(2);
                    }
                    else {
                        break;
                    }
                }
                else {
                    break;
                }
            }

            return new Span(span.ToString());
        }

        private static readonly byte[] _escapeChars = {
            (byte) '{',
            (byte) '}',
            (byte) '\\'
        };

        #region Helper Methods

        /// <summary>
        /// Checks whether the character is an ascii letter (a-z, A-Z).
        /// </summary>
        public static bool IsAsciiLetter(byte c) {
            return (c >= 'a' && c <= 'z') ||
                   (c >= 'A' && c <= 'Z');
        }

        /// <summary>
        /// Checks whether the character is an ascii letter (a-z, A-Z).
        /// </summary>
        public static bool IsAsciiLetter(byte? c) {
            return c.HasValue && IsAsciiLetter(c.Value);
        }

        /// <summary>
        /// Checks whether the character is a numeric digit (0-9).
        /// </summary>
        public static bool IsNumericDigit(byte c) {
            return c >= '0' && c <= '9';
        }

        /// <summary>
        /// Checks whether the character is a numeric digit (0-9).
        /// </summary>
        public static bool IsNumericDigit(byte? c) {
            return c.HasValue && IsNumericDigit(c.Value);
        }

        #endregion
    }
}