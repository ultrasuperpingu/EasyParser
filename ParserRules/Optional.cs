using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Octopartite.ParserRules
{
	public class Optional : MinMaxOccurence
	{
		public Optional() : base(0, 1)
		{
		}
		public override string ToString()
		{
			if (!string.IsNullOrEmpty(Name))
				return Name;

			var val = Symbol.ToString();
			return val + "?";
		}
	}
}
