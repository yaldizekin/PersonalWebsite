using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portfolio.Areas.Admin.Models
{
    public class Project
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

       
        [Required]
        public string Content { get; set; }
        [Required]

        public string Slug { get; set; }
		[Required]

		public string Link { get; set; }

		public string? ImagePath { get; set; }

        [NotMapped]
        public IFormFile? Image { get; set; }
       


    }
}