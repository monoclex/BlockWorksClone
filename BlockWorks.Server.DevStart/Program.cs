using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockWorks.Server.DevStart {
	class Program {
		static void Main(string[] args) {

			// change
			// a.multiplayer.useSecureConnections = !0;
			// in BlockWorksClient.js to
			// a.multiplayer.useSecureConnections = 0;

			Console.WriteLine($"Please be sure you set 'a.multiplayer.useSecureConnections = !0;' in BlockWorksClient.js to ' a.multiplayer.useSecureConnections = 0;'. Attempting to use HTTPs will make you use PlayerIO servers instead of development servers.");

			PlayerIO.DevelopmentServer.Server.StartWithDebugging();
		}
	}
}
