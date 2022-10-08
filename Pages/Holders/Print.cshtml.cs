using IDCardDemo.Data;
using IDCardDemo.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IDCardDemo.Pages.Holders {
    public class PrintModel : PageModel {
        private readonly IDCardDemoContext _context;

        public PrintModel(IDCardDemoContext context) {
            _context = context;
        }

        public IList<Holder> Holder { get; set; }

        public async Task OnGetAsync() {
            Holder = await _context.Holder.ToListAsync();
        }
    }
}
