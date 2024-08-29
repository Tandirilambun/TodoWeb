using TodoWeb.Models;

namespace TodoWeb.TodoVM
{
    public class ProfileVM
    {
        public User Profile { get; set; }

        public string PhotoURL { get; set; }
        public IFormFile formFile { get; set; }
    }
}
