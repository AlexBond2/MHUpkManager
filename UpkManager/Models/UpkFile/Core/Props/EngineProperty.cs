using System;
using System.Collections.Generic;
using System.Reflection;

using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Engine;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Core
{
    public class EngineProperty : UProperty
    {
        public Type StructType { get; private set; }
        public string StructName { get; private set; }
        public List<UnrealProperty> Fields { get; set; } = [];
        public ResultProperty Result { get; private set; }
        public int RemainingData { get; private set; }

        public override string ToString() => StructName;

        public EngineProperty(Type type)
        {
            StructType = type;
            var attr = type.GetCustomAttribute<UnrealStructAttribute>();
            StructName =  attr?.StructName ?? type.Name;
        }

        public override void BuildVirtualTree(VirtualNode valueTree)
        {
            foreach (var prop in Fields)
                valueTree.Children.Add(prop.VirtualTree);

            if (Result != ResultProperty.None || RemainingData > 0)
                valueTree.Children.Add(new($"Data [{Result}][{RemainingData}]"));
        }

        public override void ReadPropertyValue(UBuffer buffer, int size, UnrealProperty property)
        {
            int offset = buffer.Reader.CurrentOffset;
            Fields.Clear();
            Result = ResultProperty.Success;

            do
            {
                var prop = new UnrealProperty();
                try
                {
                    Result = prop.ReadProperty(buffer, Parent);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error reading property: {ex.Message}");
                    Result = ResultProperty.Error;
                    RemainingData = size - (buffer.Reader.CurrentOffset - offset);
                    return;
                }

                if (Result != ResultProperty.Success) break;

                Fields.Add(prop);
            }
            while (Result == ResultProperty.Success);

            RemainingData = size - (buffer.Reader.CurrentOffset - offset);
        }
    }
}
