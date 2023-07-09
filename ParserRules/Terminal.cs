using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Octopartite.ParserRules
{
	public abstract class Terminal : Rule
	{
		public Terminal(string pattern)
		{
			Pattern = pattern;
		}
		public string Pattern { get; set; }
		public override bool IsTerminal => true;

		internal override ParseNode Parse(ParseNode parent, List<Terminal> skips)
		{
			var input = parent.input;
			var index = parent.End;
			var skipped = Skip(input, index, skips);
			if (skipped.Count > 0)
			{
				index = skipped[skipped.Count - 1].Index + skipped[skipped.Count - 1].Length;
			}
			var node = Parse(parent, index);
			if (node.Success)
				node.Skipped = skipped;
			return node;
		}
		protected List<ParseNode> Skip(string input, int index, List<Terminal> skips)
		{
			List<ParseNode> skipped = new List<ParseNode>();
			ParseNode parseNode = null;
			do
			{
				for (int i = 0; i < skips.Count; i++)
				{
					parseNode = skips[i].Parse(input, index);
					if (parseNode != null && parseNode.Success)
					{
						skipped.Add(parseNode);
						index += parseNode.Length;
						break;
					}
				}
			}
			while (parseNode != null && parseNode.Success);
			return skipped;
		}
		protected abstract ParseNode Parse(string input, int index);
		protected abstract ParseNode Parse(ParseNode parent, int index);

		internal override ParseNode Backtrack(ParseNode node, List<Terminal> skips)
		{
			node.Success = false;
			return node;
		}
		public override List<Terminal> GetFirstTerminals()
		{
			List<Terminal> FirstTerminals = new List<Terminal>();
			FirstTerminals.Add(this);
			return FirstTerminals;
		}
	}

}
