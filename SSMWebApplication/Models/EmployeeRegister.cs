using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SSMWebApplication.Models
{
    public class EmployeeRegister
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmpId { get; set; }
  
        [StringLength(150)]
        public string Name { get; set; }
        
        [StringLength(5)]
        [Required(ErrorMessage = "Please select gender")]
        public string Gender { get; set; }
  
        [StringLength(150)]
        [Required(ErrorMessage = "Please select qualification")]
        public string Qualification { get; set; }

        [Required(ErrorMessage = "Please enter age")]
        [Range(16, 60, ErrorMessage = "Please enter age between 16 to 60")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Please select city")]
        public int? City { get; set; }

        [Required(ErrorMessage = "Please select state")]
        public int State { get; set; }

        [Required(ErrorMessage = "Please enter email id")]
        [StringLength(100)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter password")]
        [StringLength(50),Range(6,16,ErrorMessage ="Please enter password range between 6 and 16")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Compare("Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Please upload profile picture")]
        [StringLength(350)]
        public string ProfilePic { get; set; }

    }
}