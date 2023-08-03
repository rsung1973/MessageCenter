using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebHome.Models.ViewModel
{
    public class LoginViewModel
    {
    }

    public class PasswordViewModel : QueryViewModel
    {

        [DataType(DataType.Password)]
        [Display(Name = "密碼")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "確認密碼")]
        public string Password2 { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "圖形密碼")]
        public string lockPattern { get; set; }

        public String PID { get; set; }
        public int? UID { get; set; }
        public Guid? UUID { get; set; }
    }

}