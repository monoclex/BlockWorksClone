using PlayerIOClient;

using System;
using System.Collections.Generic;

namespace BlockWorksBotConnection
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			var cli = PlayerIO.Authenticate("blockworks-frdrlhtjneoipehnx9tmg", "public", new Dictionary<string, string>
			{
				{"userId", "guest"}
			}, null);
			var con = cli.Multiplayer.CreateJoinRoom("OW_Gray-1", "Simple-1", false, null,
				new Dictionary<string, string>
				{
					{"Username", ""}
				});

			con.OnMessage += Con_OnMessage;
			con.OnDisconnect += Con_OnDisconnect;
			con.Send("ready");

			Console.ReadLine();
		}

		private static void Con_OnDisconnect(object sender, string message) => Console.WriteLine(message);

		private static void Con_OnMessage(object sender, Message e)
		{
			var con = (Connection)sender;
		}
	}
}