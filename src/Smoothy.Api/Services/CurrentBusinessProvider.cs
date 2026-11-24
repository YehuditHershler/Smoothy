using Smoothy.Infrastructure;

namespace Smoothy.Api.Services;

/// <summary>
/// קורא את ה-BusinessId מתוך ה-Token של הבקשה הנוכחית.
/// אם אין Token (או שאין בו business_id), מחזיר null, וה-Query Filter
/// לא יחזיר שום נתון. כלומר ברירת המחדל היא "לא רואים כלום".
/// </summary>
public class CurrentBusinessProvider(IHttpContextAccessor accessor) : ICurrentBusinessProvider
{
    public Guid? BusinessId =>
        Guid.TryParse(accessor.HttpContext?.User.FindFirst("business_id")?.Value, out var id)
            ? id
            : null;
}