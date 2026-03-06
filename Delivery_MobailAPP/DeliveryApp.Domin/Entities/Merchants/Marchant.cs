using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.DomainErrors.IdentityErrors;
using DeliveryApp.Domain.DomainExceptions;
using DeliveryApp.Domain.ValueObjects;

namespace DeliveryApp.Domain.Entities.Merchants
{
    public class Merchant
    {
        public StrongID<MerchantTag> MerchantID { get; private set; }

        public PublicCode? PublicID { get; private set; }
        public string MerchantType { get; private set; } = string.Empty;

        public string MerchantName { get; private set; } = string.Empty;

        public string? Description { get; private set; }

        public string? Phone { get; private set; }

        public string? LogoURL { get; private set; }

        public string? CoverImageURL { get; private set; }

        public decimal Lat { get; private set; }

        public decimal Lng { get; private set; }

        public bool IsProfileComplete { get; private set; }
        public bool IsActive { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; }

        private Merchant() { }

        public Merchant(StrongID<MerchantTag> merchantID, string merchantType, DateTimeOffset CreatedAtUtc)
        {
            MerchantID = merchantID;
            CreatedAt = DateTimeOffset.UtcNow;
            UpdateProfileStatus();
        }
        
       
        public void UpdateMerchant(string merchantType, string merchantName, string? description, string? phone, string? logoURL, string? coverImageURL, decimal lat, decimal lng)
        {
            MerchantType = Normalize(merchantType) ?? string.Empty; 
            MerchantName = Normalize(merchantName) ?? string.Empty;
            Description = Normalize(description);
            Phone = Normalize(phone);
            LogoURL = Normalize(logoURL);
            CoverImageURL = Normalize(coverImageURL);
            Lat = lat;
            Lng = lng;
            if (IsProfileComplete && !ProfileCompletionRules()) throw new DomainRuleViolationException
                    (UserErrors.CantRemoveRequiredFieldCode, UserErrors.CantRemoveRequiredFieldMessage);
            UpdateProfileStatus();
            FieldLimits();
        }
        private bool ProfileCompletionRules() => Phone is not null && MerchantName is not null;

        private void FieldLimits()
        {
            if (MerchantName.Length > 150)
                throw new DomainValidationException(ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, field: nameof(MerchantName));

            if (Phone is not null && Phone.Length > 16)
                throw new DomainValidationException(ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, field: nameof(Phone));

            if (Description is not null && Description.Length > 500)
                throw new DomainValidationException(ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, field: nameof(Description));

            if (LogoURL is not null && LogoURL.Length > 500)
                throw new DomainValidationException(ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, field: nameof(LogoURL));

            if (CoverImageURL is not null && CoverImageURL.Length > 500)
                throw new DomainValidationException(ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, field: nameof(CoverImageURL));

            if (MerchantType.Length > 50)
                throw new DomainValidationException(ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, field: nameof(MerchantType));

            if (Lat < -90 || Lat > 90)
                throw new DomainRuleViolationException(ValidationErrors.InvalidLatCode, ValidationErrors.InvalidLatMessage);

            if (Lng < -180 || Lng > 180)
                throw new DomainRuleViolationException(ValidationErrors.InvalidLngCode, ValidationErrors.InvalidLngMessage);

        }
        public void UpdateProfileStatus()
        {
            IsProfileComplete = !string.IsNullOrWhiteSpace(MerchantName) && Lat != 0;
            IsActive = IsProfileComplete;
        }

        public void ProfileComplete()
        {
            if (!ProfileCompletionRules()) throw new DomainRuleViolationException
                    (UserErrors.ProfileFieldNotCompleteCode, UserErrors.ProfileFieldNotCompleteMessage);
                    IsActive = true; 
        }
        public void ProfileNotComplete()
        {
            PreventModificationIfInactive();
            IsActive = false; 
        }
        public void Activate()
        {
            IsActive = true;
        }
        public void AssignPublicID(PublicCode publicId)
        {
            if (PublicID is not null)
                throw new DomainConflictException(MerchantErrors.PublicIdAlreadyAssignedCode,MerchantErrors.PublicIdAlreadyAssignedMessage); 

            PublicID = publicId;
        }
        private void PreventModificationIfInactive()
        {
            if (!IsActive)
                throw new DomainRuleViolationException(MerchantErrors.ProfileIncompleteCode,MerchantErrors.ProfileIncompleteMessage);
        }
        private static string? Normalize(string? s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim();
    }
}