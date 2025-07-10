using System.Threading.Tasks;

using UpkManager.Constants;
using UpkManager.Helpers;


namespace UpkManager.Models.UpkFile.Properties
{

    public sealed class UnrealPropertyFloatValue : UnrealPropertyValueBase
    {

        #region Properties

        private float floatValue { get; set; }

        #endregion Properties

        #region Unreal Properties

        public override PropertyTypes PropertyType => PropertyTypes.FloatProperty;

        public override object PropertyValue => floatValue;

        public override string PropertyString => $"{floatValue}";

        #endregion Unreal Properties

        #region Unreal Methods

        public override void ReadPropertyValue(ByteArrayReader reader, int size, UnrealHeader header, UnrealProperty property)
        {
            floatValue = reader.ReadSingle();
        }

        #endregion Unreal Methods

        #region UnrealUpkBuilderBase Implementation

        public override int GetBuilderSize()
        {
            BuilderSize = sizeof(float);

            return BuilderSize;
        }

        public override async Task WriteBuffer(ByteArrayWriter Writer, int CurrentOffset)
        {
            await Task.Run(() => Writer.WriteSingle(floatValue));
        }

        #endregion UnrealUpkBuilderBase Implementation

    }

}
