using Octopartite.ParserRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octopartite
{
	public static class GrammarUtil
	{
		public static void GenerateParserCode(string grammarStr)
		{
			var node = new GrammarParser().Parse(grammarStr);
			if (node.Length != grammarStr.Length && node.LongestMatch != null)
			{
				var (line, col, posLine) = grammarStr.GetLineColumnLinePos(node.LongestMatch.Length);//.Take(node.LongestMatch.Length).Count(c => c == '\n') + 1;
				var posLineP1 = grammarStr.GetLinePos(line+1);
				Console.WriteLine("Error ("+line+","+col+"): ");
				Console.WriteLine(grammarStr.Substring(posLine, posLineP1 - posLine).Trim());
				Console.WriteLine("^".PadLeft(col+1, ' '));
				Console.WriteLine("Expected " +
					string.Join(",", GetExpectedTerminals(node.LongestMatch.LastNode).Select(s => s.ToString())));
				return;
			}
			node.Simplify();

			//Console.WriteLine(node.PrintTree());
			Console.WriteLine(new ParserGenerator().Generate(node));
		}

		public static List<Terminal> GetExpectedTerminals(ParseNode firstFailedNode)
		{
			List<Terminal> terms = new List<Terminal>();
			var child = firstFailedNode;
			var n = firstFailedNode.Parent;
			while (n != null)
			{
				if (n.Symbol is Concat)
				{
					Concat c = n.Symbol as Concat;
					var index = n.Nodes.IndexOf(child);
					if (c.Symbols.Count > index)
					{
						terms.AddRange(c.Symbols[index].GetFirstTerminals());
						if (index < c.Symbols.Count-1)
							break;
					}
				}
				else if (n.Symbol is Choice)
				{
					terms.AddRange(n.Symbol.GetFirstTerminals());
					if (n.Parent != null && n.Parent.Symbol is not MinMaxOccurence)
						break;
				}
				child = n;
				n = n.Parent;
			}
			terms = terms.Distinct().ToList();
			return terms;
		}
	}
}
