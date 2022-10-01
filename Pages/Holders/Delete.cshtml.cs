using IDCardDemo.Data;
using IDCardDemo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading.Tasks;

namespace IDCardDemo.Pages.Holders {
    [Authorize(Roles = "Administrator, Manager")]
    public class DeleteModel : PageModel {
        private readonly IDCardDemoContext _context;

        // Holds the root filepath of the web application; used for saves, etc.
        private readonly IWebHostEnvironment _environment;

        public DeleteModel(IDCardDemoContext context, IWebHostEnvironment environment) {
            _context = context;
            _environment = environment;
        }

        [BindProperty]
        public Holder Holder { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id) {
            if (id == null) {
                return NotFound();
            }

            Holder = await _context.Holder.FirstOrDefaultAsync(m => m.ID == id);

            if (Holder == null) {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id) {
            if (id == null) {
                return NotFound();
            }

            Holder = await _context.Holder.FindAsync(id);

            if (Holder != null) {
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
