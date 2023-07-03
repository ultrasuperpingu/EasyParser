using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyParser.ParserRules
{
	public abstract class Rule
	{
		public bool BacktrackCardinalityOps { get; set; } = true;
		public bool BacktrackChoices { get; set; } = true;
		public abstract bool IsTerminal { get; }
		public abstract ParseNode Parse(string input, int index, List<Terminal> skips);
		internal abstract ParseNode Backtrack(ParseNode node, List<Terminal> skips);
		public abstract List<Terminal> GetFirstTerminals();
		public string Name { get; set; }
	}
}
