using AutoMapper.Configuration.Conventions;
using Makanak.Domain.Contracts.Specifications;
using Makanak.Domain.Models.PropertyEntities;
using Makanak.Shared.Common;
using Makanak.Shared.Common.Params.Property_Params;
using Makanak.Shared.EnumsHelper.Booking;
using Makanak.Shared.EnumsHelper.Property;


namespace Makanak.Services.Specifications.Property_Spec
{
    public class PropertySpecifications : BaseSpecifications<Property, int>
    {
        public PropertySpecifications(string ownerId) 
            : base(p => p.OwnerId == ownerId)
        {
            AddInclude(p => p.Owner);
            AddOrderByDesc(p => p.CreatedAt);           
        }
   
        public PropertySpecifications(int propId) 
            : base(p => p.Id== propId)
        {
            AddInclude(p => p.Amenities);
            AddInclude(p => p.PropertyImages);
            AddInclude(p => p.Governorate);
            AddInclude(p => p.Owner);
        }
        
        // count properties of owner
        public PropertySpecifications(string ownerId , bool Count) 
            : base(p => p.OwnerId == ownerId)
        {
        }

        // used by get properties of owner by owner id 
        public PropertySpecifications(string ownerId, PropertyParams propertyParams, bool isCount)
        : base(x => x.OwnerId == ownerId) 
        {
            if (!isCount)
            {
                ApplyPagenation(propertyParams.PageSize, propertyParams.PageIndex);
                AddOrderByDesc(x => x.CreatedAt);
            }
        }

        public PropertySpecifications(PropertyParams propertyParams, bool isCount = false)
      : base(p =>
          // 1. فلاتر أساسية ثابتة
          (p.PropertyStatus == PropertyStatus.Accepted) &&
          (!p.IsDeleted) &&
          (p.IsAvailable) &&

          // 2. البحث بالكلام (Title or Area)
          (string.IsNullOrEmpty(propertyParams.Search) ||
           p.Title.ToLower().Contains(propertyParams.Search) ||
           p.AreaName.ToLower().Contains(propertyParams.Search)) &&

          // 3. فلاتر الموقع والنوع
          (!propertyParams.GovernorateId.HasValue || p.GovernorateId == propertyParams.GovernorateId) &&
          (!propertyParams.Type.HasValue || p.PropertyType == propertyParams.Type) &&

          // 4. السعر (صارم - Strict)
          (!propertyParams.MinPrice.HasValue || p.PricePerNight >= propertyParams.MinPrice) &&
          (!propertyParams.MaxPrice.HasValue || p.PricePerNight <= propertyParams.MaxPrice) &&

          // 5. السعة (عدد الأفراد والغرف)
          (!propertyParams.MinBedrooms.HasValue || p.Bedrooms >= propertyParams.MinBedrooms) &&
          (!propertyParams.MinMaxGuests.HasValue || p.MaxGuests >= propertyParams.MinMaxGuests) &&

          // 6. المرافق (Amenities) - صارم
          // هنا بنقول: لو اليوزر باعت AmenitiesIds، هات العقارات اللي فيها *على الأقل واحد* من دول
          // (أو ممكن تخليها All لو عاوز لازم يكون فيه كل اللي اختاره)
          (propertyParams.AmenityIds == null || propertyParams.AmenityIds.Count == 0 ||
           p.Amenities.Any(a => propertyParams.AmenityIds.Contains(a.Id))) &&

          // 7. تواريخ الحجز (Availability)
          (!propertyParams.CheckInDate.HasValue || !propertyParams.CheckOutDate.HasValue ||
           !p.Bookings.Any(b =>
               (b.Status == BookingStatus.PendingPayment || b.Status == BookingStatus.PaymentReceived || b.Status == BookingStatus.CheckedIn) &&
               (b.CheckInDate < propertyParams.CheckOutDate && b.CheckOutDate > propertyParams.CheckInDate)
           ))
      )
        {
            if (!isCount)
            {
                AddInclude(p => p.PropertyImages);
                AddInclude(p => p.Governorate);
                AddInclude(p => p.Amenities);

                // الترتيب (Sorting)
                // بنستخدم الـ Switch عشان ننفذ طلب اليوزر بالظبط
                if (propertyParams.Sort.HasValue)
                {
                    switch (propertyParams.Sort)
                    {
                        case SortingOptionsEnum.PriceAsc:
                            AddOrderBy(p => p.PricePerNight);
                            break;
                        case SortingOptionsEnum.PriceDesc:
                            AddOrderByDesc(p => p.PricePerNight);
                            break;
                        case SortingOptionsEnum.NameAsc:
                            AddOrderBy(p => p.Title);
                            break;
                        case SortingOptionsEnum.NameDesc:
                            AddOrderByDesc(p => p.Title);
                            break;
                        case SortingOptionsEnum.DateCreatedAsc: // Oldest
                            AddOrderBy(p => p.CreatedAt);
                            break;
                        case SortingOptionsEnum.DateCreatedDesc: // Newest
                        default:
                            AddOrderByDesc(p => p.CreatedAt);
                            break;
                    }
                }
                else if (propertyParams.Latitude.HasValue && propertyParams.Longitude.HasValue)
                {
                    // لو مفيش ترتيب محدد بس فيه لوكيشن، رتب بالأقرب
                    AddOrderBy(p =>
                        (p.Latitude - propertyParams.Latitude.Value) * (p.Latitude - propertyParams.Latitude.Value) +
                        (p.Longitude - propertyParams.Longitude.Value) * (p.Longitude - propertyParams.Longitude.Value)
                    );
                }
                else
                {
                    // Default Sort (Newest)
                    AddOrderByDesc(p => p.CreatedAt);
                }

                // Pagination لازم ييجي في الآخر
                ApplyPagenation(propertyParams.PageSize, propertyParams.PageIndex);
            }
        }
    }
}
