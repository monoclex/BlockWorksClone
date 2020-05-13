using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace BlockWorks.ClientCloner
{
	[PublicAPI]
	public class CrawlTask
	{
		private readonly string _resource;
		private readonly ConfiguredTaskAwaitable<MemoryStream> _get;
		private readonly List<Func<MemoryStream, Task>> _postEffects;

		public CrawlTask(string resource)
		{
			_postEffects = new List<Func<MemoryStream, Task>>();
			_resource = resource;
			_get = HttpHelpers.Get(resource).ConfigureAwait(false);
		}

		public async Task Execute()
		{
			var crawl = await Crawl();

			var needsAwaiting = new Task[_postEffects.Count];

			for (var i = 0; i < _postEffects.Count; i++)
			{
				needsAwaiting[i] = _postEffects[i](crawl);
			}

			await Task.WhenAll(needsAwaiting).ConfigureAwait(false);
		}

		// i don't like `return this` patterns on the base class
		// in the future i might refactor to an extension class
		public CrawlTask WithPostProcess(Func<MemoryStream, Task> postEffect)
		{
			_postEffects.Add(postEffect);
			return this;
		}

		public ConfiguredTaskAwaitable<MemoryStream> Crawl() => _get;
	}
}
