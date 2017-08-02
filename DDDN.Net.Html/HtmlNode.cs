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
        public string InnerText { get; private set; } = string.Empty;
        public List<IHtmlElement> Childs { get; private set; } = new List<IHtmlElement>();
        public string Id { get; set; } = null;
        public List<string> ClassNames { get; } = new List<string>();
        public Dictionary<string, string> Styles { get; } = new Dictionary<string, string>();
        public Dictionary<string, string> Attributes { get; } = new Dictionary<string, string>();

        /// <summary>
        /// Public constructor that takes the name of the node.
        /// </summary>
        /// <param name="name">Name of the node.</param>
        public HtmlNode(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(LogMsg.StrArgNullOrWhite, nameof(name));
            }

            Name = name;
        }
        /// <summary>
        /// Public constructor that takes the name and the inner text of the node.
        /// The inner text is beeing renderd before all child nodes.
        /// </summary>
        /// <param name="name">Name of the node.</param>
        /// <param name="innerText">Innter text of the node.</param>
        public HtmlNode(string name, string innerText)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(LogMsg.StrArgNullOrWhite, nameof(name));
            }

            Name = name;

            if (!string.IsNullOrEmpty(innerText))
            {
                InnerText = innerText;
            }
        }
        /// <summary>
        /// Adds a child node.
        /// </summary>
        /// <param name="child">Child node object.</param>
        public void AddChild(IHtmlNode child)
        {
            if (child == null)
            {
                throw new ArgumentNullException(nameof(child));
            }

            Childs.Add(child);
        }
        /// <summary>
        /// Renders the HTML of this node.
        /// </summary>
        /// <param name="builder">String builder to be used for rendering.</param>
        public void RenderHtml(StringBuilder builder)
        {
            if (Childs.Any() || !String.IsNullOrEmpty(InnerText))
            {
                builder.Append($"<{Name}{RenderAtributes()}>{InnerText}");

                foreach (var child in Childs)
                {
                    child.RenderHtml(builder);
                }

                builder.Append($"</{Name}>");
            }
            else
            {
                builder.Append($"<{Name}{RenderAtributes()}/>");
            }
        }
        /// <summary>
        /// Adds an attribute to the HTML Node.
        /// If the attribute already exists, only the value will be added.
        /// </summary>
        /// <param name="attrName">Name of the attribute.</param>
        /// <param name="attrVal">Value of the attribute.</param>
        public void AddAttribute(string attrName, string attrVal)
        {
            if (Attributes.ContainsKey(attrName))
            {
                Attributes[attrName] = attrVal;
            }
            else
            {
                Attributes.Add(attrName, attrVal);
            }
        }
        /// <summary>
        /// Renders the HTML of all attributes of the node.
        /// </summary>
        /// <returns></returns>
        private string RenderAtributes()
        {
            var strBuilder = new StringBuilder(256);

            if (Id != null)
            {
                strBuilder.Append($" id=\"{Id}\"");
            }

            if (ClassNames.Any())
            {
                strBuilder.Append($" class=\"");
                bool firstName = true;

                foreach (var className in ClassNames)
                {
                    if (firstName)
                    {
                        strBuilder.Append($"{className}");
                        firstName = false;
                    }
                    else
                    {
                        strBuilder.Append($", {className}");
                    }
                }

                strBuilder.Append($"\"");
            }

            if (Styles.Any())
            {
                strBuilder.Append($" style=\"");
                bool firstStyle = true;

                foreach (var style in Styles)
                {
                    if (firstStyle)
                    {
                        strBuilder.Append($"{style.Key}:{style.Value};");
                        firstStyle = false;
                    }
                    else
                    {
                        strBuilder.Append($" {style.Key}:{style.Value};");
                    }
                }

                strBuilder.Append($"\"");
            }

            if (Attributes.Any())
            {
                foreach (var attr in Attributes)
                {
                    strBuilder.Append($" {attr.Key}=\"{attr.Value}\"");
                }
            }

            return strBuilder.ToString();
        }
        /// <summary>
        /// Appends the inner text of the node. The text will be renderd before all childs.
        /// </summary>
        /// <param name="text">The inner text.</param>
        /// <returns></returns>
        public string AppendInnerText(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                InnerText += text;
            }

            return InnerText;
        }
        /// <summary>
        /// Adds a new class name to the HTML class attribute.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        public void AddClass(string className)
        {
            if (string.IsNullOrWhiteSpace(className))
            {
                throw new ArgumentException(LogMsg.StrArgNullOrWhite, nameof(className));
            }

            ClassNames.Add(className);
        }
        /// <summary>
        /// Adds a new style property an its value to the HTML style attribute.
        /// </summary>
        /// <param name="propName">Name of the style property.</param>
        /// <param name="propValue">Value of the style property.</param>
        public void AddStyleProperty(string propName, string propValue)
        {
            if (!string.IsNullOrWhiteSpace(propName) && !string.IsNullOrWhiteSpace(propValue))
            {
                Styles.Add(propName, propValue);
            }
        }
    }
}
