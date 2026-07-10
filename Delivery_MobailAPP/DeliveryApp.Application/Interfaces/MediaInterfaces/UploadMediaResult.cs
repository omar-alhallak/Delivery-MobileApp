namespace DeliveryApp.Application.Interfaces.MediaInterfaces
{
    public sealed record UploadMediaResult(string Path, string FileName, string ContentType, long Length);
}