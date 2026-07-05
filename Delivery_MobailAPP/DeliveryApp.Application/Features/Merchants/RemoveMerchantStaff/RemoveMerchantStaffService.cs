using DeliveryApp.Domain.Enums.MerchantEnums;
using DeliveryApp.Application.Interfaces.MerchantRepositoresInterfaces;

namespace DeliveryApp.Application.Features.Merchants.RemoveMerchantStaff
{
    public sealed class RemoveMerchantStaffService
    {
        private readonly IRemoveMerchantStaffRepository _repository;

        public RemoveMerchantStaffService(IRemoveMerchantStaffRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<RemoveMerchantStaffResponse> ExecuteAsync(Guid currentUserId, RemoveMerchantStaffRequest request, CancellationToken ct = default)
        {
            var input = Validate(request);

            var currentUserID = UserID.From(currentUserId);

            var merchant = await _repository.GetMerchantByIdAsync(input.MerchantID, ct);

            if (merchant is null) throw new Exception("Merchant not found.");

            var currentRelation = await _repository.GetMerchantUserAsync(input.MerchantID, currentUserID, ct);

            if (currentRelation is null || !currentRelation.IsActive)
                throw new Exception("Merchant access denied.");

            if (currentRelation.Role != MerchantUserRole.Owner)
                throw new Exception("Only owner can remove staff.");

            if (input.StaffUserID == currentUserID)
                throw new Exception("Owner cannot remove himself.");

            var staffRelation = await _repository.GetMerchantUserAsync(input.MerchantID, input.StaffUserID, ct);

            if (staffRelation is null)
                throw new Exception("Staff user not found in this merchant.");

            if (!staffRelation.IsActive)
                throw new Exception("Staff user is already inactive.");

            if (staffRelation.Role == MerchantUserRole.Owner)
                throw new Exception("Cannot remove another owner.");

            staffRelation.Deactivate();

            await _repository.SaveChangesAsync(ct);

            return new RemoveMerchantStaffResponse
            {
                MerchantId = input.MerchantID.Value,
                StaffUserId = input.StaffUserID.Value,
                IsActive = staffRelation.IsActive
            };
        }

        private static RemoveMerchantStaffValidatedInput Validate(RemoveMerchantStaffRequest request)
        {
            if (request is null)
                throw new Exception("Request is required.");

            if (request.MerchantId == Guid.Empty)
                throw new Exception("Merchant id is required.");

            if (request.StaffUserId == Guid.Empty)
                throw new Exception("Staff user id is required.");

            return new RemoveMerchantStaffValidatedInput(MerchantID.From(request.MerchantId), UserID.From(request.StaffUserId));
        }

        private sealed record RemoveMerchantStaffValidatedInput(MerchantID MerchantID,UserID StaffUserID);
    }
}