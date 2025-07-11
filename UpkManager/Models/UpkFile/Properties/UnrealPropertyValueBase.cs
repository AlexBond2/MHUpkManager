﻿using System.Threading.Tasks;

using UpkManager.Constants;
using UpkManager.Helpers;


namespace UpkManager.Models.UpkFile.Properties
{

    public class UnrealPropertyValueBase : UnrealUpkBuilderBase
    {

        #region Properties

        protected ByteArrayReader DataReader { get; set; }

        #endregion Properties

        #region Unreal Properties

        public virtual PropertyTypes PropertyType => PropertyTypes.UnknownProperty;

        public virtual object PropertyValue => DataReader.GetBytes();

        public virtual string PropertyString => $"{DataReader.GetBytes().Length:N0} Bytes of Data";

        public VirtualNode VirtualTree { get => GetVirtualTree(); }

        protected virtual VirtualNode GetVirtualTree()
        {
            return new(ToString());
        }

        public virtual void BuildVirtualTree(VirtualNode valueTree)
        {
            valueTree.Children.Add(new(PropertyString));
        }

        public override string ToString()
        {
            return PropertyString;
        }

        #endregion Unreal Properties

        #region Unreal Methods

        public virtual void ReadPropertyValue(ByteArrayReader reader, int size, UnrealHeader header, UnrealProperty property)
        {
            DataReader = reader.ReadByteArray(size);
        }

        public virtual void SetPropertyValue(object value)
        {
            ByteArrayReader reader = value as ByteArrayReader;

            if (reader == null) return;

            DataReader = reader;
        }

        #endregion Unreal Methods

        #region UnrealUpkBuilderBase Implementation

        public override int GetBuilderSize()
        {
            BuilderSize = DataReader?.GetBytes().Length ?? 0;

            return BuilderSize;
        }

        public override async Task WriteBuffer(ByteArrayWriter Writer, int CurrentOffset)
        {
            await Writer.WriteBytes(DataReader?.GetBytes());
        }

        #endregion UnrealUpkBuilderBase Implementation

    }

}
