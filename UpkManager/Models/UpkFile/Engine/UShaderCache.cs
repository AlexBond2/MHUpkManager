using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Engine
{
    [UnrealClass("ShaderCache")]
    public class UShaderCache : UObject
    {
        [StructField]
        public int ShaderCachePriority { get; set; }

        [StructField]
        public EShaderPlatform Platform { get; set; }

        public override void ReadBuffer(UBuffer buffer)
        {
            base.ReadBuffer(buffer);
            ShaderCachePriority = buffer.ReadInt32();
            Platform = (EShaderPlatform)buffer.ReadByte();
            // TODO
        }
    }

    public enum EShaderPlatform
    {
        SP_PCD3D_SM3 = 0,
        SP_PS3 = 1,
        SP_XBOXD3D = 2,
        SP_PCD3D_SM4 = 3,
        SP_PCD3D_SM5 = 4,
        SP_NGP = 5,
        SP_PCOGL = 6,
        SP_WIIU = 7,

        SP_NumPlatforms = 8,
        SP_NumBits = 4,
    };
}
