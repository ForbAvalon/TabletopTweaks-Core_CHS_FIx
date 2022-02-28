﻿using Kingmaker.Blueprints.Classes;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.UnitLogic.FactLogic;
using TabletopTweaks.Core.Utilities;
using static TabletopTweaks.Core.Main;

namespace TabletopTweaks.Core.NewContent.MythicFeats {
    static class MythicCombatReflexes {
        public static void AddMythicCombatReflexes() {
            var CombatReflexes = Resources.GetBlueprint<BlueprintFeature>("0f8939ae6f220984e8fb568abbdfba95");
            var CombatReflexesMythicFeature = Helpers.CreateBlueprint<BlueprintFeature>("CombatReflexesMythicFeature", bp => {
                bp.m_Icon = CombatReflexes.m_Icon;
                bp.SetName("Combat Reflexes (Mythic)");
                bp.SetDescription("You strike viciously whenever your foe gives you an opening.\n" +
                    "You can make any number of additional attacks of opportunity per round.");
                bp.IsClassFeature = true;
                bp.Ranks = 1;
                bp.Groups = new FeatureGroup[] { FeatureGroup.MythicFeat };
                bp.AddComponent<AddStatBonus>(c => {
                    c.Stat = StatType.AttackOfOpportunityCount;
                    c.Descriptor = ModifierDescriptor.UntypedStackable;
                    c.Value = 1000;
                });
                bp.AddPrerequisiteFeature(CombatReflexes);
            });
            if (TTTContext.AddedContent.MythicFeats.IsDisabled("MythicCombatReflexes")) { return; }
            FeatTools.AddAsMythicFeat(CombatReflexesMythicFeature);
        }
    }
}
