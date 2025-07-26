using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UpkManager.Models.UpkFile.Core;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Classes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class StructFieldAttribute(string typeName = null, bool skip = false) : Attribute
    {
        public string TypeName { get; } = typeName;
        public bool Skip { get; } = skip;
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

    [AttributeUsage(AttributeTargets.Class)]
    public class UnrealStructAttribute(string structName) : Attribute
    {
        public string StructName { get; } = structName;
    }

    [UnrealClass("Object")]
    public class UObject// : UnrealUpkBuilderBase
    {
        [StructField]
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
                var fieldNode = BuildPropVirtualTree(prop); 
                node.Children.Add(fieldNode);
            }

            return node;
        }

        private VirtualNode BuildPropVirtualTree(PropertyInfo prop)
        {
            var attr = prop.GetCustomAttribute<StructFieldAttribute>();
            bool skip = attr.Skip;
            string displayName = prop.Name;
            string typeName = attr.TypeName ?? GetTypeName(prop.PropertyType);

            var fieldNode = new VirtualNode($"{displayName} ::{typeName}");

            object value = prop.GetValue(this);
            if (value == null)
            {
                fieldNode.Children.Add(new("null"));
            }
            else if (value is IEnumerable enumerable && value is not string)
            {
                fieldNode.Text += "[]";
                var arrayNode = CoreProperty.BuildArrayVirtualTree(typeName, enumerable, skip);
                fieldNode.Children.Add(arrayNode);
            }
            else if (value is IAtomicStruct atomic)
            {
                CoreProperty.BuildStructVirtualTree(fieldNode, atomic);
            }
            else
            {
                fieldNode.Children.Add(new(value.ToString()));
            }

            return fieldNode;
        }

        public static IEnumerable<PropertyInfo> GetTreeViewFields(object obj)
        {
            Type type = obj.GetType();
            foreach (var field in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (field.IsDefined(typeof(StructFieldAttribute)))
                    yield return field;
            }
        }

        public static IEnumerable<PropertyInfo> GetPropertyFields(object obj)
        {
            Type type = obj.GetType();
            foreach (var field in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (field.IsDefined(typeof(PropertyFieldAttribute)) && field.CanWrite)
                    yield return field;
            }
        }

        public static IEnumerable<PropertyInfo> GetStructFields(object obj)
        {
            Type type = obj.GetType();
            foreach (var field in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (field.IsDefined(typeof(StructFieldAttribute)) && field.CanWrite)
                    yield return field;
            }
        }

        private string GetTypeName(Type type)
        {
            var atomicAttr = type.GetCustomAttribute<AtomicStructAttribute>();
            if (atomicAttr != null)
                return atomicAttr.Name;

            if (type.IsGenericType)
            {
                string mainType = type.Name.Split('`')[0];
                var args = type.GetGenericArguments();
                return $"{mainType}<{string.Join(", ", args.Select(GetTypeName))}>";
            }

            return type.Name switch
            {
                "Single" => "Float",
                "Int32" => "Int",
                _ => type.Name
            };
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
                AssignValue(prop, this, value);
            }
        }

        private static void AssignValue(PropertyInfo prop, object target, object value)
        {
            if (value == null) return;

            var targetType = prop.PropertyType;

            if (targetType.IsEnum && value is string str)
            {
                if (Enum.TryParse(targetType, str, ignoreCase: true, out var enumValue))
                    prop.SetValue(target, enumValue);
            }
            else if (value is object[] objArray)
            {
                AssignArrayValues(prop, target, objArray, targetType);
            }
            else if (value is CoreProperty coreProp && typeof(IAtomicStruct).IsAssignableFrom(targetType))
            {
                if (targetType.IsInstanceOfType(coreProp.Atomic))
                    prop.SetValue(target, coreProp.Atomic);
            }
            else if (value is EngineProperty engineProp) 
            { 
                var structObj = ReconstructStructure(engineProp.StructType, engineProp.Fields);
                if (structObj != null && targetType.IsInstanceOfType(structObj))
                    prop.SetValue(target, structObj);
            }
            else if (targetType.IsInstanceOfType(value))
            {
                prop.SetValue(target, value);
            }
            else
            {
                var converted = TryChangeType(value, targetType);
                if (converted != null)
                    prop.SetValue(target, converted);
            }
        }

        private static void SetStructProperties(object unrealStruct, List<UnrealProperty> properties)
        {
            foreach (var prop in GetStructFields(unrealStruct))
            {
                object value = GetPropertyObjectValue(properties, prop.Name);
                AssignValue(prop, unrealStruct, value);
            }
        }

        public static bool AssignArrayValues(PropertyInfo prop, object target, object[] objArray, Type targetType)
        {
            if (targetType.IsArray)
            {
                var elementType = targetType.GetElementType();
                if (elementType == null) return false;

                Array typedArray = Array.CreateInstance(elementType, objArray.Length);
                for (int i = 0; i < objArray.Length; i++)
                    typedArray.SetValue(TryChangeType(objArray[i], elementType), i);

                prop.SetValue(target, typedArray);
                return true;
            }

            if (targetType.IsGenericType)
            {
                var genericDef = targetType.GetGenericTypeDefinition();

                if (genericDef == typeof(UArray<>))
                {
                    var elementType = targetType.GetGenericArguments()[0];
                    var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));

                    foreach (var item in objArray)
                        list.Add(TryChangeType(item, elementType));

                    var uArrayInstance = Activator.CreateInstance(targetType, list);
                    prop.SetValue(target, uArrayInstance);
                    return true;
                }
            }

            return false;
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
                result = buffer.ReadProperty(property, this);
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

        public object GetPropertyObjectValue(string name)
        {
            var value = GetProperty(name)?.Value;
            return value != null ? ExtractValue(value) : null;
        }

        public static object GetPropertyObjectValue(List<UnrealProperty> properties, string name)
        {
            var value = properties.FirstOrDefault(p => p.NameIndex.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))?.Value;
            return value != null ? ExtractValue(value) : null;
        }

        private static object[] GetValueArray(UProperty[] array)
        {
            return [.. array.Select(ExtractValue)];
        }

        public static object ReconstructStructure(Type structType, List<UnrealProperty> fields)
        {
            if (structType == null) return null;
            //   throw new InvalidOperationException("StructType is not set.");

            object instance = Activator.CreateInstance(structType);
            if (instance == null) return null;
            SetStructProperties(instance, fields);

            return instance;
        }

        private static object ExtractValue(UProperty value)
        {
            return value switch
            {
                UByteProperty b => b.EnumValue,
                UIntProperty i => i.PropertyValue,
                UFloatProperty f => f.PropertyValue,
                UBoolProperty bo => bo.PropertyValue,
                UNameProperty n => n.PropertyValue,
                UStrProperty s => s.PropertyString,
                UStructProperty sv => sv.StructValue,
                UObjectProperty o => o.Object,
                CoreProperty core => core.Atomic,
                EngineProperty eng => ReconstructStructure(eng.StructType, eng.Fields),
                UArrayProperty av => GetValueArray(av.Array),
                _ => null
            };
        }
    }
}
