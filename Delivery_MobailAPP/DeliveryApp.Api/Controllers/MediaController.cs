using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DeliveryApp.Domain.Enums.MediaEnums;
using DeliveryApp.Application.Features.UploadMedia;
using DeliveryApp.Application.Features.Media.UploadMedia;

namespace DeliveryApp.API.Controllers
{
    [ApiController]
    [Route("api/media")]
    public sealed class MediaController : ControllerBase
    {
        private readonly UploadMediaService _uploadMediaService;

        public MediaController(UploadMediaService uploadMediaService)
        {
            _uploadMediaService = uploadMediaService ?? throw new ArgumentNullException(nameof(uploadMediaService));
        }

        [Authorize]
        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<UploadMediaResponse>> Upload([FromForm] UploadMediaApiRequest request, CancellationToken ct)
        {
            if (request.File is null) throw new Exception("Image file is required.");

            await using var stream = request.File.OpenReadStream();

            var applicationRequest = new UploadMediaRequest
            {
                Content = stream,
                FileName = request.File.FileName,
                ContentType = request.File.ContentType,
                Length = request.File.Length,
                MediaType = request.MediaType
            };

            var response = await _uploadMediaService.ExecuteAsync(applicationRequest, ct);

            return Ok(response);
        }
    }

    public sealed class UploadMediaApiRequest
    {
        public IFormFile File { get; set; } = null!;

        public MediaType MediaType { get; set; }
    }
}