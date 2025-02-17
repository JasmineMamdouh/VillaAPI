using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Data;
using VillaAPI.Models;

namespace VillaAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Villa> Villas { get; set; }


        //add ctor that expects a dbcontext options 'DI'
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        //we can seed villa table with records by overriding the OnModelCreating
        //then add other migration to insert those data into the db
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Villa>().HasData(
                new Villa
                {
                    Id = 4,
                    Name = "Diamond Villa",
                    Details = "Big terrace and garden",
                    Occupancy = 4,
                    Rate = 220,
                    Sqft = 550,
                    Amenity = "",
                    ImageUrl = "",
                    CreatedDate = new DateTime(2024, 1, 1, 12, 0, 0)
                }
                );
        }
    }
}
