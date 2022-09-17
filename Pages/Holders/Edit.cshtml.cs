using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IDCardDemo.Data;
using IDCardDemo.Models;

namespace IDCardDemo.Pages.Holders
{
    public class EditModel : PageModel
    {
        private readonly IDCardDemo.Data.IDCardDemoContext _context;

        // References to the HTML elements populated from the code-behind using loops, etc.
        public IEnumerable<SelectListItem> Heights { get; set; }
        public IEnumerable<SelectListItem> EyeColor { get; set; }

        public EditModel(IDCardDemo.Data.IDCardDemoContext context)
        {
            _context = context;

            // Use loops to populate large dropdown lists
            Heights = Enumerable.Range(24, 72).Select(x => new SelectListItem
            {
                Value = x.ToString(),
                Text = String.Format("{0}\" ({1}\' {2}\")", x, (int)x / 12, x % 12),
                Selected = x == 69,
            });

            Dictionary<string, string> EyeColorDict = new Dictionary<string, string>() {
                { "BLK", "Black" },
                { "BLU", "Blue" },
                { "BRO", "Brown" },
                { "GRY", "Grey" },
                { "GRN", "Green" },
                { "HAZ", "Hazel" },
                { "MAR", "Maroon" },
                { "MUL", "Multicolor" },
                { "PNK", "Pink" },
                { "UNK", "Unknown" }
            };

            EyeColor = new SelectList(EyeColorDict, "Key", "Value");
        }

        [BindProperty]
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

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Holder).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HolderExists(Holder.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool HolderExists(int id)
        {
            return _context.Holder.Any(e => e.ID == id);
        }
    }
}
