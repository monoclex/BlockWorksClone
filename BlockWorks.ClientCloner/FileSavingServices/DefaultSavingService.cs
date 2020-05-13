using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BlockWorks.ClientCloner.FileSavingServices
{
	public class DefaultSavingService : IFileSaveService
	{
		public async Task Save(Stream stream, string file)
		{
			using (var fs = File.Open(file, FileMode.OpenOrCreate))
			{
				var ms = stream.ToMemoryStream();

				await ms.CopyToAsync(fs);
			}
		}
	}
}
