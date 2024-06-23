using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace UdritDhakal_SMS.Data
{
    public class AppUser : IdentityUser
    {
        [Display(Name = "First Name")]
        public String FirstName { get; set; }
        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }
        [Display(Name = "Last Name")]
        public String LastName { get; set; }
        //[Display(Name = "Student Name")]
        //public int StudentId { get; set; }
        [Display(Name = "Address")]
        public string Address { get; set; }
        [Display(Name = "User Role")]
        public string UserRoleId { get; set; }
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }
        public string PictureUrl { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
