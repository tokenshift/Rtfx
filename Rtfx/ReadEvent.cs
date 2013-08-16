using System;

namespace Rtfx {
    /// <summary>
    /// The type of a token parsed from an RTF file.
    /// </summary>
    public enum EventType {
        Binary,
        ControlWord,
        GroupStart,
        GroupEnd,
        Span
    }

    /// <summary>
    /// A token parsed from an RTF file.
    /// </summary>
    public class ReadEvent : EventArgs {
        /// <summary>
        /// The type of the token.
        /// </summary>
        public EventType Type { get; set; }

        /// <summary>
        /// Whether the token is a starred (\*) control word.
        /// </summary>
        public bool Starred { get; set; }

        /// <summary>
        /// The text contained by/name of the parsed token.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Any numeric parameter associated with a control word.
        /// </summary>
        public Int32? Parameter { get; set; }

        /// <summary>
        /// Consumed binary data.
        /// </summary>
        public byte[] Data { get; set; }

        public override string ToString() {
            return Text == null ? string.Format("{{{0}}}", Type) : string.Format("{{{0}:{1}}}", Type, Text);
        }
    }
}