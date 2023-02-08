using RimWorld;
using Verse;
using HarmonyLib;

namespace ATR_Implants_Patch
{
    [StaticConstructorOnStartup]
    public static class PatchImplants
    {
        public static BodyPartDef ATR_ArtificialBrain = DefDatabase<BodyPartDef>.GetNamed("ATR_ArtificialBrain");
        static PatchImplants()
        {
            new Harmony("twsta.ATRImplantsPatch").PatchAll();
        }
        [HarmonyPatch(typeof(CompUseEffect_InstallImplant), nameof(CompUseEffect_InstallImplant.DoEffect))]
        static class CompUseEffect_InstallImplant_DoEffect
        {
            static void Prefix(ref Pawn user, CompUseEffect_InstallImplant __instance, out BodyPartDef __state)
            {
                __state = __instance.Props.bodyPart;                                                                                        // Save original human "Brain" BodyPartDef target to reset later.
                if (user.health.hediffSet.GetBrain().def == ATR_ArtificialBrain)                                                            // If source of the implant user's consciousness is "ATR_ArtificialBrain" BodyPartDef,
                {
                    __instance.Props.bodyPart = user.health.hediffSet.GetBrain().def;                                                       // then replace BodyPartDef target with the Android user's BodyPartDef.
                }
            }
            static void Postfix(ref Pawn user, CompUseEffect_InstallImplant __instance, BodyPartDef __state)
            {
                if (user.health.hediffSet.GetBrain().def == ATR_ArtificialBrain)                                                            // If source of the implant user's consciousness is "ATR_ArtificialBrain" BodyPartDef,
                {
                    __instance.Props.bodyPart = __state;                                                                                    // then restore the original human "Brain" BodyPartDef target from __state.
                }
            }
        }
        [HarmonyPatch(typeof(CompUseEffect_InstallImplant), nameof(CompUseEffect_InstallImplant.CanBeUsedBy))]
        static class CompUseEffect_InstallImplant_CanBeUsedBy
        {
            public static void Postfix(ref Pawn p, ref string failReason, ref bool __result)
            {
                string noBodyPart = (string)("InstallImplantNoBodyPart".Translate() + ": " + "brain");                                      // Vanilla string expected by context menu (taken from decompiled code)
                if (__result == false)                                                                                                      // If CanBeUsedBy() returned false,
                {
                    if (p.health.hediffSet.GetBrain().def == ATR_ArtificialBrain && failReason == noBodyPart)                               // and source of the pawn's consciousness is "ATR_ArtificialBrain" BodyPartDef,
                    {                                                                                                                       // or is failing due to not having a human "Brain" BodyPartDef,
                        __result = true;                                                                                                    // allow the Android pawn to install Mechlink implant.
                    }
                }
            }
        }
    }
}
