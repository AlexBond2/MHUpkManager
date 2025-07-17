using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UpkManager.Models.UpkFile.Properties;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Classes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class TreeNodeFieldAttribute(string typeName = null) : Attribute
    {
        public string TypeName { get; } = typeName;
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class PropertyFieldAttribute() : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class UnrealClassAttribute(string className) : Attribute
    {
        public string ClassName { get; } = className;
    }

    [UnrealClass("Object")]
    public class UObject// : UnrealUpkBuilderBase
    {
        [TreeNodeField]
        public int NetIndex { get; private set; } = -1;
        public List<UnrealProperty> Properties { get; } = [];

        public virtual VirtualNode GetVirtualNode()
        {
            var node = new VirtualNode(GetType().Name);

            if (Properties.Count > 0)
            {
                var fieldNode = new VirtualNode($"Properties");
                foreach (var prop in Properties)
                    fieldNode.Children.Add(prop.VirtualTree);
                node.Children.Add(fieldNode);
            }

            foreach (var prop in GetTreeViewFields(this))
            {
                var attr = prop.GetCustomAttribute<TreeNodeFieldAttribute>();

                string displayName = prop.Name;
                string typeName = attr.TypeName ?? GetTypeName(prop.PropertyType);

                var fieldNode = new VirtualNode($"{displayName} ::{typeName}");

                object value = prop.GetValue(this);
                if (value == null)
                {
                    fieldNode.Children.Add(new("null"));
                }
                else if (value is System.Collections.IEnumerable enumerable && value is not string)
                {
                    fieldNode.Text += "[]";
                    var listNode = new VirtualNode();
                    int count = 0;
                    if (enumerable is byte[] data)
                    {
                        count = data.Length;
                        listNode.Tag = data;
                    }
                    else
                    {
                        foreach (var item in enumerable)
                        {
                            listNode.Children.Add(new($"[{count}] {item}"));
                            count++;
                        }
                    }
                    listNode.Text = $"{typeName}[{count}]";
                    fieldNode.Children.Add(listNode);
                }
                else
                {
                    fieldNode.Children.Add(new (value.ToString()));
                }

                node.Children.Add(fieldNode);
            }

            return node;
        }

        private IEnumerable<PropertyInfo> GetTreeViewFields(UObject obj)
        {
            Type type = obj.GetType();
            foreach (var field in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (field.IsDefined(typeof(TreeNodeFieldAttribute)))
                    yield return field;
            }
        }

        private IEnumerable<PropertyInfo> GetPropertyFields(UObject obj)
        {
            Type type = obj.GetType();
            foreach (var field in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (field.IsDefined(typeof(PropertyFieldAttribute)) && field.CanWrite)
                    yield return field;
            }
        }

        private string GetTypeName(Type type)
        {
            if (type.IsGenericType)
            {
                string mainType = type.Name.Split('`')[0];
                var args = type.GetGenericArguments();
                return $"{mainType}<{string.Join(", ", args.Select(GetTypeName))}>";
            }
            return type.Name;
        }

        public virtual void ReadBuffer(UBuffer buffer)
        {
            NetIndex = buffer.Reader.ReadInt32();
            if (!buffer.IsAbstractClass)
            {
                ReadProperties(buffer);
                SetProperties();
            }
        }

        private void SetProperties()
        {
            foreach (var prop in GetPropertyFields(this))
            {
                object value = GetPropertyObjectValue(prop.Name);
                if (value == null) continue;
                var targetType = prop.PropertyType;

                if (targetType.IsEnum && value is string str)
                {
                    if (Enum.TryParse(prop.PropertyType, str, ignoreCase: true, out var enumValue))
                        prop.SetValue(this, enumValue);
                }
                else if (value is object[] objArray && targetType.IsArray)
                {
                    var elementType = targetType.GetElementType();
                    if (elementType != null)
                    {
                        Array typedArray = Array.CreateInstance(elementType, objArray.Length);
                        for (int i = 0; i < objArray.Length; i++)
                        {
                            var element = objArray[i];
                            if (elementType.IsInstanceOfType(element))
                                typedArray.SetValue(element, i);
                            else
                            {
                                var converted = TryChangeType(element, elementType);
                                if (converted != null)
                                    typedArray.SetValue(converted, i);
                            }
                        }
                        prop.SetValue(this, typedArray);
                    }
                }
                else if (targetType.IsInstanceOfType(value))
                {
                    prop.SetValue(this, value);
                }
                else
                {
                    var converted = TryChangeType(value, targetType);
                    if (converted != null)
                        prop.SetValue(this, converted);
                }
            }
        }

        private static object TryChangeType(object value, Type targetType)
        {
            try
            {
                return Convert.ChangeType(value, targetType);
            }
            catch
            {
                return null;
            }
        }

        private void ReadProperties(UBuffer buffer)
        {
            ResultProperty result;
            while (true)
            {
                var property = new UnrealProperty();
                result = buffer.ReadProperty(property);
                if (result != ResultProperty.Success)
                {                    
                    buffer.SetDataOffset();
                    break;
                }
                Properties.Add(property);
            }
            buffer.ResultProperty = result;
        }

        public UnrealProperty GetProperty(string name)
        {
            return Properties.FirstOrDefault(p => p.NameIndex.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }

        public UnrealPropertyValueBase GetPropertyValue(string name)
        {
            return GetProperty(name)?.Value;
        }

        public object GetPropertyObjectValue(string name)
        {
            var value = GetPropertyValue(name);
            return value != null ? ExtractValue(value) : null;
        }

        private object[] GetValueArray(UnrealPropertyValueBase[] array)
        {
            return [.. array.Select(ExtractValue)];
        }

        private object ExtractValue(UnrealPropertyValueBase value)
        {
            return value switch
            {
                UnrealPropertyByteValue b => b.EnumValue,
                UnrealPropertyIntValue i => i.PropertyValue,
                UnrealPropertyFloatValue f => f.PropertyValue,
                UnrealPropertyBoolValue bo => bo.PropertyValue,
                UnrealPropertyNameValue n => n.PropertyString,
                UnrealPropertyStringValue s => s.PropertyString,
                UnrealPropertyStructValue sv => sv.StructValue,
                UnrealPropertyArrayValue av => GetValueArray(av.Array),
                _ => null
            };
        }

        public TEnum? GetPropertyEnum<TEnum>(string name) where TEnum : struct, Enum
        {
            if (GetPropertyValue(name) is UnrealPropertyByteValue byteValue)
            {
                string enumValueStr = byteValue.EnumValue;
                if (!string.IsNullOrEmpty(enumValueStr) && Enum.TryParse(enumValueStr, true, out TEnum parsed))
                    return parsed;
            }

            return null;
        }

        /*
        public override int GetBuilderSize()
        {
            BuilderSize = sizeof(int);

            return BuilderSize;
        }

        public override Task WriteBuffer(ByteArrayWriter Writer, int CurrentOffset)
        {
            Writer.WriteInt32(NetIndex);
            return Task.CompletedTask;
        }*/
    }
}
