using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Octopartite.ParserRules
{
	public class Concat : NonTerminal
	{
		internal override ParseNode Parse(ParseNode parent, List<Terminal> skips)
		{
			var input = parent.input;
			var index = parent.End;
			ParseNode node = parent.CreateNode(this, input, parent.End);
			node.Success = true;
			for (int i = 0; i < Symbols.Count; i++)
			{
				var symbol = Symbols[i];
				var c = symbol.Parse(node, skips);
				if (c != null && c.Success)
				{
					node.Length = c.Index + c.Length - index;
					node.Nodes.Add(c);
					//c.Parent = node;
				}
				else
				{
					node.Success = false;
					//TODO: make it work when backtracking
					//TODO: add ability to disallow backtracking before a node (like with ! in prolog) ?
					if (BacktrackCardinalityOps || BacktrackChoices)
					{
						var backtrackres = Backtrack(node, skips);
						if (backtrackres != null && backtrackres.Success)
						{
							return backtrackres;
						}
					}
					SaveLongestMatch(node, c);
					break;
				}
				SaveLongestMatch(node, c);
			}
			return node;
		}
		internal override ParseNode Backtrack(ParseNode node, List<Terminal> skips)
		{
			node.Success = false;
			while (node.Nodes.Count > 0 && !node.Success)
			{
				bool backtracked = false;
				for (int i = node.Nodes.Count - 1; i >= 0; i--)
				{
					var length = node.Nodes[i].Length;
					var backtrackres = node.Nodes[i].Backtrack(skips);
					if (backtrackres == null || !backtrackres.Success)
					{
						node.Length = backtrackres.Index + backtrackres.Length - node.Index;
						node.Nodes.RemoveAt(i);
					}
					else
					{
						backtracked = true;
						node.Length = backtrackres.Index + backtrackres.Length - node.Index;
						break;
					}
				}
				if (backtracked)
				{
					node.Success = true;
					for (int i = node.Nodes.Count; i < Symbols.Count; i++)
					{
						var symbol = Symbols[i];
						var c = symbol.Parse(node, skips);
						if (c != null && c.Success)
						{
							node.Length = c.Index + c.Length - node.Index;
							node.Nodes.Add(c);
						}
						else
						{
							node.Success = false;
							break;
						}
					}
				}
			}
			return node;
		}

		public override string ToString()
		{
			if (!string.IsNullOrEmpty(Name))
				return Name;
			var val = "(";
			foreach (var symbol in Symbols)
			{
				val += symbol.ToString() + " ";
			}
			return val.TrimEnd() + ")";
		}
		internal override void CollectFirstTerminals(List<Rule> visited, List<Terminal> FirstTerminals)
		{
			visited.Add(this);

			foreach (var symbol in Symbols)
			{
				if (visited.Contains(symbol) || FirstTerminals.Contains(symbol))
					continue;
				if (symbol.IsTerminal)
					FirstTerminals.Add(symbol as Terminal);
				else
					(symbol as NonTerminal).CollectFirstTerminals(visited, FirstTerminals);
				break; // only first
			}
		}
	}
}
