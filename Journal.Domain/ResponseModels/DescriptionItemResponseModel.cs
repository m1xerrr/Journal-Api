
using Journal.Domain.Enums;
using Journal.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Domain.ResponseModels
{
    public class DescriptionItemResponseModel
    {
        public Guid Id { get; set; }

        public int DealId { get; set; }

        public int Number { get; set; }

        public string Field { get; set; }

        public DescriptionType Type { get; set; }

        public DescriptionItemResponseModel() { }
        public DescriptionItemResponseModel(DescriptionItem item) 
        {
            this.Id = item.Id;
            this.Number = item.Number;
            this.Field = item.Field;
            this.Type = item.Type;
            this.DealId = item.DealId;
        }
    }
}
