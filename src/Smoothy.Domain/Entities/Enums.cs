namespace Smoothy.Domain.Entities;
 
public enum UserRole
{
    Owner = 1,   // בעלת העסק / Admin
    Customer = 2
}
 
public enum ServiceLocationType
{
    AtBusiness = 1,   // בבית העסק
    AtCustomer = 2,   // בבית הלקוחה
    Both = 3
}
 
public enum AppointmentStatus
{
    Pending = 1,     // ממתין לאישור (לדוגמה הצעה מה-AI)
    Confirmed = 2,
    Completed = 3,
    Cancelled = 4
}