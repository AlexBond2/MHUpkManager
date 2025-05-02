using System;

using UpkManager.Models.UpkFile.Tables;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Classes
{
    [Flags]
    public enum UStateFlags : uint
    {
        Editable = 0x00000001U,
        Auto = 0x00000002U,
        Simulated = 0x00000004U,
    }

    public class UState : UStruct
    {
        public uint ProbeMask { get; private set; }
        public ushort LabelTableOffset { get; private set; }
        public uint StateFlags { get; private set; } // UStateFlags 
        public UMap<UName, UnrealNameTableIndex> FuncMap { get; private set; } // UMap<UName, UFunction>

        public override void ReadBuffer(UBuffer buffer)
        {
            base.ReadBuffer(buffer);

            ProbeMask = buffer.Reader.ReadUInt32();
            LabelTableOffset = buffer.Reader.ReadUInt16();
            StateFlags = buffer.Reader.ReadUInt32();
            FuncMap = buffer.ReadMap();
        }
    }
}
