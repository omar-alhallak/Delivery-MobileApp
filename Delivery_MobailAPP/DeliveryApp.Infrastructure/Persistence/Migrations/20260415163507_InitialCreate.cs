using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeliveryApp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "moderation");

            migrationBuilder.EnsureSchema(
                name: "customers");

            migrationBuilder.EnsureSchema(
                name: "drivers");

            migrationBuilder.EnsureSchema(
                name: "merchants");

            migrationBuilder.EnsureSchema(
                name: "engagement");

            migrationBuilder.EnsureSchema(
                name: "identity");

            migrationBuilder.EnsureSchema(
                name: "shared");

            migrationBuilder.CreateSequence(
                name: "merchant_public_seq",
                schema: "shared");

            migrationBuilder.CreateSequence(
                name: "order_public_seq",
                schema: "shared");

            migrationBuilder.CreateSequence(
                name: "user_public_seq",
                schema: "shared");

            migrationBuilder.CreateTable(
                name: "Merchant",
                schema: "merchants",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PublicID = table.Column<string>(type: "varchar(32)", unicode: false, maxLength: 32, nullable: true),
                    MerchantType = table.Column<byte>(type: "tinyint", nullable: false),
                    MerchantName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Slug = table.Column<string>(type: "varchar(80)", unicode: false, maxLength: 80, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Phone = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    LogoUrl = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    CoverImageUrl = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    DefaultPreparationTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    AverageRating = table.Column<decimal>(type: "decimal(3,2)", precision: 3, scale: 2, nullable: false),
                    RatingsCount = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Merchant", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SystemCategory",
                schema: "merchants",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MerchantType = table.Column<byte>(type: "tinyint", nullable: false),
                    CategoryName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Slug = table.Column<string>(type: "varchar(80)", unicode: false, maxLength: 80, nullable: false),
                    ImageUrl = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemCategory", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "User",
                schema: "identity",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PublicID = table.Column<string>(type: "varchar(32)", unicode: false, maxLength: 32, nullable: true),
                    Email = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Phone = table.Column<string>(type: "varchar(16)", unicode: false, maxLength: 16, nullable: true),
                    PhotoUrl = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: true),
                    IsProfileComplete = table.Column<bool>(type: "bit", nullable: false),
                    RoleMask = table.Column<int>(type: "int", nullable: false),
                    AccountStatus = table.Column<byte>(type: "tinyint", nullable: false),
                    SuspendedUntilUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CustomerAverageRating = table.Column<decimal>(type: "decimal(3,2)", precision: 3, scale: 2, nullable: false),
                    CustomerRatingsCount = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastLoginAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "VehicleType",
                schema: "drivers",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VehicleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MaxDistanceKm = table.Column<double>(type: "float", nullable: false),
                    MaxMergeExtraKm = table.Column<double>(type: "float", nullable: false),
                    MaxOrdersToBatch = table.Column<int>(type: "int", nullable: false),
                    CommissionPercent = table.Column<int>(type: "int", nullable: false),
                    RequiresLicenseAndPlate = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleType", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Zone",
                schema: "moderation",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ZoneName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsServiceable = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zone", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "MerchantCategory",
                schema: "merchants",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MerchantID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ImageUrl = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MerchantCategory", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MerchantCategory_Merchant_MerchantID",
                        column: x => x.MerchantID,
                        principalSchema: "merchants",
                        principalTable: "Merchant",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MerchantWorkingHour",
                schema: "merchants",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MerchantID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Day = table.Column<byte>(type: "tinyint", nullable: false),
                    OpenTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    CloseTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    IsClosed = table.Column<bool>(type: "bit", nullable: false),
                    IsOpenAllDay = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MerchantWorkingHour", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MerchantWorkingHour_Merchant_MerchantID",
                        column: x => x.MerchantID,
                        principalSchema: "merchants",
                        principalTable: "Merchant",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MerchantSystemCategory",
                schema: "merchants",
                columns: table => new
                {
                    MerchantID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SystemCategoryID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MerchantSystemCategory", x => new { x.MerchantID, x.SystemCategoryID });
                    table.ForeignKey(
                        name: "FK_MerchantSystemCategory_Merchant_MerchantID",
                        column: x => x.MerchantID,
                        principalSchema: "merchants",
                        principalTable: "Merchant",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MerchantSystemCategory_SystemCategory_SystemCategoryID",
                        column: x => x.SystemCategoryID,
                        principalSchema: "merchants",
                        principalTable: "SystemCategory",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Address",
                schema: "customers",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Label = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AddressType = table.Column<byte>(type: "tinyint", nullable: true),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    BuildingName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Floor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DoorInfo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    IsTemporary = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Address_User_UserID",
                        column: x => x.UserID,
                        principalSchema: "identity",
                        principalTable: "User",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MerchantUser",
                schema: "merchants",
                columns: table => new
                {
                    MerchantID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Role = table.Column<byte>(type: "tinyint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MerchantUser", x => new { x.MerchantID, x.UserID });
                    table.ForeignKey(
                        name: "FK_MerchantUser_Merchant_MerchantID",
                        column: x => x.MerchantID,
                        principalSchema: "merchants",
                        principalTable: "Merchant",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MerchantUser_User_UserID",
                        column: x => x.UserID,
                        principalSchema: "identity",
                        principalTable: "User",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                schema: "engagement",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Body = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    RelatedEntityType = table.Column<byte>(type: "tinyint", nullable: true),
                    RelatedEntityID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ReadAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Notification_User_UserID",
                        column: x => x.UserID,
                        principalSchema: "identity",
                        principalTable: "User",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Order",
                schema: "customers",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PublicID = table.Column<string>(type: "varchar(32)", unicode: false, maxLength: 32, nullable: true),
                    OrderType = table.Column<byte>(type: "tinyint", nullable: false),
                    CustomerID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MerchantID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PickupLatitude = table.Column<double>(type: "float", nullable: false),
                    PickupLongitude = table.Column<double>(type: "float", nullable: false),
                    DropoffLatitude = table.Column<double>(type: "float", nullable: false),
                    DropoffLongitude = table.Column<double>(type: "float", nullable: false),
                    DistanceKmSnapshot = table.Column<double>(type: "float", nullable: false),
                    ItemsTotalSnapshot = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DeliveryFeeSnapshot = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TipAmountSnapshot = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalAmountSnapshot = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PaymentMethod = table.Column<byte>(type: "tinyint", nullable: false),
                    PaymentStatus = table.Column<byte>(type: "tinyint", nullable: false),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    RequiredDriversCount = table.Column<int>(type: "int", nullable: false),
                    IssueReason = table.Column<byte>(type: "tinyint", nullable: false),
                    IssueNote = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CancelledByType = table.Column<byte>(type: "tinyint", nullable: true),
                    CancelledById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CancelledAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ConfirmedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeliveredAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Order_Merchant_MerchantID",
                        column: x => x.MerchantID,
                        principalSchema: "merchants",
                        principalTable: "Merchant",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Order_User_CancelledById",
                        column: x => x.CancelledById,
                        principalSchema: "identity",
                        principalTable: "User",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Order_User_CustomerID",
                        column: x => x.CustomerID,
                        principalSchema: "identity",
                        principalTable: "User",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserIdentity",
                schema: "identity",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Provider = table.Column<byte>(type: "tinyint", nullable: false),
                    ProviderUserId = table.Column<string>(type: "varchar(128)", unicode: false, maxLength: 128, nullable: true),
                    PasswordHash = table.Column<string>(type: "varchar(300)", unicode: false, maxLength: 300, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserIdentity", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UserIdentity_User_UserID",
                        column: x => x.UserID,
                        principalSchema: "identity",
                        principalTable: "User",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserSession",
                schema: "identity",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientType = table.Column<byte>(type: "tinyint", nullable: false),
                    DeviceID = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastSeenAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    RevokedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RefreshTokenHash = table.Column<byte[]>(type: "varbinary(32)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSession", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UserSession_User_UserID",
                        column: x => x.UserID,
                        principalSchema: "identity",
                        principalTable: "User",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Driver",
                schema: "drivers",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VehicleTypeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    DisabledByAdminID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DisabledAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    ActiveOrdersCount = table.Column<int>(type: "int", nullable: false),
                    LastSeenAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CurrentLatitude = table.Column<double>(type: "float", nullable: true),
                    CurrentLongitude = table.Column<double>(type: "float", nullable: true),
                    LastLocationAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    AverageRating = table.Column<decimal>(type: "decimal(3,2)", precision: 3, scale: 2, nullable: false),
                    RatingsCount = table.Column<int>(type: "int", nullable: false),
                    ApprovedByAdminID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApprovedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Driver", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Driver_User_ApprovedByAdminID",
                        column: x => x.ApprovedByAdminID,
                        principalSchema: "identity",
                        principalTable: "User",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Driver_User_DisabledByAdminID",
                        column: x => x.DisabledByAdminID,
                        principalSchema: "identity",
                        principalTable: "User",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Driver_User_ID",
                        column: x => x.ID,
                        principalSchema: "identity",
                        principalTable: "User",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Driver_VehicleType_VehicleTypeID",
                        column: x => x.VehicleTypeID,
                        principalSchema: "drivers",
                        principalTable: "VehicleType",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DriverRequest",
                schema: "drivers",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VehicleTypeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    FatherName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    NationalIdNumber = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    PersonalPhotoUrl = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: false),
                    NationalIdPhotoUrl = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: false),
                    DrivingLicensePhotoUrl = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    DrivingLicenseNumber = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    VehiclePlateNumber = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: true),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    ReviewedByAdminID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReviewedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DriverRequest", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DriverRequest_User_ReviewedByAdminID",
                        column: x => x.ReviewedByAdminID,
                        principalSchema: "identity",
                        principalTable: "User",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DriverRequest_User_UserID",
                        column: x => x.UserID,
                        principalSchema: "identity",
                        principalTable: "User",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DriverRequest_VehicleType_VehicleTypeID",
                        column: x => x.VehicleTypeID,
                        principalSchema: "drivers",
                        principalTable: "VehicleType",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ZonePolygon",
                schema: "moderation",
                columns: table => new
                {
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    ZoneID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZonePolygon", x => new { x.ZoneID, x.SortOrder });
                    table.ForeignKey(
                        name: "FK_ZonePolygon_Zone_ZoneID",
                        column: x => x.ZoneID,
                        principalSchema: "moderation",
                        principalTable: "Zone",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Product",
                schema: "merchants",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MerchantCategoryID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ImageUrl = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    BasePrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Product_MerchantCategory_MerchantCategoryID",
                        column: x => x.MerchantCategoryID,
                        principalSchema: "merchants",
                        principalTable: "MerchantCategory",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AccountWarning",
                schema: "moderation",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityType = table.Column<byte>(type: "tinyint", nullable: false),
                    EntityID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RelatedOrderID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Reason = table.Column<byte>(type: "tinyint", nullable: false),
                    ReasonDetails = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Severity = table.Column<byte>(type: "tinyint", nullable: false),
                    Decision = table.Column<byte>(type: "tinyint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedByAdminID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DecidedByAdminID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DecidedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountWarning", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AccountWarning_Order_RelatedOrderID",
                        column: x => x.RelatedOrderID,
                        principalSchema: "customers",
                        principalTable: "Order",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountWarning_User_CreatedByAdminID",
                        column: x => x.CreatedByAdminID,
                        principalSchema: "identity",
                        principalTable: "User",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountWarning_User_DecidedByAdminID",
                        column: x => x.DecidedByAdminID,
                        principalSchema: "identity",
                        principalTable: "User",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Complaint",
                schema: "moderation",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetType = table.Column<byte>(type: "tinyint", nullable: false),
                    TargetID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Reason = table.Column<byte>(type: "tinyint", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    ReviewedByAdminID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AdminResponse = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ResolvedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Complaint", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Complaint_Order_OrderID",
                        column: x => x.OrderID,
                        principalSchema: "customers",
                        principalTable: "Order",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Complaint_User_CreatedByUserID",
                        column: x => x.CreatedByUserID,
                        principalSchema: "identity",
                        principalTable: "User",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Complaint_User_ReviewedByAdminID",
                        column: x => x.ReviewedByAdminID,
                        principalSchema: "identity",
                        principalTable: "User",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderAssignment",
                schema: "customers",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DriverID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    RemovedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RemoveReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderAssignment", x => x.ID);
                    table.ForeignKey(
                        name: "FK_OrderAssignment_Order_OrderID",
                        column: x => x.OrderID,
                        principalSchema: "customers",
                        principalTable: "Order",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderAssignment_User_DriverID",
                        column: x => x.DriverID,
                        principalSchema: "identity",
                        principalTable: "User",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderItem",
                schema: "customers",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductNameSnapshot = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    VariantNameSnapshot = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    UnitPriceSnapshot = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    LineTotalSnapshot = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CustomerNote = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItem", x => x.ID);
                    table.ForeignKey(
                        name: "FK_OrderItem_Order_OrderID",
                        column: x => x.OrderID,
                        principalSchema: "customers",
                        principalTable: "Order",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rating",
                schema: "engagement",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RaterUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetType = table.Column<byte>(type: "tinyint", nullable: false),
                    RatedEntityID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Stars = table.Column<byte>(type: "tinyint", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rating", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Rating_Order_OrderID",
                        column: x => x.OrderID,
                        principalSchema: "customers",
                        principalTable: "Order",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rating_User_RaterUserID",
                        column: x => x.RaterUserID,
                        principalSchema: "identity",
                        principalTable: "User",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DriverLocation",
                schema: "drivers",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DriverID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    RecordedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DriverLocation", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DriverLocation_Driver_DriverID",
                        column: x => x.DriverID,
                        principalSchema: "drivers",
                        principalTable: "Driver",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Variant",
                schema: "merchants",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VariantName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BasePrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Variant", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Variant_Product_ProductID",
                        column: x => x.ProductID,
                        principalSchema: "merchants",
                        principalTable: "Product",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountWarning_CreatedByAdminID",
                schema: "moderation",
                table: "AccountWarning",
                column: "CreatedByAdminID");

            migrationBuilder.CreateIndex(
                name: "IX_AccountWarning_DecidedByAdminID",
                schema: "moderation",
                table: "AccountWarning",
                column: "DecidedByAdminID");

            migrationBuilder.CreateIndex(
                name: "IX_AccountWarning_Decision",
                schema: "moderation",
                table: "AccountWarning",
                column: "Decision");

            migrationBuilder.CreateIndex(
                name: "IX_AccountWarning_EntityType_EntityID_CreatedAt",
                schema: "moderation",
                table: "AccountWarning",
                columns: new[] { "EntityType", "EntityID", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_AccountWarning_EntityType_EntityID_RelatedOrderID_Reason",
                schema: "moderation",
                table: "AccountWarning",
                columns: new[] { "EntityType", "EntityID", "RelatedOrderID", "Reason" },
                unique: true,
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_AccountWarning_RelatedOrderID",
                schema: "moderation",
                table: "AccountWarning",
                column: "RelatedOrderID",
                filter: "[RelatedOrderID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Address_UserID",
                schema: "customers",
                table: "Address",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Address_UserID_IsDefault",
                schema: "customers",
                table: "Address",
                columns: new[] { "UserID", "IsDefault" },
                unique: true,
                filter: "[IsDefault] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_Complaint_CreatedByUserID_OrderID_TargetType_TargetID_Reason",
                schema: "moderation",
                table: "Complaint",
                columns: new[] { "CreatedByUserID", "OrderID", "TargetType", "TargetID", "Reason" },
                unique: true,
                filter: "[Status] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_Complaint_OrderID",
                schema: "moderation",
                table: "Complaint",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_Complaint_ReviewedByAdminID",
                schema: "moderation",
                table: "Complaint",
                column: "ReviewedByAdminID");

            migrationBuilder.CreateIndex(
                name: "IX_Complaint_Status",
                schema: "moderation",
                table: "Complaint",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Complaint_TargetType_TargetID_CreatedAt",
                schema: "moderation",
                table: "Complaint",
                columns: new[] { "TargetType", "TargetID", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Driver_ApprovedByAdminID",
                schema: "drivers",
                table: "Driver",
                column: "ApprovedByAdminID");

            migrationBuilder.CreateIndex(
                name: "IX_Driver_DisabledByAdminID",
                schema: "drivers",
                table: "Driver",
                column: "DisabledByAdminID");

            migrationBuilder.CreateIndex(
                name: "IX_Driver_VehicleTypeID_IsEnabled_IsAvailable_LastSeenAt",
                schema: "drivers",
                table: "Driver",
                columns: new[] { "VehicleTypeID", "IsEnabled", "IsAvailable", "LastSeenAt" });

            migrationBuilder.CreateIndex(
                name: "IX_DriverLocation_DriverID_RecordedAt",
                schema: "drivers",
                table: "DriverLocation",
                columns: new[] { "DriverID", "RecordedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_DriverRequest_DrivingLicenseNumber",
                schema: "drivers",
                table: "DriverRequest",
                column: "DrivingLicenseNumber",
                unique: true,
                filter: "[DrivingLicenseNumber] IS NOT NULL AND [Status] IN (1, 2)");

            migrationBuilder.CreateIndex(
                name: "IX_DriverRequest_NationalIdNumber",
                schema: "drivers",
                table: "DriverRequest",
                column: "NationalIdNumber",
                unique: true,
                filter: "[Status] IN (1, 2)");

            migrationBuilder.CreateIndex(
                name: "IX_DriverRequest_ReviewedByAdminID",
                schema: "drivers",
                table: "DriverRequest",
                column: "ReviewedByAdminID");

            migrationBuilder.CreateIndex(
                name: "IX_DriverRequest_Status_CreatedAt",
                schema: "drivers",
                table: "DriverRequest",
                columns: new[] { "Status", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_DriverRequest_UserID",
                schema: "drivers",
                table: "DriverRequest",
                column: "UserID",
                unique: true,
                filter: "[Status] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_DriverRequest_VehiclePlateNumber",
                schema: "drivers",
                table: "DriverRequest",
                column: "VehiclePlateNumber",
                unique: true,
                filter: "[VehiclePlateNumber] IS NOT NULL AND [Status] IN (1, 2)");

            migrationBuilder.CreateIndex(
                name: "IX_DriverRequest_VehicleTypeID",
                schema: "drivers",
                table: "DriverRequest",
                column: "VehicleTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Merchant_MerchantType_IsActive",
                schema: "merchants",
                table: "Merchant",
                columns: new[] { "MerchantType", "IsActive" },
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_Merchant_PublicID",
                schema: "merchants",
                table: "Merchant",
                column: "PublicID",
                unique: true,
                filter: "[PublicID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Merchant_Slug",
                schema: "merchants",
                table: "Merchant",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MerchantCategory_MerchantID",
                schema: "merchants",
                table: "MerchantCategory",
                column: "MerchantID");

            migrationBuilder.CreateIndex(
                name: "IX_MerchantCategory_MerchantID_CategoryName",
                schema: "merchants",
                table: "MerchantCategory",
                columns: new[] { "MerchantID", "CategoryName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MerchantCategory_MerchantID_IsActive_SortOrder",
                schema: "merchants",
                table: "MerchantCategory",
                columns: new[] { "MerchantID", "IsActive", "SortOrder" },
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_MerchantCategory_MerchantID_SortOrder",
                schema: "merchants",
                table: "MerchantCategory",
                columns: new[] { "MerchantID", "SortOrder" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MerchantSystemCategory_SystemCategoryID",
                schema: "merchants",
                table: "MerchantSystemCategory",
                column: "SystemCategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_MerchantUser_UserID",
                schema: "merchants",
                table: "MerchantUser",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_MerchantWorkingHour_MerchantID",
                schema: "merchants",
                table: "MerchantWorkingHour",
                column: "MerchantID");

            migrationBuilder.CreateIndex(
                name: "IX_MerchantWorkingHour_MerchantID_Day",
                schema: "merchants",
                table: "MerchantWorkingHour",
                columns: new[] { "MerchantID", "Day" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notification_RelatedEntityType_RelatedEntityID",
                schema: "engagement",
                table: "Notification",
                columns: new[] { "RelatedEntityType", "RelatedEntityID" });

            migrationBuilder.CreateIndex(
                name: "IX_Notification_UserID_IsRead_CreatedAt",
                schema: "engagement",
                table: "Notification",
                columns: new[] { "UserID", "IsRead", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Order_CancelledById",
                schema: "customers",
                table: "Order",
                column: "CancelledById");

            migrationBuilder.CreateIndex(
                name: "IX_Order_CustomerID_CreatedAt",
                schema: "customers",
                table: "Order",
                columns: new[] { "CustomerID", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Order_MerchantID_Status",
                schema: "customers",
                table: "Order",
                columns: new[] { "MerchantID", "Status" },
                filter: "[MerchantID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Order_PublicID",
                schema: "customers",
                table: "Order",
                column: "PublicID",
                unique: true,
                filter: "[PublicID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Order_Status_CreatedAt",
                schema: "customers",
                table: "Order",
                columns: new[] { "Status", "CreatedAt" },
                filter: "[Status] = 2");

            migrationBuilder.CreateIndex(
                name: "IX_OrderAssignment_DriverID_Status",
                schema: "customers",
                table: "OrderAssignment",
                columns: new[] { "DriverID", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_OrderAssignment_OrderID_DriverID",
                schema: "customers",
                table: "OrderAssignment",
                columns: new[] { "OrderID", "DriverID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderAssignment_OrderID_Status",
                schema: "customers",
                table: "OrderAssignment",
                columns: new[] { "OrderID", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_OrderID",
                schema: "customers",
                table: "OrderItem",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_Product_MerchantCategoryID",
                schema: "merchants",
                table: "Product",
                column: "MerchantCategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_Product_MerchantCategoryID_IsActive_SortOrder",
                schema: "merchants",
                table: "Product",
                columns: new[] { "MerchantCategoryID", "IsActive", "SortOrder" },
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_Product_MerchantCategoryID_ProductName",
                schema: "merchants",
                table: "Product",
                columns: new[] { "MerchantCategoryID", "ProductName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Product_MerchantCategoryID_SortOrder",
                schema: "merchants",
                table: "Product",
                columns: new[] { "MerchantCategoryID", "SortOrder" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rating_OrderID_RaterUserID_TargetType",
                schema: "engagement",
                table: "Rating",
                columns: new[] { "OrderID", "RaterUserID", "TargetType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rating_RaterUserID",
                schema: "engagement",
                table: "Rating",
                column: "RaterUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Rating_TargetType_RatedEntityID_CreatedAt",
                schema: "engagement",
                table: "Rating",
                columns: new[] { "TargetType", "RatedEntityID", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_SystemCategory_MerchantType_CategoryName",
                schema: "merchants",
                table: "SystemCategory",
                columns: new[] { "MerchantType", "CategoryName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SystemCategory_MerchantType_IsActive_SortOrder",
                schema: "merchants",
                table: "SystemCategory",
                columns: new[] { "MerchantType", "IsActive", "SortOrder" },
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_SystemCategory_MerchantType_Slug",
                schema: "merchants",
                table: "SystemCategory",
                columns: new[] { "MerchantType", "Slug" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_Email",
                schema: "identity",
                table: "User",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_User_Phone",
                schema: "identity",
                table: "User",
                column: "Phone",
                unique: true,
                filter: "[Phone] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_User_PublicID",
                schema: "identity",
                table: "User",
                column: "PublicID",
                unique: true,
                filter: "[PublicID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserIdentity_Provider_ProviderUserId",
                schema: "identity",
                table: "UserIdentity",
                columns: new[] { "Provider", "ProviderUserId" },
                unique: true,
                filter: "[ProviderUserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserIdentity_UserID_Provider",
                schema: "identity",
                table: "UserIdentity",
                columns: new[] { "UserID", "Provider" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSession_ExpiresAt",
                schema: "identity",
                table: "UserSession",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_UserSession_RefreshTokenHash",
                schema: "identity",
                table: "UserSession",
                column: "RefreshTokenHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSession_UserID_ClientType",
                schema: "identity",
                table: "UserSession",
                columns: new[] { "UserID", "ClientType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Variant_ProductID",
                schema: "merchants",
                table: "Variant",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_Variant_ProductID_IsActive_SortOrder",
                schema: "merchants",
                table: "Variant",
                columns: new[] { "ProductID", "IsActive", "SortOrder" },
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_Variant_ProductID_SortOrder",
                schema: "merchants",
                table: "Variant",
                columns: new[] { "ProductID", "SortOrder" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Variant_ProductID_VariantName",
                schema: "merchants",
                table: "Variant",
                columns: new[] { "ProductID", "VariantName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VehicleType_VehicleName",
                schema: "drivers",
                table: "VehicleType",
                column: "VehicleName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Zone_IsActive_IsServiceable",
                schema: "moderation",
                table: "Zone",
                columns: new[] { "IsActive", "IsServiceable" },
                filter: "[IsActive] = 1 AND [IsServiceable] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_Zone_ZoneName",
                schema: "moderation",
                table: "Zone",
                column: "ZoneName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountWarning",
                schema: "moderation");

            migrationBuilder.DropTable(
                name: "Address",
                schema: "customers");

            migrationBuilder.DropTable(
                name: "Complaint",
                schema: "moderation");

            migrationBuilder.DropTable(
                name: "DriverLocation",
                schema: "drivers");

            migrationBuilder.DropTable(
                name: "DriverRequest",
                schema: "drivers");

            migrationBuilder.DropTable(
                name: "MerchantSystemCategory",
                schema: "merchants");

            migrationBuilder.DropTable(
                name: "MerchantUser",
                schema: "merchants");

            migrationBuilder.DropTable(
                name: "MerchantWorkingHour",
                schema: "merchants");

            migrationBuilder.DropTable(
                name: "Notification",
                schema: "engagement");

            migrationBuilder.DropTable(
                name: "OrderAssignment",
                schema: "customers");

            migrationBuilder.DropTable(
                name: "OrderItem",
                schema: "customers");

            migrationBuilder.DropTable(
                name: "Rating",
                schema: "engagement");

            migrationBuilder.DropTable(
                name: "UserIdentity",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "UserSession",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "Variant",
                schema: "merchants");

            migrationBuilder.DropTable(
                name: "ZonePolygon",
                schema: "moderation");

            migrationBuilder.DropTable(
                name: "Driver",
                schema: "drivers");

            migrationBuilder.DropTable(
                name: "SystemCategory",
                schema: "merchants");

            migrationBuilder.DropTable(
                name: "Order",
                schema: "customers");

            migrationBuilder.DropTable(
                name: "Product",
                schema: "merchants");

            migrationBuilder.DropTable(
                name: "Zone",
                schema: "moderation");

            migrationBuilder.DropTable(
                name: "VehicleType",
                schema: "drivers");

            migrationBuilder.DropTable(
                name: "User",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "MerchantCategory",
                schema: "merchants");

            migrationBuilder.DropTable(
                name: "Merchant",
                schema: "merchants");

            migrationBuilder.DropSequence(
                name: "merchant_public_seq",
                schema: "shared");

            migrationBuilder.DropSequence(
                name: "order_public_seq",
                schema: "shared");

            migrationBuilder.DropSequence(
                name: "user_public_seq",
                schema: "shared");
        }
    }
}