namespace EF_Demo_One_To_One_AND_Many_to_One.Models;

public class CustomerBookingDetail
{
    public Guid Id { get; set; }

    /// <summary>
    /// Links back to the Booking.CustomerBookingDetails as a one-to-many relationship
    /// </summary>
    public virtual Booking? Booking { get; protected set; }
    public Guid BookingId { get; protected set; }

    protected CustomerBookingDetail() { }

    public CustomerBookingDetail(Booking booking) : this() => Booking = booking;
}
