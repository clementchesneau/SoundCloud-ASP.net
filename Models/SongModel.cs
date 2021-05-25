using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ASPCSoundCloud.Models
{
    public class SongModel
    {
        [Key]
        public int SongId { get; set; }

        public string UserId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        [Display(Name = "Name")]
        public string SongName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        [Display(Name = "Artist")]
        public string SongArtist { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(10)")]
        [Display(Name = "Genre")]
        public string SongGenre { get; set; }

        [Column(TypeName = "nvarchar(255)")]
        [Display(Name = "Song Path")]
        public string SongPath { get; set; }

        [NotMapped]
        [Display(Name = "Song File")]
        [Required]
        public IFormFile SongFile { get; set; }
    }
}
