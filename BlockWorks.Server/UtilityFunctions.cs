using BlockWorks.Server.MessageTypes;

using PlayerIO.GameLibrary;

using System.Collections.Generic;

namespace BlockWorks.Server
{
	public static class UtilityFunctions
	{
		public static uint ToBlocks(uint x, uint y, uint layer = 0)
			=> (layer & 3) << 30 | (x & 32767) << 15 | y & 32767;

		public static uint GetX(uint blockdata)
			=> blockdata >> 15 & 32767;

		public static uint GetY(uint blockdata)
			=> blockdata & 32767;

		public static uint GetLayer(uint blockdata)
			=> blockdata >> 30 & 3;

		public static Message GetOnlineMessage(this IEnumerable<Player> players)
		{
			var msgCreate = Message.Create("online");

			foreach (var i in players)
				if (i.Ready)
				{
					var jmsg = i.GetJoin(false).Pack(Target.Sending);
					for (uint k = 0; k < jmsg.Count; k++)
					{
						var j = jmsg[k];

						// gay workaround :c
						if (k == 3) msgCreate.Add(false);

						msgCreate.Add(j);
					}
				}

			return msgCreate;
		}
	}
}