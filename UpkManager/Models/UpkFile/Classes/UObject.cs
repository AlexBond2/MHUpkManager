using System.Threading.Tasks;

using UpkManager.Helpers;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Classes
{
    public class UObject : UnrealUpkBuilderBase
    {
        public int NetIndex { get; private set; } = -1;

        public virtual void ReadBuffer(UBuffer buffer)
        {
            NetIndex = buffer.Reader.ReadInt32();
        }

        public override int GetBuilderSize()
        {
            BuilderSize = sizeof(int);

            return BuilderSize;
        }

        public override Task WriteBuffer(ByteArrayWriter Writer, int CurrentOffset)
        {
            Writer.WriteInt32(NetIndex);
            return Task.CompletedTask;
        }
    }
}
