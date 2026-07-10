using DeliveryApp.Domain.Enums.MediaEnums;

namespace DeliveryApp.Application.Features.UploadMedia
{
    public sealed class UploadMediaRequest
    {
        public Stream Content { get; set; } = Stream.Null;

        public string FileName { get; set; } = string.Empty;

        public string ContentType { get; set; } = string.Empty;

        public long Length { get; set; }

        public MediaType MediaType { get; set; }
    }
}