using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace EasyParser.ParserRules
{
	public class OneOrMore : MinMaxOccurence
	{
		public OneOrMore() : base(1, int.MaxValue)
		{
		}
		public override string ToString()
		{
			if (!string.IsNullOrEmpty(Name))
				return Name;
			var val = "";

			val += Symbol.ToString();
			return val + "+";
		}
	}

}
