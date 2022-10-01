using IDCardDemo.Data;
using IDCardDemo.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace IDCardDemo.Pages.Holders {
    public class DetailsModel : PageModel {
        private readonly IDCardDemoContext _context;

        // Holds the root filepath of the web application; used for saves, etc.
        private readonly IWebHostEnvironment _environment;

        public DetailsModel(IDCardDemoContext context, IWebHostEnvironment environment) {
            _context = context;
            _environment = environment;
        }

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

        public async Task<IActionResult> OnPostPrintCard(int? id) {
            if (id == null) {
                return NotFound();
            }

            Holder = await _context.Holder.FirstOrDefaultAsync(m => m.ID == id);
            // Load images
            Bitmap id_front_bitmap = new Bitmap(Path.Combine(_environment.ContentRootPath, "wwwroot\\images", "id_card_front.png"));
            Bitmap id_back_bitmap = new Bitmap(Path.Combine(_environment.ContentRootPath, "wwwroot\\images", "id_card_back.png"));
            Graphics id_front_Graphics = Graphics.FromImage(id_front_bitmap);
            Graphics id_back_Graphics = Graphics.FromImage(id_back_bitmap);

            // Draw front of card, signature first
            Image holder_signature = Image.FromFile(Path.Combine(_environment.ContentRootPath, "wwwroot\\photos", Holder.SignaturePath));
            holder_signature = resizeImage(holder_signature, new Size(240, 90));
            Bitmap holder_signature_clear = new Bitmap(holder_signature);

            // Make signature background color transparent
            holder_signature_clear.MakeTransparent(Color.White);
            id_front_Graphics.DrawImage(holder_signature_clear, 90, 200);
            Image holder_picture = null;
            try {
                holder_picture = Image.FromFile(Path.Combine(_environment.ContentRootPath, "wwwroot\\photos", Holder.PhotoPath));
            }
            catch {
                holder_picture = Image.FromFile(Path.Combine(_environment.ContentRootPath, "wwwroot\\images", "no_picture.png"));
            }
            holder_picture = resizeImage(holder_picture, new Size(160, 160));
            id_front_Graphics.DrawImage(holder_picture, 305, 72);

            // Create font and brush, then add info
            Font drawFont = new Font("Arial", 13f, FontStyle.Bold);
            SolidBrush drawBrush = new SolidBrush(Color.Black);
            id_front_Graphics.DrawString(String.Format("{0},\r\n{1} {2}\r\n\r\n1 MAIN ST\r\nANY TOWN, MD 12345\r\n\r\nDOB: {3}\r\nSEX: {4} / HT: {5}\"\r\nEYES: {6}", Holder.LastName, Holder.FirstName, Holder.MI, Holder.DOB.ToShortDateString(), Holder.Gender, Holder.Height, Holder.EyeColor), drawFont, drawBrush, 92, 45);

            // Draw back of card
            Image holder_barcode = Image.FromFile(Path.Combine(_environment.ContentRootPath, "wwwroot\\photos", Holder.PDF417Path));
            id_back_Graphics.DrawImage(holder_barcode, 25, 170);

            // Create a new PDF document
            PdfDocument document = new PdfDocument();
            document.Info.Title = String.Format("ID Card for {0}, {1} {2}.", Holder.LastName, Holder.FirstName, Holder.MI);

            // Create empty pages
            PdfPage page1 = document.AddPage();
            page1.Height = XUnit.FromInch(3.16);
            page1.Width = XUnit.FromInch(5);
            PdfPage page2 = document.AddPage();
            page2.Height = XUnit.FromInch(3.16);
            page2.Width = XUnit.FromInch(5);

            // Get XGraphics objects for drawing
            XGraphics gfx1 = XGraphics.FromPdfPage(page1);
            XGraphics gfx2 = XGraphics.FromPdfPage(page2);
            gfx2.SmoothingMode = XSmoothingMode.HighQuality;
            XImage temp_image = null;
            temp_image = XImage.FromStream(bitmapToStream(id_front_bitmap, ImageFormat.Png));
            gfx1.DrawImage(temp_image, 0, 0);
            id_back_bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
            temp_image = XImage.FromStream(bitmapToStream(id_back_bitmap, ImageFormat.Png));
            gfx2.DrawImage(temp_image, 0, 0);
            // Save to PDF
            string pdf_filename = Path.Combine(_environment.ContentRootPath, "wwwroot\\temp", "temp_card.pdf");
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            document.Save(pdf_filename);

            id_front_bitmap.Dispose();
            id_back_bitmap.Dispose();
            id_front_Graphics.Dispose();
            id_back_Graphics.Dispose();

            // Special Thanks to CodeCaster at https://stackoverflow.com/questions/40486431/return-pdf-to-the-browser-using-asp-net-core
            var stream = new FileStream(pdf_filename, FileMode.Open);
            return new FileStreamResult(stream, "application/pdf");
        }
        // Special thanks to Guffa http://stackoverflow.com/questions/1120198/most-efficient-way-to-remove-special-characters-from-string
        public static string RemoveSpecialCharacters(string str) {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str) {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z')) {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
        public static Image resizeImage(Image imgToResize, Size size) {
            return (Image)(new Bitmap(imgToResize, size));
        }

        public static Stream bitmapToStream(Bitmap bitmap, ImageFormat format) {
            var stream = new MemoryStream();
            bitmap.Save(stream, format);
            stream.Position = 0;
            return stream;
        }
    }
}
