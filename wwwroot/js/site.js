


//var artists = [{"id": -1, "name": "Test", "votes": -1}];
var artists = [];
var artistsContainer = document.getElementById("artistsContainer");
var voteButtons;
var voteCheck = JSON.parse(localStorage.getItem("voteCheck"));
var lastFetch = new Date(sessionStorage.getItem("lastFetch"));
var currentDate = new Date();

if (voteCheck == null) {
  voteCheck = { voted: false, artistId: -1 };
}

/*
 * this code sets up the UI
 */

let displayArtistsPromise = new Promise(function (resolve, reject) {
  let tempArtists = JSON.parse(sessionStorage.getItem("artists"));
  if (tempArtists) {
    if (checkLastFetch() < 2) {
      artists = tempArtists;
    }
    else {
      console.log("time for refresh");
      fetchArtists();
    }
  }
  else {
    fetchArtists();
  }

  let checkArtists = setInterval(function () {
    if (artists.length != 0) {
      displayArtists(artists);
      clearInterval(checkArtists);
    }
  }, 100);
});

displayArtistsPromise.then($(document).ready(function () {
  let checkExist = setInterval(function () {
    if (artists.length != 0) {
      if (document.querySelector(`[data-idButton="${artists[artists.length - 1].id}"]`)) {
        ($(".loader-wrapper").fadeOut("slow"));
      }
    }
  }, 100);
}));

/*
 * end of setup
 */


/*
 * function that fetches all artists from the backend and caches them
 */
function fetchArtists() {
  fetch('/api/artist')
    .then(response => response.json())
    .then(tempArtists => { artists = tempArtists; sessionStorage.setItem("artists", JSON.stringify(artists)) })
    .then(sessionStorage.setItem("lastFetch", JSON.stringify(new Date())))
}

/*
 * function that displays all the UI that the user sees
 */
function displayArtists(artists) {
  artists.forEach(function (artist) {
    var artistDiv = document.createElement("div");
    artistDiv.classList.add("artist-div");
    artistDiv.appendChild(createNameSpan(artist.name));
    var textDiv = document.createElement("span")
    var text = document.createElement("span")
    text.innerHTML = "Number of votes: "
    textDiv.appendChild(text)
    textDiv.appendChild(createVoteSpan(artist.votes, artist.id));
    artistDiv.appendChild(textDiv);
    artistDiv.appendChild(createVoteButton(artist.name, artist.id));
    artistsContainer.appendChild(artistDiv);
  })
  setupVoteButtons();
}

/*
 * function that is used to cast a user vote
 */
function vote(id) {
  var idJson = { Id: id };


  if (voteCheck.voted == true) {
    alert("Sorry, you have already voted");
  }
  else {
    fetch('/artist/api/vote/', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json;charset=utf-8'
      },
      body: JSON.stringify(idJson)
    })
      .then(updateVoteSpan(document.querySelector(`[data-idSpan="${id}"]`)))
      .then(updateVoteButton(document.querySelector(`[data-idButton="${id}"]`)))
      .then(storeVote(id));

    clearLastFetch();
  }
}



/*******************************
 * 
 * utility functions
 * 
 *******************************/

/*
 * function that checks the difference between the time of the last data fetch and current time and returns it in minutes
 */
function checkLastFetch() {
  if (!JSON.parse(sessionStorage.getItem("lastFetch"))) { return 2; }
  let lastFetch = new Date(JSON.parse(sessionStorage.getItem("lastFetch")));
  console.log(lastFetch);
  let diff = Math.abs(new Date() - lastFetch);
  let diffMinutes = Math.floor((diff / 1000) / 60);
  console.log(diffMinutes + " minute(s)");
  return diffMinutes;
}

function clearLastFetch() {
  let pastTime = new Date(currentDate.getTime() - 5 * 60000);
  sessionStorage.setItem("lastFetch", JSON.stringify(pastTime));
}

function setupVoteButtons() {
  voteButtons = Array.from(document.getElementsByClassName("vote-button"));

  voteButtons.forEach(function (voteButton) {
    var id = parseInt(voteButton.getAttribute("data-idButton"), 10);
    voteButton.addEventListener("click", function () { vote(id) });
  });
}

function updateVoteSpan(voteSpan) {
  let oldVote = voteSpan.innerHTML.trim();
  oldVote = parseInt(oldVote, 10);
  let newVote = oldVote + 1;
  voteSpan.innerHTML = newVote;
}

function updateVoteButton(voteButton) {
  voteButton.classList.add("voted-button");
}

function storeVote(artistId) {
  voteCheck.voted = true;
  voteCheck.artistId = artistId;
  localStorage.setItem("voteCheck", JSON.stringify(voteCheck));
}

function clearStorage() {
  localStorage.clear();
  voteCheck = { voted: false, artistId: -1 };
  let voteButton = document.querySelector("[class='vote-button voted-button']");
  voteButton.classList.remove("voted-button");
  alert("Local Storage Cleared");
}

function createNameSpan(artistName) {
  var nameSpan = document.createElement("span");
  nameSpan.classList.add("name-span");
  nameSpan.innerHTML = artistName;
  return nameSpan;
}

function createVoteSpan(votes, artistId) {
  var voteSpan = document.createElement("span");
  voteSpan.classList.add("vote-span");
  voteSpan.innerHTML = votes;
  voteSpan.setAttribute("data-idSpan", artistId);
  return voteSpan;
}


function createVoteButton(artistName, artistId) {
  var voteButton = document.createElement("button");
  voteButton.innerHTML = "Vote for " + artistName;
  voteButton.classList.add("vote-button");
  if (artistId == voteCheck.artistId) {
    voteButton.classList.add("voted-button");
  }
  voteButton.setAttribute("data-idButton", artistId);
  return voteButton;
}