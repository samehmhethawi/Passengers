using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Passengers.Models.AccountModels
{
    public class ChangeDefaults
    {
        [Display(Name = "كلمة المرور السابقة")]
        public string OldPassword { get; set; }

        [Display(Name = "كلمة المرور الجديدة")]
        public string NewPassword { get; set; }

        [Compare("NewPassword")]
        [Display(Name = "إعادة إدخال كلمة المرور الجديدة")]
        public string NewPasswordConfirmed { get; set; }

        [Display(Name = "عدد السجلات في الصفحة")]
        public int DefaultRecordsPerPage { get; set; }

        [Display(Name = "خيار الطباعة الافتراضي")]
        public int DefaultPrintOption { get; set; }

        public int Type { get; set; }
    }
}