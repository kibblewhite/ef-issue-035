namespace EF_Demo_One_To_One_AND_Many_to_One.Database;

/// <summary>
/// Property 'CustomerBookingOwnershipId' on entity type 'Booking' is part of a primary or alternate key, but has a constant default value set.Constant default values are not useful for primary or alternate keys since these properties must always have non-null unique values.
/// </summary>
public class BookingsDatabaseContext : DbContext
{
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<CustomerBookingDetail> CustomerBookingDetails { get; set; }

    public BookingsDatabaseContext(DbContextOptions<BookingsDatabaseContext> options) : base(options)
    {
        Bookings = Set<Booking>();
        CustomerBookingDetails = Set<CustomerBookingDetail>();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.LogTo(Console.WriteLine);

        // All these ignore statements are put in place, to suppress the other messages, as they are not relevant to highlight the issues at hand
        // Only these message types to show during migration commands -> RelationalEventId.ModelValidationKeyDefaultValueWarning
        optionsBuilder.ConfigureWarnings(warnings => warnings
            .Ignore(CoreEventId.ContextDisposed)
            .Ignore(CoreEventId.ContextInitialized)
            .Ignore(CoreEventId.DetectChangesCompleted)
            .Ignore(CoreEventId.DetectChangesStarting)
            .Ignore(CoreEventId.ForeignKeyChangeDetected)
            .Ignore(CoreEventId.MultipleNavigationProperties)
            .Ignore(CoreEventId.SaveChangesStarting)
            .Ignore(CoreEventId.SensitiveDataLoggingEnabledWarning)
            .Ignore(CoreEventId.StartedTracking)
            .Ignore(CoreEventId.StateChanged)
            .Ignore(CoreEventId.SaveChangesCompleted)
            .Ignore(CoreEventId.ValueGenerated)
            .Ignore(RelationalEventId.CommandCreated)
            .Ignore(RelationalEventId.CommandCreating)
            .Ignore(RelationalEventId.CommandExecuted)
            .Ignore(RelationalEventId.CommandExecuting)
            .Ignore(RelationalEventId.ConnectionClosed)
            .Ignore(RelationalEventId.ConnectionClosing)
            .Ignore(RelationalEventId.ConnectionOpened)
            .Ignore(RelationalEventId.ConnectionOpening)
            .Ignore(RelationalEventId.DataReaderDisposing)
            // .Ignore(RelationalEventId.ModelValidationKeyDefaultValueWarning)
            .Ignore(RelationalEventId.TransactionCommitted)
            .Ignore(RelationalEventId.TransactionCommitting)
            .Ignore(RelationalEventId.TransactionDisposed)
            .Ignore(RelationalEventId.TransactionStarted)
            .Ignore(RelationalEventId.TransactionStarting)
        );
        // https://github.com/dotnet/efcore/issues/25381
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Many-To-One Relationship (Between Many-CustomerBookingDetails.BookingId and One-Bookings.Id)
        modelBuilder.Entity<Booking>()
            .HasMany(booking => booking.CustomerBookingDetails)
            .WithOne(customer_booking_details => customer_booking_details.Booking)
            .HasForeignKey(customer_booking_details => customer_booking_details.BookingId)
            .HasPrincipalKey(booking => booking.Id);

        // One-To-One Relationship (Between One-CustomerBookingDetail.BookingOwnershipId and One-Booking.CustomerBookingOwnershipId)
        modelBuilder.Entity<Booking>()
            .HasOne(booking => booking.CustomerBookingOwnership)
            .WithOne()
            .HasForeignKey<CustomerBookingDetail>(customer_booking_details => customer_booking_details.Id)
            .HasPrincipalKey<Booking>(booking => booking.CustomerBookingOwnershipId);

        // Auto Generate IDs
        modelBuilder.Entity<Booking>().Property(x => x.Id)
            .IsRequired()
            .HasConversion<Guid>()
            .HasColumnType("uuid")
            .HasDefaultValueSql("uuid_generate_v4()")
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<CustomerBookingDetail>().Property(x => x.Id)
            .IsRequired()
            .HasConversion<Guid>()
            .HasColumnType("uuid")
            .HasDefaultValueSql("uuid_generate_v4()")
            .ValueGeneratedOnAdd();

        // Set Primary Keys
        modelBuilder.Entity<Booking>().HasKey(x => x.Id);
        modelBuilder.Entity<CustomerBookingDetail>().HasKey(x => x.Id);

        // Making the CustomerBookingOwnershipId have an index and unique did not help at all. This is a wild goose chase...
        //modelBuilder.Entity<Booking>()
        //    .HasIndex(x => x.CustomerBookingOwnershipId)
        //    .IsUnique();

        // The property 'Booking.CustomerBookingOwnershipId' cannot be marked as nullable/optional because it has been included in the key {'CustomerBookingOwnershipId'}.
        //modelBuilder.Entity<Booking>()
        //    .Property(x => x.CustomerBookingOwnershipId)
        //    .IsRequired(false);

        modelBuilder.Entity<Booking>()
            .Property(x => x.CustomerBookingOwnershipId)
            .HasDefaultValue(Guid.Empty);
        // -> ^^^ Adding the ".HasDefaultValue(Guid.Empty)" above resolved the issue...
        // System.InvalidOperationException: 'Unable to track an entity of type 'Booking' because alternate key property 'CustomerBookingOwnershipId' is null.
        // If the alternate key is not used in a relationship, then consider using a unique index instead. Unique indexes may contain nulls, while alternate keys may not.'

        // Run postgres Extension uuid-ossp for uuid/guid auto-generation
        modelBuilder.HasPostgresExtension("uuid-ossp");

    }
}
