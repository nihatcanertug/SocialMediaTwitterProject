using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SocialMediaTwitterProject.Application.Models.DTOs
{
    public class EditProfileDTO
    {
        //Business Domain ihtiyaçlarımıza göre hazırladığımız veri transfer objelerimizin ihtiyacımız olan propertylerini ekliyoruz.
        public int Id { get; set; }
        [Required(ErrorMessage = "You must to type into name")]
        public string Name { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "User Name")]
        public string UserName { get; set; }
        public string Email { get; set; }
        public string ImagePath { get; set; }
        [NotMapped]
        public IFormFile Image { get; set; }
    }
}
