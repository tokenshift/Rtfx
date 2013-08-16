using System;
using System.Collections.Generic;
using System.IO;

namespace Rtfx {
    /// <summary>
    /// Reads and parses an RTF document
    /// </summary>
    public class Reader : IDisposable {
        private readonly InputBuffer _buffer;

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
            return RtfParser.ReadNext(_buffer);
        }

        #endregion

        private Reader(Stream input) {
            _buffer = new InputBuffer(input);
        }

        #region Factory Methods

        /// <summary>
        /// Create an RTF reader from the specified input stream.
        /// </summary>
        public static Reader Create(Stream input) {
            return new Reader(input);
        }

        #endregion
    }
}