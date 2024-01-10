using System;
using System.Collections.Generic;
using Newtonsoft.Json;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ArtistAwards
{
  public partial class PollOption
  {
    public PollOption()
    {
      UserVotes = new HashSet<UserVotes>();
    }

    public int Id { get; set; }
    public string Content { get; set; }
    public int? Votes { get; set; }
    public Guid PollId { get; set; }

    public virtual Poll Poll { get; set; }
    [JsonIgnore]
    public virtual ICollection<UserVotes> UserVotes { get; set; }
  }
}
