namespace EF_Demo_One_To_One_AND_Many_to_One;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Heylo, World!");

        BookingsDatabaseContextFactory db_ctx_factory = new();
        BookingsDatabaseContext context = db_ctx_factory.CreateDbContext(args);

        Booking booking = new();
        CustomerBookingDetail customer_booking_detail = new(booking);
        booking.AssignBookingOwnership(customer_booking_detail);

        //
        context.Bookings.Add(booking);
        // _> If the default value for the field "CustomerBookingOwnershipId" is not set this will fail with the following error:
        // System.InvalidOperationException: 'Unable to track an entity of type 'Booking' because alternate key property 'CustomerBookingOwnershipId' is null.
        // If the alternate key is not used in a relationship, then consider using a unique index instead. Unique indexes may contain nulls, while alternate keys may not.'
        //

        context.CustomerBookingDetails.Add(customer_booking_detail);
        context.SaveChanges();

        // When creating another set of details, the old context get's over written...
        CustomerBookingDetail another_customer_booking_details = new(booking);
        booking.AssignBookingOwnership(another_customer_booking_details);

        context.CustomerBookingDetails.Add(another_customer_booking_details);
        context.SaveChanges();

        Console.WriteLine("Done.");
        Console.ReadLine();
    }
}
