using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Models
{
    public class Project
    {
        public int Id { get; set; }

        [Required]
        public string Author { get; set; }

        [Required]
        public decimal Budget { get; set; }

        [Range(1, 5)]
        public int Nomination1 { get; set; }

        [Range(1, 5)]
        public int Nomination2 { get; set; }

        [Range(1, 5)]
        public int Nomination3 { get; set; }
    }
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Project> Projects { get; set; }
    }
}


