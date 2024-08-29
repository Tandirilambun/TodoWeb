using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TodoWeb.Models
{
    public class User : IdentityUser
    {

        public string Name { get; set; }

        public bool Keep_Signin { get; set; }

        public string? ProfilePhotoPath { get; set; }

        public ICollection<Todo> Todos { get; set; }

        public ICollection<Projects> Projects { get; set; }

        public ICollection<Participants> Participants { get; set; }

        public ICollection<Tasks> Tasks { get; set; }
    }
}
