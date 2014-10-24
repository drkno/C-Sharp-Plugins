using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;

namespace PluginCommon
{
    public delegate void PluginLoadingEvent(PluginLoader sender, object otherData);
    public class PluginLoader : IReadOnlyCollection<IPlugin>
    {
        private readonly string _pluginsDirectory;
        private readonly List<IPlugin> _plugins;

        public PluginLoader(string pluginsDirectory)
        {
            if (!Directory.Exists(pluginsDirectory))
            {
                throw new DirectoryNotFoundException(pluginsDirectory);
            }
            _pluginsDirectory = pluginsDirectory;
            _plugins = new List<IPlugin>();
        }

        #region Import Plugins

        public PluginLoadingEvent PluginLoadingStarted { get; set; }
        public PluginLoadingEvent PluginLoadingComplete { get; set; }
        public PluginLoadingEvent PluginLoadFailed { get; set; }
        public PluginLoadingEvent PluginLoadComplete { get; set; }

        private Thread _loaderThread;
        private bool _loadShouldContinue;

        public void GetPlugins()
        {
            _loadShouldContinue = true;
            if (PluginLoadingStarted != null)
            {
                PluginLoadingStarted(this, null);
            }

            var pluginsList = Directory.GetFiles(_pluginsDirectory, "*.dll");
            foreach (var pluginFile in pluginsList)
            {
                try
                {
                    var assembly = Assembly.LoadFrom(pluginFile);

                    foreach (var type in assembly.GetTypes())
                    {
                        try
                        {
                            var interfaceType = type.GetInterface("PluginCommon.IPlugin", true);
                            if (interfaceType == null)
                            {
                                continue;
                            }

                            var plugIn = (IPlugin)Activator.CreateInstance(assembly.GetType(type.ToString()));
                            _plugins.Add(plugIn);

                            if (PluginLoadComplete != null)
                            {
                                PluginLoadComplete(this, plugIn);
                            }
                            Debug.WriteLine("Plugin Load Successful");
                        }
                        catch (Exception e)
                        {
                            if (PluginLoadFailed != null)
                            {
                                PluginLoadFailed(this, e);
                            }
                            Debug.WriteLine("Plugin Load failed with exception:\n" + e);
                        }

                        if (!_loadShouldContinue)
                        {
                            return;
                        }
                    }
                }
                catch (Exception e)
                {
                    if (!_loadShouldContinue)
                    {
                        return;
                    }
                    Debug.WriteLine("General plugin loading exception:\n" + e);
                }
            }

            if (PluginLoadingComplete != null)
            {
                PluginLoadingComplete(this, true);
            }
        }

        public void GetPluginsBegin()
        {
            _loaderThread = new Thread(GetPlugins);
            _loaderThread.Start();
        }

        public void GetPluginsEnd()
        {
            _loadShouldContinue = false;
            if (PluginLoadingComplete != null)
            {
                PluginLoadingComplete(this, false);
            }
        }

        public void InitialisePlugins(string[] args)
        {
            foreach (var plugin in _plugins)
            {
                plugin.Initialise(args);
            }
        }

        public void DisposePlugins()
        {
            foreach (var plugin in _plugins)
            {
                plugin.Dispose();
            }
        }

        #endregion

        public IPlugin this[int index]
        {
            get { return _plugins[index]; }
        }

        public IEnumerator<IPlugin> GetEnumerator()
        {
            return _plugins.AsReadOnly().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Contains(IPlugin item)
        {
            return _plugins.Contains(item);
        }

        public int Count { get { return _plugins.Count; } }

        public static bool Register(Assembly assembly, string typeClass, object[] obj)
        {
            try
            {
                var type = assembly.GetType(typeClass);
                Debug.Assert(type != null, "type != null");
                var info = type.GetMethod("RegisterPlugin", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
                var result = info.Invoke(null, obj);
                if (result is bool)
                {
                    return (bool)result;
                }
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Registering plugin method failed with exception:\n" + e);
                return false;
            }
        }
    }
}
