using Journal.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Domain.JsonModels.User
{
    public class NoteJsonModel
    {
        public Guid Id { get; set; }

        public string Text { get; set; }

        public List<string> Symbols { get; set; }
    }
}
