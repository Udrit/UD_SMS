using System.ComponentModel.DataAnnotations;

namespace UdritDhakal_SMS.Models.Entity
{
    public class CourseInfo : BaseEntity
    {
        //public int StudentId { get; set; }
        [Required(ErrorMessage = "Please Enter Course Name")]
        [Display(Name = "Course Name")]
        public string CourseName { get; set; }
        [Required]
        [Display(Name = "Description")]
        public string CourseDescription { get; set; }
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }

        //public virtual StudentInfo StudentInfo { get; set; }
        public virtual ICollection<StudentInfo> StudentInfos { get; set; }
    }
}