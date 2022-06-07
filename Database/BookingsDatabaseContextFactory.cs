namespace EF_Demo_One_To_One_AND_Many_to_One.Database;

public class BookingsDatabaseContextFactory : IDesignTimeDbContextFactory<BookingsDatabaseContext>
{
    public BookingsDatabaseContext? context;

    public BookingsDatabaseContext CreateDbContext(string[] args)
    {
        IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false);
        IConfigurationRoot configuration = builder.Build();
        string connection_string = configuration.GetConnectionString(nameof(BookingsDatabaseContext));

        DbContextOptions<BookingsDatabaseContext> context_options = new DbContextOptionsBuilder<BookingsDatabaseContext>()
            .UseNpgsql(connection_string)
            .Options;

        context = new(context_options);
        context.Database.ExecuteSqlRaw("DROP TABLE IF EXISTS \"CustomerBookingDetails\";");
        context.Database.ExecuteSqlRaw("DROP TABLE IF EXISTS \"Bookings\";");

        // context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        // context.Database.ExecuteSqlRaw("ALTER TABLE \"Bookings\" ALTER COLUMN \"CustomerBookingOwnershipId\" TYPE UUID, ALTER COLUMN \"CustomerBookingOwnershipId\" DROP NOT NULL, ALTER COLUMN \"CustomerBookingOwnershipId\" DROP DEFAULT;");

        return context ?? new BookingsDatabaseContext(context_options);
    }
}
