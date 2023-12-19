using Advent.Shared;
using System.Diagnostics;
using System.Text;

namespace Advent.Assignments
{
    internal class Day19_2 : IAssignment
    {
        private class Tokenizer
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

        private abstract class Action
        {
        }

        private class ReturnAction : Action
        {
            public bool Accepted { get; }

            public ReturnAction(bool accepted)
            {
                Accepted = accepted;
            }

            public override string ToString()
            {
                return Accepted ? "return A;" : "return R;";
            }
        }

        private class JumpAction : Action
        {
            public string Target { get; }

            public JumpAction(string target)
            {
                Target = target ?? throw new ArgumentNullException(nameof(target));
            }

            public override string ToString()
            {
                return $"goto {Target}";
            }
        }

        enum Comparison
        {
            GreaterThan,
            LesserThan,
        }

        private class ConditionalAction : Action
        {
            public Action Action { get; }
            public Action? ElseAction { get; set; }
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

            public override string ToString()
            {
                return $"if ({Variable} {(Comparison == Comparison.LesserThan ? '<' : '>')} {Value}) {{{Action}}} else {{{ElseAction}}}";
            }
        }

        private class RuleAction : Action
        {
            public string Name { get; }
            public List<Action> Actions { get; }

            public RuleAction(string name, List<Action> actions)
            {
                Name = name ?? throw new ArgumentNullException(nameof(name));
                Actions = actions ?? throw new ArgumentNullException(nameof(actions));
            }
        }

        private class Scope
        {
            public Dictionary<string, RuleAction> Rules { get; }

            public Scope(Dictionary<string, RuleAction> rules)
            {
                Rules = rules ?? throw new ArgumentNullException(nameof(rules));
            }
        }

        private record Xmas(LineRange X, LineRange M, LineRange A, LineRange S)
        {
            public Xmas WithRange(string variable, LineRange range)
            {
                return variable switch
                {
                    "x" => this with { X = range },
                    "m" => this with { M = range },
                    "a" => this with { A = range },
                    _ => this with { S = range },
                };
            }
        }

        public string Run(IReadOnlyList<string> input)
        {
            var rules = new Dictionary<string, RuleAction>();
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

            var scope = new Scope(rules);
            var startRule = rules["in"];
            var optimized = Optimize(startRule, scope);
            //Logger.DebugLine(optimized.ToString() ?? string.Empty);
            var totalAccepted = Solve(new Xmas(new LineRange(1, 4000), new LineRange(1, 4000), new LineRange(1, 4000), new LineRange(1, 4000)), optimized);

            return totalAccepted.ToString();
        }

        private long Solve(Xmas xmas, Action? action)
        {
            if (action == null)
                return 0;

            if (action is ReturnAction returnAction)
            {
                if (returnAction.Accepted)
                    return (long)xmas.X.Length * xmas.M.Length * xmas.A.Length * xmas.S.Length;
                return 0;
            }

            if (action is ConditionalAction conditional)
            {
                var range = conditional.Variable switch
                {
                    "x" => xmas.X,
                    "m" => xmas.M,
                    "a" => xmas.A,
                    _ => xmas.S,
                };

                switch (conditional.Comparison)
                {
                    case Comparison.GreaterThan:
                        {
                            // Entire range is above the check
                            if (range.start > conditional.Value)
                                return Solve(xmas, conditional.Action);
                            // Entire range is below the check
                            if (range.end <= conditional.Value)
                                return Solve(xmas, conditional.ElseAction);

                            // Split the range
                            var offset = conditional.Value - range.start + 1;
                            var upperRange = new LineRange(range.start + offset, range.Length - offset);
                            var lowerRange = new LineRange(range.start, offset);

                            var upperResult = Solve(xmas.WithRange(conditional.Variable, upperRange), conditional.Action);
                            var lowerResult = Solve(xmas.WithRange(conditional.Variable, lowerRange), conditional.ElseAction);
                            return upperResult + lowerResult;
                        }
                    case Comparison.LesserThan:
                        {
                            // Entire range is below the check
                            if (range.end < conditional.Value)
                                return Solve(xmas, conditional.Action);
                            // Entire range is above the check
                            if (range.start >= conditional.Value)
                                return Solve(xmas, conditional.ElseAction);

                            // Split the range
                            var offset = conditional.Value - range.start;
                            var upperRange = new LineRange(range.start + offset, range.Length - offset);
                            var lowerRange = new LineRange(range.start, offset);

                            var upperResult = Solve(xmas.WithRange(conditional.Variable, upperRange), conditional.ElseAction);
                            var lowerResult = Solve(xmas.WithRange(conditional.Variable, lowerRange), conditional.Action);
                            return upperResult + lowerResult;
                        }
                }
            }

            throw new InvalidOperationException();
        }

        private static RuleAction ParseRule(Tokenizer tokenizer)
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
            
            return new RuleAction(name, actions);
        }

        private static Action ParseAction(Tokenizer tokenizer)
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
                return new ConditionalAction(action, field, value, chr == '<' ? Comparison.LesserThan : Comparison.GreaterThan);
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

        /// <summary>
        /// Optimizes an action into a ConditionalAction or ReturnAction.
        /// The branches of a ConditionalAction will also only contain ConditionalActions or ReturnActions.
        /// </summary>
        private static Action Optimize(Action action, Scope scope)
        {
            if (action is ReturnAction)
                return action;
            else if (action is JumpAction jump)
                return Optimize(scope.Rules[jump.Target], scope);
            else if (action is ConditionalAction conditional)
            {
                var resultAction = Optimize(conditional.Action, scope);
                var elseAction = conditional.ElseAction != null ? Optimize(conditional.ElseAction, scope) : null;
                // If both branches give the same result just return that branch
                if (resultAction is ReturnAction returnA && elseAction is ReturnAction returnB && returnA.Accepted == returnB.Accepted)
                {
                    return returnA;
                }
                // Actual branch that needs to be taken
                return new ConditionalAction(resultAction, conditional.Variable, conditional.Value, conditional.Comparison)
                {
                    ElseAction = elseAction
                };
            }
            else if (action is RuleAction rule)
            {
                ConditionalAction? firstConditional = null;
                ConditionalAction? lastConditional = null;

                foreach (var subAction in rule.Actions)
                {
                    var optimizedSubAction = Optimize(subAction, scope);
                    if (lastConditional != null)
                    {
                        lastConditional.ElseAction = optimizedSubAction;
                    }

                    if (optimizedSubAction is ConditionalAction conditionalAction)
                    {
                        lastConditional = conditionalAction;
                        if (firstConditional == null)
                            firstConditional = conditionalAction;
                    }
                    else
                    {
                        // This is our final else clause
                        Debug.Assert(optimizedSubAction is ReturnAction);
                        // Special case: RuleAction that reduces to a ReturnAction on the first round
                        if (lastConditional == null)
                            return optimizedSubAction;
                        break;
                    }
                }

                Debug.Assert(firstConditional != null);
                return firstConditional;
            }

            throw new NotImplementedException();
        }
    }
}
