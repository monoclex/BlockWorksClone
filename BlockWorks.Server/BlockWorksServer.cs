using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlockWorks.Server.MessageTypes;
using PlayerIO.GameLibrary;
using PlayerIO.GameLibrary.Interfaces;

namespace BlockWorks.Server
{
	public class BlockWorksPlayer : BasePlayer {
		public int Id { get; set; }

		public string Username { get {
				if (this.JoinData.TryGetValue("Username", out var uname) && uname.Length > 0)
					return uname;
				return "guest";
			}
		}

		public void Send(IMessage msg)
			=> this.Send(msg.ToPlayerIOMessage());
	}

	[RoomType("Simple-1")]
	public class BlockWorksServer : Game<BlockWorksPlayer> {
		private int _counter = 0;

		public override void UserJoined(BlockWorksPlayer player) {
			player.Id = ++this._counter;

			player.Send("world", "", "Gray World", 250u, 100u, "WS", true, 0, 4u, new byte[] { 0, 0, 0, 1, 0, 2, 0, 3 }, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }, "WE", "SS", "SE");

			player.Send(new You {
				PlayerId = player.Id,
				AccountId = player.Username,
				Username = player.Username.ToUpper() + "-1",
				AvatarId = 1,
				StaffRank = -1,
				X = 1 * 24f,
				Y = 98f * 24f
			});
		}

		public override void GotMessage(BlockWorksPlayer player, Message message) {
			player.Send("b", 1, UtilityFunctions.ToBlocks(10, 97, 0), 4u);

			

			Console.WriteLine(message.ToString());
		}
	}

	public static class UtilityFunctions {
		public static uint ToBlocks(uint x, uint y, uint c = 0)
			=> (c & 3) << 30 | (x & 32767) << 15 | y & 32767;
	}
}
