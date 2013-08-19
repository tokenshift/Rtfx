using System;
using System.Linq;

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
    public abstract class ReadEvent : EventArgs {
        /// <summary>
        /// The type of the token.
        /// </summary>
        public abstract EventType Type { get; }

        /// <summary>
        /// Whether the token is a starred (\*) control word.
        /// </summary>
        public bool Starred { get; protected set; }

        /// <summary>
        /// The text contained by/name of the parsed token.
        /// </summary>
        public string Text { get; protected set; }

        /// <summary>
        /// Any numeric parameter associated with a control word.
        /// </summary>
        public Int32? Parameter { get; protected set; }

        /// <summary>
        /// Consumed binary data.
        /// </summary>
        public byte[] Data { get; protected set; }

        public override string ToString() {
            return Text == null ? string.Format("{{{0}}}", Type) : string.Format("{{{0}:{1}}}", Type, Text);
        }
    }

    /// <summary>
    /// Binary data read from an RTF input stream.
    /// </summary>
    public class Binary : ReadEvent {
        public override EventType Type {
            get { return EventType.Binary; }
        }

        public Binary(byte[] data) {
            Data = data;
        }
    }

    /// <summary>
    /// A control word read from an RTF input stream.
    /// </summary>
    public class ControlWord : ReadEvent {
        public override EventType Type {
            get { return EventType.ControlWord; }
        }

        public ControlWord(string name, bool starred = false, int? param = null) {
            if (name.Cast<char>().Any(c => c > 127)) {
                throw new ArgumentException(string.Format("'{0}' is not a valid control word name.", name), "name");
            }

            Text = name;
            Starred = starred;
            Parameter = param;
        }
    }

    /// <summary>
    /// A group end symbol read from an RTF input stream.
    /// </summary>
    public class GroupEnd : ReadEvent {
        public override EventType Type {
            get { return EventType.GroupEnd; }
        }
    }

    /// <summary>
    /// A group start symbol read from an RTF input stream.
    /// </summary>
    public class GroupStart : ReadEvent {
        public override EventType Type {
            get { return EventType.GroupStart; }
        }
    }

    /// <summary>
    /// A span of plain text read from an RTF input stream.
    /// </summary>
    public class Span : ReadEvent {
        public override EventType Type {
            get { return EventType.Span; }
        }

        public Span(string text) {
            Text = text;
        }
    }
}