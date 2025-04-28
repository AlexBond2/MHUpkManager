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

        #endregion Properties

        #region Unreal Methods

        internal async Task ReadPropertyHeader(ByteArrayReader reader, UnrealHeader header)
        {
            try
            {
                TypeIndex = reader.ReadInt32();
                int max = 100;
                do
                {
                    var property = new UnrealProperty();
                    try
                    {
                        await property.ReadProperty(reader, header);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error reading property: {ex.Message}");
                        return;
                    }

                    if (property.Size == 0) break;

                    Properties.Add(property);
                    max--;
                }
                while (max > 0);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading property header: {ex.Message}");
            }
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
