using KendoProject.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StudentBasic.ModelView
{
    public class StudentInfoModelView
    {
        
        public int ID { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Please Enter a valid name", MinimumLength = 5)]
        [Remote("ClientNameExist", "StudentInfoes", HttpMethod = "Post", ErrorMessage = "Client name already exsits")]
        public string Name { get; set; }

        //[Display(Name = "Department Name")]
        public string dptName { get; set; }

        public int countStudent { get; set; }
        public int departmentCode { get; set; }
        public string alldept { get; set; }

        [Required]
        [Range(1,100,ErrorMessage="You must choose a Department")]
        public int DepartmentID { get; set; }
        [Required]
        [Display(Name="Joining Year")]
        [Range(1990,2016,ErrorMessage = "Invalid Joining Year")]
        public string Year { get; set; }

        public virtual Department Department { get; set; }
    }
}