using Journal.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Domain.ViewModels
{
    public class MTAccountViewModel
    {
        public Guid UserId { get; set; }
        [Required]
        public int Login { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Server { get; set; }
    }
}
