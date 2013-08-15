﻿namespace Rtfx {
    /// <summary>
    /// Utility class to consume specific tokens/entities from an RTF file.
    /// </summary>
    internal static class RtfParser {
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
        public static ReadEvent ReadControlWord(StringBuffer buffer) {
            if (buffer.CharAt(0) != '\\') {
                throw new ParseException("Expected control word", buffer.CurrentLine, buffer.CurrentColumn);
            }
            buffer.Discard(1);

            var word = new ReadEvent();

            if (buffer.CharAt(0) == '*') {
                // Handle starred control word (e.g. "\*\foo").
                buffer.Discard(1);
                word = ReadControlWord(buffer);
                word.Starred = true;
                return word;
            }

            var name = buffer.ConsumeWhile(IsAsciiLetter, 32);

            if (string.IsNullOrEmpty(name) || name.Length > 32) {
                throw new ParseException(string.Format("Invalid control word name: {0}", name), buffer.CurrentLine,
                    buffer.CurrentColumn);
            }

            word.Text = name;

            var delim = buffer.CharAt(0);
            if (IsNumericDigit(delim) || delim == '-') {
                // Parse the numeric parameter.
                var param = 1;
                if (delim == '-') {
                    param = -1;
                    buffer.Discard(1);
                }

                param = param*int.Parse(buffer.ConsumeWhile(IsNumericDigit, 10));
                word.Parameter = param;
            }

            delim = buffer.CharAt(0);
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
        public static ReadEvent ReadGroupEnd(StringBuffer buffer) {
            if (buffer.CharAt(0) != '}') {
                throw new ParseException("Expected Group End ('}')", buffer.CurrentLine, buffer.CurrentColumn);
            }

            buffer.Discard(1);

            return new ReadEvent {
                Type = EventType.GroupStart
            };
        }

        /// <summary>
        /// Reads a group start ('{') from the input stream.
        /// </summary>
        public static ReadEvent ReadGroupStart(StringBuffer buffer) {
            if (buffer.CharAt(0) != '{') {
                throw new ParseException("Expected Group Start ('{')", buffer.CurrentLine, buffer.CurrentColumn);
            }

            buffer.Discard(1);

            return new ReadEvent {
                Type = EventType.GroupStart
            };
        }

        #region Helper Methods

        /// <summary>
        /// Checks whether the character is an ascii letter (a-z, A-Z).
        /// </summary>
        public static bool IsAsciiLetter(char c) {
            return (c >= 'a' && c <= 'z') ||
                   (c >= 'A' && c <= 'Z');
        }

        /// <summary>
        /// Checks whether the character is a numeric digit (0-9).
        /// </summary>
        public static bool IsNumericDigit(char c) {
            return c >= '0' && c <= '9';
        }

        /// <summary>
        /// Checks whether the character is a numeric digit (0-9).
        /// </summary>
        public static bool IsNumericDigit(char? c) {
            return c.HasValue && IsNumericDigit(c.Value);
        }

        #endregion
    }
}