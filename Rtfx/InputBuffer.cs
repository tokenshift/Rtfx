using System;
using System.IO;

namespace Rtfx {
    /// <summary>
    /// Provides buffered input reading and lookahead.
    /// </summary>
    public class InputBuffer : IDisposable {
        private byte[] _buffer;
        private bool _eof;
        private const int InitialSize = 1024;
        private int _length;
        private int _offset;
        private readonly Stream _input;

        /// <summary>
        /// Gets the byte at the specified index.
        /// </summary>
        public byte this[int index] {
            get {
                var b = At(index);
                if (b == null) {
                    throw new ArgumentOutOfRangeException("index");
                }

                return b.Value;
            }
        }

        /// <summary>
        /// Gets the byte at the specified index.
        /// </summary>
        /// <returns>Null if the index is out of range.</returns>
        public Byte? At(int index) {
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
        /// Returns a byte array containing the first N bytes
        /// of the buffer, and discards those bytes from
        /// the buffer.
        /// </summary>
        /// <param name="count">The number of bytes to read and discard.</param>
        public byte[] Consume(int count) {
            if (count < 0) {
                throw new ArgumentOutOfRangeException("count");
            }

            if (count == 0) {
                return new byte[0];
            }

            while (_offset + count >= _length && !_eof) {
                ReadMore();
            }

            count = Math.Min(count, _length - _offset);

            var result = GetBufferFragment(_offset, count);
            _offset += count;
            return result;
        }

        /// <summary>
        /// Consumes all bytes until the specified predicate is matched.
        /// </summary>
        /// <param name="pred">A predicate that will be called with each byte.</param>
        /// <returns>An array containing all of the bytes until the predicate was matched.</returns>
        public byte[] ConsumeUntil(Func<byte, bool> pred) {
            return ConsumeWhile(b => !pred(b));
        }

        /// <summary>
        /// Consumes all bytes until the specified byte is encountered.
        /// </summary>
        /// <remarks>
        /// If EOF is encountered before a matching character is found, will
        /// return everything until EOF.
        /// </remarks>
        public byte[] ConsumeUntil(byte signal) {
            int i;

            for (i = 0; At(i) != signal; ++i) {}

            return Consume(i);
        }

        /// <summary>
        /// Consumes all bytes until the specified byte sequence is found.
        /// </summary>
        public byte[] ConsumeUntil(byte[] sequence) {
            if (sequence == null) {
                throw new ArgumentNullException("sequence");
            }

            if (sequence.Length == 0) {
                return new byte[0];
            }

            int i;
            for (i = 0; At(i).HasValue; ++i) {
                if (BufferMatches(i, sequence)) {
                    return Consume(i);
                }
            }

            return Consume(i);
        }

        /// <summary>
        /// Consumes all bytes that match the specified predicate.
        /// </summary>
        /// <param name="pred">A predicate that will be called with each byte.</param>
        /// <returns>An array containing all of the bytes that matched the predicate.</returns>
        public byte[] ConsumeWhile(Func<byte, bool> pred) {
            if (pred == null) {
                throw new ArgumentNullException("pred");
            }

            int i;
            byte? b;
            for (i = 0; (b = At(i)).HasValue && pred(b.Value); ++i) {}

            return Consume(i);
        }

        /// <summary>
        /// Consumes all bytes that match the specified predicate.
        /// </summary>
        /// <param name="pred">A predicate that will be called with each byte.</param>
        /// <param name="maxLength">The maximum length of the array that will be consumed.</param>
        /// <returns>An array containing all of the bytes that matched the predicate.</returns>
        public byte[] ConsumeWhile(Func<byte, bool> pred, int maxLength) {
            if (pred == null) {
                throw new ArgumentNullException("pred");
            }

            if (maxLength <= 0) {
                throw new ArgumentOutOfRangeException("maxLength");
            }

            int i;
            byte? b;
            for (i = 0; i < maxLength && (b = At(i)).HasValue && pred(b.Value); ++i) {}

            return Consume(i);
        }

        /// <summary>
        /// Discards the specified number of bytes from the buffer.
        /// </summary>
        /// <param name="count">The number of bytes to discard.</param>
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
        /// Returns a section of the buffer as a byte array.
        /// </summary>
        /// <param name="offset">The starting index of the byte array in the buffer.</param>
        /// <param name="length">The length of the byte array to return.</param>
        public byte[] Span(int offset, int length) {
            if (offset < 0) {
                throw new ArgumentOutOfRangeException("offset");
            }

            if (length < 0) {
                throw new ArgumentOutOfRangeException("length");
            }

            if (length == 0) {
                return new byte[0];
            }

            while (_offset + offset + length >= _length && !_eof) {
                ReadMore();
            }

            if (offset >= _length) {
                throw new ArgumentOutOfRangeException("offset");
            }

            var count = Math.Min(length, _length - _offset - offset);
            return GetBufferFragment(_offset + offset, count);
        }

        /// <summary>
        /// Returns the first N bytes of the buffer as an array.
        /// </summary>
        /// <param name="length">The number of bytes to return.</param>
        public byte[] Span(int length) {
            return Span(0, length);
        }

        #region Constructors

        public InputBuffer(Stream input) : this(input, InitialSize) {}

        public InputBuffer(Stream input, int initialSize) {
            if (initialSize < 0) throw new ArgumentException("Buffer size must be greater than 0.");

            _buffer = new byte[initialSize];
            _input = input;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Checks whether an input sequence is found at the specified
        /// location in the buffer.
        /// </summary>
        /// <param name="offset">The index in the buffer to check at.</param>
        /// <param name="sequence">The sequence to match.</param>
        /// <returns>True if the specified sequence was found at the exact location.</returns>
        private bool BufferMatches(int offset, byte[] sequence) {
            for (var i = 0; i < sequence.Length; ++i) {
                var b = At(offset + i);
                if (!b.HasValue || b.Value != sequence[i]) {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Gets a portion of the current buffer.
        /// </summary>
        /// <param name="offset">Where to start copying the buffer.</param>
        /// <param name="length">The number of bytes to copy.</param>
        private byte[] GetBufferFragment(int offset, int length) {
            var fragment = new byte[length];

            Array.Copy(_buffer, offset, fragment, 0, length);

            return fragment;
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
            var buffer = new byte[bufferSize];
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