using PlayerIO.GameLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockWorks.Server.MessageTypes {
	public interface IMessage {
		Message ToPlayerIOMessage();
	}
}
