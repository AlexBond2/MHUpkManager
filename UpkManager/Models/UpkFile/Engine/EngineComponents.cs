using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Core;
using UpkManager.Models.UpkFile.Engine.Mesh;

namespace UpkManager.Models.UpkFile.Engine
{
    [UnrealClass("BrushComponent")]
    public class UBrushComponent : UComponentProperty
    {
    }

    [UnrealClass("AudioComponent")]
    public class UAudioComponent : UComponentProperty
    {
    }

    [UnrealClass("DistributionFloatUniform")]
    public class UDistributionFloatUniform : UDistributionFloat
    {
    }

    [UnrealClass("SplineComponent")]
    public class USplineComponent : UComponentProperty
    {
    }

    [UnrealClass("SplineComponentSimplified")]
    public class USplineComponentSimplified : USplineComponent
    {
    }

    [UnrealClass("SplineAudioComponent")]
    public class USplineAudioComponent : UAudioComponent
    {
    }

    [UnrealClass("SimpleSplineAudioComponent")]
    public class USimpleSplineAudioComponent : USplineAudioComponent
    {
    }

    [UnrealClass("SimpleSplineNonLoopAudioComponent")]
    public class USimpleSplineNonLoopAudioComponent : USimpleSplineAudioComponent
    {
    }

    [UnrealClass("MultiCueSplineAudioComponent")]
    public class UMultiCueSplineAudioComponent : USplineAudioComponent
    {
    }

    [UnrealClass("ApexComponentBase")]
    public class UApexComponentBase : UMeshComponent
    {
    }

    [UnrealClass("LightEnvironmentComponent")]
    public class ULightEnvironmentComponent : UActorComponent
    {
    }

    [UnrealClass("DynamicLightEnvironmentComponent")]
    public class UDynamicLightEnvironmentComponent : ULightEnvironmentComponent
    {
    }

    [UnrealClass("ApexStaticComponent")]
    public class UApexStaticComponent : UApexComponentBase
    {
    }

    [UnrealClass("ApexStaticDestructibleComponent")]
    public class UApexStaticDestructibleComponent : UApexStaticComponent
    {
    }

    [UnrealClass("ApexDynamicComponent")]
    public class UApexDynamicComponent : UApexComponentBase
    {
    }

    [UnrealClass("ArrowComponent")]
    public class UArrowComponent : UPrimitiveComponent
    {
    }

    [UnrealClass("CameraConeComponent")]
    public class UCameraConeComponent : UPrimitiveComponent
    {
    }

    [UnrealClass("CoverGroupRenderingComponent")]
    public class UCoverGroupRenderingComponent : UPrimitiveComponent
    {
    }

    [UnrealClass("StaticMeshComponent")]
    public class UStaticMeshComponent : UComponentProperty
    {
    }

    [UnrealClass("CoverMeshComponent")]
    public class UCoverMeshComponent : UStaticMeshComponent
    {
    }

    [UnrealClass("DecalComponent")]
    public class UDecalComponent : UPrimitiveComponent
    {
    }

    [UnrealClass("LightComponent")]
    public class ULightComponent : UActorComponent
    {
    }

    [UnrealClass("DirectionalLightComponent")]
    public class UDirectionalLightComponent : ULightComponent
    {
    }

    [UnrealClass("DistributionFloatConstant")]
    public class UDistributionFloatConstant : UDistributionFloat
    {
    }

    [UnrealClass("DistributionFloatConstantCurve")]
    public class UDistributionFloatConstantCurve : UDistributionFloat
    {
    }

    [UnrealClass("DistributionFloatParameterBase")]
    public class UDistributionFloatParameterBase : UDistributionFloatConstant
    {
    }

    [UnrealClass("DistributionFloatParticleParameter")]
    public class UDistributionFloatParticleParameter : UDistributionFloatParameterBase
    {
    }

    [UnrealClass("DistributionFloatSoundParameter")]
    public class UDistributionFloatSoundParameter : UDistributionFloatParameterBase
    {
    }

    [UnrealClass("DistributionFloatUniformCurve")]
    public class UDistributionFloatUniformCurve : UDistributionFloat
    {
    }

    [UnrealClass("DistributionFloatUniformRange")]
    public class UDistributionFloatUniformRange : UDistributionFloat
    {
    }

    [UnrealClass("DistributionVectorConstant")]
    public class UDistributionVectorConstant : UDistributionVector
    {
    }

    [UnrealClass("DistributionVectorConstantCurve")]
    public class UDistributionVectorConstantCurve : UDistributionVector
    {
    }

    [UnrealClass("DistributionVectorParameterBase")]
    public class UDistributionVectorParameterBase : UDistributionVectorConstant
    {
    }

    [UnrealClass("DistributionVectorParticleParameter")]
    public class UDistributionVectorParticleParameter : UDistributionVectorParameterBase
    {
    }

    [UnrealClass("DistributionVectorUniform")]
    public class UDistributionVectorUniform : UDistributionVector
    {
    }

    [UnrealClass("DistributionVectorUniformCurve")]
    public class UDistributionVectorUniformCurve : UDistributionVector
    {
    }

    [UnrealClass("DistributionVectorUniformRange")]
    public class UDistributionVectorUniformRange : UDistributionVector
    {
    }

    [UnrealClass("DominantDirectionalLightComponent")]
    public class UDominantDirectionalLightComponent : UDirectionalLightComponent
    {
    }

    [UnrealClass("PointLightComponent")]
    public class UPointLightComponent : ULightComponent
    {
    }

    [UnrealClass("DominantPointLightComponent")]
    public class UDominantPointLightComponent : UPointLightComponent
    {
    }

    [UnrealClass("SpotLightComponent")]
    public class USpotLightComponent : UPointLightComponent
    {
    }

    [UnrealClass("DominantSpotLightComponent")]
    public class UDominantSpotLightComponent : USpotLightComponent
    {
    }

    [UnrealClass("DrawBoxComponent")]
    public class UDrawBoxComponent : UPrimitiveComponent
    {
    }

    [UnrealClass("DrawCapsuleComponent")]
    public class UDrawCapsuleComponent : UPrimitiveComponent
    {
    }

    [UnrealClass("DrawConeComponent")]
    public class UDrawConeComponent : UPrimitiveComponent
    {
    }

    [UnrealClass("DrawCylinderComponent")]
    public class UDrawCylinderComponent : UPrimitiveComponent
    {
    }

    [UnrealClass("DrawFrustumComponent")]
    public class UDrawFrustumComponent : UPrimitiveComponent
    {
    }

    [UnrealClass("DrawLightConeComponent")]
    public class UDrawLightConeComponent : UDrawConeComponent
    {
    }

    [UnrealClass("DrawSphereComponent")]
    public class UDrawSphereComponent : UPrimitiveComponent
    {
    }

    [UnrealClass("DrawLightRadiusComponent")]
    public class UDrawLightRadiusComponent : UDrawSphereComponent
    {
    }

    [UnrealClass("DrawPylonRadiusComponent")]
    public class UDrawPylonRadiusComponent : UDrawSphereComponent
    {
    }

    [UnrealClass("DrawQuadComponent")]
    public class UDrawQuadComponent : UPrimitiveComponent
    {
    }

    [UnrealClass("DrawSoundRadiusComponent")]
    public class UDrawSoundRadiusComponent : UDrawSphereComponent
    {
    }

    [UnrealClass("ParticleSystemComponent")]
    public class UParticleSystemComponent : UComponentProperty
    {
    }

    [UnrealClass("ExponentialHeightFogComponent")]
    public class UExponentialHeightFogComponent : UActorComponent
    {
    }

    [UnrealClass("FluidInfluenceComponent")]
    public class UFluidInfluenceComponent : UPrimitiveComponent
    {
    }

    [UnrealClass("FluidSurfaceComponent")]
    public class UFluidSurfaceComponent : UPrimitiveComponent
    {
    }

    [UnrealClass("FogVolumeDensityComponent")]
    public class UFogVolumeDensityComponent : UActorComponent
    {
    }

    [UnrealClass("FogVolumeConeDensityComponent")]
    public class UFogVolumeConeDensityComponent : UFogVolumeDensityComponent
    {
    }

    [UnrealClass("FogVolumeConstantDensityComponent")]
    public class UFogVolumeConstantDensityComponent : UFogVolumeDensityComponent
    {
    }

    [UnrealClass("FogVolumeLinearHalfspaceDensityComponent")]
    public class UFogVolumeLinearHalfspaceDensityComponent : UFogVolumeDensityComponent
    {
    }

    [UnrealClass("FogVolumeSphericalDensityComponent")]
    public class UFogVolumeSphericalDensityComponent : UFogVolumeDensityComponent
    {
    }

    [UnrealClass("FracturedBaseComponent")]
    public class UFracturedBaseComponent : UStaticMeshComponent
    {
    }

    [UnrealClass("FracturedSkinnedMeshComponent")]
    public class UFracturedSkinnedMeshComponent : UFracturedBaseComponent
    {
    }

    [UnrealClass("FracturedStaticMeshComponent")]
    public class UFracturedStaticMeshComponent : UComponentProperty
    {
    }

    [UnrealClass("HeadTrackingComponent")]
    public class UHeadTrackingComponent : UActorComponent
    {
    }

    [UnrealClass("HeightFogComponent")]
    public class UHeightFogComponent : UActorComponent
    {
    }

    [UnrealClass("ImageBasedReflectionComponent")]
    public class UImageBasedReflectionComponent : UStaticMeshComponent
    {
    }

    [UnrealClass("ImageReflectionComponent")]
    public class UImageReflectionComponent : UActorComponent
    {
    }

    [UnrealClass("ImageReflectionShadowPlaneComponent")]
    public class UImageReflectionShadowPlaneComponent : UPrimitiveComponent
    {
    }

    [UnrealClass("InstancedStaticMeshComponent")]
    public class UInstancedStaticMeshComponent : UStaticMeshComponent
    {
    }

    [UnrealClass("InteractiveFoliageComponent")]
    public class UInteractiveFoliageComponent : UStaticMeshComponent
    {
    }

    [UnrealClass("LandscapeComponent")]
    public class ULandscapeComponent : UPrimitiveComponent
    {
    }

    [UnrealClass("LandscapeGizmoRenderComponent")]
    public class ULandscapeGizmoRenderComponent : UPrimitiveComponent
    {
    }

    [UnrealClass("LandscapeHeightfieldCollisionComponent")]
    public class ULandscapeHeightfieldCollisionComponent : UPrimitiveComponent
    {
    }

    [UnrealClass("LensFlareComponent")]
    public class ULensFlareComponent : UPrimitiveComponent
    {
    }

    [UnrealClass("LevelGridVolumeRenderingComponent")]
    public class ULevelGridVolumeRenderingComponent : UPrimitiveComponent
    {
    }

    [UnrealClass("LineBatchComponent")]
    public class ULineBatchComponent : UPrimitiveComponent
    {
    }

    [UnrealClass("ModelComponent")]
    public class UModelComponent : UPrimitiveComponent
    {
    }

    [UnrealClass("NavMeshRenderingComponent")]
    public class UNavMeshRenderingComponent : UPrimitiveComponent
    {
    }

    [UnrealClass("NxForceFieldComponent")]
    public class UNxForceFieldComponent : UPrimitiveComponent
    {
    }

    [UnrealClass("NxForceFieldCylindricalComponent")]
    public class UNxForceFieldCylindricalComponent : UNxForceFieldComponent
    {
    }

    [UnrealClass("NxForceFieldGenericComponent")]
    public class UNxForceFieldGenericComponent : UNxForceFieldComponent
    {
    }

    [UnrealClass("NxForceFieldRadialComponent")]
    public class UNxForceFieldRadialComponent : UNxForceFieldComponent
    {
    }

    [UnrealClass("NxForceFieldTornadoComponent")]
    public class UNxForceFieldTornadoComponent : UNxForceFieldComponent
    {
    }

    [UnrealClass("OcclusionComponent")]
    public class UOcclusionComponent : UActorComponent
    {
    }

    [UnrealClass("ParticleLightEnvironmentComponent")]
    public class UParticleLightEnvironmentComponent : UDynamicLightEnvironmentComponent
    {
    }

    [UnrealClass("PathRenderingComponent")]
    public class UPathRenderingComponent : UPrimitiveComponent
    {
    }

    [UnrealClass("SceneCaptureComponent")]
    public class USceneCaptureComponent : UActorComponent
    {
    }

    [UnrealClass("SceneCaptureReflectComponent")]
    public class USceneCaptureReflectComponent : USceneCaptureComponent
    {
    }

    [UnrealClass("SceneCapturePortalComponent")]
    public class USceneCapturePortalComponent : USceneCaptureComponent
    {
    }

    [UnrealClass("RadialBlurComponent")]
    public class URadialBlurComponent : UActorComponent
    {
    }

    [UnrealClass("RB_ConstraintDrawComponent")]
    public class URB_ConstraintDrawComponent : UPrimitiveComponent
    {
    }

    [UnrealClass("RB_Handle")]
    public class URB_Handle : UActorComponent
    {
    }

    [UnrealClass("RB_RadialImpulseComponent")]
    public class URB_RadialImpulseComponent : UPrimitiveComponent
    {
    }

    [UnrealClass("RB_Spring")]
    public class URB_Spring : UActorComponent
    {
    }

    [UnrealClass("RouteRenderingComponent")]
    public class URouteRenderingComponent : UPrimitiveComponent
    {
    }

    [UnrealClass("SceneCapture2DComponent")]
    public class USceneCapture2DComponent : USceneCaptureComponent
    {
    }

    [UnrealClass("SceneCapture2DHitMaskComponent")]
    public class USceneCapture2DHitMaskComponent : USceneCaptureComponent
    {
    }

    [UnrealClass("SceneCaptureCubeMapComponent")]
    public class USceneCaptureCubeMapComponent : USceneCaptureComponent
    {
    }

    [UnrealClass("SkyLightComponent")]
    public class USkyLightComponent : ULightComponent
    {
    }

    [UnrealClass("SpeedTreeComponent")]
    public class USpeedTreeComponent : UComponentProperty
    {
    }

    [UnrealClass("SphericalHarmonicLightComponent")]
    public class USphericalHarmonicLightComponent : ULightComponent
    {
    }

    [UnrealClass("SplineMeshComponent")]
    public class USplineMeshComponent : UStaticMeshComponent
    {
    }

    [UnrealClass("SpriteComponent")]
    public class USpriteComponent : UPrimitiveComponent
    {
    }

    [UnrealClass("SVehicleSimBase")]
    public class USVehicleSimBase : UActorComponent
    {
    }

    [UnrealClass("SVehicleSimCar")]
    public class USVehicleSimCar : USVehicleSimBase
    {
    }

    [UnrealClass("SVehicleSimTank")]
    public class USVehicleSimTank : USVehicleSimCar
    {
    }

    [UnrealClass("SVehicleWheel")]
    public class USVehicleWheel : UComponent
    {
    }

    [UnrealClass("TerrainComponent")]
    public class UTerrainComponent : UPrimitiveComponent
    {
    }

    [UnrealClass("WindDirectionalSourceComponent")]
    public class UWindDirectionalSourceComponent : UActorComponent
    {
    }

    [UnrealClass("WindPointSourceComponent")]
    public class UWindPointSourceComponent : UWindDirectionalSourceComponent
    {
    }

}
