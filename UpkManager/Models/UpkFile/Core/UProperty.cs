using System;
using UpkManager.Helpers;
using UpkManager.Models.UpkFile.Core;
using UpkManager.Models.UpkFile.Tables;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Classes
{
    [UnrealClass("Property")]
    public class UProperty(UObject parent) : UField
    {
        [StructField]
        public int ArrayDim { get; private set; }
        public int ElementSize;

        [StructField]
        public PropertyFlags PropertyFlags { get; private set; }

        [StructField]
        public UName Category { get; private set; }

        [StructField("UEnum")]
        public FName ArrayEnum { get; private set; } // UEnum

        protected ByteArrayReader DataReader { get; set; }
        public virtual object PropertyValue => DataReader.GetBytes();
        public virtual string PropertyString => $"{DataReader.GetBytes().Length:N0} Bytes of Data";
        public VirtualNode VirtualTree { get => GetVirtualTree(); }
        public UObject Parent { get; private set; } = parent;

        public override void ReadBuffer(UBuffer buffer)
        {
            base.ReadBuffer(buffer);
            ArrayDim = buffer.Reader.ReadInt32();
            ElementSize = (ushort)(ArrayDim >> 16);

            PropertyFlags = (PropertyFlags)buffer.Reader.ReadUInt64();
            Category = UName.ReadName(buffer);
            ArrayEnum = buffer.ReadObject();
        }

        protected virtual VirtualNode GetVirtualTree()
        {
            return new(ToString());
        }

        public virtual void BuildVirtualTree(VirtualNode valueTree)
        {
            valueTree.Children.Add(new(PropertyString));
        }

        public override string ToString()
        {
            return PropertyString;
        }

        public virtual void ReadPropertyValue(UBuffer buffer, int size, UnrealProperty property)
        {
            DataReader = buffer.Reader.ReadByteArray(size);
        }

        public virtual void SetPropertyValue(object value)
        {
            if (value is not ByteArrayReader reader) return;
            DataReader = reader;
        }
    }

    [Flags]
    public enum PropertyFlags : ulong
    {
        Editable = 1ul << 0,          // Can be set by UnrealEd users
        Const = 1ul << 1,             // ReadOnly
        Input = 1ul << 2,             // Can be set with binds
        ExportObject = 1ul << 3,      // Export sub-object properties to clipboard
        OptionalParm = 1ul << 4,
        Net = 1ul << 5,               // Replicated
        EditFixedSize = 1ul << 6,

        Parm = 1ul << 7,              // Property is a part of the function parameters
        OutParm = 1ul << 8,           // Reference(UE3) param
        SkipParm = 1ul << 9,          // ???
        ReturnParm = 1ul << 10,
        CoerceParm = 1ul << 11,       // auto-cast

        Native = 1ul << 12,           // C++
        Transient = 1ul << 13,        // Don't save
        Config = 1ul << 14,           // Saved within .ini
        Localized = 1ul << 15,        // Language ...

        Travel = 1ul << 16,          // Keep value after travel
        EditConst = 1ul << 17,        // ReadOnly in UnrealEd
        GlobalConfig = 1ul << 18,

        Component = 1ul << 19,
        Init = 1ul << 20,
        DuplicateTransient = 1ul << 21,
        NeedCtorLink = 1ul << 22,
        NoExport = 1ul << 23,         // Don't export properties to clipboard
        NoImport = 1ul << 24,
        NoClear = 1ul << 25,          // Don't permit reference clearing.

        EditInline = 1ul << 26,
        EdFindable = 1ul << 27,
        EditInlineUse = 1ul << 28,
        Deprecated = 1ul << 29,
        DataBinding = 1ul << 30,
        SerializeText = 1ul << 31,

        EditInlineAll = EditInline | EditInlineUse,
        Instanced = ExportObject | EditInline,

        RepNotify = 1ul << 32,
        Interp = 1ul << 33,
        NonTransactional = 1ul << 34,
        EditorOnly = 1ul << 35,
        NotForConsole = 1ul << 36,
        RepRetry = 1ul << 37,
        PrivateWrite = 1ul << 38,
        ProtectedWrite = 1ul << 39,
        Archetype = 1ul << 40,
        EditHide = 1ul << 41,
        EditTextBox = 1ul << 42,
        // GAP!
        CrossLevelPassive = 1ul << 44,
        CrossLevelActive = 1ul << 45,
    }
}