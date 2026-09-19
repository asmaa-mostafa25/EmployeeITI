using System.ComponentModel.DataAnnotations;

namespace EmployeeDep.ViewModels
{
    public class SetUserNameViewModel
    {
        [Required(ErrorMessage = "Please enter a user name")]
        [StringLength(50, MinimumLength = 2,
            ErrorMessage = "The user name should be between 2 and 50 characters")]
        [Display(Name = "User Name")]
        public string UserName { get; set; }
    }
}
