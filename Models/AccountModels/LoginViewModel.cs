using System.ComponentModel.DataAnnotations;

namespace Passengers.Models.AccountModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "لا يمكن أن يكون اسم المستخدم فارغاً")]
        [Display(Name = "اسم المستخدم")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "لا يمكن أن تكون كلمة المرور فارغة")]
        [DataType(DataType.Password)]
        [Display(Name = "كلمة المرور")]
        public string Password { get; set; }

        [Display(Name = "تذكّر تسجيل الدخول")]
        public bool RememberMe { get; set; }
        public string ReturnUrl { get; set; }
    }
}