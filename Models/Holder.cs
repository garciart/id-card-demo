using System;
using System.ComponentModel.DataAnnotations;

namespace IDCardDemo.Models
{
    public class Holder
    {
        public int ID { get; set; }
        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "{0} required.")]
        [RegularExpression(@"^([ '\-A-Za-z.]{1,255})$", ErrorMessage = "Letters, spaces, apostrophes, dashes, and periods only.")]
        [StringLength(255, ErrorMessage = "{0} must be between {2} and {1} characters long.", MinimumLength = 1)]
        public string LastName { get; set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        public string MI { get; set; }
        [DataType(DataType.Date)]
        public DateTime DOB { get; set; }
        public string Gender { get; set; }
        public string Height { get; set; }
        [Display(Name = "Eye Color")]
        public string EyeColor { get; set; }
    }
}