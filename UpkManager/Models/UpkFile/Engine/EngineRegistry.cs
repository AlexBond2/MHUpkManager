using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using UpkManager.Constants;

namespace UpkManager.Models.UpkFile.Engine
{
    public class CustomStructJson
    {
        public string Name { get; set; } 
        [JsonConverter(typeof(PropertyTypesConverter))]
        public PropertyTypes Type { get; set; }
        public string Struct { get; set; } // Type == Struct
    }

    public class PropertyTypesConverter : JsonConverter<PropertyTypes>
    {
        public override PropertyTypes Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string str = reader.GetString();
            if (str == null) return PropertyTypes.UnknownProperty;

            return str.ToLowerInvariant() switch
            {
                "int" => PropertyTypes.IntProperty,
                "float" => PropertyTypes.FloatProperty,
                "bool" => PropertyTypes.BoolProperty,
                "struct" => PropertyTypes.StructProperty,
                "object" => PropertyTypes.ObjectProperty,
                "name" => PropertyTypes.NameProperty,
                "string" => PropertyTypes.StrProperty,
                _ => PropertyTypes.UnknownProperty
            };
        }

        public override void Write(Utf8JsonWriter writer, PropertyTypes value, JsonSerializerOptions options)
        {
            string str = value switch
            {
                PropertyTypes.IntProperty => "Int",
                PropertyTypes.FloatProperty => "Float",
                PropertyTypes.BoolProperty => "Bool",
                PropertyTypes.StructProperty => "Struct",
                PropertyTypes.ObjectProperty => "Object",
                PropertyTypes.NameProperty => "Name",
                PropertyTypes.StrProperty => "String",
                _ => "Unknown"
            };
            writer.WriteStringValue(str);
        }
    }

    public static class EngineRegistry
    {
        private static Dictionary<string, CustomStructJson> _structs;

        public static string LoadFromJson(string jsonPath)
        {
            string json = File.ReadAllText(jsonPath);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            List<CustomStructJson> loadedStructs;
            try
            {
                loadedStructs = JsonSerializer.Deserialize<List<CustomStructJson>>(json, options);
            }
            catch (Exception ex)
            {
                return $"Failed to deserialize JSON: {ex.Message}";
            }

            if (loadedStructs == null)
                return "No structs found in JSON.";

            var errors = new List<string>();
            var tempDict = new Dictionary<string, CustomStructJson>(StringComparer.OrdinalIgnoreCase);

            foreach (var csj in loadedStructs)
            {
                if (csj.Type == PropertyTypes.UnknownProperty)
                    errors.Add($"Unknown property type in struct '{csj.Name}'.");

                tempDict[csj.Name] = csj;
            }

            _structs = tempDict;

            if (errors.Count > 0)
                return "Warning: " + string.Join("; ", errors);

            return string.Empty;
        }

        public static bool TryGetStruct(string name, out string definition)
        {
            var found = _structs.Values
                .FirstOrDefault(s =>
                    s.Type == PropertyTypes.StructProperty &&
                    string.Equals(s.Struct, name, StringComparison.OrdinalIgnoreCase));

            if (found != null)
            {
                definition = found.Name;
                return true;
            }

            definition = null;
            return false;
        }

        public static bool TryGetProperty(string name, out CustomStructJson definition)
        {
            return _structs.TryGetValue(name, out definition);
        }
    }

}
