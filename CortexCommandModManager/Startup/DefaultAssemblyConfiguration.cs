using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CortexCommandModManager.Startup
{
    public class DefaultAssemblyConfiguration : AssemblyResolverConfiguration
    {
        public DefaultAssemblyConfiguration()
        {
            Map("Newtonsoft.Json");
            Map("SevenZipSharp");
            Map("XmlConfig");
        }
    }
}
