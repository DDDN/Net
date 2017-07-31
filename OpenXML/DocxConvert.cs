/*
* DDDN.Net.OpenXML.DocxConvert
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
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DDDN.Net.OpenXML
{
    public class DocxConvert : IDocxConvert
    {
        private FileStream DocStream;

        public DocxConvert(FileStream docStream)
        {
            DocStream = docStream;
        }

        public string ToHTMLWithStyleSheets()
        {
            IHtmlNode htmlArticle = new HtmlNode("article");

            using (WordprocessingDocument openXMLDoc = WordprocessingDocument.Open(DocStream, false))
            {
                WalkOverElements(openXMLDoc.MainDocumentPart.Document.Body.Elements(), ref htmlArticle);
            }

            var htmlBuilder = new StringBuilder(512);
            htmlArticle.Render(ref htmlBuilder);

            return htmlBuilder.ToString();
        }

        private void WalkOverElements(IEnumerable<OpenXmlElement> elements, ref IHtmlNode htmlNode)
        {
            foreach (var ele in elements)
            {
                if (ele.GetType().Equals(typeof(Paragraph)))
                {
                    var innerText = ele.InnerText;
                    IHtmlNode htmlP = null;

                    if (string.IsNullOrEmpty(innerText))
                    {
                        htmlP = new HtmlNode("p");

                    }
                    else
                    {
                        htmlP = new HtmlNode("p", innerText);
                    }

                    htmlNode.AddChild(ref htmlP);
                    WalkOverElements(ele.ChildElements, ref htmlP);
                }
            }
        }
    }
}
