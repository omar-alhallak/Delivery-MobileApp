using DeliveryApp.Domain.Entities.DriverRequest;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Domain.EntitiesTemp
{
    public class DriverRequest
    {
        [Key]
        public Guid DriverRequestID { get; set; }

        [Required]
        public Guid UserID { get; set; }

        [Required]
        [MaxLength(150)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string FatherName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string NationalIdNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string PersonalPhotoUrl { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string NationalIdPhotoUrl { get; set; } = string.Empty;

        [Required]
        public int VehicleTypeID { get; set; }

        [Required]
        [MaxLength(500)]
        public string DrivingLicensePhotoUrl { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string DrivingLicenseNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string VehiclePlateNumber { get; set; } = string.Empty;

        [Required]
        public int Status { get; set; }  

        public Guid? ReviewedByAdminId { get; set; }

        public DateTimeOffset? ReviewedAt { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DriverRequest()
        {
            DriverRequestID = Guid.NewGuid();
            CreatedAt = DateTimeOffset.UtcNow;
        }
    }
}