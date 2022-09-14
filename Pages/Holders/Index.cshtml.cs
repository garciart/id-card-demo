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
    public class IndexModel : PageModel
    {
        private readonly IDCardDemo.Data.IDCardDemoContext _context;

        public IndexModel(IDCardDemo.Data.IDCardDemoContext context)
        {
            _context = context;
        }

        public IList<Holder> Holder { get;set; }

        public async Task OnGetAsync()
        {
            Holder = await _context.Holder.ToListAsync();
        }
    }
}
