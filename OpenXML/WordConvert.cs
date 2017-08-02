/*
* DDDN.Net.OpenXML.WordConvert
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

using DDDN.Net.Html;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace DDDN.Net.OpenXML
{
    public class WordConvert : IWordConvert, IDisposable
    {
        private WordprocessingDocument Doc { get; set; }
        private Dictionary<string, WStyleInfo> WStyleInfos { get; set; } = new Dictionary<string, WStyleInfo>();
        private List<WParagraphInfo> WParagraphInfos { get; set; } = new List<WParagraphInfo>();

        public WordConvert(FileStream docStream)
        {
            if (docStream == null)
            {
                throw new ArgumentNullException(nameof(docStream));
            }

            Doc = WordprocessingDocument.Open(docStream, false);
            GetWDocInfo();
        }
        /// <summary>
        /// Renders the whole document body in HTML.
        /// </summary>
        /// <returns></returns>
        public string GetHTML(string rootHtmlTagName)
        {
            IHtmlNode rootHtmlTag = new HtmlNode(rootHtmlTagName);

            foreach (var pInfo in WParagraphInfos)
            {
                IHtmlNode pNode = new HtmlNode(HtmlTag.P);
                pNode.AddClass(pInfo.Id);
                pNode.AddStyleProperty(CssProperty.WhiteSpace, "nowrap");
                rootHtmlTag.AddChild(pNode);

                foreach (var rInfo in pInfo.Runs)
                {
                    var runNode = new HtmlNode(HtmlTag.Span, rInfo.Text);
                    runNode.AddStyleProperty(CssProperty.FontColor, $"#{rInfo.FontColor}");
                    runNode.AddStyleProperty(CssProperty.FontSize, $"{rInfo.FontSize}px");
                    pNode.AddChild(runNode);
                }
            }

            var htmlB = new StringBuilder(2048);
            rootHtmlTag.RenderHtml(htmlB);
            var html = htmlB.ToString();
            return html;
        }

        public string GetCSS()
        {
            var styles = Doc.MainDocumentPart.StyleDefinitionsPart?.Styles;

            if (styles == null)
            {
                return string.Empty;
            }

            var cssDef = new Dictionary<string, Dictionary<string, string>>();

            foreach (var style in styles.OfType<Style>())
            {
                var styleInfo = new WStyleInfo()
                {
                    StyleId = style.StyleId,
                    StyleType = style.Type,
                    BasedOnStyle = style.BasedOn
                };

                if (style.StyleRunProperties != null)
                {
                    styleInfo.RunProperty.FontColor = style.StyleRunProperties.OfType<Color>()?.First().Val;
                    styleInfo.RunProperty.FontSize = style.StyleRunProperties.OfType<FontSize>()?.First().Val;
                }

                WStyleInfos.Add(styleInfo.StyleId, styleInfo);
            }

            var cssB = new StringBuilder(2048);
            return cssB.ToString();
        }

        private void GetWDocInfo()
        {
            foreach (var para in Doc.MainDocumentPart.Document.Body.Elements<Paragraph>())
            {
                var pInfo = new WParagraphInfo()
                {
                    Id = para.ParagraphProperties.ParagraphStyleId.Val
                };

                foreach (var run in para.OfType<Run>())
                {
                    var runInfo = new WRunInfo()
                    {
                        Text = run.InnerText,
                        FontSize = run.RunProperties.FontSize?.Val,
                        FontColor = run.RunProperties.Color?.Val
                    };

                    pInfo.Runs.Add(runInfo);
                }

                WParagraphInfos.Add(pInfo);
            }
        }

        public void Dispose()
        {
            Doc.Dispose();
        }
    }
}
