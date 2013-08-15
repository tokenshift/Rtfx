using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Rtfx {
    /// <summary>
    /// Provides buffered text reading and lookahead.
    /// </summary>
    public class StringBuffer : IDisposable {
        private char[] _buffer;
        private bool _eof = false;
        private const int InitialSize = 1024;
        private int _length = 0;
        private int _offset = 0;
        private readonly TextReader _input;

        public int CurrentLine { get; private set; }
        public int CurrentColumn { get; private set; }

        /// <summary>
        /// Gets the character at the specified index.
        /// </summary>
        public char this[int index] {
            get {
                var c = CharAt(index);
                if (c == null) {
                    throw new ArgumentOutOfRangeException("index");
                }

                return c.Value;
            }
        }

        /// <summary>
        /// Gets the character at the specified index.
        /// </summary>
        /// <returns>Null if the index is out of range.</returns>
        public char? CharAt(int index) {
            if (index < 0) {
                throw new ArgumentOutOfRangeException("index");
            }

            var i = _offset + index;
            while (i >= _length && !_eof) {
                // TODO: Pass desired index to ReadMore so that
                // we only need to grow the buffer once.
                ReadMore();
                i = _offset + index;
            }

            if (i >= _length) {
                return null;
            }

            return _buffer[i];
        }

        /// <summary>
        /// Returns a string containing the first N characters
        /// of the buffer, and discards those characters from
        /// the buffer.
        /// </summary>
        /// <param name="count">The number of characters to read and discard.</param>
        public string Consume(int count) {
            if (count < 0) {
                throw new ArgumentOutOfRangeException("count");
            }

            if (count == 0) {
                return "";
            }

            while (_offset + count >= _length && !_eof) {
                ReadMore();
            }

            count = Math.Min(count, _length - _offset);

            var result = new string(_buffer, _offset, count);
            _offset += count;
            return result;
        }

        /// <summary>
        /// Consumes all characters until one of the specified
        /// characters is encountered.
        /// </summary>
        /// <remarks>
        /// If EOF is encountered before a matching character is found, will
        /// return everything until EOF.
        /// </remarks>
        public string ConsumeUntil(params char[] characters) {
            if (characters == null) {
                throw new ArgumentNullException("characters");
            }

            if (characters.Length == 0) {
                throw new ArgumentException("At least one character must be specified.");
            }

            int i;
            char? c;
            for (i = 0; (c = CharAt(i)).HasValue && !characters.Contains(c.Value); ++i) {}

            return Consume(i);
        }

        /// <summary>
        /// Consumes all characters until the specified text
        /// is found.
        /// </summary>
        public string ConsumeUntil(string text) {
            if (text == null) {
                throw new ArgumentNullException("text");
            }

            if (text.Length == 0) {
                return "";
            }

            int i;
            for (i = 0; CharAt(i).HasValue; ++i) {
                if (BufferMatches(i, text)) {
                    return Consume(i);
                }
            }

            return Consume(i);
        }

        /// <summary>
        /// Consumes all characters until the specified regular expression
        /// is matched.
        /// </summary>
        public string ConsumeUntil(Regex pattern) {
            if (pattern == null) {
                throw new ArgumentNullException("pattern");
            }

            Match match;
            while (!(match = pattern.Match(new string(_buffer, _offset, _length - _offset))).Success && !_eof) {
                ReadMore();
            }

            if (match.Success) {
                return Consume(match.Index);
            }

            return Consume(_length - _offset);
        }

        /// <summary>
        /// Consumes all characters that match the specified predicate.
        /// </summary>
        /// <param name="pred">A predicate that will be called with each character.</param>
        /// <returns>A string containing all of the characters that matched the predicate.</returns>
        public string ConsumeWhile(Func<char, bool> pred) {
            if (pred == null) {
                throw new ArgumentNullException("pred");
            }

            int i;
            char? c;
            for (i = 0; (c = CharAt(i)).HasValue && pred(c.Value); ++i) {}

            return Consume(i);
        }

        /// <summary>
        /// Consumes all characters that match the specified predicate.
        /// </summary>
        /// <param name="pred">A predicate that will be called with each character.</param>
        /// <param name="maxLength">The maximum length of the string that will be consumed.</param>
        /// <returns>A string containing all of the characters that matched the predicate.</returns>
        public string ConsumeWhile(Func<char, bool> pred, int maxLength)
        {
            if (pred == null)
            {
                throw new ArgumentNullException("pred");
            }

            if (maxLength <= 0) {
                throw new ArgumentOutOfRangeException("maxLength");
            }

            int i;
            char? c;
            for (i = 0; i < maxLength && (c = CharAt(i)).HasValue && pred(c.Value); ++i) { }

            return Consume(i);
        }

        /// <summary>
        /// Discards the specified number of characters from the buffer.
        /// </summary>
        /// <param name="count">The number of characters to discard.</param>
        public void Discard(int count) {
            if (count < 0) {
                throw new ArgumentOutOfRangeException("count");
            }

            if (count == 0) {
                return;
            }

            while (_offset + count >= _length && !_eof) {
                ReadMore();
            }

            _offset = Math.Min(_offset + count, _length);
        }

        /// <summary>
        /// Closes the input stream.
        /// </summary>
        public void Dispose() {
            _buffer = null;
            _input.Dispose();
        }

        /// <summary>
        /// Whether the end of the input stream has been reached.
        /// </summary>
        public bool Eof {
            get {
                return _eof && _offset >= _length;
            }
        }

        /// <summary>
        /// Returns a section of the buffer as a string.
        /// </summary>
        /// <param name="offset">The starting index of the string in the buffer.</param>
        /// <param name="length">The length of the string to return.</param>
        public string Span(int offset, int length) {
            if (offset < 0) {
                throw new ArgumentOutOfRangeException("offset");
            }

            if (length < 0) {
                throw new ArgumentOutOfRangeException("length");
            }

            if (length == 0) {
                return "";
            }

            while (_offset + offset + length >= _length && !_eof) {
                ReadMore();
            }

            if (offset >= _length) {
                throw new ArgumentOutOfRangeException("offset");
            }

            var count = Math.Min(length, _length - _offset - offset);
            return new string(_buffer, _offset + offset, count);
        }

        /// <summary>
        /// Returns the first N characters of the buffer as a string.
        /// </summary>
        /// <param name="length">The number of characters to return.</param>
        public string Span(int length) {
            return Span(0, length);
        }

        #region Constructors

        public StringBuffer(TextReader input) : this(input, InitialSize) {}

        public StringBuffer(TextReader input, int initialSize) {
            if (initialSize < 0) throw new ArgumentException("Buffer size must be greater than 0.");

            _buffer = new char[initialSize];
            _input = input;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Checks whether an input string is found at the specified
        /// location in the buffer.
        /// </summary>
        /// <param name="offset">The index in the buffer to check at.</param>
        /// <param name="text">The text to match.</param>
        /// <returns>True if the specified text was found at the exact location.</returns>
        private bool BufferMatches(int offset, string text) {
            for (var i = 0; i < text.Length; ++i) {
                var c = CharAt(offset + i);
                if (!c.HasValue || c.Value != text[i]) {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Reads more data from the input stream.
        /// </summary>
        private void ReadMore() {
            if (_eof) return;

            // First, ensure buffer is full.
            if (_length < _buffer.Length) {
                var count = _buffer.Length - _length;
                var read = _input.Read(_buffer, _length, count);
                _length += read;
                if (read < count) _eof = true;
                return;
            }

            // If unconsumed characters are more than half of buffer size, increase buffer size.
            var bufferSize = _buffer.Length;
            if (_offset < bufferSize/2) {
                bufferSize *= 2;
            }

            // Discard consumed characters.
            var buffer = new char[bufferSize];
            Array.Copy(_buffer, _offset, buffer, 0, _length - _offset);
            _length = _length - _offset;
            _offset = 0;
            _buffer = buffer;

            // Fill and/or grow the buffer.
            ReadMore();
        }

        #endregion
    }
}