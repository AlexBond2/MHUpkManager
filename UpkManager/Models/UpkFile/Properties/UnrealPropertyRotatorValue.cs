using System.Threading.Tasks;

using UpkManager.Constants;
using UpkManager.Helpers;

namespace UpkManager.Models.UpkFile.Properties
{
    public class UnrealPropertyRotatorValue : UnrealPropertyValueBase
    {
        public UnrealPropertyIntValue Pitch { get; }
        public UnrealPropertyIntValue Yaw { get; }
        public UnrealPropertyIntValue Roll { get; }        
        
        public UnrealPropertyRotatorValue()
        {
            Pitch = new ();
            Yaw = new ();
            Roll = new ();
        }

        public override PropertyTypes PropertyType => PropertyTypes.VectorPrioperty;
        public override string PropertyString => $"[{GetAngle(Pitch):F4}; {GetAngle(Yaw):F4}; {GetAngle(Roll):F4}]";


        private static float GetAngle(UnrealPropertyIntValue value)
        {
            return (int)value.PropertyValue / 32768.0f * 180.0f;
        }

        public override void ReadPropertyValue(ByteArrayReader reader, int size, UnrealHeader header, UnrealProperty property)
        {
            Pitch.ReadPropertyValue(reader, size, header, property);
            Yaw.ReadPropertyValue(reader, size, header, property);
            Roll.ReadPropertyValue(reader, size, header, property);
        }
    }
}
