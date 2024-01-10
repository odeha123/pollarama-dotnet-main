using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArtistAwards.HelperModels
{
  public class VoteRequest
  {
    public int UserId { get; set; }
    public int PollOptionId { get; set; }
  }
}
