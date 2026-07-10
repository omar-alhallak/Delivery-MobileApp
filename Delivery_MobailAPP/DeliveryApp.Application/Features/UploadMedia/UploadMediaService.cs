using DeliveryApp.Domain.Enums.MediaEnums;
using DeliveryApp.Application.Features.UploadMedia;
using DeliveryApp.Application.Interfaces.MediaInterfaces;

namespace DeliveryApp.Application.Features.Media.UploadMedia
{
    public sealed class UploadMediaService
    {
        private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB

        private static readonly string[] AllowedContentTypes =
        [
            "image/jpeg",
            "image/png",
            "image/webp"
        ];

        private static readonly string[] AllowedExtensions =
        [
            ".jpg",
            ".jpeg",
            ".png",
            ".webp"
        ];

        private readonly IMediaStorage _mediaStorage;

        public UploadMediaService(IMediaStorage mediaStorage)
        {
            _mediaStorage = mediaStorage ?? throw new ArgumentNullException(nameof(mediaStorage));
        }

        public async Task<UploadMediaResponse> ExecuteAsync(UploadMediaRequest request, CancellationToken ct = default)
        {
            Validate(request);

            var result = await _mediaStorage.UploadAsync
            (
                content: request.Content,
                fileName: request.FileName,
                contentType: request.ContentType,
                length: request.Length,
                mediaType: request.MediaType,
                ct: ct
            );

            return new UploadMediaResponse
            {
                Path = result.Path,
                FileName = result.FileName,
                ContentType = result.ContentType,
                Length = result.Length,
                MediaType = request.MediaType
            };
        }

        private static void Validate(UploadMediaRequest request)
        {
            if (request is null)
                throw new Exception("Request is required.");

            if (request.Content is null || request.Content == Stream.Null)
                throw new Exception("Image file is required.");

            if (!request.Content.CanRead)
                throw new Exception("Image stream cannot be read.");

            if (request.Length <= 0)
                throw new Exception("Image file is empty.");

            if (request.Length > MaxFileSize)
                throw new Exception("Image size cannot exceed 5 MB.");

            if (string.IsNullOrWhiteSpace(request.FileName))
                throw new Exception("File name is required.");

            if (string.IsNullOrWhiteSpace(request.ContentType))
                throw new Exception("Content type is required.");

            if (!Enum.IsDefined(typeof(MediaType), request.MediaType))
                throw new Exception("Invalid media type.");

            var contentType = request.ContentType.Trim().ToLowerInvariant();

            if (!AllowedContentTypes.Contains(contentType))
                throw new Exception("Only JPG, PNG and WEBP images are allowed.");

            var extension = Path
                .GetExtension(request.FileName)
                .ToLowerInvariant();

            if (!AllowedExtensions.Contains(extension))
                throw new Exception("Invalid image extension.");

            ValidateContentTypeMatchesExtension(contentType, extension);
        }

        private static void ValidateContentTypeMatchesExtension(string contentType, string extension)
        {
            var isValid = contentType switch
            {
                "image/jpeg" => extension is ".jpg" or ".jpeg",
                "image/png" => extension == ".png",
                "image/webp" => extension == ".webp",
                _ => false
            };

            if (!isValid) throw new Exception("Image extension does not match its content type.");
        }
    }
}