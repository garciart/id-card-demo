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
using System.Linq;
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

        public IActionResult OnPostPrintCard(int? id) {
            if (id == null) {
                return NotFound();
            }

            // Holder = await _context.Holder.FirstOrDefaultAsync(m => m.ID == id);
            Holder = _context.Holder.FirstOrDefault(m => m.ID == id);

            string pdf_filename = Path.Combine(_environment.ContentRootPath, "wwwroot\\temp", "temp_card.pdf");

            // Load images
            using (Bitmap id_front_bitmap = new Bitmap(Path.Combine(_environment.ContentRootPath, "wwwroot\\images", "id_card_front.png")),
                  id_back_bitmap = new Bitmap(Path.Combine(_environment.ContentRootPath, "wwwroot\\images", "id_card_back.png"))) {
                using Graphics id_front_Graphics = Graphics.FromImage(id_front_bitmap),
                       id_back_Graphics = Graphics.FromImage(id_back_bitmap);
                
                // Draw front of card, signature first
                string holder_signature_path = Path.Combine(_environment.ContentRootPath, "wwwroot\\photos", Holder.SignaturePath);
                if (!System.IO.File.Exists(holder_signature_path)) {
                    holder_signature_path = Path.Combine(_environment.ContentRootPath, "wwwroot\\images", "no_signature.png");
                }
                using (Image holder_signature = Image.FromFile(holder_signature_path)) {
                    Image resized_holder_signature = ResizeImage(holder_signature, new Size(240, 90));
                    using Bitmap holder_signature_clear = new Bitmap(resized_holder_signature);
                    // Make signature background color transparent
                    holder_signature_clear.MakeTransparent(Color.White);
                    id_front_Graphics.DrawImage(holder_signature_clear, 90, 200);
                }

                string holder_picture_path = Path.Combine(_environment.ContentRootPath, "wwwroot\\photos", Holder.PhotoPath);
                if (!System.IO.File.Exists(holder_picture_path)) {
                    holder_picture_path = Path.Combine(_environment.ContentRootPath, "wwwroot\\images", "no_picture.png");
                }
                using (Image holder_picture = Image.FromFile(holder_picture_path)) {
                    Image resized_holder_picture = ResizeImage(holder_picture, new Size(160, 160));
                    id_front_Graphics.DrawImage(resized_holder_picture, 305, 72);
                }

                // Create font and brush, then add info
                using (Font drawFont = new Font("Arial", 13f, FontStyle.Bold)) {
                    using SolidBrush drawBrush = new SolidBrush(Color.Black); id_front_Graphics.DrawString(String.Format("{0},\r\n{1} {2}\r\n\r\n1 MAIN ST\r\nANY TOWN, MD 12345\r\n\r\nDOB: {3}\r\nSEX: {4} / HT: {5}\"\r\nEYES: {6}", Holder.LastName, Holder.FirstName, Holder.MI, Holder.DOB.ToShortDateString(), Holder.Gender, Holder.Height, Holder.EyeColor), drawFont, drawBrush, 92, 45);
                }

                // Draw back of card
                string holder_barcode_path = Path.Combine(_environment.ContentRootPath, "wwwroot\\photos", Holder.PDF417Path);
                if (!System.IO.File.Exists(holder_barcode_path)) {
                    holder_barcode_path = Path.Combine(_environment.ContentRootPath, "wwwroot\\images", "no_picture.png");
                }
                using (Image holder_barcode = Image.FromFile(holder_barcode_path)) {
                    id_back_Graphics.DrawImage(holder_barcode, 25, 170);
                }

                // Create a new PDF document
                using PdfDocument document = new PdfDocument(); document.Info.Title = String.Format("ID Card for {0}, {1} {2}.", Holder.LastName, Holder.FirstName, Holder.MI);
                // Create empty pages
                PdfPage page1 = document.AddPage();
                page1.Height = XUnit.FromInch(3.16);
                page1.Width = XUnit.FromInch(5);
                PdfPage page2 = document.AddPage();
                page2.Height = XUnit.FromInch(3.16);
                page2.Width = XUnit.FromInch(5);

                // Get XGraphics objects for drawing
                using (XGraphics gfx1 = XGraphics.FromPdfPage(page1), gfx2 = XGraphics.FromPdfPage(page2)) {
                    using (XImage temp_image = XImage.FromStream(BitmapToStream(id_front_bitmap, ImageFormat.Png))) {
                        gfx1.DrawImage(temp_image, 0, 0);
                    }
                    gfx2.SmoothingMode = XSmoothingMode.HighQuality;
                    using (XImage temp_image = XImage.FromStream(BitmapToStream(id_back_bitmap, ImageFormat.Png))) {
                        id_back_bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        gfx2.DrawImage(temp_image, 0, 0);
                    }
                }
                // Save to PDF
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                document.Save(pdf_filename);
            }
            return new PhysicalFileResult(pdf_filename, "application/pdf");
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
        public static Image ResizeImage(Image imgToResize, Size size) {
            return (Image)(new Bitmap(imgToResize, size));
        }

        public static Stream BitmapToStream(Bitmap bitmap, ImageFormat format) {
            var stream = new MemoryStream();
            bitmap.Save(stream, format);
            stream.Position = 0;
            return stream;
        }
    }
}
