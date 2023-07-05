using Octopartite.ParserRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octopartite.Examples
{
	/// <summary>
	/// This grammar is not LL1. The parser have to use the callstack to backtrack.
	/// <br/>
	/// For example, when trying to parse "[test, test2, test3]]":<br/>
	/// It parse a first time the DelimitedList, then fails because it is not followed by a SQUAREBRACKETOPEN
	/// and pop the parsed DelimitedList, and then parses it again and success while finding the SQUAREBRACKETCLOSE<br/>
	/// <br/>
	/// This is of course a poorly design grammar and should be avoided but demonstrates the possibilities of the parser generator.
	/// </summary>
	public class Example3 : AbstractExample
	{
		public Example3()
		{
			Rule.DefaultBacktrackCardinalityOps = false;
			Rule.DefaultBacktrackChoices = false;

			// Start Generated Code
			Terminal ID = new RegexTerminal(@"\G[a-z_][a-z_0-9]*");
			Terminal SQUAREBRACKETOPEN = new StringTerminal("[");
			Terminal SQUAREBRACKETCLOSE = new StringTerminal("]");
			Terminal COMMA = new StringTerminal(",");
			Terminal _EOF_ = new RegexTerminal(@"\G$");
			Concat List = new Concat();
			Concat DelimitedList = new Concat();
			Start = new Concat();
			Terminal WHITESPACE = new RegexTerminal(@"\G\s+");
			ID.Name="ID";

			SQUAREBRACKETOPEN.Name="SQUAREBRACKETOPEN";

			SQUAREBRACKETCLOSE.Name="SQUAREBRACKETCLOSE";

			COMMA.Name="COMMA";

			_EOF_.Name="_EOF_";

			List.Name="List";
			Concat concat1 = new Concat();
			concat1.Symbols.Add(ID);
			ZeroOrMore unary2 = new ZeroOrMore();
			Concat concat3 = new Concat();
			concat3.Symbols.Add(COMMA);
			concat3.Symbols.Add(ID);
			unary2.Symbol = concat3;
			concat1.Symbols.Add(unary2);
			List.Symbols.Add(concat1);

			DelimitedList.Name="DelimitedList";
			Concat concat4 = new Concat();
			concat4.Symbols.Add(SQUAREBRACKETOPEN);
			Optional unary5 = new Optional();
			unary5.Symbol=List;
			concat4.Symbols.Add(unary5);
			concat4.Symbols.Add(SQUAREBRACKETCLOSE);
			DelimitedList.Symbols.Add(concat4);

			Start.Name="Start";
			Concat concat6 = new Concat();
			Choice choice7 = new Choice();
			Concat concat8 = new Concat();
			concat8.Symbols.Add(DelimitedList);
			concat8.Symbols.Add(SQUAREBRACKETOPEN);
			choice7.Symbols.Add(concat8);
			Concat concat9 = new Concat();
			concat9.Symbols.Add(DelimitedList);
			concat9.Symbols.Add(SQUAREBRACKETCLOSE);
			choice7.Symbols.Add(concat9);
			concat6.Symbols.Add(choice7);
			concat6.Symbols.Add(_EOF_);
			Start.Symbols.Add(concat6);

			WHITESPACE.Name="WHITESPACE";
			// End Generated Code

			Skips.Add(WHITESPACE);
		}

		public static void GenerateParserCode()
		{
			var grammarStr = @"
				ID -> R""[a-z_][a-z_0-9]*"";
				SQUAREBRACKETOPEN -> ""["";
				SQUAREBRACKETCLOSE -> ""]"";
				COMMA -> "","";
				_EOF_ -> R""$"";
				List->  ID (COMMA ID)*;
				DelimitedList -> SQUAREBRACKETOPEN List? SQUAREBRACKETCLOSE;
				Start -> (DelimitedList SQUAREBRACKETOPEN|DelimitedList SQUAREBRACKETCLOSE) _EOF_;
				WHITESPACE -> R""\G\s+"";
			";
			Console.WriteLine("// Example3 parser code: copy/paste it in Example3.cs (in constructor)");
			Console.WriteLine("//  and replace line:");
			Console.WriteLine("//    \"Concat Start = new Concat();\"");
			Console.WriteLine("//  by");
			Console.WriteLine("//    \"Start = new Concat();\"");
			Console.WriteLine("// Start Generated Code");
			GrammarUtil.GenerateParserCode(grammarStr);
			Console.WriteLine("// End Generated Code");
		}
	}
}
