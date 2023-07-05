using Octopartite.ParserRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octopartite.Examples
{
	/// <summary>
	/// Same idea as Example4 but for choices backtracks.
	/// </summary>
	public class Example5 : AbstractExample
	{
		public Example5()
		{
			Rule.DefaultBacktrackCardinalityOps = false;
			Rule.DefaultBacktrackChoices = true;

			// Start Generated Code
			Terminal ID = new RegexTerminal(@"\G[a-z_][a-z_0-9]*");
			Terminal ID_SUFFIX = new RegexTerminal(@"\G[a-z_][a-z_0-9]*_SUFFIX");
			Terminal _EOF_ = new RegexTerminal(@"\G$");
			Start = new Concat();
			Terminal WHITESPACE = new RegexTerminal(@"\G\s+");
			ID.Name="ID";

			ID_SUFFIX.Name="ID_SUFFIX";

			_EOF_.Name="_EOF_";

			Start.Name="Start";
			Concat concat1 = new Concat();
			Choice choice2 = new Choice();
			choice2.Symbols.Add(ID);
			choice2.Symbols.Add(ID_SUFFIX);
			concat1.Symbols.Add(choice2);
			concat1.Symbols.Add(_EOF_);
			Start.Symbols.Add(concat1);

			WHITESPACE.Name="WHITESPACE";
			// End Generated Code

			Skips.Add(WHITESPACE);
		}

		public static void GenerateParserCode()
		{
			var grammarStr = @"
				ID -> R""[a-z_][a-z_0-9]*"";
				ID_SUFFIX -> R""[a-z_][a-z_0-9]*_SUFFIX"";
				_EOF_ -> R""$"";
				Start -> (ID|ID_SUFFIX) _EOF_;
				WHITESPACE -> R""\G\s+"";
			";
			Console.WriteLine("// Example5 parser code: copy/paste it in Example5.cs (in constructor)");
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
