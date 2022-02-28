﻿using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Prerequisites;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.UnitLogic.Mechanics.Properties;
using System;
using static TabletopTweaks.Core.Main;
using TabletopTweaks.Core.NewComponents;
using TabletopTweaks.Core.NewComponents.Properties;
using TabletopTweaks.Core.Utilities;

namespace TabletopTweaks.Core.NewContent.FighterAdvancedArmorTrainings {
    public class AdvancedArmorTraining {
        public static void AddAdvancedArmorTraining() {
            var FighterClass = Resources.GetBlueprint<BlueprintCharacterClass>("48ac8db94d5de7645906c7d0ad3bcfbd");
            var ArmorTraining = Resources.GetBlueprint<BlueprintFeature>("3c380607706f209499d951b29d3c44f3");
            var FighterFeatSelection = Resources.GetBlueprint<BlueprintFeatureSelection>("41c8486641f7d6d4283ca9dae4147a9f");
            var FighterTrainingFakeLevel = Resources.GetModBlueprintReference<BlueprintUnitFactReference>("FighterTrainingFakeLevel");

            var FighterArmorTrainingProperty = Helpers.CreateBlueprint<BlueprintUnitProperty>("FighterArmorTrainingProperty", bp => {
                bp.AddComponent<CompositeCustomPropertyGetter>(c => {
                    c.CalculationMode = CompositeCustomPropertyGetter.Mode.Sum;
                    c.Properties = new CompositeCustomPropertyGetter.ComplexCustomProperty[] {
                        new CompositeCustomPropertyGetter.ComplexCustomProperty(){
                            Property = new FactRankGetter(){
                                m_Fact = FighterTrainingFakeLevel
                            }
                        },
                        new CompositeCustomPropertyGetter.ComplexCustomProperty(){
                            Property = new ClassLevelGetter(){
                                m_Class = FighterClass.ToReference<BlueprintCharacterClassReference>()
                            }
                        }
                    };
                });
            });
            var ArmorTrainingSpeedFeature = Helpers.CreateBlueprint<BlueprintFeature>("ArmorTrainingSpeedFeature", bp => {
                bp.Ranks = 5;
                bp.SetName("Armor Training Speed");
                bp.SetDescription("");
                bp.HideInUI = true;
                bp.HideInCharacterSheetAndLevelUp = true;
                bp.IsClassFeature = true;
                bp.AddComponent<ArmorSpeedPenaltyRemoval>();
            });
            var AdvancedArmorTrainingSelection = Helpers.CreateBlueprint<BlueprintFeatureSelection>("AdvancedArmorTrainingSelection", bp => {
                bp.SetName("Advanced Armor Training");
                bp.SetDescription("Beginning at 7th level, instead of increasing the benefits provided by armor training " +
                    "(reducing his armor check penalty by 1 and increasing its maximum Dexterity bonus by 1), a fighter can " +
                    "choose an advanced armor training option (see Advanced Armor Training below) . If the fighter does so, " +
                    "he still gains the ability to move at his normal speed while wearing medium armor at 3rd level, and while wearing heavy armor at 7th level.");
                bp.m_AllFeatures = new BlueprintFeatureReference[0];
                bp.m_Features = new BlueprintFeatureReference[0];
                bp.IsClassFeature = true;
                bp.AddPrerequisites(Helpers.Create<PrerequisiteClassLevel>(c => {
                    c.m_CharacterClass = FighterClass.ToReference<BlueprintCharacterClassReference>();
                    c.Level = 7;
                }));
            });
            var AdvancedArmorTraining1 = CreateAdvancedArmorFeat("AdvancedArmorTraining1", bp => {
                bp.AddPrerequisites(Helpers.Create<PrerequisiteClassLevel>(c => {
                    c.m_CharacterClass = FighterClass.ToReference<BlueprintCharacterClassReference>();
                    c.Level = 3;
                }));
                bp.AddPrerequisites(Helpers.Create<PrerequisiteFeature>(c => {
                    c.m_Feature = ArmorTraining.ToReference<BlueprintFeatureReference>();
                }));
                bp.AddPrerequisites(Helpers.Create<PrerequisiteNoFeature>(c => {
                    c.m_Feature = bp.ToReference<BlueprintFeatureReference>();
                    c.HideInUI = true;
                }));
            });
            var AdvancedArmorTraining2 = CreateAdvancedArmorFeat("AdvancedArmorTraining2", bp => {
                bp.AddPrerequisites(Helpers.Create<PrerequisiteClassLevel>(c => {
                    c.m_CharacterClass = FighterClass.ToReference<BlueprintCharacterClassReference>();
                    c.Level = 6;
                }));
                bp.AddPrerequisites(Helpers.Create<PrerequisiteFeature>(c => {
                    c.m_Feature = AdvancedArmorTraining1.ToReference<BlueprintFeatureReference>();
                    c.HideInUI = true;
                }));
                bp.AddPrerequisites(Helpers.Create<PrerequisiteNoFeature>(c => {
                    c.m_Feature = bp.ToReference<BlueprintFeatureReference>();
                    c.HideInUI = true;
                }));
            });
            var AdvancedArmorTraining3 = CreateAdvancedArmorFeat("AdvancedArmorTraining3", bp => {
                bp.AddPrerequisites(Helpers.Create<PrerequisiteClassLevel>(c => {
                    c.m_CharacterClass = FighterClass.ToReference<BlueprintCharacterClassReference>();
                    c.Level = 9;
                }));
                bp.AddPrerequisites(Helpers.Create<PrerequisiteFeature>(c => {
                    c.m_Feature = AdvancedArmorTraining2.ToReference<BlueprintFeatureReference>();
                    c.HideInUI = true;
                }));
                bp.AddPrerequisites(Helpers.Create<PrerequisiteNoFeature>(c => {
                    c.m_Feature = bp.ToReference<BlueprintFeatureReference>();
                    c.HideInUI = true;
                }));
            });
            var AdvancedArmorTraining4 = CreateAdvancedArmorFeat("AdvancedArmorTraining4", bp => {
                bp.AddPrerequisites(Helpers.Create<PrerequisiteClassLevel>(c => {
                    c.m_CharacterClass = FighterClass.ToReference<BlueprintCharacterClassReference>();
                    c.Level = 12;
                }));
                bp.AddPrerequisites(Helpers.Create<PrerequisiteFeature>(c => {
                    c.m_Feature = AdvancedArmorTraining3.ToReference<BlueprintFeatureReference>();
                    c.HideInUI = true;
                }));
                bp.AddPrerequisites(Helpers.Create<PrerequisiteNoFeature>(c => {
                    c.m_Feature = bp.ToReference<BlueprintFeatureReference>();
                    c.HideInUI = true;
                }));
            });
            var AdvancedArmorTraining5 = CreateAdvancedArmorFeat("AdvancedArmorTraining5", bp => {
                bp.AddPrerequisites(Helpers.Create<PrerequisiteClassLevel>(c => {
                    c.m_CharacterClass = FighterClass.ToReference<BlueprintCharacterClassReference>();
                    c.Level = 15;
                }));
                bp.AddPrerequisites(Helpers.Create<PrerequisiteFeature>(c => {
                    c.m_Feature = AdvancedArmorTraining4.ToReference<BlueprintFeatureReference>();
                    c.HideInUI = true;
                }));
                bp.AddPrerequisites(Helpers.Create<PrerequisiteNoFeature>(c => {
                    c.m_Feature = bp.ToReference<BlueprintFeatureReference>();
                    c.HideInUI = true;
                }));
            });
            var AdvancedArmorTraining6 = CreateAdvancedArmorFeat("AdvancedArmorTraining6", bp => {
                bp.AddPrerequisites(Helpers.Create<PrerequisiteClassLevel>(c => {
                    c.m_CharacterClass = FighterClass.ToReference<BlueprintCharacterClassReference>();
                    c.Level = 18;
                }));
                bp.AddPrerequisites(Helpers.Create<PrerequisiteFeature>(c => {
                    c.m_Feature = AdvancedArmorTraining5.ToReference<BlueprintFeatureReference>();
                    c.HideInUI = true;
                }));
                bp.AddPrerequisites(Helpers.Create<PrerequisiteNoFeature>(c => {
                    c.m_Feature = bp.ToReference<BlueprintFeatureReference>();
                    c.HideInUI = true;
                }));
            });

            var ArmorTrainingSelection = Helpers.CreateBlueprint<BlueprintFeatureSelection>("ArmorTrainingSelection", bp => {
                bp.Ranks = 4;
                bp.m_Icon = ArmorTraining.Icon;
                bp.SetName("Armor Training");
                bp.SetDescription(ArmorTraining.m_Description);
                bp.m_AllFeatures = new BlueprintFeatureReference[] {
                    ArmorTraining.ToReference<BlueprintFeatureReference>(),
                    AdvancedArmorTrainingSelection.ToReference<BlueprintFeatureReference>()
                };
                bp.m_Features = new BlueprintFeatureReference[0];
                bp.IsClassFeature = true;
                bp.AddComponent(Helpers.Create<SelectionDefaultFeature>(c => {
                    c.DefaultFeature = ArmorTraining.ToReference<BlueprintFeatureReference>();
                }));
            });

            if (TTTContext.AddedContent.FighterAdvancedArmorTraining.IsDisabled("Feats")) { return; }
            FeatTools.AddAsFeat(
                AdvancedArmorTraining1,
                AdvancedArmorTraining2,
                AdvancedArmorTraining3,
                AdvancedArmorTraining4,
                AdvancedArmorTraining5,
                AdvancedArmorTraining6
            );
            FighterFeatSelection.AddFeatures(
                AdvancedArmorTraining1,
                AdvancedArmorTraining2,
                AdvancedArmorTraining3,
                AdvancedArmorTraining4,
                AdvancedArmorTraining5,
                AdvancedArmorTraining6
            );

            BlueprintFeatureSelection CreateAdvancedArmorFeat(string name, Action<BlueprintFeatureSelection> init = null) {
                var ArmorTrainingFeat = Helpers.CreateBlueprint<BlueprintFeatureSelection>(name, bp => {
                    bp.SetName("Advanced Armor Training");
                    bp.SetDescription("Select one advanced armor training option.");
                    bp.Groups = new FeatureGroup[] {
                        FeatureGroup.CombatFeat,
                        FeatureGroup.Feat
                    };
                    bp.m_AllFeatures = new BlueprintFeatureReference[0];
                    bp.m_Features = new BlueprintFeatureReference[0];
                    bp.IsClassFeature = true;
                    bp.HideNotAvailibleInUI = true;
                });
                init?.Invoke(ArmorTrainingFeat);
                return ArmorTrainingFeat;
            }
        }
        public static void AddToAdvancedArmorTrainingSelection(params BlueprintFeature[] features) {
            var AdvancedArmorTrainingSelection = Resources.GetModBlueprint<BlueprintFeatureSelection>("AdvancedArmorTrainingSelection");
            var AdvancedArmorTraining1 = Resources.GetModBlueprint<BlueprintFeatureSelection>("AdvancedArmorTraining1");
            var AdvancedArmorTraining2 = Resources.GetModBlueprint<BlueprintFeatureSelection>("AdvancedArmorTraining2");
            var AdvancedArmorTraining3 = Resources.GetModBlueprint<BlueprintFeatureSelection>("AdvancedArmorTraining3");
            var AdvancedArmorTraining4 = Resources.GetModBlueprint<BlueprintFeatureSelection>("AdvancedArmorTraining4");
            var AdvancedArmorTraining5 = Resources.GetModBlueprint<BlueprintFeatureSelection>("AdvancedArmorTraining5");
            var AdvancedArmorTraining6 = Resources.GetModBlueprint<BlueprintFeatureSelection>("AdvancedArmorTraining6");

            AdvancedArmorTrainingSelection.AddFeatures(features);
            AdvancedArmorTraining1.AddFeatures(features);
            AdvancedArmorTraining2.AddFeatures(features);
            AdvancedArmorTraining3.AddFeatures(features);
            AdvancedArmorTraining4.AddFeatures(features);
            AdvancedArmorTraining5.AddFeatures(features);
            AdvancedArmorTraining6.AddFeatures(features);
        }
    }
}
