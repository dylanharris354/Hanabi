using HenabiAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HenabiAPI.Data
{
    public class HenabiDBContext : DbContext 
    {
        public HenabiDBContext(DbContextOptions<HenabiDBContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=Henabi.db");
        }

        public DbSet<Card> Cards => Set<Card>();
        public DbSet<Game> Games => Set<Game>();
        public DbSet<GameCard> GameCards => Set<GameCard>();
        public DbSet<Player> Players => Set<Player>();
        public DbSet<Hint> Hints => Set<Hint>();
    }
}
