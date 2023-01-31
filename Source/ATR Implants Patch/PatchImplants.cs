using RimWorld;
using Verse;
using HarmonyLib;

namespace ATR_Implants_Patch
{
    [StaticConstructorOnStartup]
    public static class PatchImplants
    {
        public static string ATR_BrainShortLabel = "artificial brain";
        public static string humanBrain = "brain";
        static PatchImplants()
        {
            new Harmony("twsta.ATRImplantsPatch").PatchAll();
        }
        [HarmonyPatch(typeof(CompUseEffect_InstallImplant), nameof(CompUseEffect_InstallImplant.DoEffect))]
        static class CompUseEffect_InstallImplant_DoEffect
        {
            static void Prefix(ref Pawn user, CompUseEffect_InstallImplant __instance, out BodyPartDef __state)
            {
                __state = __instance.Props.bodyPart;                                                                                        // Save original human brain BodyPartDef for later.
                BodyPartDef getPawnBrain = user.health.hediffSet.GetBrain().def;                                                            // What is pawn brain?
                if (getPawnBrain.LabelShort == ATR_BrainShortLabel)                                                                         // If pawn has artificial brain,
                {
                    __instance.Props.bodyPart = getPawnBrain;                                                                               // then replace target with the BodyPartDef from the Android, which contains artificial brain.
                }
            }
            static void Postfix(ref Pawn user, CompUseEffect_InstallImplant __instance, BodyPartDef __state)
            {
                BodyPartDef getPawnBrain = user.health.hediffSet.GetBrain().def;                                                            // What is pawn brain?
                if (getPawnBrain.LabelShort == ATR_BrainShortLabel)                                                                         // If pawn has artificial brain,
                {
                    __instance.Props.bodyPart = __state;                                                                                    // then restore the original BodyPartDef, __state, which contains human brain.
                }
            }
        }
        [HarmonyPatch(typeof(CompUseEffect_InstallImplant), nameof(CompUseEffect_InstallImplant.CanBeUsedBy))]
        static class CompUseEffect_InstallImplant_CanBeUsedBy
        {
            public static void Postfix(ref Pawn p, ref string failReason, ref bool __result)
            {
                BodyPartDef getPawnBrain = p.health.hediffSet.GetBrain().def;
                string psychicallyDeaf = (string)"InstallImplantPsychicallyDeaf".Translate();                                               // Set strings that the game expects to be used for the context menu
                string noBodyPart = (string)("InstallImplantNoBodyPart".Translate() + ": " + humanBrain);
                if (__result == false)                                                                                                      // If CanBeUsedBy() returned false,
                {
                    if ((getPawnBrain.LabelShort == ATR_BrainShortLabel) && (failReason == psychicallyDeaf || failReason == noBodyPart))    // and pawn is Android or is failing due to "stock" limitations,
                    {
                        __result = true;                                                                                                    // allow the Android pawn to install Mechlink implant.
                    }
                }
            }
        }
    }
}
