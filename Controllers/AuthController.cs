using ArtistAwards.Data;
using ArtistAwards.Helper_Models;
using ArtistAwards.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BC = BCrypt.Net.BCrypt;

namespace ArtistAwards.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  [EnableCors("EnableCORS")]
  public class AuthController : ControllerBase
  {
    public AuthController(IConfiguration configuration, UserService userService, AppDbContext dbContext, IHttpContextAccessor httpContextAccessor, ConfigService configService)
    {
      Config = configuration;
      UserService = userService;
      DbContext = dbContext;
      HttpContextAccessor = httpContextAccessor;
      ConfigService = configService;
    }

    private IConfiguration Config { get; }
    private UserService UserService;
    private AppDbContext DbContext;
    private Regex EmailRegex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
    private IHttpContextAccessor HttpContextAccessor;
    private ConfigService ConfigService;

    [HttpPost, Route("login")]
    public IActionResult Login([FromBody] LoginModel model)
    {
      ValidateAuthModel(model);
      var response = UserService.AuthenticateUser(model.Email, model.Password);
      if (response == null)
        return BadRequest(new { message = "Email or Password is incorrect" });

      SetAuthTokens(response);

      return Ok(new { message = "Login successful" });
    }

    [HttpPost, Route("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
      ValidateAuthModel(model);

      var user = new User { Name = model.Name, Email = model.Email };
      string passwordHash = BC.HashPassword(model.Password);
      user.Passwordhash = passwordHash;
      User checkUser = DbContext.Users.SingleOrDefault(u => u.Email == user.Email);
      if (checkUser != null)
      {
        return BadRequest(new { message = "User already exists" });
      }
      var response = await UserService.CreateUser(user);
      SetAuthTokens(response);

      return Ok(new { message = "Registration successful" });
    }

    [HttpPost, Route("logout")]
    public IActionResult Logout()
    {
      var accessToken = Request.Cookies["accessToken"];
      var refreshToken = Request.Cookies["refreshToken"];
      var result = UserService.LogOut(accessToken, refreshToken);
      if (result == false) { return BadRequest(new { message = "We encountered an error while trying to log you out" }); }
      else { UnsetAuthTokens(); return Ok(); }
    }

    [HttpPost, Route("refreshtoken")]
    public IActionResult RefreshToken()
    {
      var refreshToken = Request.Cookies["refreshToken"];
      if (refreshToken == null) { return BadRequest(new { message = "User has been logged out" }); }
      var result = UserService.RefreshToken(refreshToken);
      if (result == null) { UnsetAuthTokens(); return BadRequest(new { message = "User has been logged out" }); }

      SetAuthTokens(result);
      return Ok();
    }

    /*
     * Helper methods
     */

    public void SetAuthTokens(AuthResponse response)
    {

      //var accessCookieOptions = new CookieOptions
      //{
      //  HttpOnly = false,
      //  Expires = DateTime.UtcNow.AddDays(1),
      //  SameSite = SameSiteMode.None,
      //  Secure = false
      //};

      //var refreshCookieOptions = new CookieOptions
      //{
      //  HttpOnly = true,
      //  Expires = DateTime.UtcNow.AddDays(30),
      //  SameSite = SameSiteMode.None,
      //  Secure = false
      //};

      //Response.Cookies.Append("accessToken", response.JwtToken, accessCookieOptions);
      //Response.Cookies.Append("refreshToken", response.RefreshToken, refreshCookieOptions);

      Response.Cookies.Append("accessToken", response.JwtToken, ConfigService.GetAccessCookieOptions());
      Response.Cookies.Append("refreshToken", response.RefreshToken, ConfigService.GetRefreshCookieOptions());
    }

    public void UnsetAuthTokens()
    {

      //Response.Cookies.Delete("accessToken");
      //Response.Cookies.Delete("refreshToken");

      Response.Cookies.Append("accessToken", "", ConfigService.GetLogoutAccessCookieOptions());
      Response.Cookies.Append("refreshToken", "", ConfigService.GetLogoutRefreshCookieOptions());

      //var accessCookieOptions = new CookieOptions
      //{
      //  HttpOnly = false,
      //  Expires = DateTime.UtcNow.AddMinutes(15),
      //  SameSite = SameSiteMode.None,
      //  Secure = false,
      //  IsEssential = true
      //};

      //var refreshCookieOptions = new CookieOptions
      //{
      //  HttpOnly = true,
      //  Expires = DateTime.UtcNow.AddDays(30),
      //  SameSite = SameSiteMode.None,
      //  Secure = false,
      //  IsEssential = true
      //};

      //Response.Cookies.Append("accessToken", "invalid", accessCookieOptions);
      //Response.Cookies.Append("refreshToken", "invalid", refreshCookieOptions);
    }

    public BadRequestObjectResult ValidateAuthModel(AuthModel model)
    {
      Match emailMatch = EmailRegex.Match(model.Email);

      if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
      {
        return BadRequest(new { message = "Email and Password must be filled" });
      }
      if (!emailMatch.Success)
      {
        return BadRequest(new { message = "Invalid Email" });
      }

      return null;
    }
  }




  public interface AuthModel
  {
    public string Email { get; set; }
    public string Password { get; set; }
  }

  public class LoginModel : AuthModel
  {
    public string Email { get; set; }
    public string Password { get; set; }
  }

  public class RegisterModel : AuthModel
  {
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
  }

}
