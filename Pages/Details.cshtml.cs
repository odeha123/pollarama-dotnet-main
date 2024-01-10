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
    public class DetailsModel : PageModel
    {
        private readonly ArtistAwards.Data.ArtistContext _context;

        public DetailsModel(ArtistAwards.Data.ArtistContext context)
        {
            _context = context;
        }

        public Artist Artist { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Artist = await _context.Artist.FirstOrDefaultAsync(m => m.Id == id);

            if (Artist == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
