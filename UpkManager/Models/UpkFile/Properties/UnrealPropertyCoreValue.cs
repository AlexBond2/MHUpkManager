using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UpkManager.Constants;
using UpkManager.Helpers;
using UpkManager.Models.UpkFile.Core;

namespace UpkManager.Models.UpkFile.Properties
{
    public static class UnrealPropertyFactory
    {
        public static UnrealPropertyValueBase Create(string type)
        {
            if (CoreRegistry.Instance.TryGetProperty(type, out var prop))
                return new UnrealPropertyCoreValue(prop);

            return type switch
            {
                "Int32" => new UnrealPropertyIntValue(),
                "Boolean" => new UnrealPropertyBoolValue(),
                "Single" => new UnrealPropertyFloatValue(),
                _ => new UnrealPropertyValueBase()
            };
        }
    }

    public class UnrealPropertyCoreValue : UnrealPropertyValueBase
    {
        public string StructName { get; }
        public IAtomicStruct Atomic { get; private set; }
        public List<(string Name, UnrealPropertyValueBase Value)> Fields { get; } = [];
        public override PropertyTypes PropertyType => PropertyTypes.StructProperty;

        public UnrealPropertyCoreValue(Type structType)
        {
            StructName = structType.Name;
            Atomic = (IAtomicStruct)Activator.CreateInstance(structType)!;

            var props = structType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetCustomAttribute<StructFieldAttribute>() != null);

            foreach (var prop in props)
            {
                var unrealValue = UnrealPropertyFactory.Create(prop.PropertyType.Name);
                Fields.Add((prop.Name, unrealValue));
            }
        }

        public override string PropertyString
        {
            get
            {
                if (string.IsNullOrEmpty(Atomic.Format)) return StructName;

                SetValuesToStruct();
                return Atomic.Format;
            }
        }

        private void SetValuesToStruct()
        {
            var structType = Atomic.GetType();

            foreach (var (name, value) in Fields)
            {
                var prop = structType.GetProperty(name);
                if (prop == null) continue;

                object? val = value switch
                {
                    UnrealPropertyIntValue intVal => intVal.PropertyValue,
                    UnrealPropertyFloatValue floatVal => floatVal.PropertyValue,
                    UnrealPropertyBoolValue boolVal => boolVal.PropertyValue,
                    UnrealPropertyCoreValue coreVal => coreVal.Atomic,
                    _ => null
                };

                if (val != null)
                    prop.SetValue(Atomic, val);
            }
        }

        public override void BuildVirtualTree(VirtualNode valueTree)
        {
            if (!string.IsNullOrEmpty(Atomic.Format))
                base.BuildVirtualTree(valueTree);
            else
                for (int i = 0; i < Fields.Count; i++)
                {
                    var (name, value) = Fields[i];

                    var prop = Atomic.GetType().GetProperty(name);
                    string typeName = prop?.PropertyType.Name ?? "Unknown";

                    var node = new VirtualNode($"{name} ::{typeName}");
                    value.BuildVirtualTree(node);
                    valueTree.Children.Add(node);
                }
        }

        public override void ReadPropertyValue(ByteArrayReader reader, int size, UnrealHeader header, UnrealProperty property)
        {
            foreach (var (_, value) in Fields)
                value.ReadPropertyValue(reader, size, header, property);
        }
    }

}
