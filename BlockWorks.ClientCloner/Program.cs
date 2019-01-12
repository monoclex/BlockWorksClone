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

		private static void Main()
			=> new Program().MainAsync().GetAwaiter().GetResult();

		public async Task MainAsync()
		{
			Console.WriteLine("");
			Console.WriteLine("Once the crawling is completed, you will NOT be able to open 'index.html' and have everything work. You MUST have an HTTP server to use it with.");
			Console.WriteLine("\tMongoose works pretty good in my case - https://cesanta.com/binary.html");
			Console.WriteLine("For you linux users, just install nginx and host it via that.");
			Console.WriteLine("");
			Console.WriteLine("I hope you have a swell remake experience!");
			Console.WriteLine("");

			await CrawlHtml(BlockWorksURL, "index.html");
			Console.WriteLine($"Done crawling!");
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

			var crawlTasks = new List<Task>();

			foreach (var i in scripts)
			{
				var scriptSrc = i.Attributes["src"].Value;

				crawlTasks.Add(CrawlJs(baseUrl, scriptSrc));
			}

			await Task.WhenAll(crawlTasks);
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
			var crawlTasks = new List<Task>();

			foreach (var i in await AwkwardParse(rawStr, "loadImage(\"", "\")", '"'))
				crawlTasks.Add(CrawlMisc(baseUrl, i));

			foreach (var i in await AwkwardParse(rawStr, "{src:[\"", "\"]}", '"'))
				crawlTasks.Add(CrawlMisc(baseUrl, i));

			foreach (var i in await AwkwardParse(rawStr, "urls:[\"", "\"]", '"'))
				crawlTasks.Add(CrawlCss(baseUrl, i));

			await Task.WhenAll(crawlTasks);
			await clean;
		}

		public async Task CrawlCss(string baseUrl, string css)
		{
			var url = $"{baseUrl}/{css}";

			Console.WriteLine($"Crawling CSS {url}");

			var data = await Get(url);
			var ms = await Copy(data);
			var clean = Clean(data, CleanType.Css, css);

			var rawStr = Encoding.UTF8.GetString(ms.ToArray());

			var crawlTasks = new List<Task>();

			foreach (var i in await AwkwardParse(rawStr, "url('", "')", '\''))
				if (!i.Contains('?'))
					crawlTasks.Add(CrawlMisc(baseUrl, $"fonts/{i}"));

			await Task.WhenAll(crawlTasks);
			await clean;
		}

		public async Task CrawlMisc(string baseUrl, string end)
		{
			var url = $"{baseUrl}/{end}";

			Console.WriteLine($"Crawling MISC {url}");

			var data = await Get(url);
			var ms = await Copy(data);

			await Save(data, end);
		}

		public async Task<MemoryStream> Copy(MemoryStream main)
		{
			var ms = new MemoryStream();
			main.Seek(0, SeekOrigin.Begin);
			await main.CopyToAsync(ms);
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
				await Save(await Post(BeautifierURL, postData), relUrl);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Unable to clean {relUrl} (maybe the service is down) \n\t{ex.Message} - {ex.StackTrace}");

				await Save(data, relUrl);
			}
		}

		public async Task Save(MemoryStream ms, string relUrl)
		{
			var file = $"BlockWorksClientClone/{relUrl}";

			var fullPath = Path.GetFullPath(file);

			var dir = Path.GetDirectoryName(fullPath);

			if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

			using (var fs = File.OpenWrite(fullPath))
				await ms.CopyToAsync(fs);
		}

		private Task<IEnumerable<string>> AwkwardParse(string input, string begin, string end, char splitBy)
		{
			var data = input.Split(begin);
			for (int i = 1; i < data.Length; i++)
				data[i] = begin + data[i].Split(end)[0] + end;

			var res = new List<string>();

			foreach (var i in data)
			{
				if (i.Contains(begin))
					res.AddRange(GetUrls(i, splitBy));
			}

			return Task.FromResult<IEnumerable<string>>(res);
		}

		private async Task<MemoryStream> Post(string url, string postdata)
		{
			var wr = HttpWebRequest.CreateHttp(url);

			wr.Method = "POST";
			wr.ContentLength = postdata.Length;
			wr.ContentType = "application/x-www-form-urlencoded";

			wr.AutomaticDecompression =
				DecompressionMethods.GZip |
				DecompressionMethods.Deflate;

			wr.Timeout = 20_000;

			using (var stream = wr.GetRequestStream())
			{
				var byteData = Encoding.ASCII.GetBytes(postdata);
				await stream.WriteAsync(byteData, 0, postdata.Length);
			}

			var ms = new MemoryStream();

			using (var r = await wr.GetResponseAsync())
			using (var s = r.GetResponseStream())
				await s.CopyToAsync(ms);

			Console.WriteLine($"[POST] Posted to {url} (resp. len: {ms.Length})");

			ms.Seek(0, SeekOrigin.Begin);
			return ms;
		}

		private async Task<MemoryStream> Get(string url)
		{
			var wr = HttpWebRequest.CreateHttp(url);

			wr.Method = "GET";

			wr.AutomaticDecompression =
				DecompressionMethods.GZip |
				DecompressionMethods.Deflate;

			var ms = new MemoryStream();

			using (var r = await wr.GetResponseAsync())
			using (var s = r.GetResponseStream())
				await s.CopyToAsync(ms);

			Console.WriteLine($"[GET] Downloaded {url}");

			return ms;
		}

		private string[] GetUrls(string s, char splitChar)
		{
			var spl = s.Split(splitChar);
			var er = new string[(spl.Length - 1) / 2];

			for (int i = 1; i < spl.Length; i += 2)
				er[(i - 1) / 2] = spl[i];

			return er;
		}
	}
}