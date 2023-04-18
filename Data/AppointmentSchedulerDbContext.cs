using Application_Scheduler.Models;
using Microsoft.EntityFrameworkCore;

namespace Application_Scheduler.Data
{
    public class AppointmentSchedulerDbContext : DbContext
    {
        public AppointmentSchedulerDbContext(DbContextOptions<AppointmentSchedulerDbContext> options) : base(options)
        {

        }

        public DbSet<UserModel> Users { get; set; }
        public DbSet<AppointmentModel> Appointments { get; set; }
    }
}
