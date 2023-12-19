using Advent.Shared;
using System.Text;

namespace Advent.Assignments
{
    internal class Day19_1 : IAssignment
    {
        private abstract class Action
        {
            public abstract Result Execute(Scope scope, Item item);
        }

        private class ReturnAction : Action
        {
            public bool Accepted { get; }

            public ReturnAction(bool accepted)
            {
                Accepted = accepted;
            }

            public override Result Execute(Scope scope, Item item)
            {
                return Accepted ? Result.Accepted : Result.Rejected;
            }
        }

        private class JumpAction : Action
        {
            public string Target { get; }

            public JumpAction(string target)
            {
                Target = target ?? throw new ArgumentNullException(nameof(target));
            }

            public override Result Execute(Scope scope, Item item)
            {
                return scope.Rules[Target].Execute(scope, item);
            }
        }

        enum Comparison
        {
            GreaterThan,
            LesserThen,
        }

        enum Result
        {
            Unknown,
            Accepted,
            Rejected,
        }

        private class ConditionalAction : Action
        {
            public Action Action { get; }
            public string Variable { get; }
            public int Value { get; }
            public Comparison Comparison { get; }

            public ConditionalAction(Action action, string variable, int value, Comparison comparison)
            {
                Action = action ?? throw new ArgumentNullException(nameof(action));
                Variable = variable ?? throw new ArgumentNullException(nameof(variable));
                Value = value;
                Comparison = comparison;
            }

            public override Result Execute(Scope scope, Item item)
            {
                var value = Variable switch
                {
                    "x" => item.X,
                    "m" => item.M,
                    "a" => item.A,
                    "s" => item.S,
                    _ => -1,
                };

                if (Comparison == Comparison.LesserThen)
                {
                    if (value < Value)
                        return Action.Execute(scope, item);
                }
                else
                {
                    if (value > Value)
                        return Action.Execute(scope, item);
                }

                return Result.Unknown;
            }
        }

        private class Rule
        {
            public string Name { get; }
            public List<Action> Actions { get; }

            public Rule(string name, List<Action> actions)
            {
                Name = name ?? throw new ArgumentNullException(nameof(name));
                Actions = actions ?? throw new ArgumentNullException(nameof(actions));
            }

            public Result Execute(Scope scope, Item item)
            {
                foreach (var action in Actions)
                {
                    var result = action.Execute(scope, item);
                    if (result != Result.Unknown)
                        return result;
                }

                throw new InvalidOperationException();
                return Result.Unknown;
            }
        }

        private class Scope
        {
            public Dictionary<string, Rule> Rules { get; }

            public Scope(Dictionary<string, Rule> rules)
            {
                Rules = rules ?? throw new ArgumentNullException(nameof(rules));
            }
        }

        private record Item(int X, int M, int A, int S);

        public string Run(IReadOnlyList<string> input)
        {
            var rules = new Dictionary<string, Rule>();
            var i = 0;
            var tokenizer = new Tokenizer();
            for (; i < input.Count; i++)
            {
                var line = input[i];
                if (line.Length == 0)
                    break;

                tokenizer.Input = line;
                var rule = ParseRule(tokenizer);
                rules.Add(rule.Name, rule);
            }
            // Skip the empty line
            i++;

            var scope = new Scope(rules);
            var startRule = rules["in"];
            var totalAccepted = 0;

            for (; i < input.Count; i++)
            {
                // Always X M A S
                var values = input[i].ExtractInts();
                var item = new Item(values[0], values[1], values[2], values[3]);
                var result = startRule.Execute(scope, item);
                if (result != Result.Accepted)
                    continue;

                totalAccepted += item.X + item.M + item.A + item.S;
            }

            return totalAccepted.ToString();
        }

        public class Tokenizer
        {
            public string Input
            {
                get => _input;
                set 
                {
                    _input = value;
                    _index = 0;
                }
            }
            private string _input = string.Empty;
            private int _index;

            public char Peek()
            {
                return _input[_index];
            }

            public char Next()
            {
                return _input[_index++];
            }

            public string NextIdentifier()
            {
                var start = _index;
                while (char.IsLetter(_input[_index]))
                    _index++;
                return _input.Substring(start, _index - start);
            }

            public int NextNumber()
            {
                var num = 0;
                while (char.IsDigit(_input[_index]))
                {
                    num *= 10;
                    num += _input[_index] - '0';
                    _index++;
                }
                return num;
            }

            public void AssertNext(char value)
            {
                var next = Next();
                if (next != value)
                {
                    throw new InvalidDataException($"Expected '{value}', got '{next}'");
                }
            }
        }

        private Rule ParseRule(Tokenizer tokenizer)
        {
            var actions = new List<Action>();

            var name = tokenizer.NextIdentifier();
            tokenizer.AssertNext('{');

            while (true)
            {
                var chr = tokenizer.Peek();
                if (chr == '}')
                    break;
                var action = ParseAction(tokenizer);
                actions.Add(action);
                if (tokenizer.Peek() != ',')
                {
                    tokenizer.AssertNext('}');
                    break;
                }
                tokenizer.AssertNext(',');
            }
            
            return new Rule(name, actions);
        }

        private Action ParseAction(Tokenizer tokenizer)
        {
            var name = tokenizer.NextIdentifier();
            var chr = tokenizer.Peek();
            if (chr == '<' || chr == '>')
            {
                // Conditional action
                tokenizer.Next();
                var field = name;
                var value = tokenizer.NextNumber();
                tokenizer.AssertNext(':');
                var action = ParseAction(tokenizer);
                return new ConditionalAction(action, field, value, chr == '<' ? Comparison.LesserThen : Comparison.GreaterThan);
            }
            else
            {
                // Regular action
                if (name == "A")
                    return new ReturnAction(true);
                else if (name == "R")
                    return new ReturnAction(false);
                else
                    return new JumpAction(name);
            }
        }
    }
}
