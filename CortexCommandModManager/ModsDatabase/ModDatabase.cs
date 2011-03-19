using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Newtonsoft.Json;

namespace CortexCommandModManager.ModsDatabase
{
    public class ModDatabase
    {
        private static readonly Uri ModDatabaseUrl = new Uri(@"http://dev.cortexmods.com/");

        public void GetAllModsAsync(Action<IList<ModDatabaseMod>> callback)
        {
            var client = CreateClient();

            client.DownloadStringCompleted += (o, e) => 
            {
                var items = JsonConvert.DeserializeObject<IList<ModDatabaseMod>>(e.Result);
                callback(items);
            };

            client.DownloadStringAsync(MakeApiUrl("getAllMods"));
        }

        public void DownloadImageAsync(ModDatabaseMod mod, Action<byte[]> callback)
        {
            var url = new Uri(ModDatabaseUrl, mod.Screenshot);
            var client = CreateClient();

            client.DownloadDataCompleted += (o, e) => callback(e.Result);
            client.DownloadDataAsync(url);
        }

        public void DownloadModAsync(ModDatabaseMod mod, Action<byte[]> callback, DownloadProgressChangedEventHandler progressChanged = null )
        {
            var url = new Uri(ModDatabaseUrl, mod.DownloadLink);
            var client = CreateClient();

            client.DownloadDataCompleted += (o, e) => callback(e.Result);
            client.DownloadProgressChanged += progressChanged;

            client.DownloadDataAsync(url);
        }

        private Uri MakeApiUrl(string method)
        {
            return new Uri(ModDatabaseUrl, "/api/ccmm/" + method);
        }

        private WebClient CreateClient()
        {
            return new WebClient
            {
                Proxy = null
            };
        }
    }
}
