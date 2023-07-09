using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Octopartite.ParserRules
{
	public abstract class Rule
	{
		public static bool DefaultBacktrackCardinalityOps { get; set; } = false;
		public bool BacktrackCardinalityOps { get; set; } = DefaultBacktrackCardinalityOps;
		public static bool DefaultBacktrackChoices { get; set; } = false;
		public bool BacktrackChoices { get; set; } = DefaultBacktrackChoices;
		public abstract bool IsTerminal { get; }
		public ParseNode Parse(string input, List<Terminal> skips)
		{
			return Parse(input, 0, skips);
		}
		public ParseNode Parse(string input, int index, List<Terminal> skips)
		{
			ParseNode node = new ParseNode(this, input, index);
			return Parse(input, index, skips, node);
		}
		public ParseNode Parse(string input, int index, List<Terminal> skips, ParseNode tree)
		{
			Parse(tree, skips);
			return tree;
		}
		internal abstract ParseNode Parse(ParseNode parent, List<Terminal> skips);
		internal abstract ParseNode Backtrack(ParseNode node, List<Terminal> skips);
		public abstract List<Terminal> GetFirstTerminals();
		public string Name { get; set; }
	}
}
