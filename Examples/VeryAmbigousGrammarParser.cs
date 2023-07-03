using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EasyParser.ParserRules;


namespace EasyParser.Examples
{
	public class VeryAmbigousGrammarParser
	{
		public Concat Start = null;
		List<Terminal> Skips = new List<Terminal>();
		public VeryAmbigousGrammarParser()
		{
			Terminal ID = new RegexTerminal(@"[a-zA-Z_][a-zA-Z_0-9]*");
			Terminal LIST = new RegexTerminal(@"list");
			Terminal END = new RegexTerminal(@"end");
			Terminal TEST1 = new RegexTerminal(@"TEST1");
			Terminal TEST2 = new RegexTerminal(@"TEST0");
			Concat List = new Concat();
			Start = new Concat();
			Terminal WHITESPACE = new RegexTerminal(@"\G\s+");
			ID.Name="ID";

			LIST.Name="LIST";

			END.Name="END";

			TEST1.Name="TEST1";

			TEST2.Name="TEST2";

			List.Name="List";
			Concat concat1 = new Concat();
			Optional unary2 = new Optional();
			unary2.Symbol=ID;
			concat1.Symbols.Add(unary2);
			Choice choice3 = new Choice();
			choice3.Symbols.Add(ID);
			choice3.Symbols.Add(LIST);
			concat1.Symbols.Add(choice3);
			concat1.Symbols.Add(LIST);
			ZeroOrMore unary4 = new ZeroOrMore();
			unary4.Symbol=ID;
			concat1.Symbols.Add(unary4);
			concat1.Symbols.Add(END);
			List.Symbols.Add(concat1);

			Start.Name="Start";
			Choice choice5 = new Choice();
			Concat concat6 = new Concat();
			concat6.Symbols.Add(List);
			concat6.Symbols.Add(TEST1);
			choice5.Symbols.Add(concat6);
			Concat concat7 = new Concat();
			concat7.Symbols.Add(List);
			concat7.Symbols.Add(TEST2);
			choice5.Symbols.Add(concat7);
			Start.Symbols.Add(choice5);

			WHITESPACE.Name="WHITESPACE";

			Skips.Add(WHITESPACE);
		}
		public ParseNode Parse(string input)
		{
			return Start.Parse(input, 0, Skips);
		}
	}
}
