using HenabiAPI.DTOs;
using HenabiAPI.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HenabiAPI.Models
{
    public class Card
    {
        public int Id { get; set; }

        public int Number { get; set; }

        public CardColor Color { get; set; }

        public CardDTO MapToDTO()
        {
            return new CardDTO()
            {
                Id = Id,
                Number = Number,
                ColorId = Color,
                Color = Enum.GetName(typeof(CardColor), Color)
            };
        }
    }
}
