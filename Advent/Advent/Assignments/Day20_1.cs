using Advent.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Advent.Assignments
{
    internal class Day20_1 : IAssignment
    {
        private record Signal(Node From, Node To, bool High);

        private abstract class Node
        {
            public List<Node> Inputs { get; protected set; }
            public List<Node> Outputs { get; protected set; }
            public string Name { get; }

            public Node(Node node)
            {
                Name = node.Name;
                Inputs = node.Inputs;
                Outputs = node.Outputs;

                foreach (var input in Inputs)
                {
                    var index = input.Outputs.IndexOf(node);
                    input.Outputs[index] = this;
                }

                foreach (var output in Outputs)
                {
                    var index = output.Inputs.IndexOf(node);
                    output.Inputs[index] = this;
                }
            }

            public Node(string name)
            {
                Name = name;
                Inputs = new List<Node>();
                Outputs = new List<Node>();
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
            public BroadcastNode(Node node) : base(node)
            {
            }

            public BroadcastNode() : base("broadcaster")
            {
            }

            public override IReadOnlyList<Signal> ProcessSignal(Node from, bool high)
                => OutputAll(high);

            public override string ToString()
            {
                return "broadcaster";
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

            public FlipFlopNode(Node node) : base(node)
            {
            }

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

            public ConjunctionNode(Node node) : base(node)
            {
            }

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

        private Dictionary<string, Node> GetNodes(IReadOnlyList<string> input)
        {
            var nodes = new Dictionary<string, Node>();
            foreach (var line in input)
            {
                var parts = line.Split(" -> ", StringSplitOptions.None);
                var nameWithPrefix = parts[0];
                var connections = parts[1].Split(',', StringSplitOptions.TrimEntries);

                Node? node;

                // Make sure the main node is added
                if (nameWithPrefix == "broadcaster")
                {
                    nodes.TryGetValue(nameWithPrefix, out node);
                    if (node != null)
                        node = new BroadcastNode(node);
                    else
                        node = new BroadcastNode();
                    nodes[nameWithPrefix] = node;
                }
                else if (nameWithPrefix.StartsWith("&"))
                {
                    var name = nameWithPrefix.Substring(1);
                    nodes.TryGetValue(name, out node);
                    if (node != null)
                        node = new ConjunctionNode(node);
                    else
                        node = new ConjunctionNode(name);
                    nodes[name] = node;
                }
                else if (nameWithPrefix.StartsWith("%"))
                {
                    var name = nameWithPrefix.Substring(1);
                    nodes.TryGetValue(name, out node);
                    if (node != null)
                        node = new FlipFlopNode(node);
                    else
                        node = new FlipFlopNode(name);
                    nodes[name] = node;
                }
                else
                {
                    continue;
                }

                // Create the connections
                foreach (var connectionName in connections)
                {
                    if (!nodes.TryGetValue(connectionName, out var connection))
                    {
                        // Create a (temporary) VoidNode
                        connection = new VoidNode(connectionName);
                        nodes[connectionName] = connection;
                    }

                    node.ConnectTo(connection);
                }
            }
            return nodes;
        }

        public string Run(IReadOnlyList<string> input)
        {
            var nodes = GetNodes(input);
            var broadcaster = (BroadcastNode)nodes["broadcaster"];

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
