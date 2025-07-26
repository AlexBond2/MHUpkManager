using DDSLib.Constants;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Engine.Texture
{
    public class FTexture2DMipMap
    {
        public int SizeX { get; set; }
        public int SizeY { get; set; }
        public FileFormat OverrideFormat { get; set; }
        public byte[] Data { get; set; } // UntypedBulkData

        public static FTexture2DMipMap ReadMipMap(UBuffer buffer)
        {
            var mipMap = new FTexture2DMipMap
            {
                Data = buffer.ReadBulkData(),
                SizeX = buffer.Reader.ReadInt32(),
                SizeY = buffer.Reader.ReadInt32()
            };
            return mipMap;
        }
        public override string ToString()
        {
            return $"[{SizeX} x {SizeY}] [{Data?.Length}]";
        }
    }
}
