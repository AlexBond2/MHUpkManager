using System;
using System.Threading.Tasks;

using UpkManager.Constants;
using UpkManager.Helpers;
using UpkManager.Models.UpkFile.Core;
using UpkManager.Models.UpkFile.Engine;
using UpkManager.Models.UpkFile.Tables;


namespace UpkManager.Models.UpkFile.Properties
{
    public sealed class UnrealPropertyStructValue : UnrealPropertyValueBase
    {
        public enum UStructTypes
        {
            Vector,
            Rotator,
            Guid,
            Box
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
        public UnrealPropertyValueBase StructValue { get; private set; }

        #endregion Unreal Properties

        #region Unreal Methods

        protected override VirtualNode GetVirtualTree()
        {
            var valueTree = base.GetVirtualTree();

            StructValue?.BuildVirtualTree(valueTree);

            return valueTree;
        }

        public override void ReadPropertyValue(ByteArrayReader reader, int size, UnrealHeader header, UnrealProperty property)
        {
            StructNameIndex.ReadNameTableIndex(reader, header);

            var structType = StructNameIndex.Name;
            if (EngineRegistry.TryGetStruct(structType, out var type))
            {
                StructValue = new UnrealPropertyEngineValue(type);
                StructValue.ReadPropertyValue(reader, size, header, property);                
            } 
            else if (CoreRegistry.Instance.TryGetProperty(structType, out var prop))
            {
                StructValue = new UnrealPropertyCoreValue(prop);
                StructValue.ReadPropertyValue(reader, size, header, property);
            }
            else
            {
                base.ReadPropertyValue(reader, size, header, property);
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
