using BlockWorks.Server.MessageTypes;

using PlayerIO.GameLibrary;

using System;
using System.Linq;

namespace BlockWorks.Server
{
	[RoomType("Simple-1")]
	public class BlockWorksServer : Game<Player>
	{
		private int _counter = 0;
		private World _world = new World(250, 100, WorldColor.Gray);

		public override void UserJoined(Player player)
		{
			player.Id = ++_counter;

			player.Send(_world.Serialize()); // player.Send("world", "", "Gray World", 250u, 100u, "WS", true, 0, 4u, new byte[] { 0, 0, 0, 1, 0, 2, 0, 3 }, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }, "WE", "SS", "SE");

			player.Send(player.GetJoin(true));

			if (Players.Count() > 1)
				player.Send(Players.GetOnlineMessage());
		}

		private void TellReady(string type, params object[] args)
			=> TellReady(Message.Create(type, args));

		private void TellReady(Message m)
		{
			foreach (var i in Players)
				if (i.Ready)
					i.Send(m);
		}

		public override void GotMessage(Player player, Message message)
		{
			if (!player.Ready)
			{
				switch (message.Type)
				{
					case "ready":
					{
						TellReady(player.GetJoin(false).Pack(Target.Sending));

						player.Ready = true;
					}
					break;

					default: break;
				}
			}
			else
			{
				switch (message.Type)
				{
					case Fly.Type:
					{
						var fly = new Fly();

						if (fly.Unpack(message))
						{
							fly.Id = player.Id;

							TellReady(Fly.Type, player.Id, fly.FlyState);
						}
					}
					break;

					case Block.Type:
					{
						var b = new Block();

						if (b.Unpack(message))
						{
							b.Id = player.Id;

							if (_world.TrySetBlock(b.X, b.Y, new WorldBlock {
								Id = Id.Gray,
								Placer = (uint)b.Id
							}))
							{
								TellReady(b.Pack(Target.Sending));
							}
						}
					}
					break;

					default: break;
				}
			}
		}
	}
}