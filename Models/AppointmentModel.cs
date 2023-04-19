using Microsoft.EntityFrameworkCore.Storage;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Application_Scheduler.Models
{
    public class AppointmentModel
    {

        [Key]
        [Required]
        public int AppointmentId { get; set; }

        [Required]
        public string? AppointmentTitle { get; set; }

        [Required]
        public string? AppointmentDescription { get; set; }

        [Required]
        public string? AppointmentType { get; set;}

        [Required]
        public string? AppointmentDate { get; set;}

        [Required]
        public string? AppointmentTime { get; set; }

        [Required]
        public string? Duration { get; set; }

        [Required]
        public string? AppointmentStatus { get; set; }

        [Required]
        [ForeignKey("USERS")]
        public int UserId { get; set; }

        [Required]
        public bool SetReminder { get; set; }

    }
}
