using System;

namespace UpkManager.Models.UpkFile.Objects.Textures
{
    public enum EPixelFormat
    {
        PF_Unknown,                     // 0
        PF_A32B32G32R32F,               // 1
        PF_A8R8G8B8,                    // 2
        PF_G8,                          // 3
        PF_G16,                         // 4
        PF_DXT1,                        // 5
        PF_DXT3,                        // 6
        PF_DXT5,                        // 7
        PF_UYVY,                        // 8
        PF_FloatRGB,                    // 9
        PF_FloatRGBA,                   // 10
        PF_DepthStencil,                // 11
        PF_ShadowDepth,                 // 12
        PF_FilteredShadowDepth,         // 13
        PF_R32F,                        // 14
        PF_G16R16,                      // 15
        PF_G16R16F,                     // 16
        PF_G16R16F_FILTER,              // 17
        PF_G32R32F,                     // 18
        PF_A2B10G10R10,                 // 19
        PF_A16B16G16R16,                // 20
        PF_D24,                         // 21
        PF_R16F,                        // 22
        PF_R16F_FILTER,                 // 23
        PF_BC5,                         // 24
        PF_V8U8,                        // 25
        PF_A1,                          // 26
        PF_FloatR11G11B10,              // 27
        PF_A4R4G4B4,                    // 28
        PF_R5G6B5,                      // 29
        PF_MAX                          // 30
    };

    [Flags]
    public enum ETextureCreateFlags : ulong
    {
        None = 0,
        RenderTargetable = 1ul << 0,
        ResolveTargetable = 1ul << 1,
        DepthStencilTargetable = 1ul << 2,
        ShaderResource = 1ul << 3,
        SRGB = 1ul << 4,
        CPUWritable = 1ul << 5,
        NoTiling = 1ul << 6,
        VideoDecode = 1ul << 7,
        Dynamic = 1ul << 8,
        InputAttachmentRead = 1ul << 9,
        Foveation = 1ul << 10,
        Tiling3D = 1ul << 11,
        Memoryless = 1ul << 12,
        GenerateMipCapable = 1ul << 13,
        FastVRAMPartialAlloc = 1ul << 14,
        DisableSRVCreation = 1ul << 15,
        DisableDCC = 1ul << 16,
        UAV = 1ul << 17,
        Presentable = 1ul << 18,
        CPUReadback = 1ul << 19,
        OfflineProcessed = 1ul << 20,
        FastVRAM = 1ul << 21,
        HideInVisualizeTexture = 1ul << 22,
        Virtual = 1ul << 23,
        TargetArraySlicesIndependently = 1ul << 24,
        Shared = 1ul << 25,
        NoFastClear = 1ul << 26,
        DepthStencilResolveTarget = 1ul << 27,
        Streamable = 1ul << 28,
        NoFastClearFinalize = 1ul << 29,
        Atomic64Compatible = 1ul << 30,
        ReduceMemoryWithTilingMode = 1ul << 31,
        AtomicCompatible = 1ul << 33,
        External = 1ul << 34,
        MultiGPUGraphIgnore = 1ul << 35,
        ReservedResource = 1ul << 37,
        ImmediateCommit = 1ul << 38,
        ForceIntoNonStreamingMemoryTracking = 1ul << 39,
        Invalid = 1ul << 40,
    }
}
