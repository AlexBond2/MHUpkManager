using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Engine.MarvelGame
{
    [UnrealClass("BoxComponent")]
    public class UBoxComponent : UDrawBoxComponent
    {
    }

    [UnrealClass("MarvelFX")]
    public class UMarvelFX : UActorComponent
    {
    }

    [UnrealClass("FXAnimation")]
    public class UFXAnimation : UMarvelFX
    {
    }

    [UnrealClass("ConditionFXAnimation")]
    public class UConditionFXAnimation : UFXAnimation
    {
    }

    [UnrealClass("FXMaterialParameter")]
    public class UFXMaterialParameter : UMarvelFX
    {
    }

    [UnrealClass("FXAttachmentMaterialParameter")]
    public class UFXAttachmentMaterialParameter : UFXMaterialParameter
    {
    }

    [UnrealClass("ConditionFXAttachmentMaterialParameter")]
    public class UConditionFXAttachmentMaterialParameter : UFXAttachmentMaterialParameter
    {
    }

    [UnrealClass("FXCameraShake")]
    public class UFXCameraShake : UMarvelFX
    {
    }

    [UnrealClass("ConditionFXCameraShake")]
    public class UConditionFXCameraShake : UFXCameraShake
    {
    }

    [UnrealClass("FXDecal")]
    public class UFXDecal : UMarvelFX
    {
    }

    [UnrealClass("ConditionFXDecal")]
    public class UConditionFXDecal : UFXDecal
    {
    }

    [UnrealClass("FXHide")]
    public class UFXHide : UMarvelFX
    {
    }

    [UnrealClass("ConditionFXHide")]
    public class UConditionFXHide : UFXHide
    {
    }

    [UnrealClass("ConditionFXMaterialParameter")]
    public class UConditionFXMaterialParameter : UFXMaterialParameter
    {
    }

    [UnrealClass("ConditionFXMaterialSwap")]
    public class UConditionFXMaterialSwap : UMarvelFX
    {
    }

    [UnrealClass("FXMeshAttachment")]
    public class UFXMeshAttachment : UMarvelFX
    {
    }

    [UnrealClass("ConditionFXMeshAttachment")]
    public class UConditionFXMeshAttachment : UFXMeshAttachment
    {
    }

    [UnrealClass("FXMeshScale")]
    public class UFXMeshScale : UMarvelFX
    {
    }

    [UnrealClass("ConditionFXMeshScale")]
    public class UConditionFXMeshScale : UFXMeshScale
    {
    }

    [UnrealClass("FXMeshSwap")]
    public class UFXMeshSwap : UMarvelFX
    {
    }

    [UnrealClass("ConditionFXMeshSwap")]
    public class UConditionFXMeshSwap : UFXMeshSwap
    {
    }

    [UnrealClass("FXParticle")]
    public class UFXParticle : UMarvelFX
    {
    }

    [UnrealClass("ConditionFXParticle")]
    public class UConditionFXParticle : UFXParticle
    {
    }

    [UnrealClass("FXPhysicsWeight")]
    public class UFXPhysicsWeight : UMarvelFX
    {
    }

    [UnrealClass("ConditionFXPhysicsWeight")]
    public class UConditionFXPhysicsWeight : UFXPhysicsWeight
    {
    }

    [UnrealClass("FXPostProcessing")]
    public class UFXPostProcessing : UMarvelFX
    {
    }

    [UnrealClass("ConditionFXPostProcessing")]
    public class UConditionFXPostProcessing : UFXPostProcessing
    {
    }

    [UnrealClass("FXSound")]
    public class UFXSound : UMarvelFX
    {
    }

    [UnrealClass("ConditionFXSound")]
    public class UConditionFXSound : UFXSound
    {
    }

    [UnrealClass("ConditionFXSoundDamageType")]
    public class UConditionFXSoundDamageType : UConditionFXSound
    {
    }

    [UnrealClass("EntityFXParticle")]
    public class UEntityFXParticle : UFXParticle
    {
    }

    [UnrealClass("FXAnimatedActor")]
    public class UFXAnimatedActor : UMarvelFX
    {
    }

    [UnrealClass("EntityFXAnimatedActor")]
    public class UEntityFXAnimatedActor : UFXAnimatedActor
    {
    }

    [UnrealClass("EntityFXAnimation")]
    public class UEntityFXAnimation : UFXAnimation
    {
    }

    [UnrealClass("EntityFXAnimationComplex")]
    public class UEntityFXAnimationComplex : UFXAnimation
    {
    }

    [UnrealClass("FXBeam")]
    public class UFXBeam : UMarvelFX
    {
    }

    [UnrealClass("EntityFXBeam")]
    public class UEntityFXBeam : UFXBeam
    {
    }

    [UnrealClass("EntityFXCameraShake")]
    public class UEntityFXCameraShake : UFXCameraShake
    {
    }

    [UnrealClass("EntityFXDecal")]
    public class UEntityFXDecal : UFXDecal
    {
    }

    [UnrealClass("EntityFXMaterialParameter")]
    public class UEntityFXMaterialParameter : UFXMaterialParameter
    {
    }

    [UnrealClass("EntityFXMeshAttachment")]
    public class UEntityFXMeshAttachment : UFXMeshAttachment
    {
    }

    [UnrealClass("EntityFXMeshScale")]
    public class UEntityFXMeshScale : UFXMeshScale
    {
    }

    [UnrealClass("EntityFXMeshSwap")]
    public class UEntityFXMeshSwap : UFXMeshSwap
    {
    }

    [UnrealClass("FXPhysicalForce")]
    public class UFXPhysicalForce : UMarvelFX
    {
    }

    [UnrealClass("EntityFXPhysicalForce")]
    public class UEntityFXPhysicalForce : UFXPhysicalForce
    {
    }

    [UnrealClass("EntityFXSound")]
    public class UEntityFXSound : UFXSound
    {
    }

    [UnrealClass("EntityFXSoundGroundMaterial")]
    public class UEntityFXSoundGroundMaterial : UEntityFXSound
    {
    }

    [UnrealClass("FXAnimatedActorMaterialParameter")]
    public class UFXAnimatedActorMaterialParameter : UFXMaterialParameter
    {
    }

    [UnrealClass("FXKnockables")]
    public class UFXKnockables : UMarvelFX
    {
    }

    [UnrealClass("InWorldTextComponent")]
    public class UInWorldTextComponent : UActorComponent
    {
    }

    [UnrealClass("MarvelEntityCompAnimationComplex")]
    public class UMarvelEntityCompAnimationComplex : UActorComponent
    {
    }

    [UnrealClass("MarvelEntityCompDeathFX")]
    public class UMarvelEntityCompDeathFX : UActorComponent
    {
    }

    [UnrealClass("MarvelEntityCompSounds")]
    public class UMarvelEntityCompSounds : UActorComponent
    {
        [PropertyField]
        public UArray<int> BanterTargets { get; set; }
    }

    [UnrealClass("MarvelGFxFloatingNumberComp")]
    public class UMarvelGFxFloatingNumberComp : UActorComponent
    {
    }

    [UnrealClass("MarvelDecalComponent")]
    public class UMarvelDecalComponent : UDecalComponent
    {
    }

    [UnrealClass("MarvelGFxActorComp")]
    public class UMarvelGFxActorComp : UActorComponent
    {
    }

    [UnrealClass("MarvelGFxActorTooltipComp")]
    public class UMarvelGFxActorTooltipComp : UMarvelGFxActorComp
    {
    }

    [UnrealClass("MarvelEntityCompAnimationSimple")]
    public class UMarvelEntityCompAnimationSimple : UActorComponent
    {
    }

    [UnrealClass("MarvelEntityCompBossIndicator")]
    public class UMarvelEntityCompBossIndicator : UEntityFXParticle
    {
    }

    [UnrealClass("MarvelEntityCompBuddyIndicator")]
    public class UMarvelEntityCompBuddyIndicator : UEntityFXParticle
    {
    }

    [UnrealClass("MarvelEntityCompInteractIndicator")]
    public class UMarvelEntityCompInteractIndicator : UActorComponent
    {
    }

    [UnrealClass("MarvelEntityCompInteractOverlay")]
    public class UMarvelEntityCompInteractOverlay : UEntityFXParticle
    {
    }

    [UnrealClass("MarvelEntityCompMissionMarker")]
    public class UMarvelEntityCompMissionMarker : UMarvelDecalComponent
    {
    }

    [UnrealClass("MarvelEntityCompObjectiveMarker")]
    public class UMarvelEntityCompObjectiveMarker : UEntityFXDecal
    {
    }

    [UnrealClass("MarvelEntityCompPlayerIndicator")]
    public class UMarvelEntityCompPlayerIndicator : UActorComponent
    {
    }

    [UnrealClass("MarvelEntityCompPlayerTargetIndicator")]
    public class UMarvelEntityCompPlayerTargetIndicator : UEntityFXParticle
    {
    }

    [UnrealClass("MarvelEntityCompPowers")]
    public class UMarvelEntityCompPowers : UActorComponent
    {
    }

    [UnrealClass("MarvelEntityCompSpawnFX")]
    public class UMarvelEntityCompSpawnFX : UActorComponent
    {
    }

    [UnrealClass("MarvelEntityCompTargetIndicator")]
    public class UMarvelEntityCompTargetIndicator : UEntityFXParticle
    {
    }

    [UnrealClass("MarvelEntityCompTargetLockOnIndicator")]
    public class UMarvelEntityCompTargetLockOnIndicator : UMarvelEntityCompPlayerTargetIndicator
    {
    }

    [UnrealClass("MarvelEntityCompThrowable")]
    public class UMarvelEntityCompThrowable : UActorComponent
    {
    }

    [UnrealClass("MarvelEntityCompTurret")]
    public class UMarvelEntityCompTurret : UActorComponent
    {
    }

    [UnrealClass("MarvelGFxActorChatBubbleComp")]
    public class UMarvelGFxActorChatBubbleComp : UMarvelGFxActorComp
    {
    }

    [UnrealClass("MarvelGFxActorStatusComp")]
    public class UMarvelGFxActorStatusComp : UMarvelGFxActorComp
    {
    }

    [UnrealClass("PowerFXAnimation")]
    public class UPowerFXAnimation : UFXAnimation
    {
    }

    [UnrealClass("PowerFXAnimationLooping")]
    public class UPowerFXAnimationLooping : UPowerFXAnimation
    {
    }

    [UnrealClass("PowerFXMeshAttachment")]
    public class UPowerFXMeshAttachment : UFXMeshAttachment
    {
    }

    [UnrealClass("MarvelPPComponent")]
    public class UMarvelPPComponent : UActorComponent
    {
    }

    [UnrealClass("MarvelPP_CinematicTransition")]
    public class UMarvelPP_CinematicTransition : UMarvelPPComponent
    {
    }

    [UnrealClass("MarvelPP_EscMenu")]
    public class UMarvelPP_EscMenu : UMarvelPPComponent
    {
    }

    [UnrealClass("MarvelPP_FadeInFadeOut")]
    public class UMarvelPP_FadeInFadeOut : UMarvelPPComponent
    {
    }

    [UnrealClass("MarvelPP_NearDeath")]
    public class UMarvelPP_NearDeath : UMarvelPPComponent
    {
    }

    [UnrealClass("MarvelPP_StageLock")]
    public class UMarvelPP_StageLock : UMarvelPPComponent
    {
    }

    [UnrealClass("MarvelVisualSwapComp")]
    public class UMarvelVisualSwapComp : UActorComponent
    {
    }

    [UnrealClass("MarvelUIComp")]
    public class UMarvelUIComp : UActorComponent
    {
    }

    [UnrealClass("MarvelUIDecalComponent")]
    public class UMarvelUIDecalComponent : UDecalComponent
    {
    }

    [UnrealClass("MarvelUIPrimaryResourceComp")]
    public class UMarvelUIPrimaryResourceComp : UMarvelUIComp
    {
    }

    [UnrealClass("MarvelUISecondaryResourceComp")]
    public class UMarvelUISecondaryResourceComp : UMarvelUIComp
    {
    }

    [UnrealClass("NaviFragmentRenderComponent")]
    public class UNaviFragmentRenderComponent : UPrimitiveComponent
    {
    }

    [UnrealClass("NaviPathRenderingComponent")]
    public class UNaviPathRenderingComponent : UPrimitiveComponent
    {
    }

    [UnrealClass("NaviRenderingComponent")]
    public class UNaviRenderingComponent : UPrimitiveComponent
    {
    }

    [UnrealClass("PowerFXAnimatedActor")]
    public class UPowerFXAnimatedActor : UFXAnimatedActor
    {
    }

    [UnrealClass("PowerFXAnimatedActorMaterialParameter")]
    public class UPowerFXAnimatedActorMaterialParameter : UFXAnimatedActorMaterialParameter
    {
    }

    [UnrealClass("PowerFXAttachmentMaterialParameter")]
    public class UPowerFXAttachmentMaterialParameter : UFXAttachmentMaterialParameter
    {
    }

    [UnrealClass("PowerFXAttachmentMaterialSwap")]
    public class UPowerFXAttachmentMaterialSwap : UMarvelFX
    {
    }

    [UnrealClass("PowerFXBeam")]
    public class UPowerFXBeam : UFXBeam
    {
    }

    [UnrealClass("PowerFXCameraManipulation")]
    public class UPowerFXCameraManipulation : UMarvelFX
    {
    }

    [UnrealClass("PowerFXCameraShake")]
    public class UPowerFXCameraShake : UFXCameraShake
    {
    }

    [UnrealClass("PowerFXCameraTeleport")]
    public class UPowerFXCameraTeleport : UPowerFXCameraManipulation
    {
    }

    [UnrealClass("PowerFXDecal")]
    public class UPowerFXDecal : UFXDecal
    {
    }

    [UnrealClass("PowerFXHide")]
    public class UPowerFXHide : UFXHide
    {
    }

    [UnrealClass("PowerFXLevitateObject")]
    public class UPowerFXLevitateObject : UMarvelFX
    {
    }

    [UnrealClass("PowerFXMaterialParameter")]
    public class UPowerFXMaterialParameter : UFXMaterialParameter
    {
    }

    [UnrealClass("PowerFXMeshMove")]
    public class UPowerFXMeshMove : UMarvelFX
    {
    }

    [UnrealClass("PowerFXMeshScale")]
    public class UPowerFXMeshScale : UFXMeshScale
    {
    }

    [UnrealClass("PowerFXMeshSwap")]
    public class UPowerFXMeshSwap : UFXMeshSwap
    {
    }

    [UnrealClass("PowerFXParticle")]
    public class UPowerFXParticle : UFXParticle
    {
    }

    [UnrealClass("PowerFXPhysicalForce")]
    public class UPowerFXPhysicalForce : UFXPhysicalForce
    {
    }

    [UnrealClass("PowerFXPhysicsWeight")]
    public class UPowerFXPhysicsWeight : UFXPhysicsWeight
    {
    }

    [UnrealClass("PowerFXPostProcessing")]
    public class UPowerFXPostProcessing : UFXPostProcessing
    {
    }

    [UnrealClass("PowerFXProjectile")]
    public class UPowerFXProjectile : UMarvelFX
    {
    }

    [UnrealClass("PowerFXSound")]
    public class UPowerFXSound : UFXSound
    {
    }

    [UnrealClass("PowerFXSoundDamageType")]
    public class UPowerFXSoundDamageType : UPowerFXSound
    {
    }

    [UnrealClass("PowerFXSoundGroundMaterial")]
    public class UPowerFXSoundGroundMaterial : UPowerFXSound
    {
    }

    [UnrealClass("PowerFXTurretAim")]
    public class UPowerFXTurretAim : UMarvelFX
    {
    }

    [UnrealClass("ProjectileFXAnimation")]
    public class UProjectileFXAnimation : UMarvelFX
    {
    }

    [UnrealClass("ProjectileFXBeam")]
    public class UProjectileFXBeam : UFXBeam
    {
    }

    [UnrealClass("ProjectileFXCameraShake")]
    public class UProjectileFXCameraShake : UFXCameraShake
    {
    }

    [UnrealClass("ProjectileFXDecal")]
    public class UProjectileFXDecal : UFXDecal
    {
    }

    [UnrealClass("ProjectileFXMaterialParameter")]
    public class UProjectileFXMaterialParameter : UFXMaterialParameter
    {
    }

    [UnrealClass("ProjectileFXParticle")]
    public class UProjectileFXParticle : UFXParticle
    {
    }

    [UnrealClass("ProjectileFXPhysicalForce")]
    public class UProjectileFXPhysicalForce : UFXPhysicalForce
    {
    }

    [UnrealClass("ProjectileFXSound")]
    public class UProjectileFXSound : UFXSound
    {
    }

    [UnrealClass("TaserTrapComponent")]
    public class UTaserTrapComponent : UActorComponent
    {
    }

    [UnrealClass("TickableActorComponent")]
    public class UTickableActorComponent : UActorComponent
    {
    }
}
