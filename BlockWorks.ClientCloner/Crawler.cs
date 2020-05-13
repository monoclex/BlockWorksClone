using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace BlockWorks.ClientCloner
{
	[PublicAPI]
	public static class Crawler
	{
		public static CrawlTask CrawlHtml(string url, string fileName, IFileSaveService saveService)
		{
			return new CrawlTask(url)
				.WithPostProcess(async (ms) => await saveService.Save(ms, fileName).ConfigureAwait(false))
				.WithPostProcess(async (ms) =>
				{

				});
		}
	}
}
