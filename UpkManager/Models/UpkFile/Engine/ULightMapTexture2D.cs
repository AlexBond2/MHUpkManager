using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Engine
{
    public enum ELightMapFlags
    {
        LMF_None = 0,
        LMF_Streamed = 0x00000001,
    }

    [UnrealClass("LightMapTexture2D")]
    public class ULightMapTexture2D : UTexture2D
    {
        [StructField]
        public ELightMapFlags LightMapFlags { get; set; }

        public override void ReadBuffer(UBuffer buffer)
        {
            base.ReadBuffer(buffer);

            LightMapFlags = (ELightMapFlags)buffer.Reader.ReadUInt32();
        }
    }
}
