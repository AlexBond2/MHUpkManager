using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UpkManager.Helpers;

namespace UpkManager.Models.UpkFile.Properties
{

    // https://github.com/mtz007/PUBG-SDK/blob/master/SDK/PUBG_Engine_structs.hpp

    public enum CustomPropertyStruct
    {
        ColorMaterialInput,
        ScalarMaterialInput,
        RawDistributionVector,
        RawDistributionFloat,
        FAnimNotifyEvent,
        FSkeletalMeshLODInfo,
        FTriangleSortSettings,
        FScalarParameterValue,
        FTextureParameterValue
    }

    public class UnrealPropertyStructFields
    {
        #region Constructor

        public UnrealPropertyStructFields(CustomPropertyStruct type)
        {
            StructType = type;
            Fields = [];
        }

        #endregion Constructor

        #region Properties

        public CustomPropertyStruct StructType { get; private set; }
        public List<UnrealProperty> Fields { get; set; }
        public ResultProperty Result { get; private set; }
        public int RemainingData { get; private set; }

        #endregion Properties

        #region Unreal Methods

        public override string ToString()
        {
            return StructType.ToString();
        }

        public void BuildVirtualTree(VirtualNode valueTree)
        {
            foreach (var prop in Fields)
                valueTree.Children.Add(prop.VirtualTree);

            if (Result != ResultProperty.None || RemainingData > 0)
                valueTree.Children.Add(new($"Data [{Result}][{RemainingData}]"));
        }

        public static bool CastCustomStruct(string structType, out CustomPropertyStruct type)
        {
            return Enum.TryParse(structType, true, out type);
        }

        public async Task ReadPropertyValue(ByteArrayReader reader, int size, UnrealHeader header)
        {
            int offset = reader.CurrentOffset;
            Fields.Clear();
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

                Fields.Add(prop);
            }
            while (Result == ResultProperty.Success);

            RemainingData = size - (reader.CurrentOffset - offset);
        }

        #endregion Unreal Methods
    }
}
