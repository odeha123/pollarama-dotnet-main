using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PollAwards.Services;
using Microsoft.AspNetCore.Authorization;
using ArtistAwards;
using ArtistAwards.HelperModels;
using Microsoft.AspNetCore.Cors;

namespace DotNetAPI
{
  [Route("api/[controller]")]
  [ApiController]
  [EnableCors("EnableCORS")]
  public class PollController : ControllerBase
  {
    public PollController(PollService _pollService)
    {
      PollService = _pollService;
    }

    public PollService PollService;

    //[HttpGet]
    //public IEnumerable<Poll> GetPolls()
    //{
    //  return PollService.GetPolls();
    //  //string pollsJson = JsonSerializer.Serialize(PollService.GetPolls());
    //  //return pollsJson;
    //}

    [RouteAttribute("{id}")]
    [HttpGet]
    public async Task<Poll> GetPoll(Guid id)
    {
      Poll poll = await PollService.GetPoll(id);
      return poll;
      //string pollsJson = JsonSerializer.Serialize(PollService.GetPolls());
      //return pollsJson;
    }

    [Route("createpoll")]
    [HttpPost, Authorize(Roles = "voter")]
    [HttpPost]
    public async Task<Poll> CreatePoll([FromBody] Poll poll)
    {


      await PollService.CreatePoll(poll);

      return poll;
    }

    [Route("vote")]
    [HttpPost, Authorize(Roles = "voter")]
    public IActionResult Vote([FromBody] VoteRequest voteRequest)
    {
      var result = PollService.Vote(voteRequest.UserId, voteRequest.PollOptionId);

      if (result == false) { return BadRequest(new { message = "Your vote could not be completed" }); }
      else { return Ok("Your vote was completed"); }

    }

    //[Route("checkforpoll")]
    [RouteAttribute("checkforpoll/{pollId:Guid}")]
    [HttpGet]
    public bool CheckForPoll(Guid pollId)
    {
      var result = PollService.CheckForPoll(pollId);
      return result;
    }

    [Route("popularpolls")]
    [HttpGet]
    public IEnumerable<Poll> GetPopularPolls()
    {
      return PollService.GetPopularPolls();
    }

  }
}