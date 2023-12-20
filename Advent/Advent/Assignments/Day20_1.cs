using Advent.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Assignments
{
    internal class Day20_1 : IAssignment
    {
        private record Signal(Node From, Node To, bool High);

        private abstract class Node
        {
            protected List<Node> Inputs { get; } = new List<Node>();
            protected List<Node> Outputs { get; } = new List<Node>();
            public string Name { get; }

            public Node(string name)
            {
                Name = name;
            }

            public void ConnectTo(Node other)
            {
                Outputs.Add(other);
                other.Inputs.Add(this);
            }

            public abstract IReadOnlyList<Signal> ProcessSignal(Node from, bool high);

            protected IReadOnlyList<Signal> OutputAll(bool high)
            {
                var result = new Signal[Outputs.Count];
                for (var i = 0; i < Outputs.Count; i++)
                {
                    result[i] = new Signal(this, Outputs[i], high);
                }
                return result;
            }
        }

        private class BroadcastNode : Node
        {
            public BroadcastNode() : base("broadcast")
            {
            }

            public override IReadOnlyList<Signal> ProcessSignal(Node from, bool high)
                => OutputAll(high);

            public override string ToString()
            {
                return "broadcast";
            }
        }

        private class VoidNode : Node
        {
            public VoidNode(string name) : base(name)
            {
            }

            public override IReadOnlyList<Signal> ProcessSignal(Node from, bool high)
                => Array.Empty<Signal>();

            public override string ToString()
            {
                return Name;
            }
        }

        private class FlipFlopNode : Node
        {
            private bool _on;

            public FlipFlopNode(string name) : base(name)
            {
            }

            public override IReadOnlyList<Signal> ProcessSignal(Node from, bool high)
            {
                if (high)
                    return Array.Empty<Signal>();

                _on = !_on;
                return OutputAll(_on);
            }

            public override string ToString()
            {
                return $"%{Name}";
            }
        }

        private class ConjunctionNode : Node
        {
            private ulong _lastInputs;
            private ulong _lastInputsMask;

            public ConjunctionNode(string name) : base(name)
            {
            }

            public override IReadOnlyList<Signal> ProcessSignal(Node from, bool high)
            {
                if (_lastInputsMask == 0)
                {
                    _lastInputs = 0;
                    _lastInputsMask = (1UL << Inputs.Count) - 1;
                }

                var index = Inputs.IndexOf(from);
                var bit = (1UL << index);
                if (high)
                {
                    _lastInputs |= bit;
                    if (_lastInputs == _lastInputsMask)
                        return OutputAll(false);
                }
                else
                {
                    _lastInputs &= ~bit;
                }

                return OutputAll(true);
            }

            public override string ToString()
            {
                return $"&{Name}";
            }
        }

        public string Run(IReadOnlyList<string> input)
        {
            var broadcaster = new BroadcastNode();
            var a = new FlipFlopNode("a");
            var b = new FlipFlopNode("b");
            var inv = new ConjunctionNode("inv");
            var con = new ConjunctionNode("con");
            var output = new VoidNode("output");

            broadcaster.ConnectTo(a);
            a.ConnectTo(inv);
            a.ConnectTo(con);
            inv.ConnectTo(b);
            b.ConnectTo(con);
            con.ConnectTo(output);


            //var a = new FlipFlopNode("a");
            //var b = new FlipFlopNode("b");
            //var c = new FlipFlopNode("c");
            //var inv = new ConjunctionNode("inv");
            //broadcaster.ConnectTo(a);
            //broadcaster.ConnectTo(b);
            //broadcaster.ConnectTo(c);
            //a.ConnectTo(b);
            //b.ConnectTo(c);
            //c.ConnectTo(inv);
            //inv.ConnectTo(a);

            var signalQueue = new Queue<Signal>();
            var lowCount = 0;
            var highCount = 0;

            for (var i = 0; i < 1000; i++)
            {   
                // Initial low signal from button to broadcaster
                lowCount++;
                var outputs = broadcaster.ProcessSignal(broadcaster, false);
                signalQueue.EnqueueRange(outputs);

                while (signalQueue.TryDequeue(out var signal))
                {
                    //Logger.DebugLine($"{signal.From.Name} -{(signal.High ? "high" : "low")}-> {signal.To.Name}");

                    if (signal.High)
                        highCount++;
                    else
                        lowCount++;
                    outputs = signal.To.ProcessSignal(signal.From, signal.High);
                    signalQueue.EnqueueRange(outputs);
                }

                //Logger.DebugLine($"----------------------------");
            }

            var total = lowCount * highCount;
            return total.ToString();
        }
    }
}
