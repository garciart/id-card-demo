using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using IDCardDemo.Data;
using IDCardDemo.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace IDCardDemo.Pages.Holders
{
    public class DeleteModel : PageModel
    {
        private readonly IDCardDemo.Data.IDCardDemoContext _context;

        // Holds the root filepath of the web application; used for saves, etc.
        private readonly IWebHostEnvironment _environment;

        public DeleteModel(IDCardDemo.Data.IDCardDemoContext context, IWebHostEnvironment environment) {
            _context = context;
            _environment = environment;
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Holder = await _context.Holder.FindAsync(id);

            if (Holder != null)
            {
                _context.Holder.Remove(Holder);
                System.IO.File.Delete(Path.Combine(_environment.ContentRootPath, "wwwroot\\photos", Holder.PDF417Path));
                System.IO.File.Delete(Path.Combine(_environment.ContentRootPath, "wwwroot\\photos", Holder.PhotoPath));
                System.IO.File.Delete(Path.Combine(_environment.ContentRootPath, "wwwroot\\photos", Holder.SignaturePath));
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
