﻿using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.Items.Armors;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Components;
using static TabletopTweaks.Core.Main;
using TabletopTweaks.Core.NewComponents;
using TabletopTweaks.Core.Utilities;

namespace TabletopTweaks.Core.NewContent.FighterAdvancedArmorTrainings {
    class ArmorSpecialization {
        public static void AddArmorSpecialization() {
            var FighterClass = Resources.GetBlueprint<BlueprintCharacterClass>("48ac8db94d5de7645906c7d0ad3bcfbd");
            var FighterArmorTrainingProperty = Resources.GetModBlueprintReference<BlueprintUnitPropertyReference>("FighterArmorTrainingProperty");
            var ArmorFocusLight = Resources.GetBlueprint<BlueprintFeature>("3bc6e1d2b44b5bb4d92e6ba59577cf62");

            var ArmorSpecializationSelection = Helpers.CreateBlueprint<BlueprintFeatureSelection>("ArmorSpecializationSelection", bp => {
                bp.SetName("Armor Specialization");
                bp.SetDescription("The fighter selects one specific type of armor with which he is proficient, such as light or heavy. " +
                    "While wearing the selected type of armor, the fighter adds one-quarter of his fighter level to the armor’s " +
                    "armor bonus, up to a maximum bonus of +3 for light armor, +4 for medium armor, or +5 for heavy armor. This increase to the armor " +
                    "bonus doesn’t increase the benefit that the fighter gains from feats, class abilities, or other effects that are determined by his armor’s base " +
                    "armor bonus, including other advanced armor training options. A fighter can choose this option multiple times. Each time he chooses it, he applies " +
                    "its benefit to a different type of armor.");
                bp.m_AllFeatures = new BlueprintFeatureReference[0];
                bp.m_Features = new BlueprintFeatureReference[0];
                bp.IsClassFeature = true;
            });
            var ArmorSpecializationLightEffect = Helpers.CreateBlueprint<BlueprintFeature>("ArmorSpecializationLightEffect", bp => {
                bp.SetName("Light Armor Specialization");
                bp.SetDescription("Light Armor Specialization");
                bp.IsClassFeature = true;
                bp.HideInCharacterSheetAndLevelUp = true;
                bp.Ranks = 1;
                bp.AddComponent<AddContextStatBonus>(c => {
                    c.Descriptor = ModifierDescriptor.ArmorFocus;
                    c.Stat = StatType.AC;
                    c.Value = new ContextValue {
                        ValueType = ContextValueType.Rank,
                        ValueRank = AbilityRankType.StatBonus
                    };
                });
                bp.AddContextRankConfig(c => {
                    c.m_Type = AbilityRankType.StatBonus;
                    c.m_BaseValueType = ContextRankBaseValueType.CustomProperty;
                    c.m_CustomProperty = FighterArmorTrainingProperty;
                    c.m_Progression = ContextRankProgression.DivStep;
                    c.m_StartLevel = 1;
                    c.m_StepLevel = 4;
                    c.m_Max = 3;
                    c.m_Min = 1;
                    c.m_UseMin = true;
                    c.m_UseMax = true;
                });
            });
            var ArmorSpecializationLightFeature = Helpers.CreateBlueprint<BlueprintFeature>("ArmorSpecializationLightFeature", bp => {
                bp.m_Icon = ArmorFocusLight.Icon;
                bp.SetName("Light Armor Specialization");
                bp.SetDescription("The AC bonus granted by any light armor you equip increases by 1 for every 4 fighter levels you possess up to a maximum of 3.");
                bp.IsClassFeature = true;
                bp.Ranks = 1;
                bp.AddComponent<ArmorFeatureUnlock>(c => {
                    c.NewFact = ArmorSpecializationLightEffect.ToReference<BlueprintUnitFactReference>();
                    c.RequiredArmor = new ArmorProficiencyGroup[] { ArmorProficiencyGroup.Light };
                });
            });
            var ArmorSpecializationMediumEffect = Helpers.CreateBlueprint<BlueprintFeature>("ArmorSpecializationMediumEffect", bp => {
                bp.SetName("Medium Armor Specialization");
                bp.SetDescription("Medium Armor Specialization");
                bp.IsClassFeature = true;
                bp.HideInCharacterSheetAndLevelUp = true;
                bp.Ranks = 1;
                bp.AddComponent<AddContextStatBonus>(c => {
                    c.Descriptor = ModifierDescriptor.ArmorFocus;
                    c.Stat = StatType.AC;
                    c.Value = new ContextValue {
                        ValueType = ContextValueType.Rank,
                        ValueRank = AbilityRankType.StatBonus
                    };
                });
                bp.AddContextRankConfig(c => {
                    c.m_Type = AbilityRankType.StatBonus;
                    c.m_BaseValueType = ContextRankBaseValueType.CustomProperty;
                    c.m_CustomProperty = FighterArmorTrainingProperty;
                    c.m_Progression = ContextRankProgression.DivStep;
                    c.m_StartLevel = 1;
                    c.m_StepLevel = 4;
                    c.m_Max = 4;
                    c.m_Min = 1;
                    c.m_UseMin = true;
                    c.m_UseMax = true;
                });
            });
            var ArmorSpecializationMediumFeature = Helpers.CreateBlueprint<BlueprintFeature>("ArmorSpecializationMediumFeature", bp => {
                bp.m_Icon = ArmorFocusLight.Icon;
                bp.SetName("Medium Armor Specialization");
                bp.SetDescription("The AC bonus granted by any medium armor " +
                    "you equip increases by 1 for every 4 fighter levels you possess up to a maximum of 4.");
                bp.IsClassFeature = true;
                bp.Ranks = 1;
                bp.AddComponent<ArmorFeatureUnlock>(c => {
                    c.NewFact = ArmorSpecializationMediumEffect.ToReference<BlueprintUnitFactReference>();
                    c.RequiredArmor = new ArmorProficiencyGroup[] { ArmorProficiencyGroup.Medium };
                });
            });
            var ArmorSpecializationHeavyEffect = Helpers.CreateBlueprint<BlueprintFeature>("ArmorSpecializationHeavyEffect", bp => {
                bp.SetName("Heavy Armor Specialization");
                bp.SetDescription("Heavy Armor Specialization");
                bp.IsClassFeature = true;
                bp.HideInCharacterSheetAndLevelUp = true;
                bp.Ranks = 1;
                bp.AddComponent<AddContextStatBonus>(c => {
                    c.Descriptor = ModifierDescriptor.ArmorFocus;
                    c.Stat = StatType.AC;
                    c.Value = new ContextValue {
                        ValueType = ContextValueType.Rank,
                        ValueRank = AbilityRankType.StatBonus
                    };
                });
                bp.AddContextRankConfig(c => {
                    c.m_Type = AbilityRankType.StatBonus;
                    c.m_BaseValueType = ContextRankBaseValueType.CustomProperty;
                    c.m_CustomProperty = FighterArmorTrainingProperty;
                    c.m_Progression = ContextRankProgression.DivStep;
                    c.m_StartLevel = 1;
                    c.m_StepLevel = 4;
                    c.m_Max = 5;
                    c.m_Min = 1;
                    c.m_UseMin = true;
                    c.m_UseMax = true;
                });
            });
            var ArmorSpecializationHeavyFeature = Helpers.CreateBlueprint<BlueprintFeature>("ArmorSpecializationHeavyFeature", bp => {
                bp.m_Icon = ArmorFocusLight.Icon;
                bp.SetName("Heavy Armor Specialization");
                bp.SetDescription("The AC bonus " +
                    "granted by any heavy armor you equip increases by 1 for every 4 fighter levels you possess up to a maximum of 5.");
                bp.IsClassFeature = true;
                bp.Ranks = 1;
                bp.AddComponent<ArmorFeatureUnlock>(c => {
                    c.NewFact = ArmorSpecializationHeavyEffect.ToReference<BlueprintUnitFactReference>();
                    c.RequiredArmor = new ArmorProficiencyGroup[] { ArmorProficiencyGroup.Heavy };
                });
            });

            ArmorSpecializationSelection.AddFeatures(ArmorSpecializationLightFeature, ArmorSpecializationMediumFeature, ArmorSpecializationHeavyFeature);
            if (ModContext.AddedContent.FighterAdvancedArmorTraining.IsDisabled("ArmorSpecialization")) { return; }
            AdvancedArmorTraining.AddToAdvancedArmorTrainingSelection(ArmorSpecializationSelection);
        }
    }
}
