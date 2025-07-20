

namespace UpkManager.Models.UpkFile.Tables
{

    public abstract class UnrealObjectTableEntryBase : UnrealUpkBuilderBase
    {

        #region Properties

        public int OuterReference { get; protected set; }

        public FName ObjectNameIndex { get; protected set; }

        #endregion Properties

        #region Unreal Properties

        public int TableIndex { get; set; }

        #endregion Unreal Properties

    }

}
