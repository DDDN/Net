/*
* DDDN.Net.Samples.HomeController
* 
* Copyright(C) 2017 Lukasz Jaskiewicz
* Author: Lukasz Jaskiewicz (lukasz@jaskiewicz.de, devdone@outlook.com)
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

using DDDN.Office.Docx;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;

namespace DDDN.Net.Samples
{
	public class HomeController : Controller
	{
		private IHostingEnvironment _hostingEnvironment;

		public HomeController(IHostingEnvironment hostingEnvironment)
		{
			_hostingEnvironment = hostingEnvironment ?? throw new ArgumentNullException(nameof(hostingEnvironment));
		}

		public IActionResult Index()
		{
			var docxFilePath = Path.Combine(_hostingEnvironment.WebRootPath, "docx", "Sample1.docx");
			ReadDocxFile(docxFilePath);
			return View();

		}

		private void ReadDocxFile(string path)
		{
			using (var docxStream = System.IO.File.OpenRead(path))
			{
				var docxConvert = new DocxConvert(docxStream);
				ViewData["Article"] = docxConvert.GetHTML("article");
				ViewData["ArticleCSS"] = docxConvert.GetCSS();
			}
		}
	}
}
