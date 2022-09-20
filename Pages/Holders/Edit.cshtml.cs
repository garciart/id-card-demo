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
using Microsoft.AspNetCore.Hosting;
using System.Drawing;
using System.IO;
using System.Text;
using ZXing;

namespace IDCardDemo.Pages.Holders {
    public class EditModel : PageModel {
        private readonly IDCardDemo.Data.IDCardDemoContext _context;

        // References to the HTML elements populated from the code-behind using loops, etc.
        public IEnumerable<SelectListItem> Heights { get; set; }
        public IEnumerable<SelectListItem> EyeColor { get; set; }

        // Status flags
        private static bool photoUploaded = false;
        private static bool signatureUploaded = false;
        private static string status;

        // Holds the root filepath of the web application; used for saves, etc.
        private readonly IWebHostEnvironment _environment;

        public EditModel(IDCardDemo.Data.IDCardDemoContext context, IWebHostEnvironment environment) {
            _context = context;
            _environment = environment;

            // Use loops to populate large dropdown lists
            Heights = Enumerable.Range(24, 72).Select(x => new SelectListItem {
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

        // Method to update temp photo using Ajax
        // Remember to prepend OnPost to method name
        public JsonResult OnPostUpdatePhoto([FromBody] string imageData) {
            if (String.IsNullOrWhiteSpace(imageData)) return null;
            byte[] data = Convert.FromBase64String(imageData);
            string filepath = Path.Combine(_environment.ContentRootPath, "wwwroot\\temp", "temp_photo.png");
            System.IO.File.WriteAllBytes(filepath, data);
            Holder.PhotoPath = filepath;
            photoUploaded = true;
            return new JsonResult(filepath);
        }

        // Method to update temp signature using Ajax
        // Remember to prepend OnPost to method name
        public JsonResult OnPostUpdateSignature([FromBody] string imageData) {
            if (String.IsNullOrWhiteSpace(imageData)) return null;
            byte[] data = Convert.FromBase64String(imageData);
            string filepath = Path.Combine(_environment.ContentRootPath, "wwwroot\\temp", "temp_signature.png");
            System.IO.File.WriteAllBytes(filepath, data);
            Holder.SignaturePath = filepath;
            signatureUploaded = true;
            return new JsonResult(filepath);
        }

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

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync() {
            try {
                if (!ModelState.IsValid) {
                    status = "Invalid information!";
                    // Reset the flags and return the page with errors identified
                    // The model validation fields will populate the error messages automatically
                    photoUploaded = false;
                    signatureUploaded = false;
                    return Page();
                }

                // Create the filename prefix
                //string fixedLastName = RemoveSpecialCharacters(Holder.LastName.ToLower());
                //string fixedFirstName = RemoveSpecialCharacters(Holder.FirstName.ToLower());
                //string timeStamp = DateTime.Now.ToString("yyyMMddHHmmss");
                //string userFileName = String.Format("{0}_{1}_{2}", fixedLastName.ToLower(), fixedFirstName.ToLower(), timeStamp);

                // Rename image files if the member's LastName or FirstName fields changed
                //if (!Holder.PhotoPath.StartsWith(userFileName)) {
                //    System.IO.File.Move(
                //        Path.Combine(_environment.ContentRootPath, "wwwroot\\photos", Holder.PhotoPath),
                //        Path.Combine(_environment.ContentRootPath, "wwwroot\\photos", String.Format("{0}_photo.png", userFileName))
                //        );
                //    Holder.PhotoPath = String.Format("{0}_photo.png", userFileName);

                //    System.IO.File.Move(
                //        Path.Combine(_environment.ContentRootPath, "wwwroot\\photos", Holder.SignaturePath),
                //        Path.Combine(_environment.ContentRootPath, "wwwroot\\photos", String.Format("{0}_sign.png", userFileName))
                //        );
                //    Holder.SignaturePath = String.Format("{0}_sign.png", userFileName);

                //    System.IO.File.Move(
                //        Path.Combine(_environment.ContentRootPath, "wwwroot\\photos", Holder.PDF417Path),
                //        Path.Combine(_environment.ContentRootPath, "wwwroot\\photos", String.Format("{0}_code.png", userFileName))
                //        );
                //    Holder.PDF417Path = String.Format("{0}_code.png", userFileName);
                //}

                // Update images if new ones were made
                if (photoUploaded) {
                    // Copy the temp images to photo folder and rename them using the member data
                    string photoImagePath = Path.Combine(_environment.ContentRootPath, "wwwroot\\temp", "temp_photo.png");
                    //Holder.PhotoPath = String.Format("{0}_photo.png", userFileName);
                    System.IO.File.Copy(photoImagePath, Path.Combine(_environment.ContentRootPath, "wwwroot\\photos", Holder.PhotoPath), true);
                }

                if (signatureUploaded) {
                    string signatureImagePath = Path.Combine(_environment.ContentRootPath, "wwwroot\\temp", "temp_signature.png");
                    //Holder.SignaturePath = String.Format("{0}_sign.png", userFileName);
                    System.IO.File.Copy(signatureImagePath, Path.Combine(_environment.ContentRootPath, "wwwroot\\photos", Holder.SignaturePath), true);
                }

                // Prepare barcode info
                string memberInfo = String.Format("{0},{1},{2},{3},{4},{5},{6},{7}",
                 Holder.LastName.ToUpper(),
                 Holder.FirstName.ToUpper(),
                 String.IsNullOrEmpty(Holder.MI) ? "" : Holder.MI.ToUpper(),
                 "1 MAIN ST ANY TOWN MD 12345-0000",
                 Holder.DOB.ToString("MM/dd/yyyy"),
                 Holder.Gender.ToUpper(),
                 Holder.Height,
                 Holder.EyeColor);
                // Create and save barcode
                // http://stackoverflow.com/questions/13289742/zxing-net-encode-string-to-qr-code-in-cf //
                BarcodeWriter writer = new BarcodeWriter {
                    Format = BarcodeFormat.PDF_417,
                    Options = { Width = 342, Height = 100 },
                };
                Bitmap barcodeBitmap;
                writer.Options.Margin = 0;
                barcodeBitmap = writer.Write(memberInfo);
                //Holder.PDF417Path = String.Format("{0}_code.png", userFileName);
                barcodeBitmap.Save(Path.Combine(_environment.ContentRootPath, "wwwroot\\photos", Holder.PDF417Path), System.Drawing.Imaging.ImageFormat.Png);
                barcodeBitmap.Dispose();

                _context.Attach(Holder).State = EntityState.Modified;

                try {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException e) {
                    status = String.Format("Could not update record: {0}", e);
                    if (!HolderExists(Holder.ID)) {
                        return NotFound();
                    }
                    else {
                        throw;
                    }
                }

                int holderID = Holder.ID;
                ModelState.Clear();
                return RedirectToPage("./Details", new { id = holderID });
            }
            catch (Exception e) {
                status = String.Format("Could not update record: {0}", e);
                return Page();
            }
            finally {
                ViewData["Status"] = status;
                photoUploaded = false;
                signatureUploaded = false;
            }
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

        private bool HolderExists(int id) {
            return _context.Holder.Any(e => e.ID == id);
        }
    }
}
