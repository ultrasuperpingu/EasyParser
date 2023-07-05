using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octopartite.ParserRules
{
	public class Choice : NonTerminal
	{
		public override ParseNode Parse(string input, int index, List<Terminal> skips)
		{
			ParseNode node = new ParseNode(this, input, index);
			node.Success = false;
			ParseNode best = null;
			for (int i = node.symbolIndex + 1; i < Symbols.Count; i++)
			{
				Rule symbol = Symbols[i];
				var c = symbol.Parse(node.input, node.Index, skips);
				if (c != null && c.Success)
				{
					node.Success = true;
					node.Length = c.Index + c.Length - index;
					node.Nodes.Add(c);
					c.Parent = node;
					node.symbolIndex = i;
					break;
				}
				else if (best == null || best.Length < c.Length)
				{
					best = c;
				}
			}
			if (!node.Success)
			{
				if ((node.LongestMatch == null || node.LongestMatch.Length < node.Length) && best != null)
				{
					node.LongestMatch = new ParseNode(node);
					if (best.LongestMatch != null)
						best = best.LongestMatch;
					node.LongestMatch.Length = best.Index + best.Length - index;
					node.LongestMatch.Nodes.Add(best);
					best.Parent = node.LongestMatch;
				}

			}
			return node;
		}

		internal override ParseNode Backtrack(ParseNode node, List<Terminal> skips)
		{
			node.Success = false;
			var backtrackres = node.Nodes[0].Backtrack(skips);
			if (backtrackres != null && backtrackres.Success)
			{
				node.Success = true;
				node.Length = backtrackres.Index + backtrackres.Length - node.Index;
				return node;
			}
			node.Length = 0;
			node.Nodes.Clear();
			if (BacktrackChoices)
			{
				for (int i = node.symbolIndex + 1; i < Symbols.Count; i++)
				{
					Rule symbol = Symbols[i];
					var c = symbol.Parse(node.input, node.Index, skips);
					if (c != null && c.Success)
					{
						node.Success = true;
						node.Length = c.Index + c.Length - node.Index;
						node.Nodes.Add(c);
						node.symbolIndex = i;
						break;
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
				val += symbol.ToString() + "|";
			}
			return val.TrimEnd('|') + ")";
		}

	}
}
