using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using EasyParser.ParserRules;

namespace EasyParser
{
    public class ParseNode
	{
		public ParseNode(Rule s, string input, int index)
		{
			Symbol = s;
			this.input = input;
			Index = index;
		}
		public ParseNode(Rule s, string input, int index, int length)
		{
			Symbol = s;
			this.input = input;
			Index = index;
			Length = length;
		}
		// protected?!?
		public ParseNode(ParseNode node, bool copyChild = true)
		{
			Symbol = node.Symbol;
			this.input = node.input;
			Index = node.Index;
			Length = node.Length;
			Skipped = node.Skipped;
			LongestMatch = node.LongestMatch;
			if (copyChild)
			{
				foreach (var c in node.Nodes)
				{
					var cc = new ParseNode(c, copyChild);
					Nodes.Add(cc);
					cc.Parent = this;
				}
			}
		}

		internal string input;
		public int Index { get; set; } = 0;
		public int Length { get; set; } = 0;
		public int End
		{
			get
			{
				return Index + Length;
			}
		}
		public bool Success { get; set; } = false;
		public Rule Symbol { get; set; }
		internal int symbolIndex = -1;
		public List<ParseNode> Skipped = null;
		public ParseNode LongestMatch = null;
		public ParseNode Parent { get; set; }
		public string Text
		{
			get
			{
				return this.input.Substring(Index, Length);
			}
		}
		public List<ParseNode> Nodes { get; set; } = new List<ParseNode>();
		public ParseNode LastNode
		{
			get
			{
				if (Nodes.Count == 0)
					return this;
				return Nodes[Nodes.Count - 1].LastNode;
			}
		}
		public ParseNode LastNamedNode
		{
			get
			{
				if (Nodes.Count == 0)
				{
					if(Symbol.IsTerminal || string.IsNullOrEmpty(Symbol.Name))
						return null;
					else
						return this;
				}
				var lastChild = Nodes.LastOrDefault(s => s.Length > 0, null);
				ParseNode n = null;
				if(lastChild != null)
					n = lastChild.LastNamedNode;
				if(n == null)
				{
					if (Symbol.IsTerminal || string.IsNullOrEmpty(Symbol.Name))
						return null;
					else
						return this;
				}
				return n;
			}
		}
		
		public ParseNode GetTokenNode(string name, int index)
		{
			if (index < 0)
				return null;
			// left to right
			foreach (ParseNode node in Nodes)
			{
				if (node.Symbol.Name == name)
				{
					index--;
					if (index < 0)
					{
						return node;
					}
				}
			}
			return null;
		}

		protected virtual bool IsTokenPresent(string name, int index)
		{
			ParseNode node = GetTokenNode(name, index);
			return node != null;
		}

		protected virtual string GetTerminalValue(string name, int index)
		{
			ParseNode node = GetTokenNode(name, index);
			if (node != null)
				return node.Text;
			return null;
		}

		/// <summary>
		/// Remove nodes which symbol's doesn't have a name
		/// </summary>
		public void Simplify()
		{
			for (int i=0;i<Nodes.Count;i++)
			{
				ParseNode node = Nodes[i];
				node.Simplify();
				if (node.Nodes.Count == 0 && node.Length == 0) //ZeroOrMore/Optional not matching anything
				{
					Nodes.RemoveAt(i);
					i--;
				}
				else if (string.IsNullOrEmpty(node.Symbol.Name))
				{
					Nodes.RemoveAt(i);
					Nodes.InsertRange(i, node.Nodes);
					i += node.Nodes.Count;
					i--;
				}
			}
		}

		internal ParseNode Backtrack(List<Terminal> skips)
		{
			return Symbol.Backtrack(this, skips);
		}

		public override string ToString()
		{
			return PrintTree();
		}

		public string PrintTree()
		{
			StringBuilder sb = new StringBuilder();
			int indent = 0;
			PrintNode(sb, this, indent);
			return sb.ToString();
		}

		private void PrintNode(StringBuilder sb, ParseNode node, int indent)
		{
			string space = "".PadLeft(indent, ' ');

			sb.Append(space);
			sb.AppendLine(node.Text);

			foreach (ParseNode n in node.Nodes)
				PrintNode(sb, n, indent + 2);
		}

		
	}

}
