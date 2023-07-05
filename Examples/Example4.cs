﻿using Octopartite.ParserRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octopartite.Examples
{
	public class Example4 : AbstractExample
	{
		public Example4()
		{
			Rule.DefaultBacktrackCardinalityOps = false;
			Rule.DefaultBacktrackChoices = true;

			// Start Generated Code
			Terminal LIST = new RegexTerminal(@"list");
			Terminal END = new RegexTerminal(@"end");
			Terminal ID = new RegexTerminal(@"[a-z_][a-z_0-9]*");
			Terminal _EOF_ = new RegexTerminal(@"$");
			Concat List = new Concat();
			Start = new Concat();
			Terminal WHITESPACE = new RegexTerminal(@"\G\s+");
			LIST.Name="LIST";

			END.Name="END";

			ID.Name="ID";

			_EOF_.Name="_EOF_";

			List.Name="List";
			Concat concat1 = new Concat();
			concat1.Symbols.Add(LIST);
			ZeroOrMore unary2 = new ZeroOrMore();
			unary2.Symbol=ID;
			concat1.Symbols.Add(unary2);
			concat1.Symbols.Add(END);
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
				LIST -> @""list"";
				END -> @""end"";
				ID ->@""[a-z_][a-z_0-9]*"";
				_EOF_ -> @""$"";
				List->  LIST ID* END;
				Start -> List _EOF_;
				WHITESPACE -> @""\G\s+"";
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
