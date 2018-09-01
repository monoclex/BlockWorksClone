using PlayerIO.GameLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockWorks.Server.MessageTypes {

	public class You : IMessage {

		public int PlayerId { get; set; }

		public string AccountId { get; set; }

		public string Username { get; set; }

		public uint AvatarId { get; set; }

		public int StaffRank { get; set; }

		public float X { get; set; }

		public float Y { get; set; }

		public Message ToPlayerIOMessage() {
			return Message.Create("you", PlayerId, AccountId, Username, AvatarId, StaffRank, X, Y);
		}
	}
}
