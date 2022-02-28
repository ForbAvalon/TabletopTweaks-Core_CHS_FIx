﻿using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Components;
using static TabletopTweaks.Core.Main;
using TabletopTweaks.Core.Utilities;

namespace TabletopTweaks.Core.NewContent.MythicAbilities {
    static class AbundantBombs {
        public static void AddAbundantBombs() {
            var AlchemistBombsFeature = Resources.GetBlueprint<BlueprintFeature>("c59b2f256f5a70a4d896568658315b7d");
            var AlchemistBombsResource = Resources.GetBlueprintReference<BlueprintAbilityResourceReference>("1633025edc9d53f4691481b48248edd7");

            var AbundantBombsFeature = Helpers.CreateBlueprint<BlueprintFeature>("AbundantBombsFeature", bp => {
                bp.SetName("Abundant Bombs");
                bp.SetDescription("You've leaned a way to increase the number of bombs you can use in a day.\n" +
                    "Benefit: You can throw a number of additional bombs per day equal to twice your mythic rank.");
                bp.m_Icon = AlchemistBombsFeature.Icon;
                bp.Ranks = 1;
                bp.IsClassFeature = true;
                bp.ReapplyOnLevelUp = true;
                bp.Groups = new FeatureGroup[] { FeatureGroup.MythicAbility };
                bp.AddComponent<IncreaseResourceAmountBySharedValue>(c => {
                    c.Value = new ContextValue() {
                        ValueType = ContextValueType.Rank
                    };
                    c.m_Resource = AlchemistBombsResource;
                });
                bp.AddContextRankConfig(c => {
                    c.m_BaseValueType = ContextRankBaseValueType.MythicLevel;
                    c.m_Progression = ContextRankProgression.MultiplyByModifier;
                    c.m_StepLevel = 2;
                });
                bp.AddPrerequisiteFeature(AlchemistBombsFeature);
            });

            if (TTTContext.AddedContent.MythicAbilities.IsDisabled("AbundantBombs")) { return; }
            FeatTools.AddAsMythicAbility(AbundantBombsFeature);
        }
    }
}
