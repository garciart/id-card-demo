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

        public EditModel(IDCardDemo.Data.IDCardDemoContext context)
        {
            _context = context;
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
