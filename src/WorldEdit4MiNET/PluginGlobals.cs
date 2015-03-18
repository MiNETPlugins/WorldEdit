using System;
using System.Collections.Generic;
using System.Text;
using log4net.Filter;
using MiNET;
using MiNET.Net;
using MiNET.Plugins;
using MiNET.Utils;

namespace WorldEdit4MiNET
{
	public class PluginGlobals
	{
		public static PluginContext PluginContext { get; set; }

		public static Dictionary<string, Tuple<Vector3, Vector3>> Locations = new Dictionary<string, Tuple<Vector3, Vector3>>();
		public static Dictionary<string, PlayerData> PlayerDataDictionary = new Dictionary<string, PlayerData>(); 
		public static void SendMessage(Player player, string message, string sender = "WorldEdit")
		{
			player.SendPackage(new McpeMessage() { message = message, source = sender });
		}

		public static string GetString(Vector3 vector)
		{
			return vector.X + ", " + vector.Y + ", " + vector.Z;
		}

		public static double lengthSq(double x, double y, double z){
			return (x * x) + (y * y) + (z * z);
		}
	}
}
