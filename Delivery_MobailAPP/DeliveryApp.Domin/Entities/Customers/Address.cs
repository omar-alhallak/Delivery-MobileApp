using DeliveryApp.Domain.ValueObjects;
using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.DomainExceptions;
using DeliveryApp.Domain.Enums.CustomerEnums;
using DeliveryApp.Domain.DomainErrors.AddressErrors;

namespace DeliveryApp.Domain.Entities.Customers
{
    public class Address // يمثل عنوان المستخدم
    {
        // -------------------------
        //            Key
        // -------------------------

        public AddressID ID { get; private set; } // pk معرف العنوان
        public UserID UserID { get; private set; } // صاحب العنوان

        // -------------------------
        //       Basic Info
        // -------------------------

        public string? Label { get; private set; } //  اسم مختصر للعنوان من إضافة الزبون
        public AddressType? AddressType { get; private set; } // نوع العنوان

        // -------------------------
        //         Location
        // -------------------------

        public GeoPoint Location { get; private set; } = null!; // موقع العنوان

        // -------------------------
        //         Details
        // -------------------------

        public string? BuildingName { get; private set; } // اسم أو رقم البناء
        public string? Floor { get; private set; } // الطابق
        public string? DoorInfo { get; private set; } // معلومات الباب أو الشقة
        public string? Notes { get; private set; } // ملاحظات إضافية

        // -------------------------
        //          Flags
        // -------------------------

        public bool IsDefault { get; private set; } // هل هذا هو العنوان الافتراضي
        public bool IsTemporary { get; private set; } // هل العنوان مؤقت ولم تكتمل بياناته بعد
        public bool IsActive { get; private set; } // هل العنوان نشط

        // -------------------------
        //           Dates
        // -------------------------

        public DateTimeOffset CreatedAt { get; private set; } // وقت إنشاء العنوان

        private Address() { }

        public Address(AddressID id, UserID userId, double lat, double lng, DateTimeOffset createdAtUtc)
        {
            if (id.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(id));

            if (userId.IsEmpty) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(userId));

            if (createdAtUtc == default) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(createdAtUtc));

            ID = id;
            UserID = userId;

            SetLocation(lat, lng);

            IsDefault = false;
            IsTemporary = true;
            IsActive = true;
            CreatedAt = createdAtUtc;
        }

        // -------------------------
        //         Behavior
        // -------------------------

        public void Complete(string label, AddressType addressType, string buildingName, string floor, string doorInfo, string? notes) // إكمال بيانات العنوان المؤقت وتحويله إلى عنوان كامل
        {
            EnsureTemporary();

            ValidateAddressType(addressType);

            SetLabel(label);
            SetBuildingName(buildingName);
            SetFloor(floor);
            SetDoorInfo(doorInfo);
            SetNotes(notes);

            AddressType = addressType;
            IsTemporary = false;
        }

        public void UpdateDetails(string label, AddressType addressType, string buildingName, string floor, string doorInfo, string? notes) // تعديل تفاصيل العنوان بعد إنشائه
        {
            ValidateAddressType(addressType);

            SetLabel(label);
            SetBuildingName(buildingName);
            SetFloor(floor);
            SetDoorInfo(doorInfo);
            SetNotes(notes);

            AddressType = addressType;
        }

        public void Relocate(double lat, double lng) // تغيير موقع العنوان فقط إذا كان ما يزال مؤقت
        {
            EnsureTemporary();
            SetLocation(lat, lng);
        }

        public void SetAsDefault() // تعيين هذا العنوان كعنوان افتراضي
        {
            if (IsDefault) return;

            IsDefault = true;
        }

        public void RemoveDefault() // إزالة الصفة الافتراضية عن هذا العنوان
        {
            if (!IsDefault) return;

            IsDefault = false;
        }

        public void Activate() // تفعيل العنوان
        {
            if (IsActive) return;

            IsActive = true;
        }

        public void Deactivate() // تعطيل العنوان وإزالة صفته الافتراضية
        {
            if (!IsActive) return;

            IsActive = false;
            IsDefault = false;
        }

        // -------------------------
        //        Validation
        // -------------------------

        private void EnsureTemporary() // التأكد أن العنوان ما زال مؤقتاً
        {
            if (!IsTemporary) throw new DomainRuleViolationException
                    (AddressErrors.CompletedAddressLocationCantBeChangedCode, AddressErrors.CompletedAddressLocationCantBeChangedMessage);
        }

        private static void ValidateAddressType(AddressType addressType) // التحقق من صحة نوع العنوان
        {
            if (!Enum.IsDefined(typeof(AddressType), addressType)) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(addressType));
        }

        // -------------------------
        //         Setters
        // -------------------------

        private void SetLocation(double lat, double lng) => Location = GeoPoint.Create(lat, lng); // إدخال موقع العنوان

        private void SetLabel(string value) // إدخال اسم مختصر للعنوان
        {
            if (string.IsNullOrWhiteSpace(value)) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(Label));

            value = value.Trim();

            if (value.Length > 100) throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, nameof(Label));

            Label = value;
        }

        private void SetBuildingName(string value) // إدخال اسم أو رقم البناء
        {
            if (string.IsNullOrWhiteSpace(value)) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(BuildingName));

            value = value.Trim();

            if (value.Length > 150) throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, nameof(BuildingName));

            BuildingName = value;
        }

        private void SetFloor(string value) // إدخال الطابق
        {
            if (string.IsNullOrWhiteSpace(value)) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(Floor));

            value = value.Trim();

            if (value.Length > 50) throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, nameof(Floor));

            Floor = value;
        }

        private void SetDoorInfo(string value) // إدخال معلومات الباب أو الشقة
        {
            if (string.IsNullOrWhiteSpace(value)) throw new DomainValidationException
                    (ValidationErrors.RequiredCode, ValidationErrors.RequiredMessage, nameof(DoorInfo));

            value = value.Trim();

            if (value.Length > 100) throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, nameof(DoorInfo));

            DoorInfo = value;
        }

        private void SetNotes(string? value) // إدخال الملاحظات
        {
            value = NormalizeOptional(value);

            if (value is not null && value.Length > 500) throw new DomainValidationException
                    (ValidationErrors.TooLongCode, ValidationErrors.TooLongMessage, nameof(Notes));

            Notes = value;
        }

        private static string? NormalizeOptional(string? value) // تنظيف النصوص
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim(); 
        }
    }
}