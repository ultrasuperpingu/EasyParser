using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octopartite.ParserRules
{
	public class StringTerminal : Terminal
	{
		public StringTerminal(string pattern) : base(pattern)
		{
		}

		protected override ParseNode Parse(string input, int index)
		{
			if (index + Pattern.Length <= input.Length && input.Substring(index, Pattern.Length) == Pattern)
			{
				ParseNode node = new ParseNode(this, input, index, Pattern.Length);
				node.Success = true;
				return node;
			}
			return new ParseNode(this, input, index);
		}

		public override string ToString()
		{
			if (!string.IsNullOrEmpty(Name))
				return Name;
			var val = "(" + Pattern + ")";
			return val;
		}
	}
}
