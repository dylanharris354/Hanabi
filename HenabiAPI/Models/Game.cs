using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HenabiAPI.Models
{
    public class Game
    {
        public int Id { get; set; }
        public int Hints { get; set; }
        public int Fuses { get; set; }
        public List<GameCard> Cards { get; set; }
        public List<Player> Players { get; set; }
    }
}
