using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace kopilka.ViewModels
{
    public class SendToEmailPassword
    {
        [EmailAddress(ErrorMessage = "Недопустимый формат Email")]
        [Required(ErrorMessage = "Не указан Email")]
        public string Email { get; set; }
    }
}
