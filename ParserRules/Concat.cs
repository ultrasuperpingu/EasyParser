using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octopartite.ParserRules
{
	public class Concat : NonTerminal
	{
		public override ParseNode Parse(string input, int index, List<Terminal> skips)
		{
			ParseNode node = new ParseNode(this, input, index);
			node.Success = true;
			for (int i = 0; i < Symbols.Count; i++)
			{
				var symbol = Symbols[i];
				var c = symbol.Parse(input, index + node.Length, skips);
				if (c != null && c.Success)
				{
					node.Length = c.Index + c.Length - index;
					node.Nodes.Add(c);
					c.Parent = node;
				}
				else
				{
					node.Success = false;
					//TODO: check it's working
					//TODO: add ability to disallow backtracking before a node (like with ! in prolog) ?
					if (BacktrackCardinalityOps || BacktrackChoices)
					{
						var backtrackres = Backtrack(node, skips);
						if (backtrackres != null && backtrackres.Success)
						{
							return backtrackres;
						}
					}
					if (node.LongestMatch == null &&
						(node.Length > 0 || c.Length > 0 || c.LongestMatch != null && c.LongestMatch.Length > 0))
					{
						node.LongestMatch = new ParseNode(node);
						if (c.LongestMatch != null)
							c = c.LongestMatch;
						node.LongestMatch.Length = c.Index + c.Length - index;
						node.LongestMatch.Nodes.Add(c);
						c.Parent = node.LongestMatch;
					}
					break;
				}
				if (node.LongestMatch == null && c.LongestMatch != null)
				{
					node.LongestMatch = new ParseNode(node);
					node.LongestMatch.Length = c.LongestMatch.Index + c.LongestMatch.Length - index;
					node.LongestMatch.Nodes.Add(c.LongestMatch);
					c.LongestMatch.Parent = node.LongestMatch;
				}
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
						var c = symbol.Parse(node.input, node.Index + node.Length, skips);
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
