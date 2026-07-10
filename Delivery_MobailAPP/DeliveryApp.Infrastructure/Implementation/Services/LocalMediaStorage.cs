using Microsoft.AspNetCore.Hosting;
using DeliveryApp.Domain.Enums.MediaEnums;
using DeliveryApp.Application.Interfaces.MediaInterfaces;

namespace DeliveryApp.Infrastructure.Implementation.Services
{
    public sealed class LocalMediaStorage : IMediaStorage
    {
        private readonly IWebHostEnvironment _environment;

        public LocalMediaStorage(IWebHostEnvironment environment)
        {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        public async Task<UploadMediaResult> UploadAsync(Stream content, string fileName, string contentType, long length, MediaType mediaType, CancellationToken ct = default)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();

            await ValidateFileSignatureAsync(content, extension, ct);

            var folderName = GetFolderName(mediaType);

            var webRootPath = GetWebRootPath();

            var directoryPath = Path.Combine(webRootPath, "uploads", folderName);

            Directory.CreateDirectory(directoryPath);

            var storedFileName = $"{Guid.NewGuid():N}{extension}";

            var physicalPath = Path.Combine(directoryPath, storedFileName);

            content.Position = 0;

            await using var outputStream = new FileStream
            (
                physicalPath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 81920,
                useAsync: true
            );

            await content.CopyToAsync(outputStream, ct);

            var relativePath = $"/uploads/{folderName}/{storedFileName}";

            return new UploadMediaResult
            (
                Path: relativePath,
                FileName: storedFileName,
                ContentType: contentType,
                Length: length
            );
        }

        public Task<bool> ExistsAsync(string path, MediaType mediaType, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(path))
                return Task.FromResult(false);

            var folderName = GetFolderName(mediaType);

            var expectedPrefix = $"/uploads/{folderName}/";

            if (!path.StartsWith(expectedPrefix, StringComparison.OrdinalIgnoreCase)) 
            {
                return Task.FromResult(false);
            }

            var fileName = Path.GetFileName(path);

            if (string.IsNullOrWhiteSpace(fileName))
                return Task.FromResult(false);

            var physicalPath = Path.Combine
            (
                GetWebRootPath(),
                "uploads",
                folderName,
                fileName
            );

            return Task.FromResult(File.Exists(physicalPath));
        }

        public Task DeleteAsync(string path, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(path))
                return Task.CompletedTask;

            if (!path.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase)) 
            {
                throw new Exception("Invalid media path.");
            }

            var relativePath = path
                .TrimStart('/')
                .Replace('/', Path.DirectorySeparatorChar);

            var webRootPath = Path.GetFullPath(GetWebRootPath());

            var physicalPath = Path.GetFullPath(Path.Combine(webRootPath, relativePath));

            if (!physicalPath.StartsWith(webRootPath + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)) 
            {
                throw new Exception("Invalid media path.");
            }

            if (File.Exists(physicalPath)) File.Delete(physicalPath);

            return Task.CompletedTask;
        }

        private string GetWebRootPath()
        {
            if (!string.IsNullOrWhiteSpace(_environment.WebRootPath)) 
            {
                return _environment.WebRootPath;
            }

            var webRootPath = Path.Combine(_environment.ContentRootPath, "wwwroot");

            Directory.CreateDirectory(webRootPath);

            return webRootPath;
        }

        private static string GetFolderName(MediaType mediaType)
        {
            return mediaType switch
            {
                MediaType.MerchantLogo => "merchant-logos",

                MediaType.MerchantCover => "merchant-covers",

                MediaType.ProductImage => "product-images",

                MediaType.CategoryImage => "category-images",

                MediaType.UserProfileImage => "user-profile-images",

                _ => throw new Exception("Invalid media type.")
            };
        }

        private static async Task ValidateFileSignatureAsync(Stream content, string extension, CancellationToken ct)
        {
            if (!content.CanSeek)
                throw new Exception("Image stream must support seeking.");

            var originalPosition = content.Position;

            content.Position = 0;

            var header = new byte[12];

            var bytesRead = await content.ReadAsync(header.AsMemory(0, header.Length), ct);

            content.Position = originalPosition;

            var isValid = extension switch
            {
                ".jpg" or ".jpeg" => IsJpeg(header, bytesRead),

                ".png" => IsPng(header, bytesRead),

                ".webp" => IsWebP(header, bytesRead),

                _ => false
            };

            if (!isValid)
                throw new Exception("File content is not a valid image.");
        }

        private static bool IsJpeg(byte[] header, int bytesRead)
        {
            return bytesRead >= 3
                   && header[0] == 0xFF
                   && header[1] == 0xD8
                   && header[2] == 0xFF;
        }

        private static bool IsPng(byte[] header, int bytesRead)
        {
            return bytesRead >= 8
                   && header[0] == 0x89
                   && header[1] == 0x50
                   && header[2] == 0x4E
                   && header[3] == 0x47
                   && header[4] == 0x0D
                   && header[5] == 0x0A
                   && header[6] == 0x1A
                   && header[7] == 0x0A;
        }

        private static bool IsWebP(byte[] header, int bytesRead)
        {
            return bytesRead >= 12
                   && header[0] == 0x52
                   && header[1] == 0x49
                   && header[2] == 0x46
                   && header[3] == 0x46
                   && header[8] == 0x57
                   && header[9] == 0x45
                   && header[10] == 0x42
                   && header[11] == 0x50;
        }
    }
}