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
    }
}