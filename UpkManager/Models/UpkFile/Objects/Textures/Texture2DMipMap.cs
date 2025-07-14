
using DDSLib.Constants;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Objects.Textures
{

    public class Texture2DMipMap
    {

        #region Properties

        public int SizeX { get; set; }
        public int SizeY { get; set; }
        public FileFormat OverrideFormat { get; set; }
        public byte[] Data { get; set; } // UntypedBulkData

        public static Texture2DMipMap ReadMipMap(UBuffer buffer)
        {
            var mipMap = new Texture2DMipMap();
            mipMap.Data = buffer.ReadBulkData();
            mipMap.SizeX = buffer.Reader.ReadInt32();
            mipMap.SizeY = buffer.Reader.ReadInt32();
            return mipMap;
        }
        public override string ToString()
        {
            return $"[{SizeX} x {SizeY}] [{Data?.Length}]";
        }

        #endregion Properties

    }

}
