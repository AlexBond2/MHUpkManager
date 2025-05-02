using System.Collections.Generic;

using UpkManager.Models.UpkFile.Tables;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Classes
{
    public struct FImplementedInterface(UBuffer buffer)
    {
        public UnrealNameTableIndex Class = buffer.ReadObject();
        public UnrealNameTableIndex Pointer = buffer.ReadObject();

        public static FImplementedInterface Read(UBuffer buffer)
        {
            return new (buffer);
        }
    }

    // https://github.com/EliotVU/Unreal-Library/blob/master/src/Core/Classes/UClass.cs

    public class UClass : UState
    {
        public uint ClassFlags { get; private set; }
        public UnrealNameTableIndex Within { get; private set; } // UClass 
        public UName ConfigName { get; private set; } // UName 

        public UMap<UName, UnrealNameTableIndex> ComponentDefaultObjectMap { get; private set; } // UName, UObject
        public List<FImplementedInterface> Interfaces { get; private set; }
        public List<UName> DontSortCategories { get; private set; } // UName
        public List<UName> HideCategories { get; private set; } // UName
        public List<UName> AutoExpandCategories { get; private set; } // UName
        public List<UName> AutoCollapseCategories { get; private set; } // UName
        public bool ForceScriptOrder { get; private set; }
        public List<UName> ClassGroups { get; private set; } // UName
        public string NativeClassName { get; private set; }
        public UName DLLBindName { get; private set; } // UName
        public UnrealNameTableIndex Default { get; private set; } // UObject

        public override void ReadBuffer(UBuffer buffer)
        {
            base.ReadBuffer(buffer);
            ClassFlags = buffer.Reader.ReadUInt32();
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
}
