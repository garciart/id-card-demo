using System;
using System.ComponentModel.DataAnnotations;

namespace IDCardDemo.Models
{
    public class Holder
    {
        /// <summary>
        /// <value>Property <c>ID</c> is the card holder's unique identifier.</value>
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// <value>Property <c>LastName</c> is required; can contain letters, spaces, apostrophes, dashes, and periods only; and must be less than 255 characters in length.</value>
        /// </summary>
        private string _lastName;
        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "{0} required.")]
        [RegularExpression(@"^([ '\-A-Za-z.]{1,255})$", ErrorMessage = "Letters, spaces, apostrophes, dashes, and periods only.")]
        [StringLength(255, ErrorMessage = "{0} must be between {2} and {1} characters long.", MinimumLength = 1)]
        public string LastName {
            get => _lastName;
            // Convert to uppercase before storing
            set => _lastName = !String.IsNullOrEmpty(value) ? value.ToUpper() : value;
        }

        /// <summary>
        /// <value>Property <c>FirstName</c> is required; can contain letters, spaces, apostrophes, dashes, and periods only; and must be less than 127 characters in length.</value>
        /// </summary>
        private string _firstName;
        [Display(Name = "First Name")]
        [Required(ErrorMessage = "{0} required.")]
        [RegularExpression(@"^([ '\-A-Za-z.]{1,127})$", ErrorMessage = "Letters, spaces, apostrophes, dashes, and periods only.")]
        [StringLength(127, ErrorMessage = "{0} must be between {2} and {1} characters long.", MinimumLength = 1)]
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
        [RegularExpression(@"^[A-Z]{0,1}$", ErrorMessage = "Must be a capital letter.")]
        [StringLength(1, ErrorMessage = "Middle Initial can only be one letter long.")]
        #nullable enable
        public string? MI {
            get => _mi;
            // Convert to uppercase before storing
            set => _mi = !String.IsNullOrEmpty(value) ? value.ToUpper() : value;
        }
        #nullable disable

        /// <summary>
        /// <value>Property <c>Middle Initial</c> is not required, but if present, it can only be a single capital letter.</value>
        /// </summary>
        [DataType(DataType.Date)]
        [Display(Name = "DOB")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Date of Birth required.")]
        public DateTime DOB { get; set; }

        /// <summary>
        /// <value>Property <c>Gender</c> is required and must be a capital letter 'M', 'F', or 'N'.</value>
        /// </summary>
        [Display(Name = "Sex")]
        [RegularExpression(@"^[MFN]{1}$", ErrorMessage = "Invalid Gender.")]
        [Required(ErrorMessage = "Gender required.")]
        [StringLength(1, ErrorMessage = "Gender can only be one letter long.")]
        public string Gender { get; set; }

        /// <summary>
        /// <value>Property <c>Height</c> is required and must be an integer between 24 and 96.</value>
        /// </summary>
        [Display(Name = "HT")]
        [Range(24, 96, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        [Required(ErrorMessage = "Height (in inches) required.")]
        public string Height { get; set; }

        /// <summary>
        /// <value>Property <c>EyeColor</c> is required and must be 'BLK', 'BLU', 'BRO', 'GRY', 'GRN', 'HAZ', 'MAR', 'MUL', 'PMK', or 'UNK'.</value>
        /// </summary>
        [Display(Name = "Eyes")]
        [RegularExpression("BLK|BLU|BRO|GRY|GRN|HAZ|MAR|MUL|PMK|UNK", ErrorMessage = "Invalid Eye Color")]
        [Required(ErrorMessage = "Eye Color required.")]
        public string EyeColor { get; set; }
    }
}