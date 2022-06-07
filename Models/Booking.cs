namespace EF_Demo_One_To_One_AND_Many_to_One.Models;

public class Booking
{
    public Guid Id { get; set; }

    /// <summary>
    /// List of all associated customers which have either had ownership of the booking or currently does.
    /// </summary>
    /// <remarks>This is our Many-To-One relationship</remarks>
    public virtual IReadOnlyList<CustomerBookingDetail> CustomerBookingDetails => _customerBookingDetails.ToList();
    private IList<CustomerBookingDetail> _customerBookingDetails = new List<CustomerBookingDetail>();

    /// <summary>
    /// Represents the current customer than owns this booking -> CustomerBookingDetail.BookingOwnershipId
    /// </summary>
    /// <remarks>This is our One-To-One relationship</remarks>
    public virtual CustomerBookingDetail? CustomerBookingOwnership { get; protected set; }
    public Guid CustomerBookingOwnershipId { get; protected set; }

    public Booking() { }

    public void AssignBookingOwnership(CustomerBookingDetail customerBookingDetail) => CustomerBookingOwnership = customerBookingDetail;
}
