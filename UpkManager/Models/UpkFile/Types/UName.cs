using UpkManager.Models.UpkFile.Tables;

namespace UpkManager.Models.UpkFile.Types
{
    public class UName : FName
    {
        public static UName ReadName(UBuffer buffer)
        {
            UName key = new();
            key.ReadNameTableIndex(buffer.Reader, buffer.Header);
            return key;
        }
    }
}
