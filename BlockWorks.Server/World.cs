using PlayerIO.GameLibrary;

namespace BlockWorks.Server
{
	public enum WorldColor
	{
		Gray,
		Red,
		Yellow,
		Green,
		Blue
	}

	public class World
	{
		private readonly WorldBlock[][] _blocks;

		public World(uint width, uint height, WorldColor color)
		{
			Width = width;
			Height = height;
			Color = color;

			// jagged arrays are more performance then their [,] counterparts
			_blocks = new WorldBlock[width][];

			for (var x = 0; x < width; x++)
			{
				_blocks[x] = new WorldBlock[height];
				for (var y = 0; y < height; y++)
				{
					var b = new WorldBlock();

					_blocks[x][y] = b;
				}
			}
		}

		public WorldColor Color { get; }
		public uint Width { get; }
		public uint Height { get; }

		public Message Serialize()
		{
			var msg = Message.Create("world", "", Color.ToFriendlyName(), Width, Height);

			msg.Add("WS");
			msg.Add(true, 0, 4u, new byte[] { 0, 0, 0, 1, 0, 2, 0, 3 }, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 });
			msg.Add("WE");
			msg.Add("SS");

			msg.Add("SE");

			return msg;
		}

		public bool TryGetBlock(uint x, uint y, out WorldBlock result)
		{
			if (x.InRange(0, Width) &&
				y.InRange(0, Height))
			{
				result = GetBlock(x, y);
				return true;
			}

			result = default;
			return false;
		}

		public WorldBlock GetBlock(uint x, uint y) => _blocks[x][y];

		public bool TrySetBlock(uint x, uint y, WorldBlock block)
		{
			if (x.InRange(0, Width) &&
				y.InRange(0, Height))
			{
				if (GetBlock(x, y).Id != block.Id)
					SetBlock(x, y, block);
				else return false;
				return true;
			}

			return false;
		}

		public void SetBlock(uint x, uint y, WorldBlock block) => _blocks[x][y] = block;
	}

	public enum Id
	{
		Air,
		Gray
	}

	public struct WorldBlock
	{
		public Id Id;

		public uint Placer;
	}
}