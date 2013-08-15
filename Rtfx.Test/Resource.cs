using System;
using System.IO;
using System.Reflection;

namespace Rtfx.Test {
    /// <summary>
    /// Helper methods for loading test resources.
    /// </summary>
    public static class Resource {
        /// <summary>
        /// Open the specified resource as a TextReader.
        /// </summary>
        /// <param name="name">The name of the resource to load.</param>
        public static TextReader Reader(string name) {
            var reader = new StreamReader(Stream(name));
            return reader;
        }

        /// <summary>
        /// Open the specified resource as an input stream.
        /// </summary>
        /// <param name="name">The name of the resource to load.</param>
        public static Stream Stream(string name) {
            var resource = typeof (Resource).GetTypeInfo().Assembly.GetManifestResourceStream(name);
            if (resource == null) {
                throw new Exception(string.Format("Could not find resource \"{0}.\"", name));
            }

            return resource;
        }

        /// <summary>
        /// Load the specified resource as a string.
        /// </summary>
        /// <param name="name">The name of the resource to load.</param>
        public static string String(string name) {
            using (var reader = Reader(name)) {
                return reader.ReadToEnd();
            }
        }
    }
}