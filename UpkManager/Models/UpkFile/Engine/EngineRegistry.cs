using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using UpkManager.Constants;
using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Tables;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Engine
{
    public class CustomStructJson
    {
        public string Parent { get; set; }
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

    public class EngineRegistry
    {
        private readonly Dictionary<string, CustomStructJson> _structs;
        public static EngineRegistry Instance { get; } = new EngineRegistry();

        private EngineRegistry()
        {
            _structs = new Dictionary<string, CustomStructJson>(StringComparer.OrdinalIgnoreCase);

            var unrealObjectType = typeof(UObject);
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
                foreach (var type in assembly.GetTypes())
                {
                    if (unrealObjectType.IsAssignableFrom(type))
                    {
                        var classAttr = type.GetCustomAttribute<UnrealClassAttribute>();
                        if (classAttr != null)
                            RegisterFieldsWithAttribute(type, classAttr.ClassName, typeof(PropertyFieldAttribute));
                    }

                    var structAttr = type.GetCustomAttribute<UnrealStructAttribute>();
                    if (structAttr != null)
                        RegisterFieldsWithAttribute(type, structAttr.StructName, typeof(StructFieldAttribute));
                }

        }

        private void RegisterFieldsWithAttribute(Type type, string parentName, Type attributeType)
        {
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in props)
            {
                if (prop.GetCustomAttribute(attributeType) == null)  continue;
                RegisterPropertyIfArray(prop, parentName);
            }
        }

        private void RegisterPropertyIfArray(PropertyInfo prop, string parentName)
        {
            if (!TryGetElementTypeIfArray(prop.PropertyType, out var elementType)) return;

            var propertyKind = GetPropertyType(elementType);
            var structName = propertyKind == PropertyTypes.StructProperty ? elementType.Name : null;

            //var key = $"{parentName}.{prop.Name}";
            var key = prop.Name;
            _structs[key] = new CustomStructJson
            {
                Parent = parentName,
                Name = prop.Name,
                Type = propertyKind,
                Struct = structName
            };
        }

        private static bool TryGetElementTypeIfArray(Type propertyType, out Type elementType)
        {
            elementType = null;

            if (propertyType.IsArray)
            {
                elementType = propertyType.GetElementType();
                return true;
            }

            if (propertyType.IsGenericType)
            {
                var genericDef = propertyType.GetGenericTypeDefinition();
                if (genericDef == typeof(UArray<>) || genericDef == typeof(List<>))
                {
                    elementType = propertyType.GetGenericArguments()[0];
                    return true;
                }
            }

            return false;
        }

        private static PropertyTypes GetPropertyType(Type type)
        {
            if (type == typeof(int)) return PropertyTypes.IntProperty;
            if (type == typeof(float)) return PropertyTypes.FloatProperty;
            if (type == typeof(bool)) return PropertyTypes.BoolProperty;
            if (type == typeof(string)) return PropertyTypes.StrProperty;
            if (type == typeof(UName)) return PropertyTypes.NameProperty;
            if (type == typeof(FName)) return PropertyTypes.ObjectProperty;
            if (typeof(UObject).IsAssignableFrom(type)) return PropertyTypes.ObjectProperty;
            if (type.IsClass || type.IsValueType) return PropertyTypes.StructProperty;

            return PropertyTypes.UnknownProperty;
        }

        public string LoadFromJson(string jsonPath)
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

            foreach (var csj in loadedStructs)
            {
                if (csj.Type == PropertyTypes.UnknownProperty)
                    errors.Add($"Unknown property type in struct '{csj.Name}'.");

                _structs[csj.Name] = csj;
            }

            if (errors.Count > 0)
                return "Warning: " + string.Join("; ", errors);

            return string.Empty;
        }

        public bool TryGetStruct(string name, UObject parent, out string definition)
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

        public bool TryGetProperty(string name, UObject parent, out CustomStructJson definition)
        {
            return _structs.TryGetValue(name, out definition);
        }
    }

}
