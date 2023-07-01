using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyParser
{
	public class ParserGenerator
	{
		int number = 1;
		private bool IsProductionTerminal(ParseNode production, out string pattern)
		{
			ParseNode ruleContent = null;
			//if()
			ruleContent = production.Nodes[2].Nodes[0];
			if(ruleContent != null && (ruleContent.Symbol.Name == "VERBATIM_STRING" || ruleContent.Symbol.Name == "STRING"))
			{
				pattern = ruleContent.Text;
				return true;
			}
			pattern = null;
			return false;
		}
		// Grammar -> Production*;
		public string Generate(ParseNode root)
		{
			number = 1;
			StringBuilder code = new StringBuilder();
			StringBuilder decl = new StringBuilder();
			//if (root.Symbol.Name == "Grammar")
			{
				foreach (var production in root.Nodes)
				{
					if (production.Symbol.Name == "Production")
					{
						string pattern;
						if (IsProductionTerminal(production, out pattern))
						{
							decl.AppendLine("Terminal " + production.Nodes[0].Text + "=new RegexTerminal("+pattern+");");
						}
						else
						{
							decl.AppendLine("Concat " + production.Nodes[0].Text + " = new Concat();");
						}
						code.Append(GenerateProduction(production));
					}
				}
			}
			return decl.Append(code).ToString();
		}


		// Production->IDENTIFIER ARROW Rule SEMICOLON;
		private StringBuilder GenerateProduction(ParseNode production)
		{
			//string code = production.Children[0].Text + " = new Concat();\n";
			StringBuilder code = new StringBuilder();
			code.AppendLine(production.Nodes[0].Text + ".Name=\""+production.Nodes[0].Text+"\";");
			if (production.Nodes[2].Symbol.Name == "Rule")
			{
				code.Append(GenerateRule(production.Nodes[0].Text, production.Nodes[2]));
			}
			else
			{
				throw new Exception("Internal error");
			}
			code.AppendLine();
			return code;
		}

		// Rule->VERBATIM_STRING | STRING | Subrule;
		private StringBuilder GenerateRule(string productionName, ParseNode rule)
		{
			StringBuilder code = new StringBuilder();
			if (rule.Nodes[0].Symbol.Name == "VERBATIM_STRING" || rule.Nodes[0].Symbol.Name == "STRING")
			{
				//throw new Exception("Internal error");
				//code+=productionName+".Symbols.Add("+rule.Children[0].Text+");\n";
			}
			else
			{
				code.Append(GenerateSubRule(productionName, rule.Nodes[0]));
			}
			return code;
		}

		// Subrule->ConcatRule(PIPE ConcatRule)*;
		private StringBuilder GenerateSubRule(string productionName, ParseNode parseNode, bool parentIsUnary=false)
		{
			StringBuilder code = new StringBuilder();

			if (parseNode.Nodes.Count > 1)
			{
				string subruleName = "choice"+number;
				number++;
				code.AppendLine("Choice " + subruleName + " = new Choice();");
				for (int i = 0; i < parseNode.Nodes.Count; i+=2)
				{
					code.Append(GenerateConcat(subruleName, parseNode.Nodes[i]));
				}
				if (parentIsUnary)
				{
					code.AppendLine(productionName + ".Symbol = " + subruleName + ";");
				}
				else
				{
					code.AppendLine(productionName + ".Symbols.Add(" + subruleName + ");");
				}
			}
			else if (parseNode.Nodes.Count == 1)
			{
				code.Append(GenerateConcat(productionName, parseNode.Nodes[0], parentIsUnary));
			}
			else
			{
				throw new Exception("Internal error");
			}
			return code;
		}
		
		// ConcatRule->Symbol+;
		private StringBuilder GenerateConcat(string parentName, ParseNode parseNode, bool parentIsUnary = false)
		{
			StringBuilder code = new StringBuilder();
			if (parseNode.Nodes.Count > 1)
			{
				string subruleName = "concat"+number;
				number++;
				code.AppendLine("Concat "+subruleName+" = new Concat();");
				foreach (var c in parseNode.Nodes)
				{
					code.Append(GenerateSymbol(subruleName, c));
				}
				if (parentIsUnary)
				{
					code.AppendLine(parentName + ".Symbol = " + subruleName + ";");
				}
				else
				{
					code.AppendLine(parentName + ".Symbols.Add(" + subruleName + ");");
				}
			}
			else if (parseNode.Nodes.Count == 1)
			{
				code.Append(GenerateSymbol(parentName, parseNode.Nodes[0], parentIsUnary));
			}
			else
			{
				throw new Exception("Internal error");
			}
			return code;
		}

		// Symbol-> (IDENTIFIER | (BRACKETOPEN Subrule BRACKETCLOSE) ) UNARYOPER?;
		private StringBuilder GenerateSymbol(string parentName, ParseNode parseNode, bool parentIsUnary = false)
		{
			StringBuilder code=new StringBuilder();
			if (parseNode.Nodes[0].Symbol.Name == "IDENTIFIER")
			{
				if (parseNode.Nodes.Count > 1) //unary op
				{
					string unary = parseNode.Nodes[1].Text.Trim();
					string unaryName = "unary"+number++;
					switch (unary)
					{
						case "+":
							code.AppendLine("OneOrMore " +unaryName+ " = new OneOrMore();");
							break;
						case "*":
							code.AppendLine("ZeroOrMore "+ unaryName+ " = new ZeroOrMore();");
							break;
						case "?":
							code.AppendLine("Optional " + unaryName + " = new Optional();");
							break;
						default:
							throw new Exception("Internal error");
							//code+=parentName+".Symbols.Add("+parseNode.Children[0].Text+");\n";
							//break;
					}
					code.AppendLine(unaryName + ".Symbol=" + parseNode.Nodes[0].Text + ";");
					if (parentIsUnary)
					{
						code.AppendLine(parentName + ".Symbol = " + unaryName + ";");
					}
					else
					{
						code.AppendLine(parentName + ".Symbols.Add(" + unaryName + ");");
					}
				}
				else
				{
					if (parentIsUnary)
					{
						code.AppendLine(parentName + ".Symbol = " + parseNode.Nodes[0].Text + ";");
					}
					else
					{
						code.AppendLine(parentName + ".Symbols.Add(" + parseNode.Nodes[0].Text + ");");
					}
				}
			}
			else if (parseNode.Nodes[0].Symbol.Name == "BRACKETOPEN")
			{
				//TODO: refactor unary
				if (parseNode.Nodes.Count >= 4) //unary op
				{
					string unary = parseNode.Nodes[3].Text;
					string unaryName = "unary"+number++;
					switch (unary)
					{
						case "+":
							code.AppendLine("OneOrMore " + unaryName + " = new OneOrMore();");
							
							break;
						case "*":
							code.AppendLine("ZeroOrMore "+ unaryName+ " = new ZeroOrMore();");
							break;
						case "?":
							code.AppendLine("Optional " + unaryName + " = new Optional();");
							break;
						default:
							throw new Exception("Internal error");
							//code+=GenerateSubRule(parentName, parseNode.Children[1]);
							//break;
					}
					code.Append(GenerateSubRule(unaryName, parseNode.Nodes[1], true));
					code.AppendLine(parentName + ".Symbols.Add(" + unaryName + ");");
				}
				else
				{
					code.Append(GenerateSubRule(parentName, parseNode.Nodes[1], parentIsUnary));
				}
			}
			else
			{
				throw new Exception("Internal Error");
			}
			//code+=parentName+".Symbols.Add("+subruleName+");\n";
			return code;
		}
	}
}
