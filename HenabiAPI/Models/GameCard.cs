using HenabiAPI.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HenabiAPI.Models
{
    public class GameCard
    {
        public int Id { get; set; }
        public Card Card { get; set; }
        public CardPosition Position { get; set; }
        [JsonIgnore]
        public Game Game { get; set; }
        public Player Player { get; set; }

    }
}
