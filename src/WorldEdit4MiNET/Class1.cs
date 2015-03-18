using MiNET.Plugins;
using MiNET.Plugins.Attributes;
using log4net;

namespace WorldEdit4MiNET
{
	[Plugin(Author = "kennyvv", Description = "A WorldEdit plugin for MiNET", PluginName = "We4MiNET", PluginVersion = "1.0")]
    public partial class We4MiNet : IPlugin
    {
		private static readonly ILog Log = LogManager.GetLogger(typeof(We4MiNet));

	    public void OnEnable(PluginContext context)
	    {
		    PluginGlobals.PluginContext = context;
			Log.Info("WorldEdit loaded!");
	    }

	    public void OnDisable()
	    {
		    //Safely shutdown 
	    }
    }
}
