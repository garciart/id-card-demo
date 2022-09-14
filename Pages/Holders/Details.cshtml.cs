using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using IDCardDemo.Data;
using IDCardDemo.Models;

namespace IDCardDemo.Pages.Holders
{
    public class DetailsModel : PageModel
    {
        private readonly IDCardDemo.Data.IDCardDemoContext _context;

        public DetailsModel(IDCardDemo.Data.IDCardDemoContext context)
        {
            _context = context;
        }

        public Holder Holder { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Holder = await _context.Holder.FirstOrDefaultAsync(m => m.ID == id);

            if (Holder == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
