using System;
using System.Collections.Generic;

using UpkManager.Helpers;

namespace UpkManager.Models.UpkFile.Properties
{
    public class UnrealPropertyEngineValue : UnrealPropertyValueBase
    {
        #region Constructor

        public UnrealPropertyEngineValue(string structType)
        {
            StructType = structType;
            Fields = [];
        }

        #endregion Constructor

        #region Properties

        public string StructType { get; private set; }
        public List<UnrealProperty> Fields { get; set; }
        public ResultProperty Result { get; private set; }
        public int RemainingData { get; private set; }

        #endregion Properties

        #region Unreal Methods

        public override string ToString()
        {
            return StructType;
        }

        public override void BuildVirtualTree(VirtualNode valueTree)
        {
            foreach (var prop in Fields)
                valueTree.Children.Add(prop.VirtualTree);

            if (Result != ResultProperty.None || RemainingData > 0)
                valueTree.Children.Add(new($"Data [{Result}][{RemainingData}]"));
        }

        public override void ReadPropertyValue(ByteArrayReader reader, int size, UnrealHeader header, UnrealProperty property)
        {
            int offset = reader.CurrentOffset;
            Fields.Clear();
            Result = ResultProperty.Success;

            do
            {
                var prop = new UnrealProperty();
                try
                {
                    Result = prop.ReadProperty(reader, header);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error reading property: {ex.Message}");
                    Result = ResultProperty.Error;
                    RemainingData = size - (reader.CurrentOffset - offset);
                    return;
                }

                if (Result != ResultProperty.Success) break;

                Fields.Add(prop);
            }
            while (Result == ResultProperty.Success);

            RemainingData = size - (reader.CurrentOffset - offset);
        }

        #endregion Unreal Methods
    }
}
