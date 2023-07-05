

using System;
using System.Numerics;
using System.Reflection;
using System.Text;
using Octopartite;
using Octopartite.Examples;
using Octopartite.ParserRules;

//GrammarParser.GenerateParserCode();
//Example1.GenerateParserCode();
//Example2.GenerateParserCode();
//Example3.GenerateParserCode();
//Example4.GenerateParserCode();

var example1Parser = new Example1();
Console.WriteLine(example1Parser.Parse("[]"));
Console.WriteLine(example1Parser.Parse("[test test2]"));


var example2Parser = new Example2();
Console.WriteLine(example2Parser.Parse("[]"));
Console.WriteLine(example2Parser.Parse("[test, test2, test3]"));

var example3Parser = new Example3();
Console.WriteLine(example3Parser.Parse("[test, test2, test3]]"));

var example4Parser = new Example4();
Console.WriteLine(example4Parser.Parse("list end"));
Console.WriteLine(example4Parser.Parse("list test test2 end"));

var listStr = "id_SUFFIX list end list toto end TEST2";
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
	Console.WriteLine("^".PadLeft(col+2, ' '));
	Console.WriteLine("Expected " +
		string.Join(",", GrammarUtil.GetExpectedTerminals(listParseNode.LongestMatch.LastNode).Select(s => s.ToString())));
	Console.WriteLine(listParseNode.LongestMatch);
	return;
}




