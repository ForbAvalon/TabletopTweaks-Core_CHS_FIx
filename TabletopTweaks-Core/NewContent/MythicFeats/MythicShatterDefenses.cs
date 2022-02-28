﻿using Kingmaker.Blueprints.Classes;
using TabletopTweaks.Core.NewComponents;
using TabletopTweaks.Core.Utilities;
using static TabletopTweaks.Core.Main;

namespace TabletopTweaks.Core.NewContent.MythicFeats {
    static class MythicShatterDefenses {
        public static void AddMythicShatterDefenses() {
            var ShatterDefenses = Resources.GetBlueprint<BlueprintFeature>("61a17ccbbb3d79445b0926347ec07577");
            var ShatterDefensesMythicBuff = Helpers.CreateBuff("ShatterDefensesMythicBuff", bp => {
                bp.m_Icon = ShatterDefenses.m_Icon;
                bp.SetName("Shattered Defenses (Mythic)");
                bp.SetDescription("An opponent affected by Shatter Defenses is flat-footed to all attacks.");
                bp.AddComponent<ForceFlatFooted>();
            });
            var ShatterDefensesMythicFeat = Helpers.CreateBlueprint<BlueprintFeature>("ShatterDefensesMythicFeat", bp => {
                bp.m_Icon = ShatterDefenses.m_Icon;
                bp.SetName("Shatter Defenses (Mythic)");
                bp.SetDescription("Your dazzling attacks leave your opponents flummoxed and bewildered, unable to attack you or to defend themselves effectively.\n" +
                    "An opponent you affect with Shatter Defenses is flat-footed to all attacks, not just yours.");
                bp.IsClassFeature = true;
                bp.Ranks = 1;
                bp.Groups = new FeatureGroup[] { FeatureGroup.MythicFeat };
                bp.AddPrerequisiteFeature(ShatterDefenses);
            });
            if (TTTContext.Fixes.Feats.IsDisabled("ShatterDefenses")) { return; }
            if (TTTContext.AddedContent.MythicFeats.IsDisabled("MythicShatterDefenses")) { return; }
            FeatTools.AddAsMythicFeat(ShatterDefensesMythicFeat);
        }
    }
}
