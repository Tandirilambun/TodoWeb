using System.ComponentModel.DataAnnotations;

namespace TodoWeb.Models
{
    public class Category
    {
        [Key]
        public int Category_id { get; set; }

        public string Category_name { get; set; }

        public ICollection<Todo> Todo { get; set; }
    }
}
