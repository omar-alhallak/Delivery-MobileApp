using DeliveryApp.Domain.Enums.MerchantEnums;
using DeliveryApp.Application.Interfaces.MerchantRepositoresInterfaces;

namespace DeliveryApp.Application.Features.Merchants.GetMerchantStaff
{
    public sealed class GetMerchantStaffService
    {
        private readonly IGetMerchantStaffRepository _repository;

        public GetMerchantStaffService(IGetMerchantStaffRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<GetMerchantStaffResponse> ExecuteAsync(Guid currentUserId, GetMerchantStaffRequest request, CancellationToken ct = default)
        {
            var input = Validate(request);

            var currentUserID = UserID.From(currentUserId);

            var merchant = await _repository.GetMerchantByIdAsync(input.MerchantID, ct);

            if (merchant is null) throw new Exception("Merchant not found.");

            var currentRelation = await _repository.GetMerchantUserAsync(input.MerchantID, currentUserID, ct);

            if (currentRelation is null || !currentRelation.IsActive)
                throw new Exception("Merchant access denied.");

            if (currentRelation.Role != MerchantUserRole.Owner)
                throw new Exception("Only owner can view merchant staff.");

            var staff = await _repository.GetMerchantStaffAsync(input.MerchantID, ct);

            return new GetMerchantStaffResponse
            {
                MerchantId = input.MerchantID.Value,

                Staff = staff.Select(x => new MerchantStaffItemResponse
                {
                    UserId = x.UserID.Value,
                    FullName = x.FullName,
                    Phone = x.Phone,
                    PhotoUrl = x.PhotoUrl,
                    Role = x.Role,
                    IsActive = x.IsActive,
                    CreatedAt = x.CreatedAt
                }).ToList()
            };
        }

        private static GetMerchantStaffValidatedInput Validate(GetMerchantStaffRequest request)
        {
            if (request is null)
                throw new Exception("Request is required.");

            if (request.MerchantId == Guid.Empty)
                throw new Exception("Merchant id is required.");

            return new GetMerchantStaffValidatedInput(MerchantID.From(request.MerchantId));
        }

        private sealed record GetMerchantStaffValidatedInput(MerchantID MerchantID);
    }
}