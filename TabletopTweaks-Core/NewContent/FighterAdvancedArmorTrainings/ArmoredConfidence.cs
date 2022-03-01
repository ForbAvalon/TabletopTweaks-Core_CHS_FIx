﻿using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Items.Armors;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Components;
using TabletopTweaks.Core.NewComponents;
using TabletopTweaks.Core.Utilities;
using TabletopTweaks.Core.Wrappers;
using static TabletopTweaks.Core.Main;

namespace TabletopTweaks.Core.NewContent.FighterAdvancedArmorTrainings {
    class ArmoredConfidence {
        public static void AddArmoredConfidence() {
            var FighterClass = Resources.GetBlueprint<BlueprintCharacterClass>("48ac8db94d5de7645906c7d0ad3bcfbd");
            var FighterArmorTrainingProperty = Resources.GetModBlueprintReference<BlueprintUnitPropertyReference>(modContext: TTTContext, "FighterArmorTrainingProperty");

            var ArmoredConfidenceLightEffect = Helpers.CreateBlueprint<BlueprintFeature>(modContext: TTTContext, "ArmoredConfidenceLightEffect", bp => {
                bp.SetName("Armored Confidence Effect");
                bp.SetDescription("Armored Confidence");
                bp.IsClassFeature = true;
                bp.HideInCharacterSheetAndLevelUp = true;
                bp.Ranks = 1;
                bp.AddComponent<AddContextStatBonus>(c => {
                    c.Descriptor = ModifierDescriptor.UntypedStackable;
                    c.Stat = StatType.CheckIntimidate;
                    c.Value = new ContextValue {
                        ValueType = ContextValueType.Rank,
                        ValueRank = AbilityRankType.StatBonus
                    };
                });
                bp.AddComponent<AddStatBonus>(c => {
                    c.Descriptor = ModifierDescriptor.UntypedStackable;
                    c.Stat = StatType.CheckIntimidate;
                    c.Value = 1;
                });
                bp.AddContextRankConfig(c => {
                    c.m_Type = AbilityRankType.StatBonus;
                    c.m_BaseValueType = ContextRankBaseValueType.CustomProperty;
                    c.m_CustomProperty = FighterArmorTrainingProperty;
                    c.m_Progression = ContextRankProgression.DelayedStartPlusDivStep;
                    c.m_StartLevel = 7;
                    c.m_StepLevel = 4;
                    c.m_Max = 4;
                    c.m_Min = 1;
                    c.m_UseMax = true;
                });
            });

            var ArmoredConfidenceMediumEffect = Helpers.CreateBlueprint<BlueprintFeature>(modContext: TTTContext, "ArmoredConfidenceMediumEffect", bp => {
                bp.SetName("Armored Confidence Effect");
                bp.SetDescription("Armored Confidence");
                bp.IsClassFeature = true;
                bp.HideInCharacterSheetAndLevelUp = true;
                bp.Ranks = 1;
                bp.AddComponent<AddContextStatBonus>(c => {
                    c.Descriptor = ModifierDescriptor.UntypedStackable;
                    c.Stat = StatType.CheckIntimidate;
                    c.Value = new ContextValue {
                        ValueType = ContextValueType.Rank,
                        ValueRank = AbilityRankType.StatBonus
                    };
                });
                bp.AddComponent<AddStatBonus>(c => {
                    c.Descriptor = ModifierDescriptor.UntypedStackable;
                    c.Stat = StatType.CheckIntimidate;
                    c.Value = 2;
                });
                bp.AddContextRankConfig(c => {
                    c.m_Type = AbilityRankType.StatBonus;
                    c.m_BaseValueType = ContextRankBaseValueType.CustomProperty;
                    c.m_CustomProperty = FighterArmorTrainingProperty;
                    c.m_Progression = ContextRankProgression.DelayedStartPlusDivStep;
                    c.m_StartLevel = 7;
                    c.m_StepLevel = 4;
                    c.m_Max = 4;
                    c.m_Min = 1;
                    c.m_UseMax = true;
                });
            });
            var ArmoredConfidenceHeavyEffect = Helpers.CreateBlueprint<BlueprintFeature>(modContext: TTTContext, "ArmoredConfidenceHeavyEffect", bp => {
                bp.SetName("Armored Confidence Effect");
                bp.SetDescription("Armored Confidence");
                bp.IsClassFeature = true;
                bp.HideInCharacterSheetAndLevelUp = true;
                bp.Ranks = 1;
                bp.AddComponent<AddContextStatBonus>(c => {
                    c.Descriptor = ModifierDescriptor.UntypedStackable;
                    c.Stat = StatType.CheckIntimidate;
                    c.Value = new ContextValue {
                        ValueType = ContextValueType.Rank,
                        ValueRank = AbilityRankType.StatBonus
                    };
                });
                bp.AddComponent<AddStatBonus>(c => {
                    c.Descriptor = ModifierDescriptor.UntypedStackable;
                    c.Stat = StatType.CheckIntimidate;
                    c.Value = 3;
                });
                bp.AddContextRankConfig(c => {
                    c.m_Type = AbilityRankType.StatBonus;
                    c.m_BaseValueType = ContextRankBaseValueType.CustomProperty;
                    c.m_CustomProperty = FighterArmorTrainingProperty;
                    c.m_Progression = ContextRankProgression.DelayedStartPlusDivStep;
                    c.m_StartLevel = 7;
                    c.m_StepLevel = 4;
                    c.m_Max = 4;
                    c.m_Min = 1;
                    c.m_UseMax = true;
                });
            });
            var ArmoredConfidenceFeature = Helpers.CreateBlueprint<BlueprintFeature>(modContext: TTTContext, "ArmoredConfidenceFeature", bp => {
                bp.SetName("Armored Confidence");
                bp.SetDescription("While wearing armor, the fighter gains a bonus on Intimidate checks based upon the type of armor he is wearing: " +
                    "+1 for light armor, +2 for medium armor, or +3 for heavy armor. This bonus increases by 1 at 7th level and every 4 fighter " +
                    "levels thereafter, to a maximum of +4 at 19th level.");
                bp.IsClassFeature = true;
                bp.Ranks = 1;
                bp.AddComponent<ArmorFeatureUnlock>(c => {
                    c.NewFact = ArmoredConfidenceLightEffect.ToReference<BlueprintUnitFactReference>();
                    c.RequiredArmor = new ArmorProficiencyGroup[] { ArmorProficiencyGroup.Light };
                });
                bp.AddComponent<ArmorFeatureUnlock>(c => {
                    c.NewFact = ArmoredConfidenceMediumEffect.ToReference<BlueprintUnitFactReference>();
                    c.RequiredArmor = new ArmorProficiencyGroup[] { ArmorProficiencyGroup.Medium };
                });
                bp.AddComponent<ArmorFeatureUnlock>(c => {
                    c.NewFact = ArmoredConfidenceHeavyEffect.ToReference<BlueprintUnitFactReference>();
                    c.RequiredArmor = new ArmorProficiencyGroup[] { ArmorProficiencyGroup.Heavy };
                });
            });

            if (TTTContext.AddedContent.FighterAdvancedArmorTraining.IsDisabled("ArmoredConfidence")) { return; }
            AdvancedArmorTraining.AddToAdvancedArmorTrainingSelection(ArmoredConfidenceFeature);
        }
    }
}
