using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TEditor.Converters
{
    //https://github.com/dotnet/runtime/issues/1761
    /// <summary>
    /// Adapter between <see cref="System.ComponentModel.TypeConverter"/> 
    /// and <see cref="JsonConverter"/>
    /// </summary>
    public class TypeConverterJsonAdapter : JsonConverter<object>
    {
        public override object Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {

            var converter = TypeDescriptor.GetConverter(typeToConvert);
            var text = reader.GetString();
            return converter.ConvertFromString(text);
        }

        public override void Write(
            Utf8JsonWriter writer,
            object objectToWrite,
            JsonSerializerOptions options)
        {

            var converter = TypeDescriptor.GetConverter(objectToWrite);
            var text = converter.ConvertToString(objectToWrite);
            writer.WriteStringValue(text);
        }

        public override bool CanConvert(Type typeToConvert)
        {
            var hasConverter = typeToConvert.GetCustomAttributes(typeof(TypeConverterAttribute), true).Any();
            return hasConverter;
        }
    }
}
