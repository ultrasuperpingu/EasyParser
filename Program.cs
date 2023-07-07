

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
//Example5.GenerateParserCode();
//VeryAmbigousGrammarParser.GenerateParserCode();

Console.WriteLine("Example1:");
var example1Parser = new Example1();
Console.WriteLine(example1Parser.Parse("[]"));
Console.WriteLine(example1Parser.Parse("[test test2]"));

Console.WriteLine("Example2:");
var example2Parser = new Example2();
Console.WriteLine(example2Parser.Parse("[]"));
Console.WriteLine(example2Parser.Parse("[test, test2, test3]"));

Console.WriteLine("Example3:");
var example3Parser = new Example3();
Console.WriteLine(example3Parser.Parse("[test, test2, test3] ]"));

Console.WriteLine("Example4:");
var example4Parser = new Example4();
Console.WriteLine(example4Parser.Parse("list end"));
Console.WriteLine(example4Parser.Parse("list test test2 end"));

Console.WriteLine("Example5:");
var example5Parser = new Example5();
Console.WriteLine(example5Parser.Parse("test_SUFFIX"));

Console.WriteLine("VeryAmbiguousGrammar:");
var veryAmbiguousGrammar = new VeryAmbigousGrammarParser();
Console.WriteLine(veryAmbiguousGrammar.Parse("test 12 list end TEST1"));
Console.WriteLine(veryAmbiguousGrammar.Parse("id_SUFFIX list end list toto end TEST2"));
//Error:
Console.WriteLine(veryAmbiguousGrammar.Parse("id_SUFFIX list end list toto end TEST3"));





