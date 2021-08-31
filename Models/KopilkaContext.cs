using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace kopilka.Models
{
        public class KopilkaContext : DbContext
        {
            public DbSet<Range> Ranges { get; set; }

            public DbSet<User> Users { get; set; }
           
            public KopilkaContext(DbContextOptions<KopilkaContext> options)
                : base(options)

            {
               //Database.EnsureCreated();
              
            }
        }
    
}
