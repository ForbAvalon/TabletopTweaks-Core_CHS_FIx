﻿using HarmonyLib;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.UnitLogic.FactLogic;
using static TabletopTweaks.Core.Main;
using TabletopTweaks.Core.NewComponents.AbilitySpecific;
using TabletopTweaks.Core.Utilities;

namespace TabletopTweaks.Core.Reworks {
    class MythicFeats {
        [HarmonyPatch(typeof(BlueprintsCache), "Init")]
        static class BlueprintsCache_Init_Patch {
            static bool Initialized;

            static void Postfix() {
                if (Initialized) return;
                Initialized = true;
                TTTContext.Logger.LogHeader("Reworking Mythic Feats");
                PatchMythicSneakAttack();
            }
            static void PatchMythicSneakAttack() {
                if (TTTContext.Homebrew.MythicFeats.IsDisabled("MythicSneakAttack")) { return; }

                var SneakAttackerMythicFeat = Resources.GetBlueprint<BlueprintFeature>("d0a53bf03b978634890e5ebab4a90ecb");

                SneakAttackerMythicFeat.RemoveComponents<AddStatBonus>();
                SneakAttackerMythicFeat.AddComponent<MythicSneakAttack>();
                SneakAttackerMythicFeat.SetDescription("Your sneak attacks are especially deadly.\n" +
                    "Benifit: Your sneak attack dice are one size larger than normal. " +
                    "For example if you would normally roll d6s for sneak attacks you would roll d8s instead.");
                TTTContext.Logger.LogPatch("Patched", SneakAttackerMythicFeat);
            }
        }
    }
}
