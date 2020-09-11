using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace NeverFoundry.Wiki.Mvc.Services.FileManager
{
    /// <summary>
    /// A service which persists and retrieves files associated with <see cref="WikiFile"/> items.
    /// This implementation stores files in a subfolder of wwwroot named "files" (created on
    /// demand). Files with owners are placed in subfolders named for the owner's ID (sanitized by
    /// replacing any disallowed characters with an underscore). Unowned items are placed directly
    /// in the "files" folder. Files are given random filenames (i.e. the filename specified when
    /// saving is not used to determine the actual name of the file), for security purposes.
    /// </summary>
    public class LocalFileManager : IFileManager
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<IFileManager> _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="LocalFileManager"/>.
        /// </summary>
        public LocalFileManager(
            IWebHostEnvironment environment,
            IHttpContextAccessor httpContextAccessor,
            ILogger<IFileManager> logger)
        {
            _environment = environment;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        /// <summary>
        /// Load the given file from a persistence store.
        /// </summary>
        /// <param name="path">The path to the file. A relative URL is expected.</param>
        /// <returns>
        /// A <see cref="byte" /> array containing the file; or <see langword="null" /> if no such
        /// file was found.
        /// </returns>
        public async ValueTask<byte[]?> LoadFileAsync(string? path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return null;
            }
            try
            {
                var request = _httpContextAccessor.HttpContext?.Request;
                if (request is null)
                {
                    throw new Exception("Files cannot be loaded outside of an HTTP request context.");
                }
                var baseUrl = request.Scheme + "://" + request.Host + request.PathBase;
                using var wc = new WebClient { BaseAddress = baseUrl };
                return await wc.DownloadDataTaskAsync(path).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception loading file at path {Path}", path);
                return null;
            }
        }

        /// <summary>
        /// Save the given file to a persistence store.
        /// </summary>
        /// <param name="data">A byte array containing the file.</param>
        /// <param name="fileName">The file name.</param>
        /// <param name="owner">The owner of the file. May be <see langword="null" />.</param>
        /// <returns>
        /// The path of the stored file, if it was successfully saved; otherwise <see langword="null" />.
        /// </returns>
        /// <remarks>
        /// The returned path is the relative URL to the file.
        /// </remarks>
        public async ValueTask<string?> SaveFileAsync(byte[]? data, string? fileName, string? owner = null)
        {
            if (data is null)
            {
                return null;
            }
            var filesPath = Path.Combine(_environment.WebRootPath, "files");
            try
            {
                if (!Directory.Exists(filesPath))
                {
                    Directory.CreateDirectory(filesPath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during 'files' directory creation.");
                return null;
            }

            var filePathName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + Path.GetExtension(fileName);
            string filePath;

            var hasOwner = !string.IsNullOrWhiteSpace(owner);
            string? ownerPathName = null;
            if (hasOwner)
            {
                ownerPathName = string.Join('_', owner!.Split(Path.GetInvalidFileNameChars()));
                var ownerPath = Path.Combine(filesPath, ownerPathName);
                try
                {
                    if (!Directory.Exists(ownerPath))
                    {
                        Directory.CreateDirectory(ownerPath);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during 'files' subfolder creation for owner {Owner}.", owner);
                    return null;
                }
                filePath = Path.Combine(ownerPath, filePathName);
            }
            else
            {
                filePath = Path.Combine(filesPath, filePathName);
            }

            try
            {
                var file = File.Create(filePath);
                await file.WriteAsync(data.AsMemory(0, data.Length)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception saving file at path {Path}.", filePath);
                return null;
            }

            return string.IsNullOrEmpty(owner)
                ? $"/{filePathName}"
                : $"/{ownerPathName}/{filePathName}";
        }

        /// <summary>
        /// Save the given file to a persistence store.
        /// </summary>
        /// <param name="data">A stream containing the file.</param>
        /// <param name="fileName">The file name.</param>
        /// <param name="owner">The owner of the file. May be <see langword="null" />.</param>
        /// <returns>
        /// The path of the stored file, if it was successfully saved; otherwise <see langword="null" />.
        /// </returns>
        /// <remarks>
        /// The returned path is the relative URL to the file.
        /// </remarks>
        public async ValueTask<string?> SaveFileAsync(Stream? data, string? fileName, string? owner = null)
        {
            if (data is null)
            {
                return null;
            }
            var filesPath = Path.Combine(_environment.WebRootPath, "files");
            try
            {
                if (!Directory.Exists(filesPath))
                {
                    Directory.CreateDirectory(filesPath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during 'files' directory creation.");
                return null;
            }

            var filePathName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + Path.GetExtension(fileName);
            string filePath;

            var hasOwner = !string.IsNullOrWhiteSpace(owner);
            string? ownerPathName = null;
            if (hasOwner)
            {
                ownerPathName = string.Join('_', owner!.Split(Path.GetInvalidFileNameChars()));
                var ownerPath = Path.Combine(filesPath, ownerPathName);
                try
                {
                    if (!Directory.Exists(ownerPath))
                    {
                        Directory.CreateDirectory(ownerPath);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during 'files' subfolder creation for owner {Owner}.", owner);
                    return null;
                }
                filePath = Path.Combine(ownerPath, filePathName);
            }
            else
            {
                filePath = Path.Combine(filesPath, filePathName);
            }

            try
            {
                var file = File.Create(filePath);
                await data.CopyToAsync(file).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception saving file at path {Path}.", filePath);
                return null;
            }

            return string.IsNullOrEmpty(owner)
                ? $"/{filePathName}"
                : $"/{ownerPathName}/{filePathName}";
        }

        /// <summary>
        /// Load the given file from a persistence store.
        /// </summary>
        /// <param name="path">The path to the file. A relative URL is expected.</param>
        /// <returns>
        /// A <see cref="Stream" /> containing the file; or <see langword="null" /> if no such file
        /// was found.
        /// </returns>
        public async ValueTask<Stream?> StreamFileAsync(string? path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return null;
            }
            try
            {
                var request = _httpContextAccessor.HttpContext?.Request;
                if (request is null)
                {
                    throw new Exception("Files cannot be loaded outside of an HTTP request context.");
                }
                var baseUrl = request.Scheme + "://" + request.Host + request.PathBase;
                using var wc = new WebClient { BaseAddress = baseUrl };
                return await wc.OpenReadTaskAsync(path).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception loading file at path {Path}", path);
                return null;
            }
        }
    }
}
