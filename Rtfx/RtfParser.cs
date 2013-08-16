using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Rtfx {
    /// <summary>
    /// Utility class to consume specific tokens/entities from an RTF file.
    /// </summary>
    internal static class RtfParser {
        /// <summary>
        /// Reads binary data from the input stream.
        /// </summary>
        /// <param name="buffer">
        /// The input stream that the data will be read from.
        /// </param>
        /// <param name="length">
        /// The amount of data to be read.
        /// </param>
        public static ReadEvent ReadBinary(InputBuffer buffer, int length) {
            if (length < 0) {
                throw new ArgumentOutOfRangeException("length");
            }

            var data = new ReadEvent();
            data.Type = EventType.Binary;
            data.Data = buffer.Consume(length);

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
        public static ReadEvent ReadControlWord(InputBuffer buffer) {
            if (buffer.At(0) != '\\') {
                throw new ParseException("Expected control word");
            }
            buffer.Discard(1);

            var word = new ReadEvent();

            if (buffer.At(0) == '*') {
                // Handle starred control word (e.g. "\*\foo").
                buffer.Discard(1);
                word = ReadControlWord(buffer);
                word.Starred = true;
                return word;
            }

            var name = buffer.ConsumeWhile(IsAsciiLetter, 32);

            if (name.Length == 0 || name.Length > 32) {
                throw new ParseException(string.Format("Invalid control word name: {0}",
                    Encoding.UTF8.GetString(name, 0, name.Length)));
            }

            word.Type = EventType.ControlWord;
            word.Text = Encoding.UTF8.GetString(name, 0, name.Length);

            var delim = buffer.At(0);
            if (IsNumericDigit(delim) || delim == '-') {
                // Parse the numeric parameter.
                if (delim == '-') {
                    buffer.Discard(1);
                }

                var param = buffer.ConsumeWhile(IsNumericDigit, 10);
                word.Parameter = int.Parse(Encoding.UTF8.GetString(param, 0, param.Length));

                if (delim == '-') {
                    word.Parameter *= -1;
                }
            }

            delim = buffer.At(0);
            if (delim == ' ') {
                // Consume the delimiter.
                buffer.Discard(1);
            }
            // Otherwise, leave the delimiter where it is.

            return word;
        }

        /// <summary>
        /// Reads a group end ('}') from the input stream.
        /// </summary>
        public static ReadEvent ReadGroupEnd(InputBuffer buffer) {
            if (buffer.At(0) != '}') {
                throw new ParseException("Expected Group End ('}')");
            }

            buffer.Discard(1);

            return new ReadEvent {
                Type = EventType.GroupEnd
            };
        }

        /// <summary>
        /// Reads a group start ('{') from the input stream.
        /// </summary>
        public static ReadEvent ReadGroupStart(InputBuffer buffer) {
            if (buffer.At(0) != '{') {
                throw new ParseException("Expected Group Start ('{')");
            }

            buffer.Discard(1);

            return new ReadEvent {
                Type = EventType.GroupStart
            };
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
                            var word = ReadControlWord(buffer);
                            if (word.Text == "bin" && word.Parameter.HasValue) {
                                return ReadBinary(buffer, word.Parameter.Value);
                            }
                            else {
                                return word;
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
        public static ReadEvent ReadSpan(InputBuffer buffer) {
            var span = new MemoryStream();

            while (true) {
                var text = buffer.ConsumeUntil(b => _escapeChars.Contains(b));
                span.Write(text, 0, text.Length);

                byte? next;
                if (buffer.At(0) == '\\' &&
                    (next = buffer.At(1)).HasValue &&
                    !IsAsciiLetter(next)) {
                    // Handle control symbols.
                    // TODO: Translate control symbol to special character, e.g. \~ => nbsp.
                    span.WriteByte(next.Value);
                    buffer.Discard(2);
                }
                else {
                    break;
                }
            }

            return new ReadEvent {
                Type = EventType.Span,
                Text = Encoding.UTF8.GetString(span.ToArray(), 0, (int) span.Length)
            };
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