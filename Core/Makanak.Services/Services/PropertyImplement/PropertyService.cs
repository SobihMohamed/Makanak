using AutoMapper;
using Makanak.Abstraction.IServices;
using Makanak.Abstraction.IServices.PropertyService;
using Makanak.Domain.Contracts.UOW;
using Makanak.Domain.Exceptions.NotFound;
using Makanak.Domain.Models.Identity;
using Makanak.Domain.Models.PropertyEntities;
using Makanak.Services.AutoMapper.Resolver;
using Makanak.Services.Specifications.Property_Spec;
using Makanak.Shared.Common;
using Makanak.Shared.Common.Params.Property_Params;
using Makanak.Shared.Dto_s.Property;
using Makanak.Shared.EnumsHelper.Property;
using System.Xml;


namespace Makanak.Services.Services.PropertyImplement
{
    public class PropertyService(IMapper _mapper, IAttachementServices _attachementServices, IUnitOfWork _unitOfWork) : IPropertyService
    {
        public async Task<PropertyDetailDto> CreatePropertyAsync(CreatePropertyDto dto, string ownerId)
        {
            // 1 - Map CreatePropertyDto to Property entity
            var property = _mapper.Map<Property>(dto);

            // 2 - Set additional fields 
            property.OwnerId = ownerId;
            property.IsAvailable = true;
            property.PropertyStatus = PropertyStatus.Pending;

            // 3 - Main Image handling
            string mainImagesPath = Path.Combine(ownerId, "Properties", "Main");
            if (dto.MainImage != null)
            {
                var realMainPath = await _attachementServices.UploadImageAsync(dto.MainImage , mainImagesPath);
                property.MainImageUrl = realMainPath;
            }

            // 4 - Gallery Images handling 
            string galleryImagesPath = Path.Combine(ownerId, "Properties", "Gallery");
            if(dto.GalleryImages != null && dto.GalleryImages.Count > 0)
            {
                foreach (var image in dto.GalleryImages)
                {
                    var realGallaryPath = await _attachementServices.UploadImageAsync(image, galleryImagesPath);
                    property.PropertyImages.Add(new PropertyImage
                    {
                        ImageUrl = realGallaryPath
                    });
                }
            }

            // 5 - Ammenities handling
            if (dto.AmenityIds != null)
            {
                var amenityRepository = _unitOfWork.GetRepo<Amenity, int>();

                // get unique amenity IDs
                var uniqueAmenityIds = dto.AmenityIds.Distinct().ToList();

                // generate amenity specification
                var amenitySpec = new AmenitySpecifications(uniqueAmenityIds);
                var existingAmenities = await amenityRepository.GetAllWithSpecificationAsync(amenitySpec);

                // get existing amenity IDs
                var existingAmenityIds = existingAmenities.Select(a => a.Id).ToHashSet();

                // check for invalid amenity IDs
                var invalidAmenityIds = uniqueAmenityIds.Where(id => !existingAmenityIds.Contains(id)).ToList();
                if (invalidAmenityIds.Count > 0)
                    throw new AmenityNotFound(invalidAmenityIds.First());

                // re-add existing amenities
                foreach (var amenity in existingAmenities)
                {
                    property.Amenities.Add(amenity);
                }
            }

            // 6 - Save Property to database
            var propertyRepository = _unitOfWork.GetRepo<Property,int>();
            propertyRepository.AddAsync(property);

            await _unitOfWork.SaveChangesAsync();

            // Mappig & Return
            return _mapper.Map<PropertyDetailDto>(property);
        }

        public async Task<IEnumerable<PropertyDto>> GetPropertiesByOwnerId(string ownerId)
        {
            // get property repo 
            var propertyRepo = _unitOfWork.GetRepo<Property,int>();

            // generate specification 
            var specs = new PropertySpecifications(ownerId);

            // get properties 
            var properties = await propertyRepo.GetAllWithSpecificationAsync(specs);

            // mapping 
            var data = _mapper.Map<IEnumerable<PropertyDto>>(properties);

            return data;
        }

        public async Task<PropertyDetailDto> GetPropertyDetailByIdAsync(int propertyId)
        {
            // get property repo
            var propertyRepo = _unitOfWork.GetRepo<Property,int>();

            // generate specifications
            var specs = new PropertySpecifications(propertyId);

            // get property
            var property = await propertyRepo.GetByIdWithSpecificationsAsync(specs);

            if(property == null)
                throw new PropertyNotFound(propertyId);

            // mapping
            var data = _mapper.Map<PropertyDetailDto>(property);

            return data;
        }

        public async Task<PropertyDetailDto> UpdatePropertyAsync(UpdatePropertyDto dto, string ownerId)
        {
            // get property repo
            var propertyRepo = _unitOfWork.GetRepo<Property, int>();

            // generate specifications
            var spec = new PropertySpecifications(dto.Id);
            var property = await propertyRepo.GetByIdWithSpecificationsAsync(spec);

            // check if same Owner 
            if (property == null || dto.Id != property.Id || property.OwnerId != ownerId)
                throw new PropertyNotFound(dto.Id);

            // map updated fields from dto to entity
            _mapper.Map(dto , property);

            property.PropertyStatus = PropertyStatus.Pending;
            property.LastModifiedBy = ownerId;
            property.UpdatedAt = DateTime.UtcNow;

            // check main image update
            if(dto.MainImage != null)
            {
                // delete old main image
                if (!string.IsNullOrEmpty(property.MainImageUrl))
                {
                    await _attachementServices.DeleteImage(property.MainImageUrl);
                }
                // upload new main image
                string mainImagesPath = Path.Combine(ownerId, "Properties", "Main");
                var realMainPath = await _attachementServices.UploadImageAsync(dto.MainImage, mainImagesPath);
                property.MainImageUrl = realMainPath;
            }

            // check if any gallery images to delete
            if (dto.DeletedImageIds!= null && dto.DeletedImageIds.Count > 0)
            {
                // get the IDs of images to delete
                var imagesToDelete = property.PropertyImages
                    .Where(img => dto.DeletedImageIds.Contains(img.Id))
                    .ToList();
                // delete images from storage and remove from property
                foreach (var item in imagesToDelete)
                {
                    // delete from storage
                    await _attachementServices.DeleteImage(item.ImageUrl);
                    // remove from property    
                    property.PropertyImages.Remove(item);
                }
            }

            // check new gallery images to add
            if (dto.GalleryImages != null && dto.GalleryImages.Count > 0)
            {
                string galleryPath = Path.Combine(ownerId, "Properties", "Gallery");

                foreach (var file in dto.GalleryImages)
                {
                    var realPath = await _attachementServices.UploadImageAsync(file, galleryPath);

                    // add to property
                    property.PropertyImages.Add(new PropertyImage
                    {
                        ImageUrl = realPath
                    });
                }
            }

            // update amenities
            if (dto.AmenityIds != null)
            {
                var amenityRepository = _unitOfWork.GetRepo<Amenity, int>();
                
                // clear existing amenities
                property.Amenities.Clear();

                // get unique amenity IDs
                var uniqueAmenityIds = dto.AmenityIds.Distinct().ToList();
                
                // generate amenity specification
                var amenitySpec = new AmenitySpecifications(uniqueAmenityIds);
                var existingAmenities = await amenityRepository.GetAllWithSpecificationAsync(amenitySpec);

                // get existing amenity IDs
                var existingAmenityIds = existingAmenities.Select(a => a.Id).ToHashSet();

                // check for invalid amenity IDs
                var invalidAmenityIds = uniqueAmenityIds.Where(id => !existingAmenityIds.Contains(id)).ToList();
                if (invalidAmenityIds.Count > 0)
                    throw new AmenityNotFound(invalidAmenityIds.First());

                // re-add existing amenities
                foreach (var amenity in existingAmenities)
                {
                    property.Amenities.Add(amenity);
                }
            }

            // save changes to database
            propertyRepo.Update(property);
            var result = await _unitOfWork.SaveChangesAsync();
            var newPropDetailsDto = _mapper.Map<PropertyDetailDto>(property);
            return newPropDetailsDto;
        }

        public async Task<bool> DeletePropertyAsync(int propertyId, string ownerId)
        {
            var propertyRepo = _unitOfWork.GetRepo<Property, int>();

            // 1. Retrieve Property
            var property = await propertyRepo.GetByIdAsync(propertyId);

            if (property == null)
                throw new PropertyNotFound(propertyId);

            // Security Check
            if (property.OwnerId != ownerId)
                throw new UnauthorizedAccessException("You are not authorized to delete this property.");

            // 2. Soft Delete Logic
            property.IsDeleted = true;

            property.IsAvailable = false;

            property.PropertyStatus = PropertyStatus.Rejected;

            // 3. Save
            propertyRepo.Update(property);
            var result = await _unitOfWork.SaveChangesAsync();

            return result > 0;
        }

        public async Task<Pagination<PropertyDto>> GetAllAvailablePropertiesAsync(PropertyParams propertyParams)
        {
            // get repo 
            var propertyRepo = _unitOfWork.GetRepo<Property,int>();

            // get count specifications
            var countSpecs = new PropertySpecifications(propertyParams,true);

            // get properties
            var totalItems =await propertyRepo.CountAsync(countSpecs);
            if(totalItems == 0)
                return new Pagination<PropertyDto>(propertyParams.PageIndex, 
                    propertyParams.PageSize, 0, new List<PropertyDto>());

            // get data specifications
            var dataSpecs = new PropertySpecifications(propertyParams,false);

            var properties = await propertyRepo.GetAllWithSpecificationAsync(dataSpecs);
            // mapping
            var data = _mapper.Map<IReadOnlyList<PropertyDto>>(properties);

            // return paginated result
            return new Pagination<PropertyDto>(propertyParams.PageIndex, propertyParams.PageSize, totalItems, data);
        }
    }
}
