using Microsoft.EntityFrameworkCore;
using API.Models;
using System;
 

namespace API.Data
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
    }
}
