using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using UpkManager.Helpers;
using UpkManager.Models.UpkFile.Properties;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Classes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
    public class TreeNodeFieldAttribute(string typeName = null) : Attribute
    {
        public string TypeName { get; } = typeName;
    }

    public class UObject : UnrealUpkBuilderBase
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
                    foreach (var item in enumerable)
                    {
                        listNode.Children.Add(new($"[{count}] {item}"));
                        count++;
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
                ReadProperties(buffer);
        }

        private void ReadProperties(UBuffer buffer)
        {
            while (true)
            {
                var property = new UnrealProperty();
                if (!buffer.ReadProperty(property)) break;
                Properties.Add(property);
            }
        }

        public override int GetBuilderSize()
        {
            BuilderSize = sizeof(int);

            return BuilderSize;
        }

        public override Task WriteBuffer(ByteArrayWriter Writer, int CurrentOffset)
        {
            Writer.WriteInt32(NetIndex);
            return Task.CompletedTask;
        }
    }
}
