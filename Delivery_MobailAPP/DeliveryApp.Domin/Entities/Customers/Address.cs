using System;
using DeliveryApp.Domain.Enums;
using DeliveryApp.Domain.ValueObjects;
using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.DomainExceptions;
using DeliveryApp.Domain.DomainErrors.AddressErrors;

namespace DeliveryApp.Domain.Entities.Customers
{
    public class Address
    {
        public AddressID ID { get; private set; }
        public UserID UserID { get; private set; }

        public string? Label { get; private set; }
        public AddressType? AddressType { get; private set; }

        public GeoPoint Location { get; private set; } = null!;

        public string? BuildingName { get; private set; }
        public string? Floor { get; private set; }
        public string? DoorInfo { get; private set; }
        public string? Notes { get; private set; }

        public bool IsDefault { get; private set; }
        public bool IsTemporary { get; private set; }
        public bool IsActive { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; }

        private Address() { }

        public Address(AddressID id, UserID UserId, decimal lat, decimal lng, DateTimeOffset CreatedAtUtc)
        {
            if (id.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(id));

            if (UserId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(UserId));

            if (CreatedAtUtc == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(CreatedAtUtc));

            ID = id;
            UserID = UserId;

            SetLocation(lat, lng);

            IsDefault = true;
            IsTemporary = true;
            IsActive = true;
            CreatedAt = CreatedAtUtc;
        }

        // -------------------------
        //         Behavior
        // -------------------------

        public void Complete(string label, AddressType addressType, string buildingName, string floor, string doorInfo, string? notes)
        {
            CheckTemporary();

            ValidateAddressType(addressType);

            SetLabel(label);
            SetBuildingName(buildingName);
            SetFloor(floor);
            SetDoorInfo(doorInfo);
            SetNotes(notes);

            AddressType = addressType;
            IsTemporary = false;
        }

        public void UpdateDetails(string label, AddressType addressType, string buildingName, string floor, string doorInfo, string? notes)
        {
            ValidateAddressType(addressType);

            SetLabel(label);
            SetBuildingName(buildingName);
            SetFloor(floor);
            SetDoorInfo(doorInfo);
            SetNotes(notes);

            AddressType = addressType;
        }

        public void Relocate(decimal lat, decimal lng)
        {
            CheckTemporary();
            SetLocation(lat, lng);
        }

        public void SetAsDefault()
        {
            if (IsDefault) return;

            IsDefault = true;
        }

        public void RemoveDefault()
        {
            if (!IsDefault) return;

            IsDefault = false;
        }

        public void Activate()
        {
            if (IsActive) return;

            IsActive = true;
        }

        public void Deactivate()
        {
            if (!IsActive) return;

            IsActive = false;
            IsDefault = false;
        }

        // -------------------------
        //        Validation
        // -------------------------

        private void CheckTemporary()
        {
            if (!IsTemporary) throw new DomainRuleViolationException
                    (AddressErrors.CompletedAddressLocationCantBeChangedCode, AddressErrors.CompletedAddressLocationCantBeChangedMessage);
        }

        private static void ValidateAddressType(AddressType addressType)
        {
            if (!Enum.IsDefined(typeof(AddressType), addressType)) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(addressType));
        }

        // -------------------------
        //         Setters
        // -------------------------

        private void SetLocation(decimal lat, decimal lng) => Location = GeoPoint.Create(lat, lng);

        private void SetLabel(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(Label));

            value = value.Trim();

            if (value.Length > 100) throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, nameof(Label));

            Label = value;
        }

        private void SetBuildingName(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(BuildingName));

            value = value.Trim();

            if (value.Length > 150) throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, nameof(BuildingName));

            BuildingName = value;
        }

        private void SetFloor(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(Floor));

            value = value.Trim();

            if (value.Length > 50) throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, nameof(Floor));

            Floor = value;
        }

        private void SetDoorInfo(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(DoorInfo));

            value = value.Trim();

            if (value.Length > 100) throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, nameof(DoorInfo));

            DoorInfo = value;
        }

        private void SetNotes(string? value)
        {
            value = NormalizeOptional(value);

            if (value is not null && value.Length > 500) throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, nameof(Notes));

            Notes = value;
        }

        private static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}