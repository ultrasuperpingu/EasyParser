using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyParser.ParserRules
{
	public abstract class NonTerminal : Rule
	{
		public override bool IsTerminal => false;
		public List<Rule> Symbols { get; set; } = new List<Rule>();
		public override List<Terminal> GetFirstTerminals()
		{
			List<Rule> visited = new List<Rule>();
			List<Terminal> firstTerminals = new List<Terminal>();
			CollectFirstTerminals(visited, firstTerminals);
			return firstTerminals;
		}
		internal virtual void CollectFirstTerminals(List<Rule> visited, List<Terminal> firstTerminals)
		{
			visited.Add(this);

			foreach (var symbol in Symbols)
			{
				if (visited.Contains(symbol) || firstTerminals.Contains(symbol))
					continue;
				if (symbol.IsTerminal)
					firstTerminals.Add(symbol as Terminal);
				else
					(symbol as NonTerminal).CollectFirstTerminals(visited, firstTerminals);
			}
		}
	}
}
