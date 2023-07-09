using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Octopartite.ParserRules
{
	public abstract class NonTerminal : Rule
	{
		public override bool IsTerminal => false;
		public List<Rule> Symbols { get; set; } = new List<Rule>();
		public override List<Terminal> GetFirstTerminals()
		{
			List<Rule> visited = new List<Rule>();
			List<Terminal> firstTerminals = new List<Terminal>();
			CollectFirstTerminals(visited, firstTerminals);
			return firstTerminals;
		}
		internal virtual void CollectFirstTerminals(List<Rule> visited, List<Terminal> firstTerminals)
		{
			visited.Add(this);

			foreach (var symbol in Symbols)
			{
				if (visited.Contains(symbol) || firstTerminals.Contains(symbol))
					continue;
				if (symbol.IsTerminal)
					firstTerminals.Add(symbol as Terminal);
				else
					(symbol as NonTerminal).CollectFirstTerminals(visited, firstTerminals);
			}
		}

		protected virtual void SaveLongestMatch(ParseNode node, ParseNode bestChild)
		{
			if (node.Root.LongestMatch == null || node.Root.LongestMatch.End < bestChild.End)
			{
				node.Nodes.Add(bestChild);
				node.Root.LongestMatch = new ParseNode(node.Root);
				node.Root.LongestMatch.Length = bestChild.End -node.Root.LongestMatch.Index;
				node.Nodes.Remove(bestChild);
			}
			/*if ((node.LongestMatch == null || node.LongestMatch.Length < node.Length) && bestChild != null)
			{
				node.LongestMatch = new ParseNode(node);
				if (bestChild.LongestMatch != null)
					bestChild = bestChild.LongestMatch;
				node.LongestMatch.Length = bestChild.Index + bestChild.Length - node.Index;
				node.LongestMatch.Nodes.Add(bestChild);
				bestChild.Parent = node.LongestMatch;
			}*/
		}
	}
}
