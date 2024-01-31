using HenabiAPI.Models;
using HenabiAPI.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HenabiAPI.DTOs
{
    public class HintDTO
    {
        public CardColor? ColorHint { get; set; }
        public int? NumberHint { get; set; }
        public int RecievingPlayerId { get; set; }
        public int GivingPlayerId { get; set; }
        public List<int> CardIds { get; set; }
        public int GameId { get; set; }
    }
}
