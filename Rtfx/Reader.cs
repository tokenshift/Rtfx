using System;
using System.Collections.Generic;
using System.IO;

namespace Rtfx {
    /// <summary>
    /// Reads and parses an RTF document
    /// </summary>
    public class Reader : IDisposable {
        private readonly StringBuffer _buffer;

        public void Dispose() {
            _buffer.Dispose();
        }

        public IEnumerable<ReadEvent> Read() {
            ReadEvent e;
            while ((e = Next()) != null) {
                yield return e;
            }
        }

        #region RTF Parsing

        /// <summary>
        /// Read the next RTF token from the input.
        /// </summary>
        /// <returns>
        /// Null if the end of the input has been reached.
        /// </returns>
        public ReadEvent Next() {
            throw new NotImplementedException();
        }

        #endregion

        private Reader(TextReader input) {
            _buffer = new StringBuffer(input);
        }

        #region Factory Methods

        /// <summary>
        /// Create an RTF reader from the specified text input.
        /// </summary>
        public static Reader Create(TextReader input) {
            return new Reader(input);
        }

        #endregion
    }
}