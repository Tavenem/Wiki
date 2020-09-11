using System.IO;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.Mvc
{
    /// <summary>
    /// A service which persists and retrieves files associated with <see cref="WikiFile"/> items.
    /// </summary>
    public interface IFileManager
    {
        /// <summary>
        /// Load the given file from a persistence store.
        /// </summary>
        /// <param name="path">The path to the file.</param>
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
        /// The returned path may be a URI, a relative file path, or an absolute file path,
        /// depending on persistence implementation. Whatever the format, it should be accepted as
        /// an input to the <c>LoadFileAsync</c> methods of the same implementation.
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
        /// The returned path may be a URI, a relative file path, or an absolute file path,
        /// depending on persistence implementation. Whatever the format, it should be accepted as
        /// an input to the <c>LoadFileAsync</c> methods of the same implementation.
        /// </remarks>
        public ValueTask<string?> SaveFileAsync(Stream? data, string? fileName, string? owner = null);

        /// <summary>
        /// Load the given file from a persistence store.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <returns>
        /// A <see cref="Stream"/> containing the file; or <see langword="null"/> if no such file
        /// was found.
        /// </returns>
        public ValueTask<Stream?> StreamFileAsync(string? path);
    }
}
