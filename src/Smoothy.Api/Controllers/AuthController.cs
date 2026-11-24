using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smoothy.Api.Dtos;
using Smoothy.Domain.Entities;
using Smoothy.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Smoothy.Api.Services;
using Smoothy.Infrastructure.Identity;

namespace Smoothy.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(SmoothyDbContext db,
    UserManager<ApplicationUser> userManager,
    IJwtTokenService tokenService) : ControllerBase
{
//

    /// <summary>
    /// POST /api/auth/login - מחזיר Token. הסיסמה נשלחת ב-body ולא ב-URL.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {

        var user = await userManager.FindByEmailAsync(request.Email.Trim());

        // הודעה אחת לשני המקרים - לא חושפים אם האימייל קיים
        if (user is null || !await userManager.CheckPasswordAsync(user, request.Password))
            return Unauthorized("Invalid email or password.");

        

        return Ok(BuildResponse(user));
    }
//

    /// <summary>
    /// GET /api/auth/me - פרטי המשתמשת המחוברת, לפי ה-Token.
    /// </summary>
    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<MeResponse>> Me()
    {
        
        // שם ה-claim הוא "sub" כי ב-Program.cs כיבינו MapInboundClaims
        //  if (userId is null) return Unauthorized();
        var userId = User.FindFirstValue("sub");

        var user = await userManager.FindByIdAsync(userId);
        if (user is null) return NotFound();

         return Ok(new MeResponse(
            user.Id, user.FullName, user.Email ?? string.Empty,
            user.BusinessId, user.Role.ToString()));
    } 

    /// <summary>
    /// POST /api/auth/register - יוצר עסק + משתמשת בעלת עסק בטרנזקציה אחת.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterBusinessRequest request)
    {

         if (string.IsNullOrWhiteSpace(request.BusinessName))
            return BadRequest("BusinessName is required.");
        if (string.IsNullOrWhiteSpace(request.FullName))
            return BadRequest("FullName is required.");
        if (string.IsNullOrWhiteSpace(request.Email))
            return BadRequest("Email is required.");


        var email = request.Email.Trim();

        if (await userManager.FindByEmailAsync(email) is not null)
            return BadRequest("Email already exists.");

        // אם משהו נכשל לפני ה-Commit, הכול מתבטל (אין עסק "יתום" בלי משתמשת)  
        await using var transaction = await db.Database.BeginTransactionAsync();

        // 1. יצירת העסק
        var business = new Business { Name = request.BusinessName.Trim() };
        db.Businesses.Add(business);
        await db.SaveChangesAsync();

      //  2. יצירת המשתמשת וקישורה לעסק. UserManager מצפין את הסיסמה בעצמו
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FullName = request.FullName,
            BusinessId = business.Id,
            Role = UserRole.Owner,
        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors.Select(e => e.Description));

        await transaction.CommitAsync();

        return Ok(BuildResponse(user));
    }

        // בניית התשובה במקום אחד, בלי כפילות קוד
    private AuthResponse BuildResponse(ApplicationUser user) => new(
        tokenService.CreateToken(user),
        user.BusinessId,
        user.FullName,
        user.Email ?? string.Empty);


}