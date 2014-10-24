using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using PluginCommon;
using PluginInterface;

namespace ExamplePlugin
{
    public class Plugin : IPlugin
    {
        public string Name { get { return "Test"; } }
        public string Description { get { return "Desc"; } }
        public string Author { get { return "Matthew"; } }
        public Version Version { get{return new Version();} }
        public PluginOption[] ProvidesOptions { get { return null; } }
        public void Initialise(string[] args)
        {
            dynamic a = new Class1();
            PluginLoader.Register(Assembly.GetEntryAssembly(), "PluginApp.Program", new[] { a });
        }

        public void Dispose()
        {
            return;
        }
    }

    public class Class1 : IConsole
    {
        public void Run()
        {
            Console.WriteLine("Test1234");
            Console.ReadKey();
        }
    }
}
