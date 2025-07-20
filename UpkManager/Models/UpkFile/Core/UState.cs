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

    [UnrealClass("State")]
    public class UState : UStruct
    {
        [TreeNodeField]
        public uint ProbeMask { get; private set; }

        [TreeNodeField]
        public ushort LabelTableOffset { get; private set; }

        [TreeNodeField("UStateFlags")]
        public UStateFlags StateFlags { get; private set; } // UStateFlags 

        [TreeNodeField("UMap<UName, UFunction>")]
        public UMap<UName, FName> FuncMap { get; private set; } // UMap<UName, UFunction>

        public override void ReadBuffer(UBuffer buffer)
        {
            base.ReadBuffer(buffer);

            ProbeMask = buffer.Reader.ReadUInt32();
            LabelTableOffset = buffer.Reader.ReadUInt16();
            StateFlags = (UStateFlags)buffer.Reader.ReadUInt32();
            FuncMap = buffer.ReadUMap();
        }
    }
}
