using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ArtistAwards.Data;
using DotNetAPI;

namespace ArtistAwards
{
    public class IndexModel : PageModel
    {
        private readonly ArtistAwards.Data.ArtistContext _context;

        public IndexModel(ArtistAwards.Data.ArtistContext context)
        {
            _context = context;
        }

        public IList<Artist> Artist { get;set; }

        public async Task OnGetAsync()
        {
            Artist = await _context.Artist.ToListAsync();
        }
    }
}
