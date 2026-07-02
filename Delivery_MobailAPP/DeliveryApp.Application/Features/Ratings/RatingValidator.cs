using DeliveryApp.Domain.DomainErrors;
using DeliveryApp.Domain.DomainExceptions;
using DeliveryApp.Domain.Enums.EngagementEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Application.Features.Ratings
{
    public static class RatingValidator // فحص بيانات التقييم قبل إنشاء Entity
    {
        public static RatingStars ValidateStars(int stars)
        {
            if (stars < 1 || stars > 5) throw new DomainValidationException
                    (ValidationErrors.OutOfRangeCode, ValidationErrors.OutOfRangeMessage, nameof(stars));

            return (RatingStars)stars;
        }
    }
}
