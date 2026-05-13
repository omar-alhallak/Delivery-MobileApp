namespace DeliveryApp.Infrastructure.Options
{                                  
    public sealed class JwtOptions // مسؤول عن إعدادات JWT
    {                              // بياخد الإعدادت من UserSecrets
        public string SecretKey { get; init; } = null!; // المفتاح السري الي بيستخدمه السيرفر لتوليد Access Token
        public int AccessTokenMinutes { get; init; } // مدة صلاحية Access Token
        public int RefreshTokenDays { get; init; } // مدة الجلسة (Session)
    }
}