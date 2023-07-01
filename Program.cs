

using System;
using System.Reflection;
using System.Text;
using EasyParser;
using EasyParser.ParserRules;

var grammarStr = @"
BRACKETOPEN -> @""\("";
BRACKETCLOSE -> @""\)"";
IDENTIFIER -> @""[a-zA-Z_][a-zA-Z0-9_]*"";
ARROW -> @""->"";
EOF -> @""$"";
STRING -> @""\""""(\\\""""|[^\""""])*\"""""";
VERBATIM_STRING -> @""(R|@)\""""(\""""\""""|[^\""""])*\"""""";
PIPE -> @""\|"";
UNARYOPER -> @""(\*|\+|\?)"";
SEMICOLON -> @"";"";
Start -> Production * EOF;
Production->IDENTIFIER ARROW Rule SEMICOLON;
Rule->VERBATIM_STRING | STRING | Subrule;
Subrule->ConcatRule(PIPE ConcatRule)*;
ConcatRule->Symbol+;
Symbol-> (IDENTIFIER | (BRACKETOPEN Subrule BRACKETCLOSE) ) UNARYOPER ?;
";

var node = new Parser().Parse(grammarStr);
if(node.Length != grammarStr.Length)
{
	var line = grammarStr.Take(node.LongestMatch.Length).Count(c => c == '\n') + 1;
	var posLine =  grammarStr.Select((value, index) => new { value, index })
				.Where(pair => pair.value == '\n')
				.Select(pair => pair.index + 1)
				.Take(line - 1)
				.DefaultIfEmpty(1) // Handle line = 1
				.Last();
	var posLineP1 = grammarStr.Select((value, index) => new { value, index })
				.Where(pair => pair.value == '\n')
				.Select(pair => pair.index + 1)
				.Take(line)
				.DefaultIfEmpty(1) // Handle line = 1
				.Last();
	var col = node.LongestMatch.Length - posLine;
	//var rule = node.LongestMatch.LastNamedNode.Symbol as NonTerminal;
	Console.WriteLine("Error ("+line+","+col+"): ");
	Console.WriteLine(grammarStr.Substring(posLine, posLineP1 - posLine).Trim());
	Console.WriteLine("^".PadLeft(col+1, ' '));
	Console.WriteLine("Expected " +
		string.Join(",", GetExpectedTerminals(node.LongestMatch.LastNode).Select(s => s.ToString())));
	return;
}
node.Simplify();

Console.WriteLine(node.PrintTree());
Console.WriteLine(new ParserGenerator().Generate(node));

List<Terminal> GetExpectedTerminals(ParseNode firstFailedNode)
{
	List<Terminal> terms = new List<Terminal>();
	var child = firstFailedNode;
	var n = firstFailedNode.Parent;
	while (n != null)
	{
		if(n.Symbol is Concat)
		{
			Concat c = n.Symbol as Concat;
			var index = n.Nodes.IndexOf(child);
			if(c.Symbols.Count > index)
			{
				terms.AddRange(c.Symbols[index].GetFirstTerminals());
				if (index < c.Symbols.Count-1)
					break;
			}
		}
		else if(n.Symbol is Choice)
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