using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Octopartite.ParserRules;

namespace Octopartite
{
    public class GrammarParser
	{
		public Concat Start = null;
		List<Terminal> Skips = new List<Terminal>();
		
		public GrammarParser()
		{
			ParserRules.Rule.DefaultBacktrackCardinalityOps = true;
			ParserRules.Rule.DefaultBacktrackChoices = true;

			// Start Generated Code
			Terminal BRACKETOPEN = new StringTerminal("(");
			Terminal BRACKETCLOSE = new StringTerminal(")");
			Terminal IDENTIFIER = new RegexTerminal(@"[a-zA-Z_][a-zA-Z0-9_]*");
			Terminal ARROW = new StringTerminal("->");
			Terminal EOF = new RegexTerminal(@"$");
			Terminal STRING = new RegexTerminal(@"\""(\\\""|[^\""])*\""");
			Terminal VERBATIM_STRING = new RegexTerminal(@"(R|@)\""(\""\""|[^\""])*\""");
			Terminal PIPE = new StringTerminal("|");
			Terminal UNARYOPER = new RegexTerminal(@"(\*|\+|\?)");
			Terminal SEMICOLON = new StringTerminal(";");
			Terminal WHITESPACES = new RegexTerminal(@"\G\s+");
			Start = new Concat();
			Concat Production = new Concat();
			Concat Rule = new Concat();
			Concat Subrule = new Concat();
			Concat ConcatRule = new Concat();
			Concat Symbol = new Concat();
			BRACKETOPEN.Name="BRACKETOPEN";

			BRACKETCLOSE.Name="BRACKETCLOSE";

			IDENTIFIER.Name="IDENTIFIER";

			ARROW.Name="ARROW";

			EOF.Name="EOF";

			STRING.Name="STRING";

			VERBATIM_STRING.Name="VERBATIM_STRING";

			PIPE.Name="PIPE";

			UNARYOPER.Name="UNARYOPER";

			SEMICOLON.Name="SEMICOLON";

			WHITESPACES.Name="WHITESPACES";

			Start.Name="Start";
			Concat concat1 = new Concat();
			ZeroOrMore unary2 = new ZeroOrMore();
			unary2.Symbol=Production;
			concat1.Symbols.Add(unary2);
			concat1.Symbols.Add(EOF);
			Start.Symbols.Add(concat1);

			Production.Name="Production";
			Concat concat3 = new Concat();
			concat3.Symbols.Add(IDENTIFIER);
			concat3.Symbols.Add(ARROW);
			concat3.Symbols.Add(Rule);
			concat3.Symbols.Add(SEMICOLON);
			Production.Symbols.Add(concat3);

			Rule.Name="Rule";
			Choice choice4 = new Choice();
			choice4.Symbols.Add(VERBATIM_STRING);
			choice4.Symbols.Add(STRING);
			choice4.Symbols.Add(Subrule);
			Rule.Symbols.Add(choice4);

			Subrule.Name="Subrule";
			Concat concat5 = new Concat();
			concat5.Symbols.Add(ConcatRule);
			ZeroOrMore unary6 = new ZeroOrMore();
			Concat concat7 = new Concat();
			concat7.Symbols.Add(PIPE);
			concat7.Symbols.Add(ConcatRule);
			unary6.Symbol = concat7;
			concat5.Symbols.Add(unary6);
			Subrule.Symbols.Add(concat5);

			ConcatRule.Name="ConcatRule";
			OneOrMore unary8 = new OneOrMore();
			unary8.Symbol=Symbol;
			ConcatRule.Symbols.Add(unary8);

			Symbol.Name="Symbol";
			Concat concat9 = new Concat();
			Choice choice10 = new Choice();
			choice10.Symbols.Add(IDENTIFIER);
			Concat concat11 = new Concat();
			concat11.Symbols.Add(BRACKETOPEN);
			concat11.Symbols.Add(Subrule);
			concat11.Symbols.Add(BRACKETCLOSE);
			choice10.Symbols.Add(concat11);
			concat9.Symbols.Add(choice10);
			Optional unary12 = new Optional();
			unary12.Symbol=UNARYOPER;
			concat9.Symbols.Add(unary12);
			Symbol.Symbols.Add(concat9);
			// End Generated Code

			Skips.Add(WHITESPACES);
		}
		public ParseNode Parse(string input)
		{
			return Start.Parse(input, 0, Skips);
		}
		public static void GenerateParserCode()
		{
			var grammarStr = @"
				BRACKETOPEN -> ""("";
				BRACKETCLOSE -> "")"";
				IDENTIFIER -> R""[a-zA-Z_][a-zA-Z0-9_]*"";
				ARROW -> ""->"";
				EOF -> R""$"";
				STRING -> R""\""""(\\\""""|[^\""""])*\"""""";
				VERBATIM_STRING -> R""(R|@)\""""(\""""\""""|[^\""""])*\"""""";
				PIPE -> ""|"";
				UNARYOPER -> R""(\*|\+|\?)"";
				SEMICOLON -> "";"";
				WHITESPACES -> R""\G\s+"";
				Start -> Production * EOF;
				Production->IDENTIFIER ARROW Rule SEMICOLON;
				Rule->VERBATIM_STRING | STRING | Subrule;
				Subrule->ConcatRule(PIPE ConcatRule)*;
				ConcatRule->Symbol+;
				Symbol-> (IDENTIFIER | (BRACKETOPEN Subrule BRACKETCLOSE) ) UNARYOPER ?;
			";
			GrammarUtil.GenerateParserCode(grammarStr);
		}
	}
}
