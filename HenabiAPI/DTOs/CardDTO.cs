using HenabiAPI.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HenabiAPI.DTOs
{
    public class CardDTO
    {
        public int Id { get; set; }

        public int Number { get; set; }

        public CardColor ColorId { get; set; }
        public string Color { get; set; }
        public int? GameId { get; set; }
    }
}
