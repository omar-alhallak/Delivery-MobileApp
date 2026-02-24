using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeliveryApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountWarnings",
                columns: table => new
                {
                    WarningID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityType = table.Column<int>(type: "int", nullable: false),
                    EntityID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RelatedOrderID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Severity = table.Column<int>(type: "int", nullable: false),
                    CreatedByAdminId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Decision = table.Column<int>(type: "int", nullable: true),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DecidedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountWarnings", x => x.WarningID);
                });

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    AddressID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Label = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AddressType = table.Column<int>(type: "int", nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    BuildingName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Floor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DoorInfo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.AddressID);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategorieID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MerchantType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CategoriesName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ImageURL = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategorieID);
                });

            migrationBuilder.CreateTable(
                name: "Complaints",
                columns: table => new
                {
                    ComplaintID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetType = table.Column<int>(type: "int", nullable: false),
                    TargetID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ReviewedByAdminId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AdminResponse = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ResolvedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Complaints", x => x.ComplaintID);
                });

            migrationBuilder.CreateTable(
                name: "DriverLocations",
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
                    table.PrimaryKey("PK_DriverLocations", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "DriverRequests",
                columns: table => new
                {
                    DriverRequestID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    FatherName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    NationalIdNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PersonalPhotoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    NationalIdPhotoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    VehicleTypeID = table.Column<int>(type: "int", nullable: false),
                    DrivingLicensePhotoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DrivingLicenseNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    VehiclePlateNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ReviewedByAdminId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReviewedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DriverRequests", x => x.DriverRequestID);
                });

            migrationBuilder.CreateTable(
                name: "Drivers",
                columns: table => new
                {
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VehicleTypeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    IsOnline = table.Column<bool>(type: "bit", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    CurrentLat = table.Column<decimal>(type: "decimal(9,6)", nullable: true),
                    CurrentLng = table.Column<decimal>(type: "decimal(9,6)", nullable: true),
                    ActiveOrdersCount = table.Column<int>(type: "int", nullable: false),
                    ApprovedByAdminId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApprovedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Drivers", x => x.UserID);
                });

            migrationBuilder.CreateTable(
                name: "Merchants",
                columns: table => new
                {
                    MerchantID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MerchantType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MerchantName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    LogoURL = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CoverImageURL = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Lat = table.Column<decimal>(type: "decimal(9,6)", nullable: false),
                    Lng = table.Column<decimal>(type: "decimal(9,6)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Merchants", x => x.MerchantID);
                });

            migrationBuilder.CreateTable(
                name: "MerchantUsers",
                columns: table => new
                {
                    MerchantID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MerchantUsers", x => new { x.MerchantID, x.UserID });
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    NotificationID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Body = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    RelatedEntityType = table.Column<int>(type: "int", nullable: true),
                    RelatedEntityID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.NotificationID);
                });

            migrationBuilder.CreateTable(
                name: "OrderDrivers",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DriverID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Status = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    RemovedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RemoveReason = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDrivers", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    OrderItemID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductNameSnapshot = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    VariantNameSnapshot = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UnitPriceSnapshot = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    LineTotalSnapshot = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CustomerNotes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.OrderItemID);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    OrderID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderType = table.Column<int>(type: "int", nullable: false),
                    CustomerID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MerchantID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PickupLat = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PickupLng = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DropoffLat = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DropoffLng = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DistanceKmSnapshot = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ItemsTotalSnapshot = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DeliveryFeeSnapshot = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TipAmountSnapshot = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalAmountSnapshot = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentMethod = table.Column<int>(type: "int", nullable: false),
                    PaymentStatus = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RequiredDriversCount = table.Column<short>(type: "smallint", nullable: false),
                    FailureReason = table.Column<int>(type: "int", nullable: true),
                    CancelledByType = table.Column<int>(type: "int", nullable: true),
                    CancelledById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CancelledAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ConfirmedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeliveredAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.OrderID);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MerchantID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ImageURL = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductID);
                });

            migrationBuilder.CreateTable(
                name: "Ratings",
                columns: table => new
                {
                    RatingID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RaterUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RatedEntityType = table.Column<int>(type: "int", nullable: false),
                    RatedEntityID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Stars = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ratings", x => x.RatingID);
                });

            migrationBuilder.CreateTable(
                name: "UserIdentities",
                columns: table => new
                {
                    IdentiteID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Provider = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProviderUserID = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserIdentities", x => x.IdentiteID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    PhotoURL = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RoleMask = table.Column<int>(type: "int", nullable: false),
                    AccountStatus = table.Column<int>(type: "int", nullable: false),
                    IsProfileComplete = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastLoginAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserID);
                });

            migrationBuilder.CreateTable(
                name: "Variants",
                columns: table => new
                {
                    VariantID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VariantName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    BasePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Variants", x => x.VariantID);
                });

            migrationBuilder.CreateTable(
                name: "VehicleTypes",
                columns: table => new
                {
                    VehicleID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VehicleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MaxDistanceKm = table.Column<double>(type: "float", nullable: false),
                    MaxMergeExtraKm = table.Column<double>(type: "float", nullable: false),
                    CommissionPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleTypes", x => x.VehicleID);
                });

            migrationBuilder.CreateTable(
                name: "ZonePolygons",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ZoneID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Latitude = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Longitude = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZonePolygons", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Zones",
                columns: table => new
                {
                    ZoneID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ZoneName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsServiceable = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zones", x => x.ZoneID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountWarnings");

            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Complaints");

            migrationBuilder.DropTable(
                name: "DriverLocations");

            migrationBuilder.DropTable(
                name: "DriverRequests");

            migrationBuilder.DropTable(
                name: "Drivers");

            migrationBuilder.DropTable(
                name: "Merchants");

            migrationBuilder.DropTable(
                name: "MerchantUsers");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "OrderDrivers");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Ratings");

            migrationBuilder.DropTable(
                name: "UserIdentities");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Variants");

            migrationBuilder.DropTable(
                name: "VehicleTypes");

            migrationBuilder.DropTable(
                name: "ZonePolygons");

            migrationBuilder.DropTable(
                name: "Zones");
        }
    }
}
