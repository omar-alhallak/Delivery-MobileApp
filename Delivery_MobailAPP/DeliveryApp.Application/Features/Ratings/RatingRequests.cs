using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Application.Features.Ratings
{
    public sealed class RatingRequest // الطلب الذي يرسله الزبون لإنشاء أو تعديل تقييم
    {
        public int Stars { get; init; }
        public string? Comment { get; init; }
    }
}
