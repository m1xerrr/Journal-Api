using Journal.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Domain.ResponseModels
{
    public class NoteResponseModel
    {
        public Guid Id { get; set; }
        public DateTime CreationTime { get; set; }

        public DateTime LastUpdate {  get; set; }

        public string Text { get; set; }

        public List<string> Symbols { get; set; }

        public NoteResponseModel(Note note) 
        {
            this.Id = note.Id;
            this.Text = note.Text;
            this.Symbols = note.Symbols;
            this.CreationTime = note.CreationTime;
            this.LastUpdate = note.LastUpdate;
        }  
    }
}
