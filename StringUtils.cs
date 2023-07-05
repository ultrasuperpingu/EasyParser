using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octopartite
{
	public static class StringUtils
	{

		/// <summary>
		/// Get the line number (start at 1) from the position in the string
		/// </summary>
		/// <param name="text">the string</param>
		/// <param name="pos">the position</param>
		/// <returns>line number at pos</returns>
		public static int GetLineFromPos(this string text, int pos)
		{
			return text.Take(pos).Count(c => c == '\n') + 1;
		}

		/// <summary>
		/// Get the line start position
		/// </summary>
		/// <param name="text">the string</param>
		/// <param name="line">line number</param>
		/// <returns>line start position</returns>
		public static int GetLinePos(this string text, int line)
		{
			var linePos = text.Select((value, index) => new { value, index })
				.Where(pair => pair.value == '\n')
				.Select(pair => pair.index + 1)
				.Take(line - 1)
				.DefaultIfEmpty(0) // Handle line = 1
				.Last();
			if(linePos == 0 && line > 1)
			{
				return text.Length - 1;
			}
			return linePos;
		}

		/// <summary>
		/// Get line, column and line start position at the given position
		/// </summary>
		/// <param name="text">the string</param>
		/// <param name="pos">the position</param>
		/// <returns>line, column and line start position</returns>
		public static (int, int, int) GetLineColumnLinePos(this string text, int pos)
		{
			var line = GetLineFromPos(text, pos);
			var posLine = GetLinePos(text, line);
			return (line, pos - posLine, posLine);
		}
	}
}
