using ArtistAwards.Data;
using ArtistAwards.Helper_Models;
using DotNetAPI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BC = BCrypt.Net.BCrypt;

namespace ArtistAwards.Services
{
  public class UserService
  {
    public UserService(AppDbContext context, IConfiguration configuration)
    {
      ArtistContext = context;
      Config = configuration;
    }

    private AppDbContext ArtistContext;
    private IEnumerable<User> Users;
    private IConfiguration Config { get; }

    public IEnumerable<User> GetUsers()
    {
      Users = ArtistContext.Users;
      return Users;
    }

    public async Task<User> GetUser(int id)
    {
      User User = await ArtistContext.Users.FindAsync(id);
      return User;
    }

    public async Task<AuthResponse> CreateUser(User user)
    {
      ArtistContext.Users.Add(user);
      await ArtistContext.SaveChangesAsync();
      User registeredUser = ArtistContext.Users.SingleOrDefault(u => u.Email == user.Email);
      UserRole ur = new UserRole(1, registeredUser.Id);
      ArtistContext.Userroles.Add(ur);
      
      var accessToken = generateJwtToken(registeredUser);
      var refreshToken = generateRefreshToken();
      refreshToken.UserId = user.Id;
      ArtistContext.RefreshTokens.Add(refreshToken);

      await ArtistContext.SaveChangesAsync();
      return new AuthResponse(registeredUser, accessToken, refreshToken.Token);
    }

    public AuthResponse AuthenticateUser(string email, string password)
    {
      var user = ArtistContext.Users.SingleOrDefault(u => u.Email == email);
      if (user == null || !BC.Verify(password, user.Passwordhash)) return null;

      var accessToken = generateJwtToken(user);
      var refreshToken = generateRefreshToken();
      refreshToken.UserId = user.Id;
      ArtistContext.RefreshTokens.Add(refreshToken);
      //user.RefreshTokens.Add(refreshToken);
      //ArtistContext.Update(user);
      ArtistContext.SaveChanges();

      return new AuthResponse(user, accessToken, refreshToken.Token);
    }

    public bool LogOut(string accessToken, string refreshToken)
    {
      var result = false;
      var refreshTokenCheck = ArtistContext.RefreshTokens.SingleOrDefault(rt => rt.Token == refreshToken);
      if (refreshTokenCheck == null || refreshTokenCheck.IsActive == false) { }
      else
      {
        refreshTokenCheck.IsActive = false;
        ArtistContext.Update(refreshTokenCheck);
        ArtistContext.SaveChanges();
        result = true;
      }
      return result;
    }

    public AuthResponse RefreshToken(string refreshToken)
    {
      var refreshTokenCheck = ArtistContext.RefreshTokens.Include(rt => rt.User).SingleOrDefault(rt => rt.Token == refreshToken);
      if (refreshTokenCheck == null || refreshTokenCheck.IsActive == false) { return null; }

      var user = refreshTokenCheck.User;
      var newAccessToken = generateJwtToken(user);

      return new AuthResponse(user, newAccessToken, refreshToken);
    }





    //public async Task VoteAsync(int id)
    //{

    //  User UserToVote = await ArtistContext.Users.FindAsync(id);
    //  UserToVote.Votes += 1;

    //  await ArtistContext.SaveChangesAsync();
    //}



    public IEnumerable<Role> GetUserRoles(int userId)
    {
      var userRoleIds = ArtistContext.Userroles.Where(ur => ur.Userid == userId).Select(ur => ur.Roleid).ToList();
      var userRoles = ArtistContext.Roles.Where(r => userRoleIds.Contains(r.Id)).ToList();
      return userRoles;
    }

    private string generateJwtToken(User user)
    {
      var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Config.GetValue<string>("SecretKey")));
      var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
      var roles = GetUserRoles(user.Id);
      var claims = new List<Claim>();
      foreach (Role role in roles)
      {
        var claim = new Claim(ClaimTypes.Role, role.Name);
        claims.Add(claim);
      }
      var idClaim = new Claim(ClaimTypes.Sid, user.Id.ToString());
      var nameClaim = new Claim(ClaimTypes.Name, user.Name);
      claims.Add(idClaim);
      claims.Add(nameClaim);
      var tokenOptions = new JwtSecurityToken(
          issuer: "http://localhost:5000",
          audience: "http://localhost:5000",
          claims: claims,
          expires: DateTime.Now.AddMinutes(15),
          signingCredentials: signinCredentials
      );
      var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
      return tokenString;
    }

    private RefreshToken generateRefreshToken()
    {
      using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
      {
        var randomBytes = new byte[64];
        rngCryptoServiceProvider.GetBytes(randomBytes);
        return new RefreshToken
        {
          Token = Convert.ToBase64String(randomBytes),
          Expires = DateTime.UtcNow.AddDays(7),
          IsActive = true
        };
      }
    }

  }
}
