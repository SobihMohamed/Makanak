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
        public PropertySpecifications(string ownerId) : base(p => p.OwnerId == ownerId)
        {
            AddOrderByDesc(p => p.CreatedAt);           
        }
        public PropertySpecifications(int propId) : base(p => p.Id== propId)
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

        public PropertySpecifications(PropertyParams propertyParams, bool isCount = false)
            : base
            (p =>
                // main constant filters
                (p.PropertyStatus == PropertyStatus.Accepted) &&
                (!p.IsDeleted) &&
                (p.IsAvailable)
                    &&
                // Hard filters
                (string.IsNullOrEmpty(propertyParams.Search) ||
                 p.Title.ToLower().Contains(propertyParams.Search) ||
                 p.AreaName.ToLower().Contains(propertyParams.Search))
                    &&
                (!propertyParams.GovernorateId.HasValue || p.GovernorateId == propertyParams.GovernorateId)
                    &&
                (!propertyParams.Type.HasValue || p.PropertyType == propertyParams.Type)
                    &&
                //price limit is doubled to include more properties for ranking
                (!propertyParams.MaxPrice.HasValue || p.PricePerNight <= propertyParams.MaxPrice * 2)  
                    &&
                (
                    !propertyParams.CheckInDate.HasValue || !propertyParams.CheckOutDate.HasValue ||
                    !p.Bookings.Any(b =>
                        (b.Status == BookingStatus.PendingPayment || b.Status == BookingStatus.PaymentReceived || b.Status == BookingStatus.CheckedIn)
                          &&
                        (b.CheckInDate < propertyParams.CheckOutDate && b.CheckOutDate > propertyParams.CheckInDate)
                )
        )
            )
        {
            if (!isCount)
            {
                AddInclude(p => p.PropertyImages);
                AddInclude(p => p.Governorate);
                AddInclude(p => p.Amenities);

                // Pagenation
                ApplyPagenation(propertyParams.PageSize, propertyParams.PageIndex);

                // (Location Sorting)
                if (propertyParams.Latitude.HasValue && propertyParams.Longitude.HasValue)
                {
                    AddOrderBy(p =>
                        (p.Latitude - propertyParams.Latitude.Value) * (p.Latitude - propertyParams.Latitude.Value) +
                        (p.Longitude - propertyParams.Longitude.Value) * (p.Longitude - propertyParams.Longitude.Value)
                    );
                }
                else {
                    
                    // if the property apply all filters(price , bedrooms , guest number) so we will rank it higher = 1 
                    // else rank it lower = 0
                    // so we will sort by this rank desc to get the best matching properties first
                    AddOrderByDesc(p =>
                        // is inside price range?
                        (!propertyParams.MaxPrice.HasValue || p.PricePerNight <= propertyParams.MaxPrice) &&
                        (!propertyParams.MinPrice.HasValue || p.PricePerNight >= propertyParams.MinPrice) &&

                        // is inside other filters?
                        (!propertyParams.MinBedrooms.HasValue || p.Bedrooms >= propertyParams.MinBedrooms) &&
                        (!propertyParams.MinMaxGuests.HasValue || p.MaxGuests >= propertyParams.MinMaxGuests)
                    );

                    if (propertyParams.AmenityIds != null && propertyParams.AmenityIds.Count > 0)
                    {
                        AddOrderExpression(p => p.Amenities.Count(a => propertyParams.AmenityIds.Contains(a.Id)), true);
                    }

                    if (propertyParams.Sort != null && propertyParams.Sort == SortingOptionsEnum.PriceDesc)
                    {
                        AddOrderExpression(p => p.PricePerNight, true);
                    }
                    else
                    {
                        AddOrderExpression(p => p.PricePerNight, false);
                    }
                }
            }
        }
    }
}
