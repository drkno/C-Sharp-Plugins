using System;
using System.IO;
using PluginInterface;

namespace PluginApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            //string pluginDir = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Plugins");
            var pluginDir = Path.GetFullPath(@"..\..\..\..\PluginLoader\ExamplePlugin\bin\Debug");

            var loader = new PluginCommon.PluginLoader(pluginDir);
            loader.PluginLoadComplete += PluginLoadComplete;
            loader.PluginLoadFailed += PluginLoadFailed;
            loader.PluginLoadingComplete += PluginLoadingComplete;
            loader.PluginLoadingStarted += PluginLoadingStarted;
            loader.GetPlugins();
            loader.InitialisePlugins(args);
            Console.ReadKey();
        }

        private static void PluginLoadingStarted(PluginCommon.PluginLoader sender, object otherData)
        {
            Console.WriteLine("Loading Started...");
        }

        private static void PluginLoadingComplete(PluginCommon.PluginLoader sender, object otherData)
        {
            Console.WriteLine("Loading Complete");
        }

        private static void PluginLoadFailed(PluginCommon.PluginLoader sender, object otherData)
        {
            Console.WriteLine("Load of plugin failed");
        }

        private static void PluginLoadComplete(PluginCommon.PluginLoader sender, object otherData)
        {
            Console.WriteLine("Load of plugin complete");
        }

        public static bool RegisterPlugin(dynamic plugin)
        {
            Console.WriteLine("Registering plugin...");
            if (plugin is IConsole)
            {
                var p = (IConsole) plugin;
                p.Run();
                return true;
            }
            Console.WriteLine("Failed");
            return false;
        }
    }
}
