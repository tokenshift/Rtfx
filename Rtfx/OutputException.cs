using System;

namespace Rtfx
{
    /// <summary>
    /// An error that occurred while writing RTF to
    /// an output stream.
    /// </summary>
    public class OutputException : Exception
    {
        public OutputException(string message) : base(message) {}
    }
}
