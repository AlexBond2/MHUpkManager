using System.Threading.Tasks;

using UpkManager.Constants;
using UpkManager.Helpers;

namespace UpkManager.Models.UpkFile.Properties
{
    public class UnrealPropertyBoxValue : UnrealPropertyValueBase
    {
        public UnrealPropertyVectorValue Min { get; }
        public UnrealPropertyVectorValue Max { get; }
        public UnrealPropertyBoolValue IsValid { get; }

        public UnrealPropertyBoxValue()
        {
            Min = new();
            Max = new();
            IsValid = new();
        }

        public override PropertyTypes PropertyType => PropertyTypes.VectorPrioperty;

        public override void BuildVirtualTree(VirtualNode valueTree)
        {
            VirtualNode node = new($"Min ::Vector");
            Min.BuildVirtualTree(node);
            valueTree.Children.Add(node); 

            node = new($"Max ::Vector");
            Max.BuildVirtualTree(node);
            valueTree.Children.Add(node); 

            node = new($"IsValid ::BoolProperty");
            IsValid.BuildVirtualTree(node);
            valueTree.Children.Add(node);
        }

        public override void ReadPropertyValue(ByteArrayReader reader, int size, UnrealHeader header, UnrealProperty property)
        {
            Min.ReadPropertyValue(reader, size, header, property);
            Max.ReadPropertyValue(reader, size, header, property);
            IsValid.ReadPropertyValue(reader, size, header, property);
        }
    }
}
