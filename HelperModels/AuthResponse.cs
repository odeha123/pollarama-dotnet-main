using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArtistAwards.Helper_Models
{
  public class AuthResponse
  {
    public User User { get; set; }
    [JsonIgnore]
    public string JwtToken { get; set; }

    [JsonIgnore] 
    public string RefreshToken { get; set; }

    public AuthResponse(User user, string jwtToken, string refreshToken)
    {
      User = user;
      JwtToken = jwtToken;
      RefreshToken = refreshToken;
    }
  }
}
