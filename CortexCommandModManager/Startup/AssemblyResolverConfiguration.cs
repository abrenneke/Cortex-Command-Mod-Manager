using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace CortexCommandModManager.Startup
{
    public abstract class AssemblyResolverConfiguration
    {
        private const string DllDirectory = "lib";

        private IDictionary<string, string> assemblyMap;
        private IDictionary<string, string> AssemblyMap
        {
            get
            {
                if (assemblyMap == null)
                    assemblyMap = new Dictionary<string, string>();
                return assemblyMap;
            }
        }

        public string GetResolvePath(string assembly)
        {
            var dllName = GetDllName(assembly);
            return GetDllPath(dllName);
        }

        protected void Map(string assemblyName)
        {
            Map(assemblyName, TryMakeDllName(assemblyName));
        }

        protected void Map(string assemblyName, string dll)
        {
            AssemblyMap.Add(assemblyName, dll);
        }

        private string GetDllPath(string dllName)
        {
            var currentDirectory = Grabber.ModManagerDirectory;
            return Path.Combine(currentDirectory, DllDirectory, dllName);
        }

        private string GetDllName(string assembly)
        {
            if (assemblyMap.ContainsKey(assembly))
                return assemblyMap[assembly];

            return TryMakeDllName(assembly);
        }

        private string TryMakeDllName(string assemblyName)
        {
            return assemblyName + ".dll";
        }
    }
}
