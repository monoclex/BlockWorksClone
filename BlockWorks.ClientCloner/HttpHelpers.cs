using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace BlockWorks.ClientCloner
{
	[PublicAPI]
	public static class HttpHelpers
	{
		public static async Task<MemoryStream> Get(string url)
		{
			var wr = WebRequest.CreateHttp(url);

			wr.Method = "GET";
			wr.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

			var ms = new MemoryStream();

			using (var r = await wr.GetResponseAsync().ConfigureAwait(false))
			using (var s = r.GetResponseStream())
			{
				if (s != null)
				{
					await s.CopyToAsync(ms).ConfigureAwait(false);
				}
			}

			Console.WriteLine($"[GET] Downloaded {url}");

			return ms;
		}

		public static async Task<MemoryStream> Post(string url, string postdata)
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

	}
}
