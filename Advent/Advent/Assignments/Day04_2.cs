using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Assignments
{
    internal class Day04_2 : IAssignment
    {
        public string Run(IReadOnlyList<string> input)
        {
            int WinStart = input.Count > 10 ? 10 : 8;
            int WinCount = input.Count > 10 ? 10: 5;
            int GameStart = input.Count > 10 ? 42:  25;
            int GameCount = input.Count > 10 ? 25: 8;

            var wins = new int[WinCount];
            var bonusCards = new int[input.Count];
            var cardIndex = 0;
            var totalCards = 0;

            foreach (var cardLine in input)
            {
                for (var i = 0; i < WinCount; i++)
                {
                    wins[i] = ParseDoubleDigitInt(cardLine.AsSpan(WinStart + i * 3));
                }

                var score = 0;
                for (var i = 0; i < GameCount; i++)
                {
                    var num = ParseDoubleDigitInt(cardLine.AsSpan(GameStart + i * 3));
                    if (wins.Contains(num))
                        score++;
                }

                //Logger.DebugLine($"Card {cardIndex + 1} wins {score} cards and is repeated {bonusCards[cardIndex]} times");
                var totalTimes = bonusCards[cardIndex] + 1;

                for (var i = cardIndex + 1; i < cardIndex + 1 + score && i < bonusCards.Length; i++)
                {
                    //Logger.DebugLine($"\tBonus card {i + 1} added {totalTimes} times");
                    bonusCards[i] += totalTimes;
                }

                totalCards += totalTimes;
                cardIndex++;
            }
            return totalCards.ToString();
        }

        public static int ParseDoubleDigitInt(ReadOnlySpan<char> str)
        {
            if (str[0] == ' ')
                return str[1] - '0';
            return (str[0] - '0') * 10 + (str[1] - '0');
        }
    }
}
