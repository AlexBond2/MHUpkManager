using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UpkManager.Constants;
using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Tables;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Engine
{
    public class StructInfo
    {
        public string Parent { get; set; }
        public string Name { get; set; } 
        public PropertyTypes Type { get; set; }
        public string Struct { get; set; } // Type == Struct
    }

    public class EngineRegistry
    {
        private readonly Dictionary<string, StructInfo> _structs;
        public static EngineRegistry Instance { get; } = new EngineRegistry();

        private EngineRegistry()
        {
            _structs = new Dictionary<string, StructInfo>(StringComparer.OrdinalIgnoreCase);

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
            var type = prop.PropertyType;
            var name = prop.Name;

            if (_structs.ContainsKey(name))
            {
                System.Diagnostics.Debug.WriteLine($"⚠️ Warning: Duplicate property name detected: '{name}' (parent: {parentName})");
                System.Diagnostics.Debug.WriteLine($"    Existing parent: {_structs[name].Parent}, New parent: {parentName}");
            }

            if (TryGetElementTypeIfArray(type, out var elementType))
            {
                var propertyKind = GetPropertyType(elementType);
                var structName = propertyKind == PropertyTypes.StructProperty ? elementType.Name : null;

                _structs[name] = new StructInfo
                {
                    Parent = parentName,
                    Name = name,
                    Type = propertyKind,
                    Struct = structName
                };
            }
            else
            {
                var propertyKind = GetPropertyType(type);
                if (propertyKind == PropertyTypes.StructProperty)
                {
                    _structs[name] = new StructInfo
                    {
                        Parent = parentName,
                        Name = name,
                        Type = propertyKind,
                        Struct = type.Name
                    };
                }
            }
        }

        public static bool TryGetElementTypeIfArray(Type propertyType, out Type elementType)
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
            if (type == typeof(UName) || type == typeof(FName)) return PropertyTypes.NameProperty;
            if (type == typeof(FObject)) return PropertyTypes.ObjectProperty;
            if (typeof(UObject).IsAssignableFrom(type)) return PropertyTypes.ObjectProperty;
            if (type.IsClass || type.IsValueType) return PropertyTypes.StructProperty;

            return PropertyTypes.UnknownProperty;
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

        public bool TryGetProperty(string name, UObject parent, out StructInfo definition)
        {
            return _structs.TryGetValue(name, out definition);
        }
    }

}
