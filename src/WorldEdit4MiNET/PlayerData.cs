using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiNET.Blocks;

namespace WorldEdit4MiNET
{
	public class PlayerData
	{
		public string Username { get; set; }
		public List<Block> CopiedBlocks { get; set; }

		public PlayerData(string username)
		{
			Username = username;
			CopiedBlocks = new List<Block>();
		}
	}
}
