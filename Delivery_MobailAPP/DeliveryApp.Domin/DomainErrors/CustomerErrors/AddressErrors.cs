namespace DeliveryApp.Domain.DomainErrors.AddressErrors
{
    public static class AddressErrors
    {
        // إذا تم إضافة العنوان لا يمكن تغيير الموقع
        public const string CompletedAddressLocationCantBeChangedCode = "Address.Completed_Address_Location_Cant_Be_Changed";
        public const string CompletedAddressLocationCantBeChangedMessage = "Completed address location cant be changed.";
    }
}