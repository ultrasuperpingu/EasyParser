using Octopartite.ParserRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octopartite.Examples
{
	/// <summary>
	/// Really simple unambiguous grammar (LL1)
	/// </summary>
	public class Example1 : AbstractExample
	{
		public Example1()
		{
			Rule.DefaultBacktrackCardinalityOps = false;
			Rule.DefaultBacktrackChoices = false;

			// Start Generated Code
			Terminal ID = new RegexTerminal(@"[a-z_][a-z_0-9]*");
			Terminal SQUAREBRACKETOPEN = new StringTerminal(@"[");
			Terminal SQUAREBRACKETCLOSE = new StringTerminal(@"]");
			Terminal COMMA = new StringTerminal(@",");
			Terminal _EOF_ = new RegexTerminal(@"$");
			Concat List = new Concat();
			Start = new Concat();
			Terminal WHITESPACE = new RegexTerminal(@"\G\s+");
			ID.Name="ID";

			SQUAREBRACKETOPEN.Name="SQUAREBRACKETOPEN";

			SQUAREBRACKETCLOSE.Name="SQUAREBRACKETCLOSE";

			COMMA.Name="COMMA";

			_EOF_.Name="_EOF_";

			List.Name="List";
			Concat concat1 = new Concat();
			concat1.Symbols.Add(SQUAREBRACKETOPEN);
			ZeroOrMore unary2 = new ZeroOrMore();
			unary2.Symbol=ID;
			concat1.Symbols.Add(unary2);
			concat1.Symbols.Add(SQUAREBRACKETCLOSE);
			List.Symbols.Add(concat1);

			Start.Name="Start";
			Concat concat3 = new Concat();
			concat3.Symbols.Add(List);
			concat3.Symbols.Add(_EOF_);
			Start.Symbols.Add(concat3);

			WHITESPACE.Name="WHITESPACE";
			// End Generated Code

			Skips.Add(WHITESPACE);
		}


		public static void GenerateParserCode()
		{
			var grammarStr = @"
				ID ->R""[a-z_][a-z_0-9]*"";
				SQUAREBRACKETOPEN -> @""["";
				SQUAREBRACKETCLOSE -> @""]"";
				COMMA -> @"","";
				_EOF_ -> R""$"";
				List-> SQUAREBRACKETOPEN ID* SQUAREBRACKETCLOSE;
				Start -> List _EOF_;
				WHITESPACE -> R""\G\s+"";
			";
			Console.WriteLine("// Example1 parser code: copy/paste it in Example1.cs (in constructor)");
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
