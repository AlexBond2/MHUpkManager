using System.Threading.Tasks;
using UpkManager.Constants;
using UpkManager.Helpers;


namespace UpkManager.Models.UpkFile.Properties
{

    public sealed class UnrealPropertyArrayValue : UnrealPropertyValueBase
    {
        private int _size;
        #region Properties

        public object[] Array { get; set; }
        public int Size { get; private set; }
        public override PropertyTypes PropertyType => PropertyTypes.ArrayProperty;
        public override string PropertyString => $"{_size}byte[{Size}]";

        #endregion Properties

        #region Unreal Methods

        public override async Task ReadPropertyValue(ByteArrayReader reader, int size, UnrealHeader header)
        {
            Size = await Task.Run(() => reader.ReadInt32());
            size -= 4;
            await base.ReadPropertyValue(reader, size, header);
            Array = new object[Size];
            _size = size / Size;
        }

        #endregion Unreal Methods
    }

}
