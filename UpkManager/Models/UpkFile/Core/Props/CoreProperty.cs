using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Core
{
    public class CoreProperty : UProperty
    {
        public string StructName { get; }
        public IAtomicStruct Atomic { get; private set; }
        public Type StructType { get; }
        public List<(string Name, UProperty Value)> Fields { get; } = [];

        public CoreProperty(Type structType, UObject parent)
        {            
            Parent = parent;
            StructType = structType;
            StructName = structType.Name;
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

        public override string PropertyString => Atomic?.Format ?? StructName;

        public override void BuildVirtualTree(VirtualNode valueTree)
        {
            if (Atomic != null)
                BuildStructVirtualTree(valueTree, Atomic);
            else
                valueTree.Children.Add(new VirtualNode(StructName));
        }

        public override void ReadPropertyValue(UBuffer buffer, int size, UnrealProperty property)
        {
            var readMethod = StructType.GetMethod("ReadData", BindingFlags.Public | BindingFlags.Static);
            if (readMethod == null)
                throw new InvalidOperationException($"{StructType.Name} not have static ReadData(UBuffer)");

            var result = readMethod.Invoke(null, [buffer]);
            if (result is IAtomicStruct atomic)
            {
                Atomic = atomic;
            }
        }

        public static void BuildStructVirtualTree(VirtualNode fieldNode, IAtomicStruct atomic)
        {
            if (!string.IsNullOrEmpty(atomic.Format))
                fieldNode.Children.Add(new(atomic.Format));
            else
            {
                foreach (var field in GetStructFields(atomic))
                {
                    var attr = field.GetCustomAttribute<StructFieldAttribute>();
                    bool skip = attr.Skip;

                    string fieldName = field.Name;
                    string typeName = attr.TypeName ?? field.PropertyType.Name;

                    var structNode = new VirtualNode($"{fieldName} ::{typeName}");
                    
                    object fieldValue = field.GetValue(atomic);
                    if (fieldValue is IAtomicStruct chieldAtomic)
                    {
                        BuildStructVirtualTree(structNode, chieldAtomic);
                    }
                    else if (fieldValue is IEnumerable enumerable && fieldValue is not string)
                    {
                        structNode.Text += "[]";
                        var arrayNode = BuildArrayVirtualTree(typeName, enumerable, skip);
                        structNode.Children.Add(arrayNode);
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

        public static VirtualNode BuildArrayVirtualTree(string typeName, IEnumerable enumerable, bool skip)
        {
            var listNode = new VirtualNode();
            int count = 0;

            if (skip && enumerable is ICollection collection)
            {
                count = collection.Count;
            }
            else if (enumerable is byte[] data)
            {
                count = data.Length;
                listNode.Tag = data;
            }
            else
            {
                foreach (var item in enumerable)
                {
                    var itemNode = new VirtualNode($"[{count}] {item}");
                    if (item is IAtomicStruct atomic)
                        BuildStructVirtualTree(itemNode, atomic);

                    listNode.Children.Add(itemNode);
                    count++;
                }
            }
            listNode.Text = $"{typeName} [{count}]";
            return listNode;
        }
    }
}
