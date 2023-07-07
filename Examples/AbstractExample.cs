using Octopartite.ParserRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Octopartite.Examples
{
	public abstract class AbstractExample
	{
		public Concat Start = null;
		public List<Terminal> Skips = new List<Terminal>();
		public ParseNode Parse(string input)
		{
			var node = Start.Parse(input, 0, Skips);
			if (node.Length != input.Length && node.LongestMatch != null)
			{
				var (line, col, posLine) = input.GetLineColumnLinePos(node.LongestMatch.Length);
				var posLineP1 = input.GetLinePos(line+1);
				Console.WriteLine("Error ("+line+","+col+"): ");
				Console.WriteLine(input.Substring(posLine, posLineP1 - posLine).Trim());
				Console.WriteLine("^".PadLeft(col+1, ' '));
				Console.WriteLine("Expected " +
					string.Join(",", GrammarUtil.GetExpectedTerminals(node.LongestMatch.LastNode).Select(s => s.ToString())));
			}
			return node;
		}
	}
}
