using HtmlAgilityPack;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BlockWorks.ClientCloner
{
	public enum CleanType
	{
		Html = 0,
		Css = 1,
		Js = 2
	}

	internal class Program
	{
		public const string BlockWorksURL = "https://blockworks.lightwolfstudios.com/game";
		public const string BeautifierURL = "[REDACTED]";

		private static Task Main() => new Program().MainAsync();

		public async Task MainAsync()
		{
			Console.WriteLine(@"
Once the crawling is completed, you will NOT be able to open 'index.html' and
have everything work. You MUST have an HTTP server to use it with.

Mongoose works pretty good in my case - https://cesanta.com/binary.html

For you linux users, just install nginx.

Enjoy!
");

			await CrawlHtml(BlockWorksURL, "index.html").ConfigureAwait(false);

			Console.WriteLine("Done crawling!");
		}

		//TODO: refactor these crawls into their own classes or something

		public async Task CrawlHtml(string baseUrl, string html)
		{
			var url = $"{baseUrl}/{html}";

			Console.WriteLine($"Crawling HTML {url}");

			var data = await Get(url);
			var ms = await Copy(data);
			var clean = Clean(data, CleanType.Html, html);

			var doc = new HtmlDocument();
			doc.Load(ms);

			var scripts = doc.DocumentNode.Descendants()
				.Where(x => x.Name == "script");

			var crawlTasks =
				scripts.Select(i => i.Attributes["src"].Value)
				.Select(scriptSrc => CrawlJs(baseUrl, scriptSrc))
				.ToList();

			await Task.WhenAll(crawlTasks).ConfigureAwait(false);

			await clean;
		}

		public async Task CrawlJs(string baseUrl, string js)
		{
			var url = $"{baseUrl}/{js}";

			Console.WriteLine($"Crawling JS {url}");

			var data = await Get(url);
			var ms = await Copy(data);
			var clean = Clean(data, CleanType.Js, js);

			var rawStr = Encoding.UTF8.GetString(ms.ToArray());

			var crawlTasks =
			(
				from i in await AwkwardParse(rawStr, "loadImage(\"", "\")", '"').ConfigureAwait(false)
				select CrawlMisc(baseUrl, i)
			).ToList();

			crawlTasks.AddRange
			(
				from i in await AwkwardParse(rawStr, "{src:[\"", "\"]}", '"').ConfigureAwait(false)
				select CrawlMisc(baseUrl, i)
			);

			crawlTasks.AddRange
			(
				from i in await AwkwardParse(rawStr, "urls:[\"", "\"]", '"').ConfigureAwait(false)
				select CrawlCss(baseUrl, i)
			);

			await Task.WhenAll(crawlTasks).ConfigureAwait(false);

			await clean;
		}

		public async Task CrawlCss(string baseUrl, string css)
		{
			var url = $"{baseUrl}/{css}";

			Console.WriteLine($"Crawling CSS {url}");

			var data = await Get(url).ConfigureAwait(false);
			var ms = await Copy(data).ConfigureAwait(false);
			var clean = Clean(data, CleanType.Css, css);

			var rawStr = Encoding.UTF8.GetString(ms.ToArray());

			var crawlTasks =
			(
				from i in await AwkwardParse(rawStr, "url('", "')", '\'')
				where !i.Contains('?')
				select CrawlMisc(baseUrl, $"fonts/{i}")
			).ToList();

			await Task.WhenAll(crawlTasks).ConfigureAwait(false);
			await clean;
		}

		public async Task CrawlMisc(string baseUrl, string end)
		{
			var url = $"{baseUrl}/{end}";

			Console.WriteLine($"Crawling MISC {url}");

			var data = await Get(url).ConfigureAwait(false);
			var ms = await Copy(data).ConfigureAwait(false);

			await Save(data, end).ConfigureAwait(false);
		}

		public async Task<MemoryStream> Copy(MemoryStream main)
		{
			var ms = new MemoryStream();
			main.Seek(0, SeekOrigin.Begin);
			await main.CopyToAsync(ms).ConfigureAwait(false);
			main.Seek(0, SeekOrigin.Begin);
			ms.Seek(0, SeekOrigin.Begin);
			return ms;
		}

		public async Task Clean(MemoryStream data, CleanType clean, string relUrl)
		{
			var postData = $"src={HttpUtility.UrlEncode(data.ToArray())}" +
						   $"&clean={HttpUtility.UrlEncode(((int)clean).ToString())}";

			try
			{
				await Save(await Post(BeautifierURL, postData), relUrl).ConfigureAwait(false);
			}
			catch (Exception ex)
			{
				Console.WriteLine(
					$"Unable to clean {relUrl} (maybe the service is down) \n\t{ex.Message} - {ex.StackTrace}");

				await Save(data, relUrl).ConfigureAwait(false);
			}
		}

		public async Task Save(MemoryStream ms, string relUrl)
		{
			var file = $"BlockWorksClientClone/{relUrl}";

			var fullPath = Path.GetFullPath(file);

			var dir = Path.GetDirectoryName(fullPath);

			if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

			using (var fs = File.OpenWrite(fullPath))
			{
				await ms.CopyToAsync(fs).ConfigureAwait(false);
			}
		}

		private Task<IEnumerable<string>> AwkwardParse(string input, string begin, string end, char splitBy)
		{
			var data = input.Split(begin);
			for (var i = 1; i < data.Length; i++)
				data[i] = begin + data[i].Split(end)[0] + end;

			var res = new List<string>();

			foreach (var i in data)
				if (i.Contains(begin))
					res.AddRange(GetUrls(i, splitBy));

			return Task.FromResult<IEnumerable<string>>(res);
		}

		private async Task<MemoryStream> Post(string url, string postdata)
		{
			var wr = WebRequest.CreateHttp(url);

			wr.Method = "POST";
			wr.ContentLength = postdata.Length;
			wr.ContentType = "application/x-www-form-urlencoded";

			wr.AutomaticDecompression =
				DecompressionMethods.GZip |
				DecompressionMethods.Deflate;

			wr.Timeout = 20_000;

			using (var stream = await wr.GetRequestStreamAsync().ConfigureAwait(false))
			{
				var byteData = Encoding.ASCII.GetBytes(postdata);
				await stream.WriteAsync(byteData, 0, postdata.Length);
			}

			var ms = new MemoryStream();

			using (var r = await wr.GetResponseAsync().ConfigureAwait(false))
			using (var s = r.GetResponseStream())
			{
				if (s != null) await s.CopyToAsync(ms).ConfigureAwait(false);
			}

			Console.WriteLine($"[POST] Posted to {url} (resp. len: {ms.Length})");

			ms.Seek(0, SeekOrigin.Begin);
			return ms;
		}

		private async Task<MemoryStream> Get(string url)
		{
			var wr = WebRequest.CreateHttp(url);

			wr.Method = "GET";

			wr.AutomaticDecompression =
				DecompressionMethods.GZip |
				DecompressionMethods.Deflate;

			var ms = new MemoryStream();

			using (var r = await wr.GetResponseAsync())
			using (var s = r.GetResponseStream())
			{
				if (s != null) await s.CopyToAsync(ms).ConfigureAwait(false);
			}

			Console.WriteLine($"[GET] Downloaded {url}");

			return ms;
		}

		private string[] GetUrls(string s, char splitChar)
		{
			var spl = s.Split(splitChar);
			var er = new string[(spl.Length - 1) / 2];

			for (var i = 1; i < spl.Length; i += 2)
				er[(i - 1) / 2] = spl[i];

			return er;
		}
	}
}