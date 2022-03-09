﻿using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Prerequisites;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.Designers.Mechanics.Recommendations;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.UnitLogic.Commands.Base;
using Kingmaker.Utility;
using TabletopTweaks.Core.NewComponents;
using TabletopTweaks.Core.Utilities;
using TabletopTweaks.Core.Wrappers;
using static TabletopTweaks.Core.Main;

namespace TabletopTweaks.Core.NewContent.Feats {
    static class QuickChannel {
        public static void AddQuickChannel() {
            var SelectiveChannel = BlueprintTools.GetBlueprint<BlueprintFeature>("fd30c69417b434d47b6b03b9c1f568ff");
            var ExtraChannel = BlueprintTools.GetBlueprint<BlueprintFeature>("cd9f19775bd9d3343a31a065e93f0c47");

            var QuickChannel = Helpers.CreateBlueprint<BlueprintFeature>(modContext: TTTContext, "QuickChannel", bp => {
                bp.SetName("Quick Channel");
                bp.SetDescription("You may channel energy as a move action by spending 2 daily uses of that ability.");
                bp.m_Icon = SelectiveChannel.Icon;
                bp.Ranks = 1;
                bp.ReapplyOnLevelUp = true;
                bp.IsClassFeature = true;
                bp.Groups = new FeatureGroup[] { FeatureGroup.Feat };
                bp.AddComponent<AbilityActionTypeConversion>(c => {
                    c.ResourceMultiplier = 2;
                    c.ActionType = UnitCommand.CommandType.Move;
                    c.Descriptors = SpellDescriptor.ChannelNegativeHarm | SpellDescriptor.ChannelNegativeHeal | SpellDescriptor.ChannelPositiveHarm | SpellDescriptor.ChannelPositiveHeal;
                    c.CheckDescriptors = true;
                    c.RequireAoE = true;
                });
                bp.AddComponent(Helpers.Create<PureRecommendation>(c => {
                    c.Priority = RecommendationPriority.Good;
                }));
                bp.AddComponent(Helpers.Create<FeatureTagsComponent>(c => {
                    c.FeatureTags = FeatureTag.ClassSpecific;
                }));
                SelectiveChannel.GetComponents<PrerequisiteFeature>().ForEach(p => {
                    bp.AddPrerequisiteFeature(p.Feature, p.Group);
                });
                bp.AddPrerequisite<PrerequisiteStatValue>(p => {
                    p.Stat = StatType.SkillLoreReligion;
                    p.Value = 5;
                });
            });
            if (Main.TTTContext.AddedContent.Feats.IsDisabled("QuickChannel")) { return; }
            FeatTools.AddAsFeat(QuickChannel);
        }
    }
}
