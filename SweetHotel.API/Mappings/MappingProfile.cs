using AutoMapper;
using SweetHotel.API.Entities.Entities;
using SweetHotel.API.Enums;

namespace SweetHotel.API.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Category Mappings
            CreateMap<Category, DTOs.Category.CategoryDto>().ReverseMap();
            CreateMap<DTOs.Category.CreateCategoryDto, Category>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid().ToString()));
            CreateMap<DTOs.Category.UpdateCategoryDto, Category>();

            // Room Mappings
            CreateMap<Room, DTOs.Room.RoomDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null))
                .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.discount))
                .ForMember(dest => dest.Images, opt => opt.Ignore()); // Images s? ???c load riêng
            
            CreateMap<Room, DTOs.Room.RoomDetailDto>()
                .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.discount))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
                .ForMember(dest => dest.Images, opt => opt.Ignore()); // Images s? ???c load riêng
            
            CreateMap<DTOs.Room.CreateRoomDto, Room>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid().ToString()))
                .ForMember(dest => dest.discount, opt => opt.MapFrom(src => src.Discount));
            
            CreateMap<DTOs.Room.UpdateRoomDto, Room>()
                .ForMember(dest => dest.discount, opt => opt.MapFrom(src => src.Discount));

            CreateMap<Category, DTOs.Room.CategoryDto>();
            CreateMap<RoomImages, DTOs.Room.RoomImageDto>();

            // Booking Mappings
            CreateMap<Booking, DTOs.Booking.BookingDto>().ReverseMap();
            
            CreateMap<Booking, DTOs.Booking.BookingDetailDto>()
                .ForMember(dest => dest.Room, opt => opt.MapFrom(src => src.Room))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));
            
            CreateMap<DTOs.Booking.CreateBookingDto, Booking>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid().ToString()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => BookingStatus.Pending))
                .ForMember(dest => dest.TotalPrice, opt => opt.Ignore());
            
            CreateMap<DTOs.Booking.UpdateBookingDto, Booking>();

            CreateMap<Room, DTOs.Booking.RoomDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null))
                .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.discount));
            
            CreateMap<AppUser, DTOs.Booking.UserDto>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber));

            // Review Mappings
            CreateMap<Review, DTOs.Review.ReviewDto>().ReverseMap();
            
            CreateMap<Review, DTOs.Review.ReviewDetailDto>()
                .ForMember(dest => dest.Booking, opt => opt.MapFrom(src => src.Booking));
            
            CreateMap<DTOs.Review.CreateReviewDto, Review>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid().ToString()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
            
            CreateMap<DTOs.Review.UpdateReviewDto, Review>();

            CreateMap<Booking, DTOs.Review.BookingDto>();

            // RoomImages Mappings
            CreateMap<RoomImages, DTOs.RoomImages.RoomImageDto>().ReverseMap();
            
            CreateMap<DTOs.RoomImages.CreateRoomImageDto, RoomImages>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid().ToString()));
            
            CreateMap<DTOs.RoomImages.UpdateRoomImageDto, RoomImages>();
        }
    }
}
