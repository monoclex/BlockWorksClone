using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using JetBrains.Annotations;

namespace BlockWorks.ClientCloner.FileSavingServices
{
	public class BeautifierService : IFileSaveService
	{
		private readonly IFileSaveService _save;
		public string BeautifierUrl { get; }
		public CleanType CleanType { get; }

		public BeautifierService(string beautifierUrl, IFileSaveService save, CleanType cleanType)
		{
			_save = save;
			BeautifierUrl = beautifierUrl;
			CleanType = cleanType;
		}

		public async Task Save(Stream stream, string file)
		{
			var ms = stream.ToMemoryStream();

			var beautified = await Beautify(ms).ConfigureAwait(false);

			await _save.Save(beautified, file).ConfigureAwait(false);
		}

		public async Task<MemoryStream> Beautify(MemoryStream stream)
		{
			var postData = GenPostData(stream, CleanType);

			try
			{
				return await HttpHelpers.Post(BeautifierUrl, postData).ConfigureAwait(false);
			}
			catch (Exception e)
			{
				Console.WriteLine($"Unable to use the beautify service - {e}");
				throw;
			}
		}

		private static string GenPostData(MemoryStream stream, CleanType cleanType)
			=> $"src={HttpUtility.UrlEncode(stream.ToArray())}"
			+ $"&clean={HttpUtility.UrlEncode(((int)cleanType).ToString())}";
	}
}
