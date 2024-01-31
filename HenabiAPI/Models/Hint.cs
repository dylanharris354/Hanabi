using HenabiAPI.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HenabiAPI.Models
{
    public class Hint
    {
        public int Id { get; set; }
        public DateTime TimeGiven { get; set; }
        public Player ReceivingPlayer { get; set; }
        public Player GivingPlayer { get; set; }
        public List<Card> Cards { get; set; }
        public CardColor? ColorHint { get; set; }
        public int? NumberHint { get; set; }
        public Game Game { get; set; }
    }
}
