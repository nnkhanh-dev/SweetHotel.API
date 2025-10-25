# API Endpoints cho Client - SweetHotel

## Các ch?c n?ng ?ã tri?n khai

### 1. Tìm ki?m phòng tr?ng theo kho?ng th?i gian và l?c

**Endpoint:** `GET /api/Rooms/AvailableByDateRange`

**Mô t?:** Tìm ki?m các phòng không có ai ??t trong kho?ng th?i gian và l?c theo danh m?c, s? ng??i.

**Query Parameters:**
- `startDate` (required): Ngày b?t ??u (format: yyyy-MM-dd)
- `endDate` (required): Ngày k?t thúc (format: yyyy-MM-dd)
- `categoryId` (optional): ID danh m?c phòng ?? l?c
- `maxPeople` (optional): S? ng??i t?i ?a (l?c các phòng có s?c ch?a >= giá tr? này)

**Ví d?:**
```
GET /api/Rooms/AvailableByDateRange?startDate=2024-12-01&endDate=2024-12-05&categoryId=cat-001&maxPeople=2
```

**Response:**
```json
[
  {
    "id": "room-001",
    "status": 1,
    "amenities": "Wifi, TV, Air Conditioner",
    "price": 100.00,
    "discount": 10,
    "categoryId": "cat-001",
    "categoryName": "Deluxe Room"
  }
]
```

---

### 2. Xem l?ch s? ??t phòng c?a Client

**Endpoint:** `GET /api/Bookings/MyBookings/{userId}`

**Mô t?:** Xem l?ch s? ??t phòng c?a user ???c phân lo?i theo tr?ng thái.

**Path Parameters:**
- `userId` (required): ID c?a user

**Ví d?:**
```
GET /api/Bookings/MyBookings/user-123
```

**Response:**
```json
{
  "upcoming": [
    // Các booking s?p t?i (Pending, Confirmed)
  ],
  "current": [
    // Các booking ?ang s? d?ng (CheckedIn)
  ],
  "completed": [
    // Các booking ?ã hoàn thành (Completed)
  ],
  "cancelled": [
    // Các booking ?ã h?y (Cancelled)
  ],
  "all": [
    // T?t c? bookings
  ]
}
```

---

### 3. H?y ??t phòng

**Endpoint:** `POST /api/Bookings/{id}/Cancel`

**Mô t?:** Client h?y booking c?a mình (ch? áp d?ng cho booking ch?a CheckedIn ho?c Completed).

**Path Parameters:**
- `id` (required): ID c?a booking c?n h?y

**Ví d?:**
```
POST /api/Bookings/booking-123/Cancel
```

**Response:**
```json
{
  "message": "?ã h?y booking thành công",
  "bookingId": "booking-123"
}
```

**L?i có th? x?y ra:**
- Booking ?ã h?y tr??c ?ó
- Booking ?ã hoàn thành (không th? h?y)
- Booking ?ang s? d?ng (?ã check-in, không th? h?y)

---

### 4. ?ánh giá sau khi hoàn thành booking

**Endpoint:** `POST /api/Reviews`

**Mô t?:** Client ?ánh giá phòng sau khi ?ã hoàn thành booking (status = Completed).

**Request Body:**
```json
{
  "rating": 5,
  "comment": "Phòng r?t ??p và s?ch s?",
  "bookingId": "booking-123"
}
```

**Validation:**
- `rating`: B?t bu?c, t? 1 ??n 5
- `comment`: B?t bu?c, t?i ?a 1000 ký t?
- `bookingId`: B?t bu?c

**Response:**
```json
{
  "id": "review-123",
  "rating": 5,
  "comment": "Phòng r?t ??p và s?ch s?",
  "createdAt": "2024-12-01T10:00:00Z",
  "bookingId": "booking-123"
}
```

**?i?u ki?n:**
- Booking ph?i có tr?ng thái `Completed` (?ã check-out)
- M?i booking ch? ???c ?ánh giá 1 l?n

---

### 5. Xem review theo booking

**Endpoint:** `GET /api/Reviews/ByBooking/{bookingId}`

**Mô t?:** Xem review c?a m?t booking c? th?.

**Path Parameters:**
- `bookingId` (required): ID c?a booking

**Ví d?:**
```
GET /api/Reviews/ByBooking/booking-123
```

---

### 6. ??t phòng (CreateBooking)

**Endpoint:** `POST /api/Bookings`

**Mô t?:** T?o booking m?i.

**Request Body:**
```json
{
  "startDate": "2024-12-01",
  "endDate": "2024-12-05",
  "note": "Ghi chú ??c bi?t",
  "roomId": "room-001",
  "userId": "user-123"
}
```

**Response:**
```json
{
  "id": "booking-123",
  "startDate": "2024-12-01T00:00:00Z",
  "endDate": "2024-12-05T00:00:00Z",
  "status": 0,
  "note": "Ghi chú ??c bi?t",
  "totalPrice": 360.00,
  "roomId": "room-001",
  "userId": "user-123"
}
```

**Validation:**
- Phòng ph?i t?n t?i
- Phòng ph?i available trong kho?ng th?i gian ?ó
- TotalPrice ???c t? ??ng tính d?a trên giá phòng, discount và s? ngày

---

## Tr?ng thái Booking (BookingStatus)

| Value | Name | Description |
|-------|------|-------------|
| 0 | Pending | Ch? xác nh?n |
| 1 | Confirmed | ?ã xác nh?n |
| 2 | Cancelled | ?ã h?y |
| 3 | CheckedIn | ?ang s? d?ng (?ã check-in) |
| 4 | Completed | ?ã hoàn thành (?ã check-out) |
| 5 | NoShow | Không ??n (No-show) |

---

## Tr?ng thái Room (RoomStatus)

| Value | Name | Description |
|-------|------|-------------|
| 0 | Unavailable | Không kh? d?ng |
| 1 | Available | Còn tr?ng, s?n sàng cho thuê |
| 2 | Occupied | ?ang ???c thuê |
| 3 | Maintenance | ?ang b?o trì |
| 4 | Cleaning | ?ang d?n d?p |

---

## Notes

### C?p nh?t Entity Review
- ?ã thêm thu?c tính `Rating` (int, range 1-5) vào entity Review
- C?n ch?y migration ?? c?p nh?t database: `dotnet ef migrations add AddRatingToReview`
- Sau ?ó update database: `dotnet ef database update`

### Workflow Client ?i?n hình

1. **Tìm phòng:** Client tìm phòng tr?ng theo ngày và l?c theo nhu c?u
2. **??t phòng:** Client t?o booking m?i
3. **Xem l?ch s?:** Client xem các booking c?a mình
4. **H?y booking:** Client có th? h?y booking n?u ch?a check-in
5. **?ánh giá:** Sau khi check-out (status = Completed), client có th? ?ánh giá
