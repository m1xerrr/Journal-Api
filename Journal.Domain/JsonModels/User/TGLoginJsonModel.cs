using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Domain.JsonModels.User
{
    public class TGLoginJsonModel
    {
        public string Username { get; set; }

        public string TelegramId { get; set; }
    }
}
