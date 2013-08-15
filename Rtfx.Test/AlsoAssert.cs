using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Rtfx.Test {
    /// <summary>
    /// Additional assertions and utilities for unit tests.
    /// </summary>
    public static class AlsoAssert {
        /// <summary>
        /// Validates that the expected exception is thrown.
        /// </summary>
        public static void Throws<TException>(Action action) {
            if (action == null) {
                throw new ArgumentNullException("action");
            }

            Exception ex = null;
            try {
                action();
            }
            catch (Exception e) {
                ex = e;
            }

            Assert.IsNotNull(ex, "Expected an exception of type {0}.", typeof (TException));
            Assert.IsInstanceOfType(ex, typeof (TException),
                "Expected an exception of type {0}; got {1}", typeof (TException), ex.GetType());
        }

        #region Read Events

        /// <summary>
        /// Verifies that the read event is a control word.
        /// </summary>
        public static void ControlWord(Reader reader, string name, bool starred, int? param = null) {
            ControlWord(reader.Next(), name, starred, param);
        }

        /// <summary>
        /// Verifies that the read event is a control word.
        /// </summary>
        public static void ControlWord(Reader reader, string name, int? param = null) {
            ControlWord(reader.Next(), name, false, param);
        }

        /// <summary>
        /// Verifies that the read event is a group end ('}').
        /// </summary>
        public static void GroupEnd(Reader reader) {
            GroupEnd(reader.Next());
        }

        /// <summary>
        /// Verifies that the read event is a group start ('{').
        /// </summary>
        public static void GroupStart(Reader reader) {
            GroupStart(reader.Next());
        }

        /// <summary>
        /// Verifies that the read event is a span of text.
        /// </summary>
        public static void Span(Reader reader, string text) {
            Span(reader.Next(), text);
        }

        /// <summary>
        /// Verifies that the read event is a control word.
        /// </summary>
        public static void ControlWord(ReadEvent e, string name, bool starred = false, int? param = null) {
            Assert.AreEqual(EventType.ControlWord, e.Type);
            Assert.AreEqual(starred, e.Starred);
            Assert.AreEqual(param, e.Parameter);
            Assert.AreEqual(name, e.Text);
        }

        /// <summary>
        /// Verifies that the read event is a group end ('}').
        /// </summary>
        public static void GroupEnd(ReadEvent e) {
            Assert.AreEqual(EventType.GroupEnd, e.Type);
            Assert.IsFalse(e.Starred);
            Assert.IsNull(e.Parameter);
            Assert.IsNull(e.Text);
        }

        /// <summary>
        /// Verifies that the read event is a group start ('{').
        /// </summary>
        public static void GroupStart(ReadEvent e) {
            Assert.AreEqual(EventType.GroupStart, e.Type);
            Assert.IsFalse(e.Starred);
            Assert.IsNull(e.Parameter);
            Assert.IsNull(e.Text);
        }

        /// <summary>
        /// Verifies that the read event is a span of text.
        /// </summary>
        public static void Span(ReadEvent e, string text) {
            Assert.AreEqual(EventType.Span, e.Type);
            Assert.IsFalse(e.Starred);
            Assert.IsNull(e.Parameter);
            Assert.AreEqual(text, e.Text);
        }

        #endregion
    }
}