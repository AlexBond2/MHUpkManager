using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Media;
using System.Xml.Linq;
using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Core
{
    public static class PropertyFactory
    {
        public static UProperty Create(string type)
        {
            if (CoreRegistry.Instance.TryGetProperty(type, out var prop))
                return new CoreProperty(prop);

            return type switch
            {
                "Int32" => new UIntProperty(),
                "Boolean" => new UBoolProperty(),
                "Single" => new UFloatProperty(),
                _ => new UProperty()
            };
        }
    }

    public class CoreProperty : UProperty
    {
        public string StructName { get; }
        public IAtomicStruct Atomic { get; private set; }
        public List<(string Name, UProperty Value)> Fields { get; } = [];

        public CoreProperty(Type structType)
        {
            StructName = structType.Name;
            Atomic = (IAtomicStruct)Activator.CreateInstance(structType)!;

            foreach (var prop in GetStructFields(structType))
            {
                var unrealValue = PropertyFactory.Create(prop.PropertyType.Name);
                Fields.Add((prop.Name, unrealValue));
            }
        }

        public static IEnumerable<PropertyInfo> GetStructFields(object obj)
        {
            Type type = obj.GetType();
            foreach (var field in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (field.IsDefined(typeof(StructFieldAttribute)))
                    yield return field;
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

                object val = value switch
                {
                    UIntProperty intVal => intVal.PropertyValue,
                    UFloatProperty floatVal => floatVal.PropertyValue,
                    UBoolProperty boolVal => boolVal.PropertyValue,
                    CoreProperty coreVal => coreVal.Atomic,
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
                    string typeName = prop.PropertyType.Name ?? "Unknown";

                    var node = new VirtualNode($"{name} ::{typeName}");
                    value.BuildVirtualTree(node);
                    valueTree.Children.Add(node);
                }
        }

        public override void ReadPropertyValue(UBuffer buffer, int size, UnrealProperty property)
        {
            foreach (var (_, value) in Fields)
                value.ReadPropertyValue(buffer, size, property);
        }

        public static void BuildStructVirtualTree(VirtualNode fieldNode, IAtomicStruct atomic)
        {
            if (!string.IsNullOrEmpty(atomic.Format))
                fieldNode.Children.Add(new(atomic.Format));
            else
            {
                foreach (var field in GetStructFields(atomic))
                {
                    string typeName = field.PropertyType.Name ?? "Unknown";
                    string fieldName = field.Name;

                    var structNode = new VirtualNode($"{fieldName} ::{typeName}");
                    
                    object fieldValue = field.GetValue(atomic);
                    if (fieldValue is IAtomicStruct chieldAtomic)
                    {
                        BuildStructVirtualTree(structNode, chieldAtomic);
                    }
                    else if (fieldValue is not null)
                    {
                        var node = new VirtualNode(fieldValue.ToString());
                        structNode.Children.Add(node);
                    }
                    fieldNode.Children.Add(structNode);
                }
            }
        }
    }
}
