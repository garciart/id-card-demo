using IDCardDemo.Data;
using IDCardDemo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ZXing;

namespace IDCardDemo.Pages.Holders {
    [Authorize(Policy = "ManagerOrAdminOnly")]
    public class EditModel : PageModel {
        private readonly IDCardDemoContext _context;

        // References to the HTML elements populated from the code-behind using loops, etc.
        public IEnumerable<SelectListItem> Heights { get; set; }
        public IEnumerable<SelectListItem> EyeColor { get; set; }

        // Status flags
        private static bool photoUploaded = false;
        private static bool signatureUploaded = false;
        private static string status;

        // Holds the root filepath of the web application; used for saves, etc.
        private readonly IWebHostEnvironment _environment;

        public EditModel(IDCardDemoContext context, IWebHostEnvironment environment) {
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

                // Update images if new ones were made
                if (photoUploaded) {
                    // Copy the temp images to photo folder and rename them using the holder data
                    string tempImageFilePath = Path.Combine(_environment.ContentRootPath, "wwwroot\\temp", "temp_photo.png");
                    string photoImagePath = Path.Combine(_environment.ContentRootPath, "wwwroot\\photos", Holder.PhotoPath);
                    UpdateImageFiles(tempImageFilePath, photoImagePath);
                }

                if (signatureUploaded) {
                    string tempImageFilePath = Path.Combine(_environment.ContentRootPath, "wwwroot\\temp", "temp_signature.png");
                    string signatureImagePath = Path.Combine(_environment.ContentRootPath, "wwwroot\\photos", Holder.SignaturePath);
                    UpdateImageFiles(tempImageFilePath, signatureImagePath);
                }

                // Prepare barcode info
                string holderInfo = String.Format("{0},{1},{2},{3},{4},{5},{6}\",EYES:{7}",
                 Holder.LastName.ToUpper(),
                 Holder.FirstName.ToUpper(),
                 String.IsNullOrEmpty(Holder.MI) ? "" : Holder.MI.ToUpper(),
                 "1 MAIN ST ANY TOWN MD 12345-0000",
                 Holder.DOB.ToString("MM/dd/yyyy"),
                 Holder.Gender.ToUpper(),
                 Holder.Height,
                 Holder.EyeColor);
                // Create and save barcode
                BarcodeWriter writer = new BarcodeWriter {
                    Format = BarcodeFormat.PDF_417,
                    Options = { Width = 342, Height = 100, Margin = 0 },
                };
                using (Bitmap barcodeBitmap = writer.Write(holderInfo)) {
                    barcodeBitmap.Save(Path.Combine(_environment.ContentRootPath, "wwwroot\\photos", Holder.PDF417Path), System.Drawing.Imaging.ImageFormat.Png);
                }

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

        private static void UpdateImageFiles(string tempImageFilePath, string targetImageFilePath) {
            // Attempt to update the file 20 times over 5 seconds before raising an error that the file is locked
            for (int x = 0; x <= 20; x++) {
                if (!IsFileLocked(new FileInfo(tempImageFilePath)) && !IsFileLocked(new FileInfo(targetImageFilePath))) {
                    System.IO.File.Copy(tempImageFilePath, targetImageFilePath, true);
                    // byte[] tempImageBytes = System.IO.File.ReadAllBytes(tempImageFilePath);
                    // System.IO.File.WriteAllBytes(targetImageFilePath, tempImageBytes);
                    return;
                }
                // Wait 1/4 second before trying again
                System.Threading.Thread.Sleep(250);
            }
            throw new System.IO.IOException(string.Format("Images are locked."));
            
        }

        // Thanks to ChrisW https://stackoverflow.com/questions/876473/is-there-a-way-to-check-if-a-file-is-in-use
        private static bool IsFileLocked(FileInfo file) {
            // Check if file is accessible
            try {
                using FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None); stream.Close();
            }
            catch (IOException) {
                // The file is inaccessible because it is still being written; it is being processed by another thread; or it does not exist (has already been processed)
                return true;
            }
            // The file is not locked
            return false;
        }

        private bool HolderExists(int id) {
            return _context.Holder.Any(e => e.ID == id);
        }
    }
}
