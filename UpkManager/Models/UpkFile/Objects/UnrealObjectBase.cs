using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using UpkManager.Constants;
using UpkManager.Helpers;
using UpkManager.Models.UpkFile.Properties;
using UpkManager.Models.UpkFile.Tables;


namespace UpkManager.Models.UpkFile.Objects
{
    [Flags]
    public enum EObjectFlags : ulong
    {
        InSingularFunc  = 1U << 1,  // 0x00000002
        FinishDestroy   = 1U << 5,  // 0x00000020   UObject::ConditionalFinishDestroy
        Final           = 1U << 7,  // 0x00000080
        Protected       = 1U << 8,  // 0x00000100 
        PropertiesObject = 1U << 9, // 0x00000200
        ArchetypeObject = 1U << 10, // 0x00000400
        TagForcedExport = 1U << 11, // 0x00000800

        // https://github.com/stephank/surreal/blob/master/Core/Inc/UnObjBas.h

        Transactional   = 1UL << 32, // 0x00000001  Object is transactional.
        Unreachable     = 1UL << 33, // 0x00000002  Object is not reachable on the object graph.
        Public          = 1UL << 34, // 0x00000004  Object is visible outside its package.
        TagImp          = 1UL << 35, // 0x00000008  Temporary import tag in load/save.
        TagExp          = 1UL << 36, // 0x00000010	Temporary export tag in load/save.
        SourceModified  = 1UL << 37, // 0x00000020  Modified relative to source files.
        TagGarbage      = 1UL << 38, // 0x00000040	Check during garbage collection.

        f39             = 1UL << 39, // 0x00000080 
        f40             = 1UL << 40, // 0x00000100

        NeedLoad        = 1UL << 41, // 0x00000200  During load, indicates object needs loading.

        f42             = 1UL << 42, // 0x00000400   
        f43             = 1UL << 43, // 0x00000800   

        Suppress        = 1UL << 44, // 0x00001000	UnName.h. Suppressed log name.
        InEndState      = 1UL << 45, // 0x00002000  Within an EndState call.
        Transient       = 1UL << 46, // 0x00004000	Don't save object.
        PreLoading      = 1UL << 47, // 0x00008000  Data is being preloaded from file.
        LoadForClient   = 1UL << 48, // 0x00010000	In-file load for client.
        LoadForServer   = 1UL << 49, // 0x00020000	In-file load for client.
        LoadForEdit     = 1UL << 50, // 0x00040000	In-file load for client.
        Standalone      = 1UL << 51, // 0x00080000  Keep object around for editing even if unreferenced.
        NotForClient    = 1UL << 52, // 0x00100000  Don't load this object for the game client.
        NotForServer    = 1UL << 53, // 0x00200000  Don't load this object for the game server.
        NotForEdit      = 1UL << 54, // 0x00400000	Don't load this object for the editor.    
        Destroyed       = 1UL << 55, // 0x00800000	Object Destroy has already been called.
        NeedPostLoad    = 1UL << 56, // 0x01000000  Object needs to be postloaded.
        HasStack        = 1UL << 57, // 0x02000000	Has execution stack.
        Native          = 1UL << 58, // 0x04000000  Native (UClass only).
        Marked          = 1UL << 59, // 0x08000000  Marked (for debugging).
        ErrorShutdown   = 1UL << 60, // 0x10000000	ShutdownAfterError called.
        DebugPostLoad   = 1UL << 61, // 0x20000000  For debugging Serialize calls.
        DebugSerialize  = 1UL << 62, // 0x40000000  For debugging Serialize calls.
        DebugDestroy    = 1UL << 63, // 0x80000000  For debugging Destroy calls.

        ContextFlags    = NotForClient | NotForServer | NotForEdit, 
        LoadContextFlags = LoadForClient | LoadForServer | LoadForEdit, 
    }

    public class UnrealObjectBase : UnrealUpkBuilderBase
    {

        #region Constructor

        public UnrealObjectBase()
        {
            PropertyHeader = new UnrealPropertyHeader();
        }

        #endregion Constructor

        #region Properties

        public UnrealPropertyHeader PropertyHeader { get; }

        public ByteArrayReader AdditionalDataReader { get; private set; }

        #endregion Properties

        #region Unreal Properties

        public int AdditionalDataOffset { get; private set; }

        public virtual bool IsExportable => false;

        public virtual ViewableTypes Viewable => ViewableTypes.Unknown;

        public virtual ObjectTypes ObjectType => ObjectTypes.Unknown;

        public virtual string FileExtension => String.Empty;

        public virtual string FileTypeDesc => String.Empty;

        #endregion Unreal Properties

        #region Unreal Methods

        public virtual async Task ReadUnrealObject(ByteArrayReader reader, UnrealHeader header, UnrealExportTableEntry export, bool skipProperties, bool skipParse)
        {
            if (!skipProperties) await PropertyHeader.ReadPropertyHeader(reader, header);

            AdditionalDataOffset = export.SerialDataOffset + reader.CurrentOffset;

            AdditionalDataReader = await reader.Splice();
        }

        public virtual async Task SaveObject(string filename, object configuration)
        {
            await Task.CompletedTask;
        }

        public virtual async Task SetObject(string filename, List<UnrealNameTableEntry> nameTable, object configuration)
        {
            await Task.CompletedTask;
        }

        public virtual Stream GetObjectStream()
        {
            return null;
        }

        #endregion Unreal Methods

        #region UnrealUpkBuilderBase Implementation

        public override int GetBuilderSize()
        {
            BuilderSize = PropertyHeader.GetBuilderSize()
                        + AdditionalDataReader?.GetBytes().Length ?? 0;

            return BuilderSize;
        }

        public override async Task WriteBuffer(ByteArrayWriter Writer, int CurrentOffset)
        {
            await PropertyHeader.WriteBuffer(Writer, CurrentOffset);

            await Writer.WriteBytes(AdditionalDataReader?.GetBytes());
        }

        #endregion UnrealUpkBuilderBase Implementation

    }

}
