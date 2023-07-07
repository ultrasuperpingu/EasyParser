using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Octopartite.ParserRules
{
	public class MinMaxOccurence : NonTerminal
	{
		public int Min { get; set; } = 0;
		public int Max { get; set; } = int.MaxValue;
		public Rule Symbol
		{
			get => Symbols.Count > 0 ? Symbols[0] : null;
			set
			{
				if (Symbols.Count > 0)
					Symbols[0] = value;
				else
					Symbols.Add(value);
			}
		}
		public MinMaxOccurence()
		{
		}
		public MinMaxOccurence(int min, int max)
		{
			Min = min;
			Max = max;
		}
		public override ParseNode Parse(string input, int index, List<Terminal> skips)
		{
			ParseNode node = new ParseNode(this, input, index);
			ParseNode c = null;
			int count = 0;
			do
			{
				c = Symbol.Parse(node.input, node.Index + node.Length, skips);
				if (c != null && c.Success)
				{
					node.Success = true;
					node.Length = c.Index + c.Length - index;
					node.Nodes.Add(c);
					c.Parent = node;
					count++;
				}
			} while (c != null && c.Success && count < Max);
			//if (count <= Max)
			{
				SaveLongestMatch(node, c);
				/*if (node.LongestMatch == null &&
					(c.Length > 0 || c.LongestMatch != null))
				{
					node.LongestMatch = new ParseNode(node);
					if (c.LongestMatch != null)
						c = c.LongestMatch;
					node.LongestMatch.Length = c.Index + c.Length - index;
					node.LongestMatch.Nodes.Add(c);
					c.Parent = node.LongestMatch;
				}*/
			}
			node.Success = count >= Min;
			node.symbolIndex = count;
			return node;
		}

		internal override ParseNode Backtrack(ParseNode node, List<Terminal> skips)
		{
			node.Success = false;
			if (node.symbolIndex < 0) // not parsed yet: ERROR
			{
				throw new InvalidOperationException("Not parsed yet, can't backtrack");
			}
			else if (node.symbolIndex == Min) // already parse and no symbol found, fails
			{
				node.Success = false;
				node.Length = 0;
				node.Nodes.Clear();
			}
			else if (node.symbolIndex > Min) // already found at least min+1, remove one and return sucess
			{
				var length = node.Nodes[node.symbolIndex - 1].Length;
				var backtrackres = node.Nodes[node.symbolIndex - 1].Backtrack(skips);
				if (backtrackres != null && backtrackres.Success)
				{
					node.Success = true;
					node.Length = backtrackres.Index + backtrackres.Length - node.Index;
					return node;
				}
				if (BacktrackCardinalityOps)
				{
					node.Success = true;
					node.symbolIndex--;
					var removed = node.Nodes[node.symbolIndex];
					node.Nodes.RemoveAt(node.symbolIndex);
					node.Length -= length;
					if (removed.Skipped != null)
						node.Length -= removed.Skipped.Sum(s => s.Length);
				}
			}
			else // < min and > 0: error
			{
				throw new Exception("Internal Error");
			}
			return node;
		}

		public override string ToString()
		{
			if (!string.IsNullOrEmpty(Name))
				return Name;
			var val = "";

			val += Symbol.ToString();
			return val + "{" + Min + "," + Max + "}";
		}
	}
}
