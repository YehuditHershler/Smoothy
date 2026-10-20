using Microsoft.EntityFrameworkCore;
using Smoothy.Domain.Entities;

namespace Smoothy.Infrastructure;

/// <summary>
/// מספק בכל שאילתה את ה-BusinessId של המשתמש המחובר, כדי שה-DbContext יוכל
/// לסנן אוטומטית. מיושם בהמשך ב-Api כ-Service שקורא את הערך מתוך ה-JWT.
/// כרגע implementation זמני לצורך פיתוח/בדיקות.
/// </summary>
public interface ICurrentBusinessProvider
{
    Guid? BusinessId { get; }
}

public class SmoothyDbContext(
    DbContextOptions<SmoothyDbContext> options,
    ICurrentBusinessProvider currentBusiness) : DbContext(options)
{
    public DbSet<Business> Businesses => Set<Business>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<BusinessHours> BusinessHours => Set<BusinessHours>();
    public DbSet<BlockedTime> BlockedTimes => Set<BlockedTime>();
    public DbSet<Appointment> Appointments => Set<Appointment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // --- Global Query Filter: כל שאילתה על ישות שיש לה BusinessId
        // תסונן אוטומטית לפי העסק של המשתמש המחובר. כך גם אם מישהו ישכח
        // Where(x => x.BusinessId == ...) בקוד, לא תהיה דליפת מידע בין עסקים.
        modelBuilder.Entity<User>().HasQueryFilter(e => e.BusinessId == currentBusiness.BusinessId);
        modelBuilder.Entity<Customer>().HasQueryFilter(e => e.BusinessId == currentBusiness.BusinessId);
        modelBuilder.Entity<Location>().HasQueryFilter(e => e.BusinessId == currentBusiness.BusinessId);
        modelBuilder.Entity<Service>().HasQueryFilter(e => e.BusinessId == currentBusiness.BusinessId);
        modelBuilder.Entity<BusinessHours>().HasQueryFilter(e => e.BusinessId == currentBusiness.BusinessId);
        modelBuilder.Entity<BlockedTime>().HasQueryFilter(e => e.BusinessId == currentBusiness.BusinessId);
        modelBuilder.Entity<Appointment>().HasQueryFilter(e => e.BusinessId == currentBusiness.BusinessId);

        // דוגמאות אילוצים בסיסיים - נרחיב בהמשך
        modelBuilder.Entity<Service>()
            .Property(s => s.Price).HasPrecision(10, 2);
        modelBuilder.Entity<Appointment>()
            .Property(a => a.Price).HasPrecision(10, 2);

        modelBuilder.Entity<Appointment>()
            .HasIndex(a => new { a.BusinessId, a.StartTime });
    }
}

/// <summary>
/// מימוש זמני לפיתוח מוקדם/בדיקות - מחזיר תמיד null (כלומר בלי סינון).
/// יוחלף בשלב ה-Auth במימוש אמיתי שקורא BusinessId מתוך ה-JWT של המשתמש המחובר.
/// </summary>
// public class DesignTimeCurrentBusinessProvider : ICurrentBusinessProvider
// {
//     public Guid? BusinessId => null;
// }

public class DesignTimeCurrentBusinessProvider : ICurrentBusinessProvider
{
    public Guid? BusinessId => Guid.Parse("11111111-1111-1111-1111-111111111111");
}