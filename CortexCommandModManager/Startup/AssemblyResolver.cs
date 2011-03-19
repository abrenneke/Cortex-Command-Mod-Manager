using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Security.Policy;

namespace CortexCommandModManager.Startup
{
    public class AssemblyResolver
    {
        private AssemblyResolverConfiguration configuration;

        public AssemblyResolver(AssemblyResolverConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public Assembly Resolve(object sender, ResolveEventArgs e)
        {
            var assemblyName = CleanAssemblyName(e.Name);
            var path = configuration.GetResolvePath(assemblyName);

            if (!FileIsValid(path))
                return null;

            return TryLoadAssembly(path);
        }

        private Assembly TryLoadAssembly(string path)
        {
            try
            {
                return Assembly.LoadFrom(path);
            }
            catch (Exception) 
            {
                return null;
            }
        }

        private bool FileIsValid(string path)
        {
            var file = new FileInfo(path);
            return file.Exists;
        }

        private string CleanAssemblyName(string fullAssemblyName)
        {
            if (fullAssemblyName.IndexOf(',') == -1)
                return fullAssemblyName;

            return fullAssemblyName.Substring(0, fullAssemblyName.IndexOf(','));
        }
    }
}
