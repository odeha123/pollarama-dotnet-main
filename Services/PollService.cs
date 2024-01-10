using DotNetAPI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ArtistAwards;
using ArtistAwards.Data;

namespace PollAwards.Services
{
  public class PollService
  {
    public PollService(IWebHostEnvironment webHostEnvironment, AppDbContext context)
    {
      WebHostEnvironment = webHostEnvironment;
      DbContext = context;
    }

    public IWebHostEnvironment WebHostEnvironment { get; }
    private AppDbContext DbContext;

    public async Task<Poll> GetPoll(Guid id)
    {
      Poll poll = await DbContext.Polls.
        Include(p => p.Status).
        Include(p => p.PollOptions).
        Include(p => p.UserVotes).
        //Include(p => p.PollOptions.Select(po => po.Id)).
        //Include(p => p.PollOptions.Select(po => po.Content)).
        //Include(p => p.PollOptions.Select(po => po.Votes)).
        //Where().
        FirstOrDefaultAsync(p => p.Id == id);
      return poll;
    }

    public async Task<Poll> CreatePoll(Poll poll)
    {
      DbContext.Add(poll);
      await DbContext.SaveChangesAsync();

      return poll;
    }

    public bool Vote(int userId, int pollOptionId)
    {
      var result = false;
      var user = DbContext.Users.SingleOrDefault(u => u.Id == userId);
      if (user == null) return result;
      var pollOption = DbContext.PollOptions.SingleOrDefault(po => po.Id == pollOptionId);
      if (pollOption == null) return result;
      var poll = DbContext.Polls.SingleOrDefault(p => p.Id == pollOption.PollId);
      if (poll == null || poll.StatusId != 1) return result;
      var checkHasVoted = DbContext.UserVotes.SingleOrDefault(uv => uv.UserId == userId && uv.PollId == poll.Id);
      if (checkHasVoted != null) return result;

      var newVote = new UserVotes {PollId = poll.Id, UserId = user.Id, PollOptionId = pollOption.Id };
      pollOption.Votes += 1;
      DbContext.UserVotes.Add(newVote);
      DbContext.Update(pollOption);
      DbContext.SaveChanges();

      result = true;
      return result;
    }

    public bool CheckForPoll(Guid pollId)
    {
      var result = DbContext.Polls.Any(p => p.Id == pollId);
      return result;
    }

    public IEnumerable<Poll> GetPopularPolls()
    {
      var polls = DbContext.Polls.Take(10);

      return polls;
    }

  }
}
