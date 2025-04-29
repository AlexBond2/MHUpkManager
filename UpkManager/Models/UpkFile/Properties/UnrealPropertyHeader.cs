using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UpkManager.Helpers;


namespace UpkManager.Models.UpkFile.Properties
{

    public sealed class UnrealPropertyHeader : UnrealUpkBuilderBase
    {

        #region Constructor

        internal UnrealPropertyHeader()
        {
            Properties = new List<UnrealProperty>();
        }

        #endregion Constructor

        #region Properties

        public int TypeIndex { get; private set; }

        public List<UnrealProperty> Properties { get; }

        public ResultProperty Result {  get; private set; }
        public int RemainingData { get; private set; }

        #endregion Properties

        #region Unreal Methods

        internal async Task ReadPropertyHeader(ByteArrayReader reader, UnrealHeader header)
        {
            try
            {
                TypeIndex = reader.ReadInt32();
                Result = ResultProperty.Success;
                do
                {
                    var property = new UnrealProperty();
                    try
                    {
                        Result = await property.ReadProperty(reader, header);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error reading property: {ex.Message}");
                        Result = ResultProperty.Error;
                        RemainingData = reader.Remaining;
                        return;
                    }

                    if (Result != ResultProperty.Success) break;

                    Properties.Add(property);
                }
                while (Result == ResultProperty.Success);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading property header: {ex.Message}");
                Result = ResultProperty.Error;
            }

            RemainingData = reader.Remaining;
        }

        internal List<UnrealProperty> GetProperty(string name)
        {
            return Properties.Where(p => p.NameIndex.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }

        #endregion Unreal Methods

        #region UnrealUpkBuilderBase Implementation

        public override int GetBuilderSize()
        {
            BuilderSize = sizeof(int)
                        + Properties.Sum(p => p.GetBuilderSize());

            return BuilderSize;
        }

        public override async Task WriteBuffer(ByteArrayWriter Writer, int CurrentOffset)
        {
            Writer.WriteInt32(TypeIndex);

            foreach (UnrealProperty property in Properties) await property.WriteBuffer(Writer, CurrentOffset);
        }

        #endregion UnrealUpkBuilderBase Implementation

    }

}
