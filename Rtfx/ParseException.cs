using System;

namespace Rtfx {
    /// <summary>
    /// An error that occurred while attempting to parse
    /// an RTF file.
    /// </summary>
    public class ParseException : Exception {
        public ParseException(string message) :
            base(message) {}
    }
}