using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyParser.ParserRules
{
	public class ZeroOrMore : MinMaxOccurence
	{
		public ZeroOrMore() : base()
		{
		}
		public override string ToString()
		{
			if (!string.IsNullOrEmpty(Name))
				return Name;
			var val = "";

			val += Symbol.ToString();
			return val + "*";
		}
	}
}
