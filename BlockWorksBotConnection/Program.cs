using System;

namespace BlockWorksBotConnection
{
	class Program
	{
		static void Main(string[] args)
		{
			var cli = PlayerIOClient.PlayerIO.Authenticate("blockworks-frdrlhtjneoipehnx9tmg", "public", new System.Collections.Generic.Dictionary<string, string> {
				{ "userId", "guest" },
			}, null);
			var con = cli.Multiplayer.CreateJoinRoom("OW_Gray-1", "Simple-1", false, null, new System.Collections.Generic.Dictionary<string, string> {
				{ "Username", "" }
			});

			con.OnMessage += Con_OnMessage;
			con.OnDisconnect += Con_OnDisconnect;
			con.Send("ready");

			Console.ReadLine();
		}

		private static void Con_OnDisconnect(object sender, string message) => Console.WriteLine(message);

		private static void Con_OnMessage(object sender, PlayerIOClient.Message e)
		{
			var con = (PlayerIOClient.Connection)sender;

		}
	}
}
