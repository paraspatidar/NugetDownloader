using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace NugetWorker.Utility
{
    public sealed class ObjectConverter
    {

        private static readonly Lazy<ObjectConverter> lazy =
         new Lazy<ObjectConverter>(() => new ObjectConverter());

        public static ObjectConverter Instance { get { return lazy.Value; } }

        private ObjectConverter()
        {

        }

        
        public NugetSettings GetWatcherSettingsFromJson(string json)
        {
            return JsonConvert.DeserializeObject<NugetSettings>(json);
        }

        

    }
}
