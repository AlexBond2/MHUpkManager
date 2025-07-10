using System.Threading.Tasks;

using UpkManager.Helpers;


namespace UpkManager.Models.UpkFile.Compression
{

    public class UnrealCompressedChunk
    {

        #region Properties

        public int UncompressedOffset { get; protected set; }
        public int UncompressedSize { get; protected set; }
        public int CompressedOffset { get; protected set; }
        public int CompressedSize { get; protected set; }

        public UnrealCompressedChunkHeader Header { get; protected set; }

        #endregion Properties

        #region Unreal Methods

        public virtual void ReadCompressedChunk(ByteArrayReader reader)
        {
            UncompressedOffset = reader.ReadInt32();
            UncompressedSize = reader.ReadInt32();

            CompressedOffset = reader.ReadInt32();
            CompressedSize = reader.ReadInt32();

            Header = new UnrealCompressedChunkHeader();

            Header.ReadCompressedChunkHeader(reader.Branch(CompressedOffset), 1, UncompressedSize, CompressedSize);
        }

        #endregion Unreal Methods

    }

}

