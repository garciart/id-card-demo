using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using IDCardDemo.Data;
using IDCardDemo.Models;

namespace IDCardDemo.Pages.Holders
{
    public class CreateModel : PageModel
    {
        private readonly IDCardDemo.Data.IDCardDemoContext _context;

        public CreateModel(IDCardDemo.Data.IDCardDemoContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Holder Holder { get; set; }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Holder.Add(Holder);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
