using System;
using System.IO;

namespace Rtfx
{
    /// <summary>
    /// Writes RTF data to an output stream.
    /// </summary>
    public class Writer :IDisposable {
        private readonly Stream _output;

        public void Dispose() {
            _output.Dispose();
        }

        public Writer(Stream output) {
            if (output == null) {
                throw new ArgumentNullException("output");
            }

            _output = output;
        }

        #region RTF Output

        /// <summary>
        /// Writes a control word to the output stream.
        /// </summary>
        public void Control(string name, int? param = null) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes binary data to the output stream.
        /// </summary>
        public void Data(byte[] buffer) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes a group end symbol ('}') to the output stream.
        /// </summary>
        public void GroupEnd() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes a group start symbol ('{') to the output stream.
        /// </summary>
        public void GroupStart()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes plain text content to the output stream.
        /// </summary>
        public void Span(string content) {
            throw new NotImplementedException();
        }

        #endregion
    }
}
