using System.Text.RegularExpressions;
using DeliveryApp.Domain.Entities.Merchants;
using DeliveryApp.Domain.Enums.MerchantEnums;
using DeliveryApp.Domain.Enums.IdentityEnums;
using DeliveryApp.Application.Interfaces.MerchantRepositoresInterfaces;

namespace DeliveryApp.Application.Features.Merchants.AddMerchantStaff
{
    public sealed partial class AddMerchantStaffService
    {
        private readonly IAddMerchantStaffRepository _repository;

        public AddMerchantStaffService(IAddMerchantStaffRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<AddMerchantStaffResponse> ExecuteAsync(Guid currentUserId, AddMerchantStaffRequest request, CancellationToken ct = default)
        {
            var input = Validate(request);

            var currentUserID = UserID.From(currentUserId);

            var merchant = await _repository.GetMerchantByIdAsync(input.MerchantID, ct);

            if (merchant is null)
                throw new Exception("Merchant not found.");

            var currentRelation = await _repository.GetMerchantUserAsync(input.MerchantID, currentUserID, ct);

            if (currentRelation is null || !currentRelation.IsActive)
                throw new Exception("Merchant access denied.");

            if (currentRelation.Role != MerchantUserRole.Owner)
                throw new Exception("Only owner can add staff.");

            var staffUser = await _repository.GetUserByPhoneAsync(input.Phone, ct);

            if (staffUser is null)
                throw new Exception("User not found.");

            if (staffUser.AccountStatus == AccountStatus.Banned)
                throw new Exception("User is banned.");

            if (staffUser.ID == currentUserID)
                throw new Exception("Owner cannot add himself as staff.");

            var existingRelation = await _repository.GetMerchantUserAsync(input.MerchantID, staffUser.ID, ct);

            if (existingRelation is not null)
            {
                if (existingRelation.IsActive)
                    throw new Exception("User already has access to this merchant.");

                existingRelation.Activate();
                existingRelation.ChangeRole(MerchantUserRole.Staff);
            }
            else
            {
                var merchantUser = new MerchantUser
                (
                    merchantId: input.MerchantID,
                    userId: staffUser.ID,
                    role: MerchantUserRole.Staff,
                    createdAtUtc: DateTimeOffset.UtcNow
                );

                await _repository.AddMerchantUserAsync(merchantUser, ct);
            }

            await _repository.SaveChangesAsync(ct);

            return new AddMerchantStaffResponse
            {
                MerchantId = input.MerchantID.Value,
                StaffUserId = staffUser.ID.Value,
                IsActive = true
            };
        }

        private static AddMerchantStaffValidatedInput Validate(AddMerchantStaffRequest request)
        {
            if (request is null)
                throw new Exception("Request is required.");

            if (request.MerchantId == Guid.Empty)
                throw new Exception("Merchant id is required.");

            var phone = ValidatePhone(request.Phone);

            return new AddMerchantStaffValidatedInput(MerchantID.From(request.MerchantId), phone);
        }

        private static string ValidatePhone(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new Exception("Phone is required.");

            value = SpaceRegex().Replace(value.Trim(), " ");

            if (!PhoneRegex().IsMatch(value))
                throw new Exception("Invalid phone format.");

            return value;
        }

        [GeneratedRegex(@"^\+963 9\d{8}$")]
        private static partial Regex PhoneRegex();

        [GeneratedRegex(@"\s+")]
        private static partial Regex SpaceRegex();

        private sealed record AddMerchantStaffValidatedInput(MerchantID MerchantID,string Phone);
    }
}