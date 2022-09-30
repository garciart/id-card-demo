using System;
using System.ComponentModel.DataAnnotations;

namespace IDCardDemo.Models {
    public class Holder {
        /// <summary>
        /// <value>Property <c>ID</c> is the card holder's unique identifier.</value>
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// <value>Property <c>LastName</c> is required; can contain letters, spaces, apostrophes, dashes, and periods only; and must be less than 32 characters in length.</value>
        /// </summary>
        private string _lastName;
        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "{0} required.")]
        [RegularExpression(@"^([A-Za-z])([ '\-.A-Za-z]*)([A-Za-z])(\.?)$", ErrorMessage = "Letters, spaces, apostrophes, dashes, and periods only.")]
        [StringLength(32, MinimumLength = 2, ErrorMessage = "{0} must be between {2} and {1} characters long.")]
        public string LastName {
            get => _lastName;
            // Convert to uppercase before storing
            set => _lastName = !String.IsNullOrEmpty(value) ? value.ToUpper() : value;
        }

        /// <summary>
        /// <value>Property <c>FirstName</c> is required; can contain letters, spaces, apostrophes, dashes, and periods only; and must be less than 32 characters in length.</value>
        /// </summary>
        private string _firstName;
        [Display(Name = "First Name")]
        [Required(ErrorMessage = "{0} required.")]
        [RegularExpression(@"^([A-Za-z])([ '\-.A-Za-z]*)([A-Za-z])(\.?)$", ErrorMessage = "Letters, spaces, apostrophes, dashes, and periods only.")]
        [StringLength(32, MinimumLength = 2, ErrorMessage = "{0} must be between {2} and {1} characters long.")]
        public string FirstName {
            get => _firstName;
            // Convert to uppercase before storing
            set => _firstName = !String.IsNullOrEmpty(value) ? value.ToUpper() : value;
        }

        /// <summary>
        /// <value>Property <c>Middle Initial</c> is not required, but if present, it must be a single capital letter.</value>
        /// </summary>
        private string _mi;
        [Display(Name = "MI")]
        [RegularExpression(@"^[A-Z]?$", ErrorMessage = "Middle Initial must be a capital letter.")]
        [StringLength(1, MinimumLength = 0, ErrorMessage = "Middle Initial must be between {2} and {1} characters long.")]
#nullable enable
        public string? MI {
            get => _mi;
            // Convert to uppercase before storing
            set => _mi = !String.IsNullOrEmpty(value) ? value.ToUpper() : value;
        }
#nullable disable

        /// <summary>
        /// <value>Property <c>DOB</c> is required, and must be in yyyy-MM-dd format (e.g., 2001-06-12).</value>
        /// </summary>
        [DataType(DataType.Date)]
        [Display(Name = "DOB")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Date of Birth required.")]
        [NoFutureDOB(ErrorMessage = "Date of Birth cannot be in the future.")]
        public DateTime DOB { get; set; }

        /// <summary>
        /// <value>Property <c>Gender</c> is required and must be a capital letter 'M', 'F', or 'N'.</value>
        /// </summary>
        [Display(Name = "Sex")]
        [RegularExpression(@"^[MFN]$", ErrorMessage = "Invalid Gender.")]
        [Required(ErrorMessage = "Gender required.")]
        public string Gender { get; set; }

        /// <summary>
        /// <value>Property <c>Height</c> is required and must be an integer between 24 and 96.</value>
        /// </summary>
        [Display(Name = "HT")]
        [Range(24, 96, ErrorMessage = "Height must be between {1} and {2} inches.")]
        [Required(ErrorMessage = "Height (in inches) required.")]
        public string Height { get; set; }

        /// <summary>
        /// <value>Property <c>EyeColor</c> is required and must be 'BLK', 'BLU', 'BRO', 'GRY', 'GRN', 'HAZ', 'MAR', 'MUL', 'PMK', or 'UNK'.</value>
        /// </summary>
        [Display(Name = "Eyes")]
        [RegularExpression("BLK|BLU|BRO|GRY|GRN|HAZ|MAR|MUL|PMK|UNK", ErrorMessage = "Invalid Eye Color")]
        [Required(ErrorMessage = "Eye Color required.")]
        public string EyeColor { get; set; }

        /// <summary>
        /// <value>Property <c>PhotoPath</c> holds the filepath to the card holder's photo on the server.</value>
        /// </summary>
        [Display(Name = "Photo")]
        public string PhotoPath { get; set; }

        /// <summary>
        /// <value>Property <c>SignaturePath</c> holds the filepath to the card holder's signature on the server.</value>
        /// </summary>
        [Display(Name = "Signature")]
        public string SignaturePath { get; set; }

        /// <summary>
        /// <value>Property <c>PDF417Path</c> holds the filepath, on the server, to the PDF-417 barcode generated for the card holder.</value>
        /// </summary>
        [Display(Name = "PDF-417")]
        public string PDF417Path { get; set; }
    }

    public class NoFutureDOB : ValidationAttribute {
        public override bool IsValid(object value) {
            var dateValue = value as DateTime? ?? new DateTime();
            if (dateValue.Date > DateTime.Now.Date) {
                return false;
            }
            return true;
        }
    }
}