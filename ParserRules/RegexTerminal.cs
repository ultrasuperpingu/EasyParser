using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Octopartite.ParserRules
{
	public class RegexTerminal : Terminal
	{
		public RegexTerminal(string pattern) : base(pattern)
		{
			r = new Regex(pattern, RegexOptions.Compiled);
		}
		protected Regex r;
		protected override ParseNode Parse(string input, int index)
		{
			var m = r.Match(input, index);
			if (m.Success && m.Index == index)
			{
				ParseNode node = new ParseNode(this, input, index, m.Length);
				node.Success = true;
				return node;
			}
			return new ParseNode(this, input, index);
		}

		public override string ToString()
		{
			if (!string.IsNullOrEmpty(Name))
				return Name;
			var val = "(Regex(" + Pattern + "))";
			return val;
		}
	}
}
