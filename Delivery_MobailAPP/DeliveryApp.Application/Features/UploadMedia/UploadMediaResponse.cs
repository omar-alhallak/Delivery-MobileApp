using DeliveryApp.Domain.Enums.MediaEnums;

namespace DeliveryApp.Application.Features.UploadMedia
{
    public class UploadMediaResponse
    {
        public string Path { get; set; } = string.Empty;

        public string FileName { get; set; } = string.Empty;

        public string ContentType { get; set; } = string.Empty;

        public long Length { get; set; }

        public MediaType MediaType { get; set; }
    }
}