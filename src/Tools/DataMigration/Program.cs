using Domain.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

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
sourceServices.AddLogging();
sourceServices.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(sourceConnectionString));
sourceServices.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

var targetServices = new ServiceCollection();
targetServices.AddLogging();
targetServices.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(targetConnectionString));
targetServices.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

using var sourceProvider = sourceServices.BuildServiceProvider();
using var targetProvider = targetServices.BuildServiceProvider();

await using var sourceContext = sourceProvider.GetRequiredService<ApplicationDbContext>();
await using var targetContext = targetProvider.GetRequiredService<ApplicationDbContext>();

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
    var sourceUsers = await RetryAsync(() => sourceContext.Users.AsNoTracking().ToListAsync());
    var uniqueUserCount = sourceUsers.Select(u => u.Id).Distinct().Count();
    if (uniqueUserCount != sourceUsers.Count)
    {
        Console.WriteLine($"[WARN] Source users contain duplicate IDs. Unique: {uniqueUserCount}, Total: {sourceUsers.Count}");
    }
    var targetUserIds = (await RetryAsync(() => targetContext.Users.AsNoTracking()
        .Select(u => u.Id)
        .ToListAsync())).ToHashSet();
    Console.WriteLine($"Found {sourceUsers.Count} users in source database");

    foreach (var user in sourceUsers)
    {
        if (!targetUserIds.Contains(user.Id))
        {
            user.CreatedDate = NormalizeUtc(user.CreatedDate);
            user.LastActivityDate = NormalizeUtcNullable(user.LastActivityDate);
            user.StreakLastUpdated = NormalizeUtcNullable(user.StreakLastUpdated);
            targetContext.Users.Add(user);
            targetUserIds.Add(user.Id);
            Console.WriteLine($"  [OK] Migrated user: {user.Email}");
        }
        else
        {
            Console.WriteLine($"  [SKIP] User already exists: {user.Email}");
        }
    }
    await targetContext.SaveChangesAsync();
    Console.WriteLine("[OK] Users migrated");

    Console.WriteLine();
    Console.WriteLine("Step 2: Migrating identity data...");

    var roles = await RetryAsync(() => sourceContext.Roles.AsNoTracking().ToListAsync());
    var targetRoleIds = (await RetryAsync(() => targetContext.Roles.AsNoTracking()
        .Select(r => r.Id)
        .ToListAsync())).ToHashSet();
    Console.WriteLine($"Found {roles.Count} roles");
    foreach (var role in roles)
    {
        if (!targetRoleIds.Contains(role.Id))
        {
            targetContext.Roles.Add(role);
            targetRoleIds.Add(role.Id);
        }
    }

    var userRoles = await RetryAsync(() => sourceContext.UserRoles.AsNoTracking().ToListAsync());
    var targetUserRoles = (await RetryAsync(() => targetContext.UserRoles.AsNoTracking()
        .Select(ur => $"{ur.UserId}:{ur.RoleId}")
        .ToListAsync())).ToHashSet();
    Console.WriteLine($"Found {userRoles.Count} user roles");
    foreach (var userRole in userRoles)
    {
        var key = $"{userRole.UserId}:{userRole.RoleId}";
        if (!targetUserRoles.Contains(key))
        {
            targetContext.UserRoles.Add(userRole);
            targetUserRoles.Add(key);
        }
    }

    await targetContext.SaveChangesAsync();
    Console.WriteLine("[OK] Identity data migrated");

    Console.WriteLine();
    Console.WriteLine("Step 3: Migrating hunting parties...");
    var parties = await RetryAsync(() => sourceContext.HuntingParties.AsNoTracking().ToListAsync());
    var targetPartyIds = (await RetryAsync(() => targetContext.HuntingParties.AsNoTracking()
        .Select(p => p.Id)
        .ToListAsync())).ToHashSet();
    Console.WriteLine($"Found {parties.Count} hunting parties");

    foreach (var party in parties)
    {
        if (!targetPartyIds.Contains(party.Id))
        {
            party.CreatedDate = NormalizeUtc(party.CreatedDate);
            targetContext.HuntingParties.Add(party);
            targetPartyIds.Add(party.Id);
            Console.WriteLine($"  [OK] Migrated party: {party.Name}");
        }
    }
    await targetContext.SaveChangesAsync();

    Console.WriteLine();
    Console.WriteLine("Step 4: Migrating party memberships...");
    var memberships = await RetryAsync(() => sourceContext.HuntingPartyMemberships.AsNoTracking().ToListAsync());
    var uniqueMembershipCount = memberships.Select(m => m.Id).Distinct().Count();
    if (uniqueMembershipCount != memberships.Count)
    {
        Console.WriteLine($"[WARN] Source memberships contain duplicate IDs. Unique: {uniqueMembershipCount}, Total: {memberships.Count}");
    }
    var targetMembershipIds = (await RetryAsync(() => targetContext.HuntingPartyMemberships.AsNoTracking()
        .Select(m => m.Id)
        .ToListAsync())).ToHashSet();
    Console.WriteLine($"Found {memberships.Count} memberships");

    foreach (var membership in memberships)
    {
        if (!targetMembershipIds.Contains(membership.Id))
        {
            membership.JoinedDate = NormalizeUtc(membership.JoinedDate);
            targetContext.HuntingPartyMemberships.Add(membership);
            targetMembershipIds.Add(membership.Id);
            await SaveChangesWithRetryAsync(targetContext, "membership");
        }
    }
    Console.WriteLine("[OK] Memberships migrated");

    Console.WriteLine();
    Console.WriteLine("Step 5: Migrating applications...");
    var applications = await RetryAsync(() => sourceContext.Applications.AsNoTracking().ToListAsync());
    var targetApplicationIds = (await RetryAsync(() => targetContext.Applications.AsNoTracking()
        .Select(a => a.Id)
        .ToListAsync())).ToHashSet();
    Console.WriteLine($"Found {applications.Count} applications");

    foreach (var app in applications)
    {
        if (!targetApplicationIds.Contains(app.Id))
        {
            app.CreatedDate = NormalizeUtc(app.CreatedDate);
            app.UpdatedDate = NormalizeUtc(app.UpdatedDate);
            app.LastAIParsedDate = NormalizeUtcNullable(app.LastAIParsedDate);
            targetContext.Applications.Add(app);
            targetApplicationIds.Add(app.Id);
            await SaveChangesWithRetryAsync(targetContext, "application");
        }
    }
    Console.WriteLine("[OK] Applications migrated");

    Console.WriteLine();
    Console.WriteLine("Step 6: Migrating timeline events...");
    var events = await RetryAsync(() => sourceContext.TimelineEvents.AsNoTracking().ToListAsync());
    var targetEventIds = (await RetryAsync(() => targetContext.TimelineEvents.AsNoTracking()
        .Select(e => e.Id)
        .ToListAsync())).ToHashSet();
    Console.WriteLine($"Found {events.Count} timeline events");

    foreach (var evt in events)
    {
        if (!targetEventIds.Contains(evt.Id))
        {
            evt.Timestamp = NormalizeUtc(evt.Timestamp);
            evt.CreatedDate = NormalizeUtc(evt.CreatedDate);
            evt.UpdatedDate = NormalizeUtcNullable(evt.UpdatedDate);
            targetContext.TimelineEvents.Add(evt);
            targetEventIds.Add(evt.Id);
            await SaveChangesWithRetryAsync(targetContext, "timeline event");
        }
    }
    Console.WriteLine("[OK] Timeline events migrated");

    Console.WriteLine();
    Console.WriteLine("Step 7: Migrating activity events...");
    var activityEvents = await RetryAsync(() => sourceContext.ActivityEvents.AsNoTracking().ToListAsync());
    var targetActivityEventIds = (await RetryAsync(() => targetContext.ActivityEvents.AsNoTracking()
        .Select(ae => ae.Id)
        .ToListAsync())).ToHashSet();
    Console.WriteLine($"Found {activityEvents.Count} activity events");

    foreach (var activityEvent in activityEvents)
    {
        if (!targetActivityEventIds.Contains(activityEvent.Id))
        {
            activityEvent.CreatedDate = NormalizeUtc(activityEvent.CreatedDate);
            targetContext.ActivityEvents.Add(activityEvent);
            targetActivityEventIds.Add(activityEvent.Id);
            await SaveChangesWithRetryAsync(targetContext, "activity event");
        }
    }
    Console.WriteLine("[OK] Activity events migrated");

    Console.WriteLine();
    Console.WriteLine("=== Validation ===");
    Console.WriteLine();

    var sourceUserCount = await RetryAsync(() => sourceContext.Users.CountAsync());
    var targetUserCount = await RetryAsync(() => targetContext.Users.CountAsync());
    WriteCount("Users", sourceUserCount, targetUserCount);

    var sourceAppCount = await RetryAsync(() => sourceContext.Applications.CountAsync());
    var targetAppCount = await RetryAsync(() => targetContext.Applications.CountAsync());
    WriteCount("Applications", sourceAppCount, targetAppCount);

    var sourceEventCount = await RetryAsync(() => sourceContext.TimelineEvents.CountAsync());
    var targetEventCount = await RetryAsync(() => targetContext.TimelineEvents.CountAsync());
    WriteCount("Timeline Events", sourceEventCount, targetEventCount);

    var sourceActivityCount = await RetryAsync(() => sourceContext.ActivityEvents.CountAsync());
    var targetActivityCount = await RetryAsync(() => targetContext.ActivityEvents.CountAsync());
    WriteCount("Activity Events", sourceActivityCount, targetActivityCount);

    var sourcePartyCount = await RetryAsync(() => sourceContext.HuntingParties.CountAsync());
    var targetPartyCount = await RetryAsync(() => targetContext.HuntingParties.CountAsync());
    WriteCount("Hunting Parties", sourcePartyCount, targetPartyCount);

    Console.WriteLine();
    Console.WriteLine("=== Migration Complete ===");
}
catch (Exception ex)
{
    Console.WriteLine();
    Console.WriteLine($"ERROR: {ex.Message}");
    if (ex.InnerException != null)
    {
        Console.WriteLine($"Inner error: {ex.InnerException.Message}");
    }
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

static DateTime NormalizeUtc(DateTime value)
{
    return value.Kind switch
    {
        DateTimeKind.Utc => value,
        DateTimeKind.Unspecified => DateTime.SpecifyKind(value, DateTimeKind.Utc),
        _ => value.ToUniversalTime()
    };
}

static DateTime? NormalizeUtcNullable(DateTime? value)
{
    return value.HasValue ? NormalizeUtc(value.Value) : null;
}

static async Task<T> RetryAsync<T>(Func<Task<T>> operation, int maxAttempts = 3)
{
    for (var attempt = 1; attempt <= maxAttempts; attempt++)
    {
        try
        {
            return await operation();
        }
        catch (Exception ex) when (attempt < maxAttempts && IsTransient(ex))
        {
            Console.WriteLine($"[WARN] Transient error, retrying (attempt {attempt + 1}/{maxAttempts})...");
            await Task.Delay(TimeSpan.FromSeconds(2 * attempt));
        }
    }

    throw new InvalidOperationException("Retry attempt count exhausted.");
}

static bool IsTransient(Exception ex)
{
    if (ex is NpgsqlException || ex is SqlException || ex is TimeoutException || ex is System.IO.IOException)
    {
        return true;
    }

    return ex.InnerException != null && IsTransient(ex.InnerException);
}

static async Task SaveChangesWithRetryAsync(
    DbContext context,
    string entityLabel,
    int maxAttempts = 3)
{
    for (var attempt = 1; attempt <= maxAttempts; attempt++)
    {
        try
        {
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();
            return;
        }
        catch (DbUpdateException ex) when (IsDuplicateKey(ex))
        {
            Console.WriteLine($"[WARN] Duplicate key detected while saving {entityLabel}, skipping.");
            context.ChangeTracker.Clear();
            return;
        }
        catch (Exception ex) when (attempt < maxAttempts && IsTransient(ex))
        {
            Console.WriteLine($"[WARN] Transient error while saving {entityLabel}, retrying (attempt {attempt + 1}/{maxAttempts})...");
            await Task.Delay(TimeSpan.FromSeconds(2 * attempt));
        }
    }

    throw new InvalidOperationException($"Retry attempts exhausted while saving {entityLabel}.");
}

static bool IsDuplicateKey(Exception ex)
{
    if (ex is PostgresException pgEx)
    {
        return pgEx.SqlState == "23505";
    }

    return ex.InnerException != null && IsDuplicateKey(ex.InnerException);
}
