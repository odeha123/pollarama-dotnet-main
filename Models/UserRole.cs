using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ArtistAwards
{
  public partial class UserRole
  {
    public int Id { get; set; }
    public int Userid { get; set; }
    public int Roleid { get; set; }

    //public virtual Role Role { get; set; }
    //public virtual User User { get; set; }
    public virtual Role Role { get; set; }
    public virtual User User { get; set; }

    public UserRole()
    {
    }


    public UserRole(int roleId, int userId)
    {
      Userid = userId;
      Roleid = roleId;
    }
  }
}
