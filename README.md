# ef-issue-035
EF Demo One-To-One AND Many-To-One with/in the same model. Issue only appears during the `dotnet ef migration` command

## Geting started

- Hopefully this should just compile without issue.
- You will need to define a PostgreSQL database that you can connect to with a user which can create and drop tables.
	- Update the `appsettings.json` with your updated connection string.

Finally, to see the error in question:
- Open up your 'Package Manage Console' and enter the following command:
- `dotnet ef migrations script`

## What should you see as part of the issue:

```
warn: YYYY-MM-DD HH:MM:SS.mmm RelationalEventId.ModelValidationKeyDefaultValueWarning[20600] (Microsoft.EntityFrameworkCore.Model.Validation) 
      Property 'CustomerBookingOwnershipId' on entity type 'Booking' is part of a primary or alternate key, but has a constant default value set.
	  Constant default values are not useful for primary or alternate keys since these properties must always have non-null unique values.
```

## The issue at hand...

Well, without setting the default value to `Guid.Empty` the program will fail during runtime.
However, adding the default value results in the warning message about for `RelationalEventId.ModelValidationKeyDefaultValueWarning`

The code snippit here for adding the default value.

```csharp
modelBuilder.Entity<Booking>()
    .Property(x => x.CustomerBookingOwnershipId)
    .HasDefaultValue(Guid.Empty);
```

The failure message when the above code snippit is removed/commented out is:
```
Microsoft.EntityFrameworkCore.DbUpdateException: 'An error occurred while saving the entity changes. See the inner exception for details.'

PostgresException: 23503: insert or update on table "CustomerBookingDetails" violates foreign key constraint "FK_CustomerBookingDetails_Bookings_Id"

DETAIL: Key (Id)=(b163b697-6525-4423-9fde-1e7522f4da08) is not present in table "Bookings".
```

### NuGet Packages

The nuget packages in use are:
- Microsoft.EntityFrameworkCore.Design
- Microsoft.Extensions.Configuration.Json
- Npgsql.EntityFrameworkCore.PostgreSQL

You should be able to update these nuget pacakages into version EF 7 and the same issue will remain.
