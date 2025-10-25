namespace SweetHotel.API.Enums
{
    /// <summary>
    /// Tr?ng thái ??t phòng
    /// </summary>
    public enum BookingStatus
    {
        /// <summary>
        /// Ch? xác nh?n
        /// </summary>
        Pending = 0,
        
        /// <summary>
        /// ?ã xác nh?n
        /// </summary>
        Confirmed = 1,
        
        /// <summary>
        /// ?ã h?y
        /// </summary>
        Cancelled = 2,
        
        /// <summary>
        /// ?ang s? d?ng (khách ?ã check-in)
        /// </summary>
        CheckedIn = 3,
        
        /// <summary>
        /// ?ã hoàn thành (khách ?ã check-out)
        /// </summary>
        Completed = 4,
        
        /// <summary>
        /// Không ??n (No-show)
        /// </summary>
        NoShow = 5
    }
}
