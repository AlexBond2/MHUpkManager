using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Engine.Texture
{
    [UnrealClass("Texture")]
    public class UTexture: USurface
    {
        [PropertyField]
        public TextureGroup LODGroup { get; set; }

        [StructField("UntypedBulkData")]
        public byte[] SourceArt { get; set; } // UntypedBulkData

        public override void ReadBuffer(UBuffer buffer)
        {
            base.ReadBuffer(buffer);
            SourceArt = buffer.ReadBulkData();
        }

        public enum TextureGroup
        {
            TEXTUREGROUP_World,             // 0
            TEXTUREGROUP_WorldNormalMap,    // 1
            TEXTUREGROUP_WorldSpecular,     // 2
            TEXTUREGROUP_Character,         // 3
            TEXTUREGROUP_CharacterNormalMap,// 4
            TEXTUREGROUP_CharacterSpecular, // 5
            TEXTUREGROUP_Weapon,            // 6
            TEXTUREGROUP_WeaponNormalMap,   // 7
            TEXTUREGROUP_WeaponSpecular,    // 8
            TEXTUREGROUP_Vehicle,           // 9
            TEXTUREGROUP_VehicleNormalMap,  // 10
            TEXTUREGROUP_VehicleSpecular,   // 11
            TEXTUREGROUP_Cinematic,         // 12
            TEXTUREGROUP_Effects,           // 13
            TEXTUREGROUP_EffectsNotFiltered,// 14
            TEXTUREGROUP_Skybox,            // 15
            TEXTUREGROUP_UI,                // 16
            TEXTUREGROUP_Lightmap,          // 17
            TEXTUREGROUP_RenderTarget,      // 18
            TEXTUREGROUP_MobileFlattened,   // 19
            TEXTUREGROUP_ProcBuilding_Face, // 20
            TEXTUREGROUP_ProcBuilding_LightMap,// 21
            TEXTUREGROUP_Shadowmap,         // 22
            TEXTUREGROUP_ColorLookupTable,  // 23
            TEXTUREGROUP_Terrain_Heightmap, // 24
            TEXTUREGROUP_Terrain_Weightmap, // 25
            TEXTUREGROUP_ImageBasedReflection,// 26
            TEXTUREGROUP_Bokeh,             // 27
            TEXTUREGROUP_MAX                // 28
        };
    }
}
