using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Domain.Exceptions.NotFound
{
    public class ReviewNotFound(int reviewId) : NotFoundException_Base($"Review with Id {reviewId} Not Found") 
    {
    }
}
