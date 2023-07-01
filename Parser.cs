using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using EasyParser.ParserRules;

namespace EasyParser
{
    public class Parser
	{
		public Concat Grammar = null;
		List<Terminal> Skips = new List<Terminal>();
		//Decls
		public Parser()
		{
			Terminal BRACKETOPEN = new RegexTerminal(@"\(");
			BRACKETOPEN.Name="BRACKETOPEN";
			Terminal BRACKETCLOSE = new RegexTerminal(@"\)");
			BRACKETCLOSE.Name="BRACKETCLOSE";
			Terminal IDENTIFIER = new RegexTerminal(@"[a-zA-Z_][a-zA-Z0-9_]*");
			IDENTIFIER.Name="IDENTIFIER";
			Terminal ARROW = new RegexTerminal(@"->");
			ARROW.Name="ARROW";
			Terminal EOF = new RegexTerminal(@"$");
			EOF.Name="EOF";
			Terminal STRING = new RegexTerminal(@"\""(\\\""|[^\""])*\""");
			STRING.Name="STRING";
			Terminal VERBATIM_STRING = new RegexTerminal(@"(R|@)\""(\""\""|[^\""])*\""");
			VERBATIM_STRING.Name="VERBATIM_STRING";
			Terminal PIPE = new RegexTerminal(@"\|");
			PIPE.Name="PIPE";
			Terminal UNARYOPER = new RegexTerminal(@"(\*|\+|\?)");
			UNARYOPER.Name="UNARYOPER";
			Terminal SEMICOLON = new RegexTerminal(";");
			SEMICOLON.Name="SEMICOLON";
			Terminal SHARPSHARP = new RegexTerminal("##");
			SEMICOLON.Name="SHARPSHARP";
			Terminal PLOUP = new RegexTerminal("PLOUP");
			SEMICOLON.Name="PLOUP";

			Terminal WHITESPACES = new RegexTerminal(@"\G\s+");
			SEMICOLON.Name="WHITESPACES";
			Skips.Add(WHITESPACES);



			//Production->IDENTIFIER ARROW Rule;
			//Rule->VERBATIM_STRING | STRING | Subrule;
			//Subrule->ConcatRule(PIPE ConcatRule)*;
			//ConcatRule->Symbol+;
			//Symbol-> (IDENTIFIER | (BRACKETOPEN Subrule BRACKETCLOSE) ) UNARYOPER ?;

			Concat subrule = new Concat();
			subrule.Name = "SubRule";

			//Symbol-> (IDENTIFIER | (BRACKETOPEN Subrule BRACKETCLOSE) ) UNARYOPER ?;
			Concat symbol = new Concat();
			symbol.Name="Symbol";
			Choice idOrBracket = new Choice();
			//idOrBracket.Name = "idOrBracket";
			Concat bracket = new Concat();
			//bracket.Name = "bracket";
			bracket.Symbols.Add(BRACKETOPEN);
			bracket.Symbols.Add(subrule);
			bracket.Symbols.Add(BRACKETCLOSE);
			idOrBracket.Symbols.Add(IDENTIFIER);
			idOrBracket.Symbols.Add(bracket);
			Optional unaryOption = new Optional();
			//unaryOption.Name="unaryOption";
			unaryOption.Symbol = UNARYOPER;
			symbol.Symbols.Add(idOrBracket);
			symbol.Symbols.Add(unaryOption);

			//ConcatRule->Symbol+;
			OneOrMore concatRule = new OneOrMore();
			concatRule.Name = "ConcatRule";
			concatRule.Symbol = symbol;

			//Subrule->ConcatRule(PIPE ConcatRule)*;
			ZeroOrMore pipeConcatStar = new ZeroOrMore();
			//pipeConcatStar.Name="pipeConcatStar";
			Concat pipeConcat = new Concat();
			//pipeConcat.Name="pipeConcat";
			pipeConcat.Symbols.Add(PIPE);
			pipeConcat.Symbols.Add(concatRule);
			pipeConcatStar.Symbol = pipeConcat;

			subrule.Symbols.Add(concatRule);
			subrule.Symbols.Add(pipeConcatStar);


			//Rule->VERBATIM_STRING | STRING | Subrule;
			Choice rule = new Choice();
			rule.Symbols.Add(VERBATIM_STRING);
			rule.Symbols.Add(STRING);
			rule.Symbols.Add(subrule);
			rule.Name = "Rule";

			//Production->IDENTIFIER ARROW Rule SEMICOLON;
			Concat production = new Concat();
			production.Name = "Production";
			production.Symbols.Add(IDENTIFIER);
			production.Symbols.Add(ARROW);
			production.Symbols.Add(rule);
			production.Symbols.Add(SEMICOLON);

			Grammar = new Concat();
			Grammar.Name = "Grammar";
			ZeroOrMore produtionStar = new ZeroOrMore();
			produtionStar.Symbol = production;
			Grammar.Symbols.Add(produtionStar);
			Console.WriteLine(Grammar.ToString());
		}
		public ParseNode Parse(string input)
		{
			return Grammar.Parse(input, 0, Skips);
		}
	}
}
