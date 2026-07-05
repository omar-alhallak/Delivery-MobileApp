using DeliveryApp.Domain.Entities.Merchants;
using DeliveryApp.Application.Interfaces.MerchantRepositoresInterfaces;

namespace DeliveryApp.Application.Features.Merchants.SetWorkingHours
{
    public sealed class SetMerchantWorkingHoursService
    {
        private readonly ISetMerchantWorkingHoursRepository _repository;

        public SetMerchantWorkingHoursService(ISetMerchantWorkingHoursRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<SetMerchantWorkingHoursResponse> ExecuteAsync(Guid currentUserId, SetMerchantWorkingHoursRequest request, CancellationToken ct = default)
        {
            var input = Validate(request);

            var currentUserID = UserID.From(currentUserId);

            var merchant = await _repository.GetMerchantByIdAsync(input.MerchantID, ct);

            if (merchant is null) throw new Exception("Merchant not found.");

            var relation = await _repository.GetMerchantUserAsync(input.MerchantID, currentUserID, ct);

            if (relation is null || !relation.IsActive)
                throw new Exception("Merchant access denied.");

            var existingHours = await _repository.GetWorkingHoursAsync(input.MerchantID, ct);

            foreach (var day in input.Days)
            {
                var existing = existingHours.FirstOrDefault(x => x.Day == day.Day);

                if (existing is null)
                {
                    var workingHour = new MerchantWorkingHour
                    (
                        id: MerchantWorkingHourID.New(),
                        merchantId: input.MerchantID,
                        dayOfWeek: day.Day,
                        openTime: day.OpenTime,
                        closeTime: day.CloseTime,
                        isClosed: day.IsClosed,
                        isOpenAllDay: day.IsOpenAllDay
                    );

                    await _repository.AddWorkingHourAsync(workingHour, ct);
                }
                else
                {
                    existing.UpdateWorkingHours
                    (
                        openTime: day.OpenTime,
                        closeTime: day.CloseTime,
                        isClosed: day.IsClosed,
                        isOpenAllDay: day.IsOpenAllDay
                    );
                }
            }

            await _repository.SaveChangesAsync(ct);

            return new SetMerchantWorkingHoursResponse
            {
                MerchantId = input.MerchantID.Value,
                DaysCount = input.Days.Count
            };
        }

        private static SetMerchantWorkingHoursValidatedInput Validate(SetMerchantWorkingHoursRequest request)
        {
            if (request is null)
                throw new Exception("Request is required.");

            if (request.MerchantId == Guid.Empty)
                throw new Exception("Merchant id is required.");

            if (request.Days is null || request.Days.Count == 0)
                throw new Exception("Working hours are required.");

            if (request.Days.Count > 7)
                throw new Exception("Working hours cannot be more than 7 days.");

            var duplicatedDay = request.Days.GroupBy(x => x.Day).FirstOrDefault(x => x.Count() > 1);

            if (duplicatedDay is not null)
                throw new Exception("Duplicate working day is not allowed.");

            var merchantID = MerchantID.From(request.MerchantId);

            var days = request.Days.Select(ValidateDay).ToList();

            return new SetMerchantWorkingHoursValidatedInput(merchantID, days);
        }

        private static WorkingHourValidatedItem ValidateDay(WorkingHourItem item)
        {
            if (!Enum.IsDefined(typeof(DayOfWeek), item.Day))
                throw new Exception("Invalid working day.");

            if (item.IsClosed && item.IsOpenAllDay)
                throw new Exception("Day cannot be closed and open all day at the same time.");

            if (item.IsClosed)
            {
                if (!string.IsNullOrWhiteSpace(item.OpenTime) || !string.IsNullOrWhiteSpace(item.CloseTime)) 
                    throw new Exception("Closed day cannot have working hours.");

                return new WorkingHourValidatedItem
                (
                   item.Day,
                   null,
                   null,
                   true,
                   false
                );
            }

            if (item.IsOpenAllDay)
            {
                if (!string.IsNullOrWhiteSpace(item.OpenTime) || !string.IsNullOrWhiteSpace(item.CloseTime)) 
                    throw new Exception("Open all day cannot have specific hours.");

                return new WorkingHourValidatedItem
                (
                    item.Day,
                    null,
                    null,
                    false,
                    true
                );
            }

            var openTime = ParseTime(item.OpenTime, nameof(item.OpenTime));
            var closeTime = ParseTime(item.CloseTime, nameof(item.CloseTime));

            if (openTime == closeTime)
                throw new Exception("Open time and close time cannot be equal.");

            return new WorkingHourValidatedItem
            (
                item.Day,
                openTime,
                closeTime,
                false,
                false
            );
        }

        private static TimeOnly ParseTime(string? value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new Exception($"{fieldName} is required.");

            value = value.Trim();

            if (!TimeOnly.TryParseExact(value, "HH:mm", out var time))
                throw new Exception($"{fieldName} must be HH:mm.");

            return time;
        }

        private sealed record SetMerchantWorkingHoursValidatedInput(MerchantID MerchantID,List<WorkingHourValidatedItem> Days);

        private sealed record WorkingHourValidatedItem
        (
            DayOfWeek Day,
            TimeOnly? OpenTime,
            TimeOnly? CloseTime,
            bool IsClosed,
            bool IsOpenAllDay
        );
    }
}