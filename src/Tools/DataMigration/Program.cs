using Domain.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

Console.WriteLine("=== Big Job Hunter Pro - Data Migration Tool ===");
Console.WriteLine();

var sourceConnectionString = ReadConnectionString("Enter Azure SQL Server connection string");
var targetConnectionString = ReadConnectionString("Enter Supabase PostgreSQL connection string");

if (string.IsNullOrWhiteSpace(sourceConnectionString) || string.IsNullOrWhiteSpace(targetConnectionString))
{
    Console.WriteLine("ERROR: Both connection strings are required.");
    return;
}

Console.WriteLine();
Console.WriteLine("Setting up database contexts...");

var sourceServices = new ServiceCollection();
sourceServices.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(sourceConnectionString));
sourceServices.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

var targetServices = new ServiceCollection();
targetServices.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(targetConnectionString));
targetServices.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

using var sourceProvider = sourceServices.BuildServiceProvider();
using var targetProvider = targetServices.BuildServiceProvider();

await using var sourceContext = sourceProvider.GetRequiredService<ApplicationDbContext>();
await using var targetContext = targetProvider.GetRequiredService<ApplicationDbContext>();
var userManager = targetProvider.GetRequiredService<UserManager<ApplicationUser>>();

try
{
    Console.WriteLine("Testing connections...");
    await sourceContext.Database.CanConnectAsync();
    Console.WriteLine("[OK] Source (Azure SQL) connected");

    await targetContext.Database.CanConnectAsync();
    Console.WriteLine("[OK] Target (Supabase PostgreSQL) connected");

    Console.WriteLine();
    Console.WriteLine("=== Starting Data Migration ===");
    Console.WriteLine();

    Console.WriteLine("Step 1: Migrating users...");
    var sourceUsers = await sourceContext.Users.AsNoTracking().ToListAsync();
    Console.WriteLine($"Found {sourceUsers.Count} users in source database");

    foreach (var user in sourceUsers)
    {
        var existingUser = await userManager.FindByIdAsync(user.Id);
        if (existingUser == null)
        {
            var result = await userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                Console.WriteLine($"  [OK] Migrated user: {user.Email}");
            }
            else
            {
                Console.WriteLine($"  [FAIL] Failed to migrate user: {user.Email}");
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"    - {error.Description}");
                }
            }
        }
        else
        {
            Console.WriteLine($"  [SKIP] User already exists: {user.Email}");
        }
    }

    Console.WriteLine();
    Console.WriteLine("Step 2: Migrating identity data...");

    var roles = await sourceContext.Roles.AsNoTracking().ToListAsync();
    Console.WriteLine($"Found {roles.Count} roles");
    foreach (var role in roles)
    {
        if (!await targetContext.Roles.AnyAsync(r => r.Id == role.Id))
        {
            targetContext.Roles.Add(role);
        }
    }

    var userRoles = await sourceContext.UserRoles.AsNoTracking().ToListAsync();
    Console.WriteLine($"Found {userRoles.Count} user roles");
    foreach (var userRole in userRoles)
    {
        if (!await targetContext.UserRoles.AnyAsync(ur => ur.UserId == userRole.UserId && ur.RoleId == userRole.RoleId))
        {
            targetContext.UserRoles.Add(userRole);
        }
    }

    await targetContext.SaveChangesAsync();
    Console.WriteLine("[OK] Identity data migrated");

    Console.WriteLine();
    Console.WriteLine("Step 3: Migrating hunting parties...");
    var parties = await sourceContext.HuntingParties.AsNoTracking().ToListAsync();
    Console.WriteLine($"Found {parties.Count} hunting parties");

    foreach (var party in parties)
    {
        if (!await targetContext.HuntingParties.AnyAsync(p => p.Id == party.Id))
        {
            targetContext.HuntingParties.Add(party);
            Console.WriteLine($"  [OK] Migrated party: {party.Name}");
        }
    }
    await targetContext.SaveChangesAsync();

    Console.WriteLine();
    Console.WriteLine("Step 4: Migrating party memberships...");
    var memberships = await sourceContext.HuntingPartyMemberships.AsNoTracking().ToListAsync();
    Console.WriteLine($"Found {memberships.Count} memberships");

    foreach (var membership in memberships)
    {
        if (!await targetContext.HuntingPartyMemberships.AnyAsync(m => m.Id == membership.Id))
        {
            targetContext.HuntingPartyMemberships.Add(membership);
        }
    }
    await targetContext.SaveChangesAsync();
    Console.WriteLine("[OK] Memberships migrated");

    Console.WriteLine();
    Console.WriteLine("Step 5: Migrating applications...");
    var applications = await sourceContext.Applications.AsNoTracking().ToListAsync();
    Console.WriteLine($"Found {applications.Count} applications");

    foreach (var app in applications)
    {
        if (!await targetContext.Applications.AnyAsync(a => a.Id == app.Id))
        {
            targetContext.Applications.Add(app);
        }
    }
    await targetContext.SaveChangesAsync();
    Console.WriteLine("[OK] Applications migrated");

    Console.WriteLine();
    Console.WriteLine("Step 6: Migrating timeline events...");
    var events = await sourceContext.TimelineEvents.AsNoTracking().ToListAsync();
    Console.WriteLine($"Found {events.Count} timeline events");

    foreach (var evt in events)
    {
        if (!await targetContext.TimelineEvents.AnyAsync(e => e.Id == evt.Id))
        {
            targetContext.TimelineEvents.Add(evt);
        }
    }
    await targetContext.SaveChangesAsync();
    Console.WriteLine("[OK] Timeline events migrated");

    Console.WriteLine();
    Console.WriteLine("Step 7: Migrating activity events...");
    var activityEvents = await sourceContext.ActivityEvents.AsNoTracking().ToListAsync();
    Console.WriteLine($"Found {activityEvents.Count} activity events");

    foreach (var activityEvent in activityEvents)
    {
        if (!await targetContext.ActivityEvents.AnyAsync(ae => ae.Id == activityEvent.Id))
        {
            targetContext.ActivityEvents.Add(activityEvent);
        }
    }
    await targetContext.SaveChangesAsync();
    Console.WriteLine("[OK] Activity events migrated");

    Console.WriteLine();
    Console.WriteLine("=== Validation ===");
    Console.WriteLine();

    var sourceUserCount = await sourceContext.Users.CountAsync();
    var targetUserCount = await targetContext.Users.CountAsync();
    WriteCount("Users", sourceUserCount, targetUserCount);

    var sourceAppCount = await sourceContext.Applications.CountAsync();
    var targetAppCount = await targetContext.Applications.CountAsync();
    WriteCount("Applications", sourceAppCount, targetAppCount);

    var sourceEventCount = await sourceContext.TimelineEvents.CountAsync();
    var targetEventCount = await targetContext.TimelineEvents.CountAsync();
    WriteCount("Timeline Events", sourceEventCount, targetEventCount);

    var sourceActivityCount = await sourceContext.ActivityEvents.CountAsync();
    var targetActivityCount = await targetContext.ActivityEvents.CountAsync();
    WriteCount("Activity Events", sourceActivityCount, targetActivityCount);

    var sourcePartyCount = await sourceContext.HuntingParties.CountAsync();
    var targetPartyCount = await targetContext.HuntingParties.CountAsync();
    WriteCount("Hunting Parties", sourcePartyCount, targetPartyCount);

    Console.WriteLine();
    Console.WriteLine("=== Migration Complete ===");
}
catch (Exception ex)
{
    Console.WriteLine();
    Console.WriteLine($"ERROR: {ex.Message}");
    Console.WriteLine(ex.StackTrace);
}

static string? ReadConnectionString(string prompt)
{
    Console.Write($"{prompt}: ");
    return Console.ReadLine();
}

static void WriteCount(string label, int source, int target)
{
    var status = source == target ? "[OK]" : "[MISMATCH]";
    Console.WriteLine($"{label}: {source} (source) -> {target} (target) {status}");
}
