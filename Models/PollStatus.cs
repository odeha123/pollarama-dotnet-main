using System;
using System.Collections.Generic;
using Newtonsoft.Json;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ArtistAwards
{
    public partial class PollStatus
    {
        public PollStatus()
        {
            Polls = new HashSet<Poll>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        [JsonIgnore]
        public virtual ICollection<Poll> Polls { get; set; }
    }
}
