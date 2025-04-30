using System;
using System.Threading.Tasks;

using UpkManager.Constants;
using UpkManager.Helpers;
using UpkManager.Models.UpkFile.Tables;


namespace UpkManager.Models.UpkFile.Properties
{

    public sealed class UnrealPropertyStructValue : UnrealPropertyValueBase
    {
        public enum UStructTypes
        {
            Vector,
            Rotator,
            Guid
        }

        #region Constructor

        public UnrealPropertyStructValue()
        {
            StructNameIndex = new UnrealNameTableIndex();
        }

        #endregion Constructor

        #region Properties
        public UnrealNameTableIndex StructNameIndex { get; set; }

        #endregion Properties

        #region Unreal Properties
        public override PropertyTypes PropertyType => PropertyTypes.StructProperty;
        public override string PropertyString => StructNameIndex.Name;
        public UnrealPropertyStructFields CustomStruct { get; private set; }

        #endregion Unreal Properties

        #region Unreal Methods

        protected override VirtualNode GetVirtualTree()
        {
            var valueTree = base.GetVirtualTree();
            var structType = StructNameIndex.Name;

            if (Enum.TryParse(structType, true, out UStructTypes type))
            {
                byte[] data = DataReader.GetBytes();
                switch (type)
                {
                    case UStructTypes.Vector:

                        float x = BitConverter.ToSingle(data, 0);
                        float y = BitConverter.ToSingle(data, 4);
                        float z = BitConverter.ToSingle(data, 8);

                        valueTree.Children.Add(new($"[{x:F4}; {y:F4}; {z:F4}]"));
                        break;

                    case UStructTypes.Rotator:

                        float pitch = BitConverter.ToInt32(data, 0) / 32768.0f * 180.0f;
                        float yaw = BitConverter.ToInt32(data, 4) / 32768.0f * 180.0f;
                        float roll = BitConverter.ToInt32(data, 8) / 32768.0f * 180.0f;

                        valueTree.Children.Add(new($"[{pitch:F4}; {yaw:F4}; {roll:F4}]"));
                        break;

                    case UStructTypes.Guid:

                        var guid = new Guid(data);

                        valueTree.Children.Add(new($"{guid}"));
                        break;
                }
            }
            else
            {
                CustomStruct?.BuildVirtualTree(valueTree);
            }

            return valueTree;
        }

        public override async Task ReadPropertyValue(ByteArrayReader reader, int size, UnrealHeader header, UnrealProperty property)
        {
            await Task.Run(() => StructNameIndex.ReadNameTableIndex(reader, header));

            var structType = StructNameIndex.Name;
            if (UnrealPropertyStructFields.CastCustomStruct(structType, out CustomPropertyStruct type))
            {
                CustomStruct = new UnrealPropertyStructFields(type);
                await CustomStruct.ReadPropertyValue(reader, size, header);                
            } 
            else
            {
                await base.ReadPropertyValue(reader, size, header, property);
            }
        }

        #endregion Unreal Methods

        #region UnrealUpkBuilderBase Implementation

        public override int GetBuilderSize()
        {
            BuilderSize = StructNameIndex.GetBuilderSize()
                        + base.GetBuilderSize();

            return BuilderSize;
        }

        public override async Task WriteBuffer(ByteArrayWriter Writer, int CurrentOffset)
        {
            await StructNameIndex.WriteBuffer(Writer, CurrentOffset);

            await base.WriteBuffer(Writer, CurrentOffset);
        }

        #endregion UnrealUpkBuilderBase Implementation

    }

}
