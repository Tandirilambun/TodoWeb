using Microsoft.AspNetCore.Mvc.Rendering;
using TodoWeb.Models;

namespace TodoWeb.TodoVM
{
    public class TodoViewModel
    {
        public List<Todo> Todoes { get; set; }

        public SelectList? Categories { get; set; }

        public Todo TodoSingle { get; set; }

        public int Note_id { get; set; }

        public string Note_title { get; set; }

        public string Note_description { get; set; }

        public bool Note_completed { get; set; }

        public int Category_note_id { get; set; }

    }
}
