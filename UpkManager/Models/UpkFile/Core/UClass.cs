using System;
using System.Collections.Generic;

using UpkManager.Models.UpkFile.Tables;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Classes
{
    // https://github.com/EliotVU/Unreal-Library/blob/master/src/Core/Classes/UClass.cs

    public class UClass : UState
    {
        [TreeNodeField]
        public EClassFlags ClassFlags { get; private set; }

        [TreeNodeField("UClass")]
        public UnrealNameTableIndex Within { get; private set; } // UClass 

        [TreeNodeField("UName")]
        public UName ConfigName { get; private set; } // UName 

        [TreeNodeField("UMap<UName, UObject>")]
        public UMap<UName, UnrealNameTableIndex> ComponentDefaultObjectMap { get; private set; } // UName, UObject

        [TreeNodeField("FImplementedInterface")]
        public List<FImplementedInterface> Interfaces { get; private set; }

        [TreeNodeField("UName")]
        public List<UName> DontSortCategories { get; private set; } // UName

        [TreeNodeField("UName")]
        public List<UName> HideCategories { get; private set; } // UName

        [TreeNodeField("UName")]
        public List<UName> AutoExpandCategories { get; private set; } // UName

        [TreeNodeField("UName")]
        public List<UName> AutoCollapseCategories { get; private set; } // UName

        [TreeNodeField]
        public bool ForceScriptOrder { get; private set; }

        [TreeNodeField("UName")]
        public List<UName> ClassGroups { get; private set; } // UName

        [TreeNodeField]
        public string NativeClassName { get; private set; }

        [TreeNodeField]
        public UName DLLBindName { get; private set; } // UName

        [TreeNodeField("UObject")]
        public UnrealNameTableIndex Default { get; private set; } // UObject


        public override void ReadBuffer(UBuffer buffer)
        {
            buffer.IsAbstractClass = true;
            base.ReadBuffer(buffer);
            ClassFlags = (EClassFlags)buffer.Reader.ReadUInt32();
            Within = buffer.ReadObject();
            ConfigName = UName.ReadName(buffer);

            ComponentDefaultObjectMap = buffer.ReadMap();

            Interfaces = buffer.ReadList(FImplementedInterface.Read);
            DontSortCategories = buffer.ReadList(UName.ReadName);
            HideCategories = buffer.ReadList(UName.ReadName);
            AutoExpandCategories = buffer.ReadList(UName.ReadName);
            AutoCollapseCategories = buffer.ReadList(UName.ReadName);

            ForceScriptOrder = buffer.ReadBool();
            ClassGroups = buffer.ReadList(UName.ReadName);
            NativeClassName = buffer.ReadString();
            DLLBindName = UName.ReadName(buffer);

            Default = buffer.ReadObject();
        }
    }

    public struct FImplementedInterface(UBuffer buffer)
    {
        public UnrealNameTableIndex Class = buffer.ReadObject();
        public UnrealNameTableIndex Pointer = buffer.ReadObject();

        public static FImplementedInterface Read(UBuffer buffer)
        {
            return new(buffer);
        }
    }

    [Flags]
    public enum EClassFlags : ulong
    {
        None = 0x00000000U,
        Abstract = 0x00000001U,
        Compiled = 0x00000002U,
        Config = 0x00000004U,
        Transient = 0x00000008U,
        Parsed = 0x00000010U,
        Localized = 0x00000020U,
        SafeReplace = 0x00000040U,
        Native = 0x00000080U,
        NoExport = 0x00000100U,
        Placeable = 0x00000200U,
        PerObjectConfig = 0x00000400U,
        NativeReplication = 0x00000800U,
        EditInlineNew = 0x00001000U,
        CollapseCategories = 0x00002000U,
        Interface = 0x00004000U,
        Unknown_00080000 = 0x00008000U,
        Unknown_00100000 = 0x00100000U,
        Instanced = 0x00200000U,
        NeedProps = 0x00400000U,
        HasComponents = 0x00800000U,
        Hidden = 0x01000000U,
        Deprecated = 0x02000000U,
        HideDropDown = 0x04000000U,
        Exported = 0x08000000U,
        Intrinsic = 0x10000000u,
        NativeOnly = 0x20000000U,
    }
}
