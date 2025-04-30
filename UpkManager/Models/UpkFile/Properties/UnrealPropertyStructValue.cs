using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using UpkManager.Constants;
using UpkManager.Helpers;
using UpkManager.Models.UpkFile.Tables;


namespace UpkManager.Models.UpkFile.Properties
{
    public enum PropertyCollectionStruct
    {
        ColorMaterialInput,
        ScalarMaterialInput,
        RawDistributionVector,
        RawDistributionFloat
    }

    public sealed class UnrealPropertyStructValue : UnrealPropertyValueBase
    {

        #region Constructor

        public UnrealPropertyStructValue()
        {
            StructNameIndex = new UnrealNameTableIndex();
            PropertyCollection = [];
        }

        #endregion Constructor

        #region Properties
        private List<UnrealProperty> PropertyCollection { get; set; }
        public UnrealNameTableIndex StructNameIndex { get; set; }

        #endregion Properties

        #region Unreal Properties
        public ResultProperty Result { get; private set; }
        public int RemainingData { get; private set; }
        public override PropertyTypes PropertyType => PropertyTypes.StructProperty;

        public override string PropertyString => StructNameIndex.Name;

        #endregion Unreal Properties

        #region Unreal Methods

        protected override VirtualNode GetVirtualTree()
        {
            var valueTree = base.GetVirtualTree();
            var structType = StructNameIndex.Name;
            if (PropertyType == PropertyTypes.StructProperty)
            {
                if (structType == "vector")
                {
                    byte[] data = DataReader.GetBytes();
                    
                    float x = BitConverter.ToSingle(data, 0);
                    float y = BitConverter.ToSingle(data, 4);
                    float z = BitConverter.ToSingle(data, 8);

                    valueTree.Children.Add(new ($"[{x:F4}; {y:F4}; {z:F4}]"));                    
                }

                if (structType == "rotator")
                {
                    byte[] data = DataReader.GetBytes();

                    float pitch = BitConverter.ToInt32(data, 0) / 32768.0f * 180.0f;
                    float yaw = BitConverter.ToInt32(data, 4) / 32768.0f * 180.0f;
                    float roll = BitConverter.ToInt32(data, 8) / 32768.0f * 180.0f;

                    valueTree.Children.Add(new($"[{pitch:F4}; {yaw:F4}; {roll:F4}]"));
                }

                if (structType == "guid")
                {
                    byte[] data = DataReader.GetBytes();

                    var guid = new Guid(data);

                    valueTree.Children.Add(new($"{guid}"));
                }

                if (Enum.TryParse(structType, true, out PropertyCollectionStruct type))
                {
                    foreach (var prop in PropertyCollection)
                        valueTree.Children.Add(prop.VirtualTree);

                    if (Result != ResultProperty.None || RemainingData != 0)
                        valueTree.Children.Add(new ($"Data [{Result}][{RemainingData}]"));
                }

            }

            return valueTree;
        }

        public override async Task ReadPropertyValue(ByteArrayReader reader, int size, UnrealHeader header, UnrealProperty property)
        {
            await Task.Run(() => StructNameIndex.ReadNameTableIndex(reader, header));
            int offset = reader.CurrentOffset;
            var structType = StructNameIndex.Name;
            if (Enum.TryParse(structType, true, out PropertyCollectionStruct type))
            {
                PropertyCollection.Clear();
                Result = ResultProperty.Success;

                do
                {
                    var prop = new UnrealProperty();
                    try
                    {
                        Result = await prop.ReadProperty(reader, header);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error reading property: {ex.Message}");
                        Result = ResultProperty.Error;
                        RemainingData = size - (reader.CurrentOffset - offset);
                        return;
                    }

                    if (Result != ResultProperty.Success) break;

                    PropertyCollection.Add(prop);
                }
                while (Result == ResultProperty.Success);

                RemainingData = size - (reader.CurrentOffset - offset);
                return;
            }

            await base.ReadPropertyValue(reader, size, header, property);
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
