using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kopilka.Models
{
    public class Range
    {
        public int Id { get; set; }
        public int MoneyboxRange { get; set; }
        public bool IsDonated { get; set; }
        public int UserId { get; set; }
        public User User;
    }
}
