namespace DeliveryApp.Domain.Enums.IdentityEnums
{
    [Flags] // bitMask
    public enum UserRole : int // صلاحيات الحساب
    {
        None = 0,
        Customer = 1 << 0,  // 1
        Driver = 1 << 1,    // 2
        Admin = 1 << 2,     // 4
    }
}