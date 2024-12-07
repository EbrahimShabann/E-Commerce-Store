using Bulky.Models.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models.ViewModels
{
    public class UserVM
    {
        public ApplicationUser ApplicationUser { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> RolesList { get; set; } 
        public IEnumerable<SelectListItem> CompaniesList { get; set; } 
    }
}
