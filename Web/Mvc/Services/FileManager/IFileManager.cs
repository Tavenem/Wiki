﻿using System.IO;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.Mvc
{
    /// <summary>
    /// A service which persists and retrieves files associated with <see cref="WikiFile"/> items.
    /// </summary>
    public interface IFileManager
    {
        /// <summary>
        /// Remove the given file from a persistence store.
        /// </summary>
        /// <param name="path">The path to the file. A relative URL is expected.</param>
        /// <returns>
        /// <see langword="true"/> if the file was successfully removed; otherwise <see
        /// langword="false"/>. Also returns <see langword="true"/> if the given file does not exist
        /// (to indicate no issues "removing" it).
        /// </returns>
        public ValueTask<bool> DeleteFileAsync(string? path);

        /// <summary>
        /// Load the given file from a persistence store.
        /// </summary>
        /// <param name="path">The path to the file. A relative URL is expected.</param>
        /// <returns>
        /// A <see cref="byte"/> array containing the file; or <see langword="null"/> if no such
        /// file was found.
        /// </returns>
        public ValueTask<byte[]?> LoadFileAsync(string? path);

        /// <summary>
        /// Save the given file to a persistence store.
        /// </summary>
        /// <param name="data">A byte array containing the file.</param>
        /// <param name="fileName">The file name.</param>
        /// <param name="owner">The owner of the file. May be <see langword="null"/>.</param>
        /// <returns>
        /// The path of the stored file, if it was successfully saved; otherwise <see
        /// langword="null"/>.
        /// </returns>
        /// <remarks>
        /// The returned path is the relative URL to the file.
        /// </remarks>
        public ValueTask<string?> SaveFileAsync(byte[]? data, string? fileName, string? owner = null);

        /// <summary>
        /// Save the given file to a persistence store.
        /// </summary>
        /// <param name="data">A stream containing the file.</param>
        /// <param name="fileName">The file name.</param>
        /// <param name="owner">The owner of the file. May be <see langword="null"/>.</param>
        /// <returns>
        /// The path of the stored file, if it was successfully saved; otherwise <see
        /// langword="null"/>.
        /// </returns>
        /// <remarks>
        /// The returned path is the relative URL to the file.
        /// </remarks>
        public ValueTask<string?> SaveFileAsync(Stream? data, string? fileName, string? owner = null);

        /// <summary>
        /// Load the given file from a persistence store.
        /// </summary>
        /// <param name="path">The path to the file. A relative URL is expected.</param>
        /// <returns>
        /// A <see cref="Stream"/> containing the file; or <see langword="null"/> if no such file
        /// was found.
        /// </returns>
        public ValueTask<Stream?> StreamFileAsync(string? path);
    }
}
