/*
* DDDN.Net.Html.HtmlNode
* 
* Copyright(C) 2017 Lukasz Jaskiewicz
* Author: Lukasz Jaskiewicz (lukasz@jaskiewicz.de)
*
* This program is free software; you can redistribute it and/or modify it under the terms of the
* GNU General Public License as published by the Free Software Foundation; version 2 of the License.
*
* This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied
* warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the GNU General Public License for more details.
*
* You should have received a copy of the GNU General Public License along with this program; if not, write
* to the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/

using DDDN.Logging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DDDN.Net.Html
{
	public class HtmlNode : IHtmlNode
	{
		public string Name { get; }
		public string InnerText { get; private set; } = null;
		public List<IHtmlElement> Childs { get; private set; } = new List<IHtmlElement>();
		public List<string> ClassNames { get; private set; } = new List<string>();

		private HtmlNode() { }

		public HtmlNode(string name)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentException(LogMsg.StrArgNullOrWhite, nameof(name));
			}

			Name = name;
		}

		public HtmlNode(string name, string innerText)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentException(LogMsg.StrArgNullOrWhite, nameof(name));
			}

			if (string.IsNullOrEmpty(innerText))
			{
				throw new ArgumentException(LogMsg.StrArgNullOrEmpty, nameof(innerText));
			}

			Name = name;
			InnerText = innerText;
		}

		public void AddChild(IHtmlNode child)
		{
			if (child == null)
			{
				throw new ArgumentNullException(nameof(child));
			}

			Childs.Add(child);
		}

		public void Render(StringBuilder builder)
		{
			if (Childs.Any() || !String.IsNullOrEmpty(InnerText))
			{
				builder.Append($"<{Name}{GetClassAttribute()}>{InnerText}");

				foreach (var child in Childs)
				{
					child.Render(builder);
				}

				builder.Append($"</{Name}>");
			}
			else
			{
				builder.Append($"<{Name}{GetClassAttribute()}/>");
			}
		}

		public void AddClassName(string className)
		{
			ClassNames.Add(className);
		}

		private string GetClassAttribute()
		{
			if (!ClassNames.Any())
			{
				return "";
			}
			var strBuilder = new StringBuilder(" class=\"", 128);

			bool commata = false;

			foreach (var className in ClassNames)
			{
				if (commata)
				{
					strBuilder.Append($", {className}");

				}
				else
				{
					strBuilder.Append(className);
					commata = true;
				}
			}

			strBuilder.Append("\" ");

			return strBuilder.ToString();
		}
	}
}
