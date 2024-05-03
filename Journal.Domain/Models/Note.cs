using Journal.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Domain.Models
{
    public class Note
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime LastUpdate {  get; set; }

        public string Text { get; set; }

        public List<string> Symbols { get; set; }

        public User User { get; set; }

        public Note() { 
            List<Note> notes = new List<Note>();
        }
    }
}
