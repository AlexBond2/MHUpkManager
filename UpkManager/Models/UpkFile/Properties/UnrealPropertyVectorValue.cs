using System.Threading.Tasks;

using UpkManager.Constants;
using UpkManager.Helpers;

namespace UpkManager.Models.UpkFile.Properties
{
    public class UnrealPropertyVectorValue : UnrealPropertyValueBase
    {
        public UnrealPropertyFloatValue X { get; }
        public UnrealPropertyFloatValue Y { get; }
        public UnrealPropertyFloatValue Z { get; }

        public UnrealPropertyVectorValue() 
        {
            X = new ();
            Y = new ();
            Z = new ();
        }

        public override PropertyTypes PropertyType => PropertyTypes.VectorPrioperty;
        public override string PropertyString => $"[{X.PropertyValue:F4}; {Y.PropertyValue:F4}; {Z.PropertyValue:F4}]";

        public override async Task ReadPropertyValue(ByteArrayReader reader, int size, UnrealHeader header, UnrealProperty property)
        {
            await X.ReadPropertyValue(reader, size, header, property);
            await Y.ReadPropertyValue(reader, size, header, property);
            await Z.ReadPropertyValue(reader, size, header, property);
        }
    }
}
