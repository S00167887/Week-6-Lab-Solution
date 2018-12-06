using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Week6.DataDomain
{
    public class Student
    {
        [Key]
        [Display(Name ="Student ID")]
        public string StudentID { get; set; }
        [Display(Name = "Frist Name")]
        public string FirstName { get; set; }
        [Display(Name = "Second Name")]
        public string SecondName { get; set; }
    }
}
