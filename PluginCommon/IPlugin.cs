using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginCommon
{
    public class PluginOption
    {
        public string Prototype { get; private set; }
        public string Description { get; private set; }
        public Action<string> Action { get; private set; }

        public PluginOption(string prototype, string description, Action<string> action)
        {
            Prototype = prototype;
            Description = description;
            Action = action;
        }
    }

    public interface IPlugin
    {
        string Name { get; }
        string Description { get; }
        string Author { get; }
        Version Version { get; }

        PluginOption[] ProvidesOptions { get; }

        void Initialise(string[] args);
        void Dispose();
    }
}
