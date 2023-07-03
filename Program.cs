

using System;
using System.Reflection;
using System.Text;
using EasyParser;
using EasyParser.Examples;
using EasyParser.ParserRules;

//GenerateBNFGrammarParserCode();
//GenerateVeryAmbiguousGrammarParserCode();


var listStr = "toto list list toto end TEST1";
var listParseNode = new VeryAmbigousGrammarParser().Parse(listStr);
Console.WriteLine(listParseNode);
if (listParseNode.Length != listStr.Length && listParseNode.LongestMatch != null)
{
	var line = listStr.Take(listParseNode.LongestMatch.Length).Count(c => c == '\n') + 1;
	var posLine = listStr.Select((value, index) => new { value, index })
				.Where(pair => pair.value == '\n')
				.Select(pair => pair.index + 1)
				.Take(line - 1)
				.DefaultIfEmpty(1) // Handle line = 1
				.Last();
	var posLineP1 = listStr.Select((value, index) => new { value, index })
				.Where(pair => pair.value == '\n')
				.Select(pair => pair.index + 1)
				.Take(line)
				.DefaultIfEmpty(1) // Handle line = 1
				.Last();
	var col = listParseNode.LongestMatch.Length - posLine;
	Console.WriteLine("Error ("+line+","+col+"): ");
	Console.WriteLine(listStr);
	Console.WriteLine("^".PadLeft(col+1, ' '));
	Console.WriteLine("Expected " +
		string.Join(",", GetExpectedTerminals(listParseNode.LongestMatch.LastNode).Select(s => s.ToString())));
	Console.WriteLine(listParseNode.LongestMatch);
	return;
}
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

void GenerateVeryAmbiguousGrammarParserCode()
{
	var grammarStr = @"
		ID ->@""[a-zA-Z_][a-zA-Z_0-9]*"";
		LIST -> @""list"";
		END -> @""end"";
		TEST1 -> @""TEST1"";
		TEST2 -> @""TEST0"";
		List->ID? (ID|LIST) LIST ID* END;
		Start -> (List TEST1|List TEST2);
		WHITESPACE -> @""\G\s+"";
";
	GenerateParserCode(grammarStr);
}
void GenerateBNFGrammarParserCode()
{
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
		WHITESPACE -> @""\G\s+"";
		Start -> Production * EOF;
		Production->IDENTIFIER ARROW Rule SEMICOLON;
		Rule->VERBATIM_STRING | STRING | Subrule;
		Subrule->ConcatRule(PIPE ConcatRule)*;
		ConcatRule->Symbol+;
		Symbol-> (IDENTIFIER | (BRACKETOPEN Subrule BRACKETCLOSE) ) UNARYOPER ?;
";
	GenerateParserCode(grammarStr);
}
void GenerateParserCode(string grammarStr)
{
	var node = new Parser().Parse(grammarStr);
	if (node.Length != grammarStr.Length && node.LongestMatch != null)
	{
		var line = grammarStr.Take(node.LongestMatch.Length).Count(c => c == '\n') + 1;
		var posLine = grammarStr.Select((value, index) => new { value, index })
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
}