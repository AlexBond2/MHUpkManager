using System;
using System.Threading.Tasks;

using UpkManager.Constants;
using UpkManager.Helpers;
using UpkManager.Models.UpkFile.Tables;


namespace UpkManager.Models.UpkFile.Properties
{

    public sealed class UnrealPropertyStructValue : UnrealPropertyValueBase
    {

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

        #endregion Unreal Properties

        #region Unreal Methods

        protected override VirtualNode GetVirtualTree()
        {
            var valueTree = base.GetVirtualTree();

            if (PropertyType == PropertyTypes.StructProperty)
            {
                if (PropertyString == "vector")
                {
                    byte[] data = DataReader.GetBytes();
                    
                    float x = BitConverter.ToSingle(data, 0);
                    float y = BitConverter.ToSingle(data, 4);
                    float z = BitConverter.ToSingle(data, 8);

                    valueTree.Children.Add(new ($"[{x:F4}; {y:F4}; {z:F4}]"));                    
                }
            }

            return valueTree;
        }

        public override async Task ReadPropertyValue(ByteArrayReader reader, int size, UnrealHeader header)
        {
            await Task.Run(() => StructNameIndex.ReadNameTableIndex(reader, header));

            await base.ReadPropertyValue(reader, size, header);
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
