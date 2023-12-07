using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Advent.Assignments
{
    internal class Day07_2 : IAssignment
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

        private static int JokerIndex = 0;
        private static readonly char[] CardOrder = new char[] { 'J', '2', '3', '4', '5', '6', '7', '8', '9', 'T', 'Q', 'K', 'A' };

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
            var cardSeenMask = 0U;
            Span<int> cardCount = stackalloc int[CardOrder.Length];
            for (var i = 0; i < hand.Length; i++)
            {
                var card = hand[i];
                var cardScore = Array.IndexOf(CardOrder, card);
                cardCount[cardScore]++;
                if (card == 'J')
                    continue;

                cardSeenMask |= 1U << cardScore;
            }

            var uniqueCardCount = BitOperations.PopCount(cardSeenMask);
            var jokerCount = cardCount[JokerIndex];
            if (uniqueCardCount <= 1)
            {
                // One card or 5 jokers
                return HandType.FiveOfAKind;
            }
            else if (uniqueCardCount == 2)
            {
                // FOK or FH
                var maxSameCard = 0;
                for (var i = 0; i < cardCount.Length; i++)
                {
                    if (i == JokerIndex)
                        continue;
                    if (cardCount[i] > maxSameCard)
                        maxSameCard = cardCount[i];
                }

                if (maxSameCard == 4)
                {
                    return HandType.FourOfAKind;
                }

                if (jokerCount > 0)
                {
                    if (maxSameCard == 2 && jokerCount == 1)
                    {
                        return HandType.FullHouse;
                    }
                    return HandType.FourOfAKind;
                }
                return HandType.FullHouse;
            }
            else if (uniqueCardCount == 3)
            {
                // TOK or TP
                if (jokerCount > 0)
                {
                    return HandType.ThreeOfAKind;
                }

                var maxSameCard = 0;
                for (var i = 0; i < cardCount.Length; i++)
                {
                    if (i == JokerIndex)
                        continue;
                    if (cardCount[i] > maxSameCard)
                        maxSameCard = cardCount[i];
                }
                if (maxSameCard == 3)
                {
                    return HandType.ThreeOfAKind;
                }
                return HandType.TwoPair;
            }
            else if (uniqueCardCount == 4)
            {
                // OP
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
