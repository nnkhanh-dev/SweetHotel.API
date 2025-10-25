# Tóm t?t c?p nh?t - Thêm ?nh phòng vào response

## Các thay ??i ?ã th?c hi?n:

### 1. C?p nh?t RoomDto
**File:** `SweetHotel.API\DTOs\Room\RoomDto.cs`

- Thêm thu?c tính `Images` vào `RoomDto`:
```csharp
public List<RoomImageDto>? Images { get; set; }
```

Bây gi? khi tr? v? danh sách phòng, m?i phòng s? có kèm theo danh sách ?nh.

### 2. C?p nh?t RoomsController
**File:** `SweetHotel.API\Controllers\RoomsController.cs`

?ã c?p nh?t t?t c? các endpoint tr? v? danh sách phòng ?? load images:

- `GET /api/Rooms` - L?y t?t c? phòng
- `GET /api/Rooms/Available` - L?y phòng available
- `GET /api/Rooms/ByCategory/{categoryId}` - L?y phòng theo danh m?c
- `GET /api/Rooms/ByPriceRange` - L?y phòng theo kho?ng giá
- `GET /api/Rooms/AvailableByDateRange` - Tìm phòng tr?ng theo ngày
- `POST /api/Rooms` - T?o phòng m?i (response có images)

**Logic thêm images:**
```csharp
foreach (var roomDto in roomsDto)
{
    var images = await _unitOfWork.RoomImages.GetByRoomIdAsync(roomDto.Id);
    roomDto.Images = _mapper.Map<List<RoomImageDto>>(images);
}
```

### 3. C?p nh?t MappingProfile
**File:** `SweetHotel.API\Mappings\MappingProfile.cs`

- Thêm `.ForMember(dest => dest.Images, opt => opt.Ignore())` vào mapping c?a RoomDto
- Images s? ???c load riêng trong Controller thay vì qua AutoMapper

## K?t qu?:

Bây gi? khi g?i API l?y danh sách phòng, response s? có d?ng:

```json
[
  {
    "id": "room-001",
    "status": 1,
    "amenities": "Wifi, TV, Air Conditioner",
    "price": 100.00,
    "discount": 10,
    "categoryId": "cat-001",
    "categoryName": "Deluxe Room",
    "images": [
      {
        "id": "img-001",
        "path": "/images/room1-1.jpg"
      },
      {
        "id": "img-002",
        "path": "/images/room1-2.jpg"
      }
    ]
  }
]
```

## L?u ý:

- Endpoint `GET /api/Rooms/{id}` (GetRoom) ?ã có s?n logic load images t? tr??c
- T?t c? các endpoint ??u load ??y ?? images cho t?ng phòng
- N?u phòng ch?a có ?nh, `images` s? là m?t m?ng r?ng `[]`
- Performance: V?i danh sách nhi?u phòng, vi?c load images có th? ?nh h??ng performance. Cân nh?c:
  - S? d?ng pagination
  - Ho?c t?o endpoint riêng không load images cho tr??ng h?p c?n t?c ?? cao
  - Ho?c implement eager loading v?i Include trong Repository

## Build Status:
? **Build successful**
