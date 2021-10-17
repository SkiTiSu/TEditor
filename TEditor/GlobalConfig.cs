using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using TEditor.Converters;

namespace TEditor
{
    public sealed class GlobalConfig
    {
        private static readonly Lazy<GlobalConfig> lazy = new(() => new GlobalConfig());
        public static GlobalConfig Instance => lazy.Value;

        private GlobalConfig()
        {
            JsonSerializerOptions options = new()
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = false,
            };
            options.Converters.Add(new TypeConverterJsonAdapter());
            JsonOptions = options;
        }

        public JsonSerializerOptions JsonOptions { get; private set; }
    }
}
