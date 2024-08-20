using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TodoWeb.Models
{
    public class User : IdentityUser
    {
        [Key]
        public int User_id { get; set; }

        public string Username { get; set; }

        public string User_email { get; set; }

        public string User_password { get; set; }

        public bool Keep_Signin { get; set; }

        public ICollection<Todo> Todos { get; set; }

        public ICollection<Projects> Projects { get; set; }

        public ICollection<Participants> Participants { get; set; }

        public ICollection<Tasks> Tasks { get; set; }
    }
}
