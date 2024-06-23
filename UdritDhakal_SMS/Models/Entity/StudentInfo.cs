using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UdritDhakal_SMS.Models.Entity
{
    public class StudentInfo : BaseEntity
    {
        [Display(Name = "Course Name")]
        public int CourseId { get; set; }
        [Required]
        [Display(Name = "Student Name")]
        public string Name { get; set; }
        [Display(Name = "Email ")]
        public string Email { get; set; }
        [Required]
        [StringLength(20)]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string Class { get; set; }
        [Required]
        public string Section { get; set; }
        public string ImageUrl { get; set; }
        [NotMapped]
        public IFormFile ImageFile { get; set; }
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public virtual CourseInfo CourseInfo { get; set; }

    }
}
