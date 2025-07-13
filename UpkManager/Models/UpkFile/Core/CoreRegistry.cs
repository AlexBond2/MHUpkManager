using System.Collections.Generic;
using System.Text.Json;
using System.IO;
using System;

namespace UpkManager.Models.UpkFile.Core
{
    public class CustomCoreFieldJson
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }

    public class CustomCoreJson
    {
        public string Name { get; set; }
        public List<CustomCoreFieldJson> Fields { get; set; }
        public string Format { get; set; }
    }

    public static class CoreRegistry
    {
        private static Dictionary<string, CustomCoreJson> _structs;

        public static string LoadFromJson(string jsonPath)
        {
            string json = File.ReadAllText(jsonPath);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            List<CustomCoreJson> loadedStructs;
            try
            {
                loadedStructs = JsonSerializer.Deserialize<List<CustomCoreJson>>(json, options);
            }
            catch (Exception ex)
            {
                return $"Failed to deserialize JSON: {ex.Message}";
            }

            if (loadedStructs == null)
                return "No structs found in JSON.";

            var errors = new List<string>();
            var tempDict = new Dictionary<string, CustomCoreJson>(StringComparer.OrdinalIgnoreCase);

            foreach (var csj in loadedStructs)
                tempDict[csj.Name] = csj;

            _structs = tempDict;

            if (errors.Count > 0)
                return "Warning: " + string.Join("; ", errors);

            return string.Empty;
        }

        public static bool TryGetProperty(string name, out CustomCoreJson definition)
        {
            return _structs.TryGetValue(name, out definition);
        }
    }
}
