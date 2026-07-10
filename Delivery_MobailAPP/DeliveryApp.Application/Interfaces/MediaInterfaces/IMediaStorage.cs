using DeliveryApp.Domain.Enums.MediaEnums;

namespace DeliveryApp.Application.Interfaces.MediaInterfaces
{
    public interface IMediaStorage
    {
        Task<UploadMediaResult> UploadAsync(Stream content, string fileName, string contentType, long length, MediaType mediaType, CancellationToken ct = default);

        Task<bool> ExistsAsync(string path, MediaType mediaType, CancellationToken ct = default);

        Task DeleteAsync(string path, CancellationToken ct = default);
    }
}