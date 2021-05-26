using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace kopilka.ViewModels
{
    public class RegisterModel
    {
        [EmailAddress(ErrorMessage="Недопустимый формат Email")]
        [Required(ErrorMessage = "Не указан Email")]
        public string Login { get; set; }
        [RegularExpression(@"^[A-Za-z0-9]{6,30}$",ErrorMessage ="неверный формат пароля, пароль должен содержать в себе латинские буквы, допускаются цифры")]
        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }
    }
}

