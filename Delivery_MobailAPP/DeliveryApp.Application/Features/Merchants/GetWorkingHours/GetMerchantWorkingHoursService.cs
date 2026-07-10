using DeliveryApp.Application.Interfaces.MerchantRepositoresInterfaces;

namespace DeliveryApp.Application.Features.Merchants.GetWorkingHours
{
    public sealed class GetMerchantWorkingHoursService
    {
        private readonly IGetMerchantWorkingHoursRepository _repository;

        public GetMerchantWorkingHoursService(IGetMerchantWorkingHoursRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<GetMerchantWorkingHoursResponse> ExecuteAsync(Guid currentUserId, GetMerchantWorkingHoursRequest request, CancellationToken ct = default)
        {
            var input = Validate(request);

            var currentUserID = UserID.From(currentUserId);

            var merchant = await _repository.GetMerchantByIdAsync(input.MerchantID, ct);

            if (merchant is null) throw new Exception("Merchant not found.");

            var relation = await _repository.GetMerchantUserAsync(input.MerchantID, currentUserID, ct);

            if (relation is null || !relation.IsActive) throw new Exception("Merchant access denied.");

            var workingHours = await _repository.GetWorkingHoursAsync(input.MerchantID, ct);

            return new GetMerchantWorkingHoursResponse
            {
                MerchantId = input.MerchantID.Value,

                Days = workingHours.OrderBy(x => GetDayOrder(x.Day)).Select(x => new MerchantWorkingHourItemResponse
                    {
                        Day = x.Day,

                        OpenTime = x.OpenTime?.ToString("HH:mm"),

                        CloseTime = x.CloseTime?.ToString("HH:mm"),

                        IsClosed = x.IsClosed,

                        IsOpenAllDay = x.IsOpenAllDay
                    })
                    .ToList()
            };
        }

        private static GetMerchantWorkingHoursValidatedInput Validate(GetMerchantWorkingHoursRequest request)
        {
            if (request is null)
                throw new Exception("Request is required.");

            if (request.MerchantId == Guid.Empty)
                throw new Exception("Merchant id is required.");

            return new GetMerchantWorkingHoursValidatedInput(MerchantID.From(request.MerchantId));
        }

        private static int GetDayOrder(DayOfWeek day)
        {
            return day == DayOfWeek.Sunday ? 7 : (int)day;
        }

        private sealed record GetMerchantWorkingHoursValidatedInput(MerchantID MerchantID);
    }
}