namespace SweetHotel.API.Enums
{
    /// <summary>
    /// Tr?ng thái phòng
    /// </summary>
    public enum RoomStatus
    {
        /// <summary>
        /// Không kh? d?ng
        /// </summary>
        Unavailable = 0,
        
        /// <summary>
        /// Còn tr?ng, s?n sàng cho thuê
        /// </summary>
        Available = 1,
        
        /// <summary>
        /// ?ang ???c thuê
        /// </summary>
        Occupied = 2,
        
        /// <summary>
        /// ?ang b?o trì
        /// </summary>
        Maintenance = 3,
        
        /// <summary>
        /// ?ang d?n d?p
        /// </summary>
        Cleaning = 4
    }
}
