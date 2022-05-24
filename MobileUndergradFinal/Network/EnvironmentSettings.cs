using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace Network
{
    public sealed class EnvironmentSettings
    {
        private static EnvironmentSettings _settings;
        public EnvironmentInformation Information { get; }

        private EnvironmentSettings(EnvironmentInformation information)
        {
            Information = information;
        }

        public static EnvironmentSettings Instance
        {
            get
            {
                _settings ??= new EnvironmentSettings(ReadInformation());
                return _settings;
            }
        }

        private static EnvironmentInformation ReadInformation()
        {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MobileUndergradFinal.settings.json");
            var jsonFile = new StreamReader(stream).ReadToEnd();
            return JsonConvert.DeserializeObject<EnvironmentInformation>(jsonFile);
        }
    }
}
