using System;

using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Core;
using UpkManager.Models.UpkFile.Tables;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Engine.Material
{
    [UnrealClass("ShaderCache")]
    public class UShaderCache : UObject
    {
        [StructField]
        public int ShaderCachePriority { get; set; }

        [StructField]
        public EShaderPlatform Platform { get; set; }

        [StructField]
        public FCompressedShaderCodeCache DummyCache { get; set; }

        [StructField("Shader")]
        public UArray<FShader> Shaders { get; set; }

        [StructField("MaterialShaderMap")]
        public UArray<FMaterialShaderMap> MaterialShaderMaps { get; set; }

        public override void ReadBuffer(UBuffer buffer)
        {
            base.ReadBuffer(buffer);
            ShaderCachePriority = buffer.ReadInt32();
            Platform = (EShaderPlatform)buffer.ReadByte();
            DummyCache = FCompressedShaderCodeCache.ReadData(buffer);

            Shaders = buffer.ReadArray(FShader.ReadData);
            MaterialShaderMaps = buffer.ReadArray(FMaterialShaderMap.ReadData);
        }
    }

    public class FMaterialShaderMap  : IAtomicStruct
    {
        public FStaticParameterSet StaticParameters { get; set; }
        public int ShaderMapVersion { get; set; }
        public int ShaderMapLicenseeVersion { get; set; }
        public int SkipOffset { get; set; }

        [StructField("ShaderRef")]
        public UMap<FName, FShaderRef> ShaderMap { get; set; }

        [StructField("MeshMaterialShaderMap")]
        public UArray<FMeshMaterialShaderMap> MeshShaderMaps { get; set; }
        public FGuid MaterialId { get; set; }
        public string FriendlyName { get; set; }
        public static FMaterialShaderMap ReadData(UBuffer buffer)
        {
            var matShader = new FMaterialShaderMap();

            matShader.StaticParameters = FStaticParameterSet.ReadData(buffer);

            matShader.ShaderMapVersion = buffer.ReadInt32();
            matShader.ShaderMapLicenseeVersion = buffer.ReadInt32();
            matShader.SkipOffset = buffer.ReadInt32();

            matShader.ShaderMap = buffer.ReadMap(UBuffer.ReadName, FShaderRef.ReadData);

            matShader.MeshShaderMaps = buffer.ReadArray(FMeshMaterialShaderMap.ReadData);
            matShader.MaterialId = FGuid.ReadData(buffer);
            matShader.FriendlyName = buffer.ReadString();

            buffer.SkipOffset(matShader.SkipOffset);
            // StaticParameters
            // UniformExpressionSet
            // Platform
            return matShader;
        }

        public string Format => "";

        public override string ToString() => $"MaterialShaderMap<{MaterialId}, {FriendlyName}>";
    }

    public class FMeshMaterialShaderMap : IAtomicStruct
    {
        [StructField("ShaderRef")]
        public UMap<FName, FShaderRef> Shaders { get; set; }
        public FName VertexFactoryType { get; set; }
        public static FMeshMaterialShaderMap ReadData(UBuffer buffer)
        {
            var shaderMap = new FMeshMaterialShaderMap
            {
                Shaders = buffer.ReadMap(UBuffer.ReadName, FShaderRef.ReadData),
                VertexFactoryType = buffer.ReadName()
            };
            return shaderMap;
        }
        public string Format => "";
        public override string ToString() => $"{VertexFactoryType}";
    }

    public class FShaderRef
    {
        public FGuid Id { get; set; }
        public FName Type { get; set; }

        public static FShaderRef ReadData(UBuffer buffer)
        {
            var shader = new FShaderRef
            {
                Id = FGuid.ReadData(buffer),
                Type = buffer.ReadName()
            };
            return shader;
        }
        public override string ToString() => $"{Id}";
    }

    public class FShader : IAtomicStruct
    {
        public FName Type { get; set; }

        public FGuid Id { get; set; }

        public FSHAHash Hash { get; set; }

        public int SkipOffset { get; set; }

        // public UArray<short> SerializeTable { get; set; }
        public EShaderPlatform TargetPlatform { get; set; }
        public byte TargetFrequency { get; set; }

        [StructField("ShaderData")]
        public byte[] Code { get; set; }

        public static FShader ReadData(UBuffer buffer)
        {
            var shader = new FShader();

            shader.Type = buffer.ReadName();
            shader.Id = FGuid.ReadData(buffer);
            shader.Hash = FSHAHash.ReadData(buffer);

            shader.SkipOffset = buffer.ReadInt32();

            int tableSize = buffer.ReadInt32();

            buffer.Reader.Skip(tableSize * 2);
            //shader.SerializeTable = buffer.ReadArray(UBuffer.ReadInt16);

            shader.TargetPlatform = (EShaderPlatform)buffer.ReadByte();
            shader.TargetFrequency = buffer.ReadByte();

            shader.Code = buffer.ReadBytes();

            buffer.SkipOffset(shader.SkipOffset);

            /* 
            shader.ParameterMapCRC = buffer.ReadUInt32();

            shader.ShaderId = FGuid.ReadData(buffer);
            shader.ShaderType = buffer.ReadName();
            shader.ShaderHash = FSHAHash.ReadData(buffer);

            shader.NumInstructions = buffer.ReadInt32();*/

            return shader;
        }

        public string Format => "";

        public override string ToString() => $"{Id} ::Shader<{Type}>";
    }

    public class FSHAHash : IAtomicStruct
    {
        public byte[] Hash { get; set; }

        public string Format => BitConverter.ToString(Hash).Replace("-", "");

        public static FSHAHash ReadData(UBuffer buffer)
        {
            return new FSHAHash
            {
                Hash = buffer.Reader.ReadBytes(20)
            };
        }        
    }

    public class FCompressedShaderCodeCache : IAtomicStruct
    {
        [StructField("UMap<FShaderType, FTypeSpecificCompressedShaderCode>")]
        public UMap<FObject, FTypeSpecificCompressedShaderCode> ShaderTypeCompressedShaderCode { get; set; } // FShaderType
        public string Format => "";

        public static FCompressedShaderCodeCache ReadData(UBuffer buffer)
        {
            return new FCompressedShaderCodeCache
            {
                ShaderTypeCompressedShaderCode = buffer.ReadMap(UBuffer.ReadObject, FTypeSpecificCompressedShaderCode.ReadData)
            };
        }
    }

    public class FTypeSpecificCompressedShaderCode
    {
        public static FTypeSpecificCompressedShaderCode ReadData(UBuffer buffer)
        {
            // Not used
            return new FTypeSpecificCompressedShaderCode();
        }
    }

    public enum EShaderPlatform
    {
        SP_PCD3D_SM3 = 0,
        SP_PS3 = 1,
        SP_XBOXD3D = 2,
        SP_PCD3D_SM4 = 3,
        SP_PCD3D_SM5 = 4,
        SP_NGP = 5,
        SP_PCOGL = 6,
        SP_WIIU = 7,

        SP_NumPlatforms = 8,
        SP_NumBits = 4,
    };
}
