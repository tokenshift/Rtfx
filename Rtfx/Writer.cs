using System;
using System.IO;
using System.Linq;
using Harmful;

namespace Rtfx
{
    /// <summary>
    /// Writes RTF data to an output stream.
    /// </summary>
    public class Writer :IDisposable {
        private readonly Stream _output;
        private readonly TextWriter _writer;

        private static readonly char[] _escapeChars = {
            '{',
            '}',
            '\\'
        };

        public void Dispose() {
            _writer.Dispose();
            _output.Dispose();
        }

        public Writer(Stream output) {
            if (output == null) {
                throw new ArgumentNullException("output");
            }

            _output = output;
            _writer = new StreamWriter(output);
        }

        #region RTF Output

        /// <summary>
        /// Writes a control word to the output stream.
        /// </summary>
        public void Control(string name, int? param = null) {
            if (name == null) {
                throw new ArgumentNullException("name");
            }

            if (name.Length == 0 || name.Length > 32) {
                throw new ArgumentOutOfRangeException("name", "Control word must be between 1 and 32 characters long.");
            }

            if (param.HasValue) {
                _writer.Write(@"\{0}{1:0} ", name, param);
            }
            else {
                _writer.Write(@"\{0} ", name);
            }
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
            _writer.Write("}");
        }

        /// <summary>
        /// Writes a group start symbol ('{') to the output stream.
        /// </summary>
        public void GroupStart() {
            _writer.Write("{");
        }

        /// <summary>
        /// Writes plain text content to the output stream.
        /// </summary>
        public void Span(string text) {
            if (text == null) {
                throw new ArgumentNullException("text");
            }

            int start, end;
            for (start = 0, end = 0; end < text.Length; ++end) {
                if (_escapeChars.Contains(text[end])) {
                    _writer.Write(text.Substring(start, end - start));

                    // Write control symbol.
                    _writer.Write('\\');
                    _writer.Write(text[end]);
                    start = end = end + 1;
                }
                else if (text[end] > 127) {
                    _writer.Write(text.Substring(start, end - start));

                    // Write unicode character.
                    uint code;
                    start = end = end + Utf16.ToCode(out code, text, end);
                    _writer.Write(@"\u{0}?", code);
                }
            }

            if (start < end) {
                _writer.Write(text.Substring(start, end - start));
            }
        }

        #endregion
    }
}
