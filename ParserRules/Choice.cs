using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Octopartite.ParserRules
{
	public class Choice : NonTerminal
	{
		internal override ParseNode Parse(ParseNode parent, List<Terminal> skips)
		{
			var input = parent.input;
			var index = parent.End;
			ParseNode node = parent.CreateNode(this, input, index);
			node.Success = false;
			ParseNode bestFailed = null;
			for (int i = node.symbolIndex + 1; i < Symbols.Count; i++)
			{
				Rule symbol = Symbols[i];
				var c = symbol.Parse(node, skips);
				if (c != null && c.Success)
				{
					node.Success = true;
					node.Length = c.Index + c.Length - index;
					node.Nodes.Add(c);
					//c.Parent = node;
					node.symbolIndex = i;
					break;
				}
				else if (bestFailed == null || bestFailed.Length < c.Length)
				{
					bestFailed = c;
				}
			}
			if (!node.Success)
			{
				SaveLongestMatch(node, bestFailed);
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
					var c = symbol.Parse(node, skips);
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
