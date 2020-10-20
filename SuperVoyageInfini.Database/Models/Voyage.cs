using SuperVoyageInfini.Resources;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SuperVoyageInfini.Database.Models
{
    public class Voyage
    {   
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Name", ResourceType = typeof(AppResource))]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public bool IsPublic { get; set; }
        public bool IsPending { get; set; }
        public string Image { get; set; }
        public string Color { get; set; }

        public virtual ApplicationUser User { get; set; }

        [InverseProperty("VoyagesParticipant")]
        public virtual List<ApplicationUser> Participants { get; set; } = new List<ApplicationUser>();
    }
}
