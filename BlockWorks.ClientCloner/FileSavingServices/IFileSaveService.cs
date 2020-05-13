using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BlockWorks.ClientCloner
{
	public interface IFileSaveService
	{
		Task Save(Stream stream, string file);
	}
}
