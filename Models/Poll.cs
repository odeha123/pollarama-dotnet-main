using Newtonsoft.Json;
using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ArtistAwards
{
    public partial class Poll
    {
        public Poll()
        {
            PollOptions = new HashSet<PollOption>();
            UserVotes = new HashSet<UserVotes>();
        }

        public Guid Id { get; set; }
        public string Title { get; set; }
        public int CreatorId { get; set; }
        public int StatusId { get; set; }

        public virtual PollStatus Status { get; set; }
        public virtual ICollection<PollOption> PollOptions { get; set; }
        //[JsonIgnore]
        public virtual ICollection<UserVotes> UserVotes { get; set; }
    }
}
