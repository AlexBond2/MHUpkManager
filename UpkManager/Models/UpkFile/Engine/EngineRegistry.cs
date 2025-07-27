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
        public Type StuctType { get; set; }
    }

    public class EngineRegistry
    {
        private readonly List<StructInfo> _structs; 
        private readonly Dictionary<Type, string> _unrealClassNames;
        private readonly Dictionary<Type, string> _unrealStructNames;
        public static EngineRegistry Instance { get; } = new EngineRegistry();

        private EngineRegistry()
        {
            _structs = [];
            _unrealClassNames = [];
            _unrealStructNames = [];

            var unrealObjectType = typeof(UObject);
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
                foreach (var type in assembly.GetTypes())
                {
                    var classAttr = type.GetCustomAttribute<UnrealClassAttribute>();
                    if (classAttr != null)
                    {
                        _unrealClassNames[type] = classAttr.ClassName;

                        if (unrealObjectType.IsAssignableFrom(type))
                            RegisterFieldsWithAttribute(type, classAttr.ClassName, typeof(PropertyFieldAttribute));
                    }

                    var structAttr = type.GetCustomAttribute<UnrealStructAttribute>();
                    if (structAttr != null)
                    {
                        _unrealStructNames[type] = structAttr.StructName;
                        RegisterFieldsWithAttribute(type, structAttr.StructName, typeof(StructFieldAttribute));
                    }
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

        public static string GetStructName(Type type)
        {
            var attr = type.GetCustomAttribute<UnrealStructAttribute>();
            return attr?.StructName ?? type.Name;
        }

        private void RegisterPropertyIfArray(PropertyInfo prop, string parent)
        {
            var type = prop.PropertyType;
            var name = prop.Name;

            if (TryGetElementTypeIfArray(type, out var elementType))
            {
                var propertyKind = GetPropertyType(elementType);
                var structName = propertyKind == PropertyTypes.StructProperty ? GetStructName(elementType) : null;

                var info = new StructInfo
                {
                    Parent = parent,
                    Name = name,
                    Type = propertyKind,
                    Struct = structName,
                    StuctType = elementType
                };

                _structs.Add(info);
            }
            else
            {
                var propertyKind = GetPropertyType(type);
                if (propertyKind == PropertyTypes.StructProperty)
                {
                    var info = new StructInfo
                    {
                        Parent = parent,
                        Name = name,
                        Type = propertyKind,
                        Struct = GetStructName(type),
                        StuctType = type
                    };

                    _structs.Add(info);
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

        private IEnumerable<string> EnumerateParentClassNames(object obj)
        {
            var type = obj.GetType();

            while (type != null && type != typeof(UObject))
            {
                if (_unrealClassNames.TryGetValue(type, out var className))
                    yield return className;
                else if (_unrealStructNames.TryGetValue(type, out var structName))
                    yield return structName;

                type = type.BaseType;
            }
        }

        private HashSet<string> GetParentClassChain(UObject parent)
        {
            if (parent == null || parent.GetType() == typeof(UObject))
                return null;

            return EnumerateParentClassNames(parent).ToHashSet(StringComparer.OrdinalIgnoreCase);
        }

        public bool TryGetStruct(string structName, UObject parent, out Type structType)
        {
            var classChain = GetParentClassChain(parent);

            var match = _structs.FirstOrDefault(info =>
                info.Type == PropertyTypes.StructProperty &&
                string.Equals(info.Struct, structName, StringComparison.OrdinalIgnoreCase) &&
                (classChain == null || classChain.Contains(info.Parent)));

            match ??= _structs.FirstOrDefault(info =>
                    info.Type == PropertyTypes.StructProperty &&
                    string.Equals(info.Struct, structName, StringComparison.OrdinalIgnoreCase));

            if (match != null)
            {
                structType = match.StuctType;
                return true;
            }

            structType = null;
            return false;
        }

        public bool TryGetProperty(string name, UObject parent, out StructInfo result)
        {
            result = null;
            if (string.IsNullOrEmpty(name)) return false;

            var classChain = GetParentClassChain(parent);

            result = _structs.FirstOrDefault(info =>
                    string.Equals(info.Name, name, StringComparison.OrdinalIgnoreCase) &&
                    classChain != null &&
                    classChain.Contains(info.Parent));

            if (result != null)
                return true;

            result = _structs.FirstOrDefault(info =>
                string.Equals(info.Name, name, StringComparison.OrdinalIgnoreCase));

            return result != null;
        }

    }
}
