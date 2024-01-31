using HenabiAPI.Models;
using HenabiAPI.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HenabiAPI.Data
{
    public static class DBInitiallizer
    {
        public static void Initialize(HenabiDBContext context)
        {
            if (context.Cards.Any())
            {
                return;
            }

            List<CardColor> colors = new List<CardColor>()
            {
                CardColor.Blue,
                CardColor.Green,
                CardColor.Red,
                CardColor.White,
                CardColor.Yellow
            };

            List<int> numbers = new List<int>()
            {
                1,
                1,
                1,
                2,
                2,
                3,
                3,
                4,
                4,
                5
            };

            List<Card> cards = new List<Card>();

            foreach (CardColor currentColor in colors)
            {
                foreach (int currentNum in numbers)
                {
                    cards.Add(new Card()
                    {
                        Color = currentColor,
                        Number = currentNum
                    });
                }
            }

            context.Cards.AddRange(cards);
            context.SaveChanges();
        }
    }
}
