using System;
using System.Threading;
using MiNET;
using MiNET.Blocks;
using MiNET.Plugins.Attributes;
using MiNET.Utils;
using MiNET.Worlds;

namespace WorldEdit4MiNET
{
	public partial class We4MiNet
	{
		//Position Setters
		/*[PacketHandler, Receive]
		public void UseItem(McpeUseItem data, Player source)
		{
			
		}*/

		[Command(Command = "we")]
		public void AboutCommand(Player source)
		{
			PluginGlobals.SendMessage(source, "WorldEdit 4 MiNET - Version 1.0\nDeveloped by Kennyvv");
			PluginGlobals.SendMessage(source, "Commands: /paste, /copy, /pos1, /pos2, /sphere, /hsphere, /set");
		}

		[Command(Command = "paste")]
		public void PasteCommand(Player source)
		{
			PlayerData pData;
			var hasData = PluginGlobals.PlayerDataDictionary.TryGetValue(source.Username, out pData);

			if (!hasData || pData.CopiedBlocks.Count == 0) //No data exists or No blocks have been copied?
			{
				PluginGlobals.SendMessage(source, "Please copy blocks first!");
				return;
			}

			var pLoc = source.KnownPosition;

			var count = 0;
			foreach (var i in pData.CopiedBlocks)
			{
				i.Coordinates = new BlockCoordinates(i.Coordinates.X + (int) pLoc.X, i.Coordinates.Y + (int) pLoc.Y,
					i.Coordinates.Z + (int) pLoc.Z);
				source.Level.SetBlock(i);
				count++;
			}
			PluginGlobals.SendMessage(source, "Blocks placed: " + count);
		}

		[Command(Command = "copy")]
		public void CopyCommand(Player source)
		{
			PlayerData pData;
			var hasData = PluginGlobals.PlayerDataDictionary.TryGetValue(source.Username, out pData);

			if (pData == null) pData = new PlayerData(source.Username);

			//if (!hasData)
			//{
			//PluginGlobals.PlayerDataDictionary.Add(source.Username, new PlayerData(source.Username));
			//}

			new Thread(() =>
			{
				if (PluginGlobals.Locations.ContainsKey(source.Username))
				{
					Tuple<Vector3, Vector3> coords;
					var d = PluginGlobals.Locations.TryGetValue(source.Username, out coords);
					if (d)
					{
						var startX = Math.Min(coords.Item1.X, coords.Item2.X);
						var endX = Math.Max(coords.Item1.X, coords.Item2.X);

						var startY = Math.Min(coords.Item1.Y, coords.Item2.Y);
						var endY = Math.Max(coords.Item1.Y, coords.Item2.Y);

						var startZ = Math.Min(coords.Item1.Z, coords.Item2.Z);
						var endZ = Math.Max(coords.Item1.Z, coords.Item2.Z);

						pData.CopiedBlocks.Clear(); //Clear OLD buffer.

						var changed = 0;
						for (var x = startX; x <= endX; x++)
						{
							for (var y = startY; y <= endY; y++)
							{
								for (var z = startZ; z <= endZ; z++)
								{
									var currentBlock = source.Level.GetBlock((int) x, (int) y, (int) z);
									currentBlock.Coordinates = new BlockCoordinates((int) (x - startX), (int) (y - startY), (int) (z - startZ));
									pData.CopiedBlocks.Add(currentBlock);
									changed++;
								}
							}
						}

						if (hasData)
						{
							PluginGlobals.PlayerDataDictionary.Remove(source.Username);
						}

						PluginGlobals.PlayerDataDictionary.Add(source.Username, pData);

						PluginGlobals.SendMessage(source, "Blocks copied: " + changed);
					}
				}
				else
				{
					PluginGlobals.SendMessage(source, "Please make a selection first!");
				}
			}).Start();
		}

		[Command(Command = "replace", Description = "Replaces the specified block type with another.")]
		public void ReplaceCommand(Player source, int blockId, int newBlockId)
		{
			new Thread(() =>
			{
				if (PluginGlobals.Locations.ContainsKey(source.Username))
				{
					Tuple<Vector3, Vector3> coords;
					var d = PluginGlobals.Locations.TryGetValue(source.Username, out coords);
					if (d)
					{
						var startX = Math.Min(coords.Item1.X, coords.Item2.X);
						var endX = Math.Max(coords.Item1.X, coords.Item2.X);

						var startY = Math.Min(coords.Item1.Y, coords.Item2.Y);
						var endY = Math.Max(coords.Item1.Y, coords.Item2.Y);

						var startZ = Math.Min(coords.Item1.Z, coords.Item2.Z);
						var endZ = Math.Max(coords.Item1.Z, coords.Item2.Z);

						var changed = 0;
						for (var x = startX; x <= endX; x++)
						{
							for (var y = startY; y <= endY; y++)
							{
								for (var z = startZ; z <= endZ; z++)
								{
									var currentBlock = source.Level.GetBlock((int) x, (int) y, (int) z);
									if (currentBlock.Id == blockId)
									{
										var block = BlockFactory.GetBlockById((byte) newBlockId);
										block.Coordinates = new BlockCoordinates((int) x, (int) y, (int) z);
										source.Level.SetBlock(block);
										changed++;
									}
								}
							}
						}

						PluginGlobals.SendMessage(source, "Blocks changed: " + changed);
					}
				}
				else
				{
					PluginGlobals.SendMessage(source, "Please make a selection first!");
				}
			}).Start();
		}

		[Command(Command = "set", Description = "Set's block's in the current selection to the specified block")]
		public void SetCommand(Player source, int blockId)
		{
			new Thread(() =>
			{
				if (PluginGlobals.Locations.ContainsKey(source.Username))
				{
					Tuple<Vector3, Vector3> coords;
					var d = PluginGlobals.Locations.TryGetValue(source.Username, out coords);
					if (d)
					{
						var startX = Math.Min(coords.Item1.X, coords.Item2.X);
						var endX = Math.Max(coords.Item1.X, coords.Item2.X);

						var startY = Math.Min(coords.Item1.Y, coords.Item2.Y);
						var endY = Math.Max(coords.Item1.Y, coords.Item2.Y);

						var startZ = Math.Min(coords.Item1.Z, coords.Item2.Z);
						var endZ = Math.Max(coords.Item1.Z, coords.Item2.Z);

						var changed = 0;
						for (var x = startX; x <= endX; x++)
						{
							for (var y = startY; y <= endY; y++)
							{
								for (var z = startZ; z <= endZ; z++)
								{
									var block = BlockFactory.GetBlockById((byte) blockId);
									block.Coordinates = new BlockCoordinates((int) x, (int) y, (int) z);
									source.Level.SetBlock(block);
									changed++;
								}
							}
						}

						PluginGlobals.SendMessage(source, "Blocks changed: " + changed);
					}
				}
				else
				{
					PluginGlobals.SendMessage(source, "Please set your positions first!");
				}
			}).Start();
		}

		[Command(Command = "pos1")]
		public void SetPosition1(Player source)
		{
			if (PluginGlobals.Locations.ContainsKey(source.Username))
			{
				Tuple<Vector3, Vector3> coords;
				var d = PluginGlobals.Locations.TryGetValue(source.Username, out coords);
				if (d)
				{
					var coords2 = coords.Item2;
					var coords1 = source.KnownPosition.ToVector3();
					coords1.Floor();

					coords = new Tuple<Vector3, Vector3>(coords1, coords2);
					PluginGlobals.Locations.Remove(source.Username);
					PluginGlobals.Locations.Add(source.Username, coords);
				}
				else
				{
					PluginGlobals.SendMessage(source, "Something went wrong!");
					return;
				}
			}
			else
			{
				PluginGlobals.Locations.Add(source.Username,
					new Tuple<Vector3, Vector3>(source.KnownPosition.ToVector3(), new Vector3(0, 0, 0)));
			}

			Tuple<Vector3, Vector3> coords5;
			var d2 = PluginGlobals.Locations.TryGetValue(source.Username, out coords5);

			if (d2) PluginGlobals.SendMessage(source, "Position 1 Set!");
		}

		[Command(Command = "pos2")]
		public void SetPosition2(Player source)
		{
			if (PluginGlobals.Locations.ContainsKey(source.Username))
			{
				Tuple<Vector3, Vector3> coords;
				var d = PluginGlobals.Locations.TryGetValue(source.Username, out coords);
				if (d)
				{
					var coords2 = coords.Item1;
					var coords1 = source.KnownPosition.ToVector3();
					coords1.Floor();

					coords = new Tuple<Vector3, Vector3>(coords2, coords1);
					PluginGlobals.Locations.Remove(source.Username);
					PluginGlobals.Locations.Add(source.Username, coords);
				}
				else
				{
					PluginGlobals.SendMessage(source, "Something went wrong!");
					return;
				}
			}
			else
			{
				PluginGlobals.Locations.Add(source.Username,
					new Tuple<Vector3, Vector3>(new Vector3(0, 0, 0), source.KnownPosition.ToVector3()));
			}
			Tuple<Vector3, Vector3> coords5;
			var d2 = PluginGlobals.Locations.TryGetValue(source.Username, out coords5);

			if (d2) PluginGlobals.SendMessage(source, "Position 2 Set!");
		}

		[Command(Command = "hsphere")]
		public void HSphereCommand(Player player, int blockid, int radius)
		{
			Sphere(player.Level, BlockFactory.GetBlockById((byte) blockid), player.KnownPosition.ToVector3(), radius, radius,
				radius, false);
			PluginGlobals.SendMessage(player, "Hollow Sphere Created!");
		}

		[Command(Command = "sphere")]
		public void SphereCommand(Player player, int blockid, int radius)
		{
			Sphere(player.Level, BlockFactory.GetBlockById((byte) blockid), player.KnownPosition.ToVector3(), radius, radius,
				radius, true);
			PluginGlobals.SendMessage(player, "Sphere Created!");
		}

		private void Sphere(Level level, Block block, Vector3 position, double radiusX, double radiusY, double radiusZ,
			bool filled)
		{
			new Thread(() =>
			{
				radiusX += 0.5;
				radiusY += 0.5;
				radiusZ += 0.5;

				var invRadiusX = 1/radiusX;
				var invRadiusY = 1/radiusY;
				var invRadiusZ = 1/radiusZ;

				var ceilRadiusX = (int) Math.Ceiling(radiusX);
				var ceilRadiusY = (int) Math.Ceiling(radiusY);
				var ceilRadiusZ = (int) Math.Ceiling(radiusZ);

				var nextXn = 0.0;
				var breakX = false;

				for (var x = 0; x <= ceilRadiusX && breakX == false; x++)
				{
					var xn = nextXn;
					nextXn = (x + 1)*invRadiusX;
					var nextYn = 0.0;
					var breakY = false;
					for (var y = 0; y <= ceilRadiusY && breakY == false; y++)
					{
						var yn = nextYn;
						nextYn = (y + 1)*invRadiusY;
						var nextZn = 0.0;

						for (var z = 0; z <= ceilRadiusZ; z++)
						{
							var zn = nextZn;
							nextZn = (z + 1)*invRadiusZ;
							var distanceSq = PluginGlobals.lengthSq(xn, yn, zn);
							if (distanceSq > 1)
							{
								if (z == 0)
								{
									if (y == 0)
									{
										breakX = true;
										breakY = true;
										break;
									}
									breakY = true;
								}
								break;
							}

							if (filled == false)
							{
								if (PluginGlobals.lengthSq(nextXn, yn, zn) <= 1 && PluginGlobals.lengthSq(xn, nextYn, zn) <= 1 &&
								    PluginGlobals.lengthSq(xn, yn, nextZn) <= 1)
								{
									continue;
								}
							}

							PlaceBlock(level, block, position, new Vector3(x, y, z));
							PlaceBlock(level, block, position, new Vector3(-x, y, z));
							PlaceBlock(level, block, position, new Vector3(x, -y, z));
							PlaceBlock(level, block, position, new Vector3(x, y, -z));
							PlaceBlock(level, block, position, new Vector3(-x, -y, z));
							PlaceBlock(level, block, position, new Vector3(x, -y, -z));
							PlaceBlock(level, block, position, new Vector3(-x, y, -z));
							PlaceBlock(level, block, position, new Vector3(-x, -y, -z));
						}
					}
				}
			}).Start();
		}

		private void PlaceBlock(Level lvl, Block block, Vector3 mainPosition, Vector3 offset)
		{
			var mp = mainPosition + offset;
			block.Coordinates = mp;

			lvl.SetBlock(block);
		}
	}
}