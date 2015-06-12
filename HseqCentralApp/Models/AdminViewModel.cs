using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace HseqCentralApp.Models
{
    public class RoleViewModel
    {
        public string Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "RoleName")]
        public string Name { get; set; }

        public ICollection<SelectListItem> UsersList { get; set; }
    }


    public class GroupViewModel
    {
        public GroupViewModel()
        {
            this.UsersList = new List<SelectListItem>();
            this.RolesList = new List<SelectListItem>();
        }
        
        [Required(AllowEmptyStrings = false)]
        public string Id { get; set; }
        
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public ICollection<SelectListItem> UsersList { get; set; }
        
        public ICollection<SelectListItem> RolesList { get; set; }
    }

    public class EditUserViewModel
    {
        public EditUserViewModel()
        {
            this.RolesList = new List<SelectListItem>();
            this.GroupsList = new List<SelectListItem>();
        }
        
        public string Id { get; set; }
        
        
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Department")]
        public string Department { get; set; }

        // We will still use this, so leave it here:
        public ICollection<SelectListItem> RolesList { get; set; }

        // Add a GroupsList Property:
        public ICollection<SelectListItem> GroupsList { get; set; }
    }

}