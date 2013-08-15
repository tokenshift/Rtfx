using System;

namespace Rtfx {
    /// <summary>
    /// An error that occurred while attempting to parse
    /// an RTF file.
    /// </summary>
    public class ParseException : Exception {
        public readonly int Line;
        public readonly int Column;

        public ParseException(string message, int line, int col) :
            base(message) {
            Line = line;
            Column = col;
        }
    }
}
