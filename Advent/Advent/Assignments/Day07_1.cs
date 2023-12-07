using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Assignments
{
    internal class Day07_1 : IAssignment
    {
        internal enum HandType
        {
            FiveOfAKind = 6,
            FourOfAKind = 5,
            FullHouse = 4,
            ThreeOfAKind = 3,
            TwoPair = 2,
            OnePair = 1,
            HighCard = 0,
        }

        private static readonly char[] CardOrder = new char[] { '2', '3', '4', '5', '6', '7', '8', '9', 'T', 'J', 'Q', 'K', 'A' };

        private record HandWithBid(string Hand, HandType HandType, int Bid);
        public string Run(IReadOnlyList<string> input)
        {
            var hands = new List<HandWithBid>();
            foreach (var item in input)
            {
                var parts = item.Split(' ', StringSplitOptions.TrimEntries);
                var hand = parts[0];
                var bid = int.Parse(parts[1]);
                var type = GetHandType(hand);
                hands.Add(new HandWithBid(hand, type, bid));
            }
            hands.Sort(Compare);
            var total = 0;
            checked
            {
                for (var i = 0; i < hands.Count; i++)
                {
                    total += hands[i].Bid * (i + 1);
                }
            }
            return total.ToString();
        }

        private static int Compare(HandWithBid right, HandWithBid left)
        {
            var type = Math.Sign(right.HandType - left.HandType);
            if (type != 0)
                return type;
            if (left.Hand.Length != right.Hand.Length)
                ThrowException("Hand size mismatch");

            for (var i = 0; i < left.Hand.Length; i++)
            {
                var leftCardValue = Array.IndexOf(CardOrder, left.Hand[i]);
                var rightCardValue = Array.IndexOf(CardOrder, right.Hand[i]);
                var value = Math.Sign(rightCardValue - leftCardValue);
                if (value != 0)
                    return value;
            }

            return 0;
        }

        private static HandType GetHandType(string hand)
        {
            var histogram = new int[CardOrder.Length];
            var histogramMask = 0U;
            for (var i = 0; i < hand.Length; i++)
            {
                var histIndex = Array.IndexOf(CardOrder, hand[i]);
                histogram[histIndex]++;
                histogramMask |= (1U << histIndex);
            }
            var uniqueCards = BitOperations.PopCount(histogramMask);

            if (uniqueCards == 1)
            {
                return HandType.FiveOfAKind;
            }
            else if (uniqueCards == 2)
            {
                var maxSameCard = 0;
                for (var i = 0; i < histogram.Length; i++)
                {
                    if (histogram[i] > maxSameCard)
                        maxSameCard = histogram[i];
                }
                if (maxSameCard == 4)
                {
                    return HandType.FourOfAKind;
                }
                return HandType.FullHouse;
            }
            else if (uniqueCards == 3)
            {
                var maxSameCard = 0;
                for (var i = 0; i < histogram.Length; i++)
                {
                    if (histogram[i] > maxSameCard)
                        maxSameCard = histogram[i];
                }
                if (maxSameCard == 3)
                {
                    return HandType.ThreeOfAKind;
                }
                return HandType.TwoPair;
            }
            else if (uniqueCards == 4)
            {
                return HandType.OnePair;
            }
            else
            {
                return HandType.HighCard;
            }
        }

        [DoesNotReturn]
        private static void ThrowException(string message)
        {
            throw new Exception(message);
        }
    }
}
