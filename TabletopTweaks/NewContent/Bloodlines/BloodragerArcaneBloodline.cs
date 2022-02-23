﻿using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Designers.EventConditionActionSystem.Actions;
using Kingmaker.ElementsSystem;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Actions;
using Kingmaker.UnitLogic.Mechanics.Components;
using Kingmaker.UnitLogic.Mechanics.Conditions;
using Kingmaker.UnitLogic.Mechanics.Properties;
using System.Collections.Generic;
using TabletopTweaks.Core.Extensions;
using TabletopTweaks.Core.NewComponents.Properties;
using TabletopTweaks.Core.Utilities;

namespace TabletopTweaks.Core.NewContent.Bloodlines {
    static class BloodragerArcaneBloodline {

        public static void AddArcaneBloodrageReworkToggles() {
            // Arcane Bloodrage
            var BloodragerClass = Resources.GetBlueprint<BlueprintCharacterClass>("d77e67a814d686842802c9cfd8ef8499");
            var BloodragerArcaneBloodline = Resources.GetBlueprint<BlueprintProgression>("aeff0a749e20ffe4b9e2846eae29c386");
            var BloodragerStandartRageBuff = Resources.GetBlueprint<BlueprintBuff>("5eac31e457999334b98f98b60fc73b2f");
            var BloodragerArcaneSpellFeature = Resources.GetBlueprint<BlueprintFeature>("3584b932341ecf14fbaaa87bf337c2cf");
            var BloodragerArcaneSpellAbility = Resources.GetBlueprint<BlueprintAbility>("3151dfeeb202e38448d1fea1e8bc237e");

            var Blur = Resources.GetBlueprint<BlueprintAbility>("14ec7a4e52e90fa47a4c8d63c69fd5c1");
            var BlurBuff = Resources.GetBlueprint<BlueprintBuff>("dd3ad347240624d46a11a092b4dd4674");

            var ProtectionFromArrows = Resources.GetBlueprint<BlueprintAbility>("c28de1f98a3f432448e52e5d47c73208");
            var ProtectionFromArrowsBuff = Resources.GetBlueprint<BlueprintBuff>("241ee6bd8c8767343994bce5dc1a95e0");

            var BloodragerArcaneProgressionProperty = Helpers.CreateBlueprint<BlueprintUnitProperty>("BloodragerArcaneProgressionProperty", bp => {
                bp.AddComponent<CompositeCustomPropertyGetter>(c => {
                    c.CalculationMode = CompositeCustomPropertyGetter.Mode.Highest;
                    c.Properties = new CompositeCustomPropertyGetter.ComplexCustomProperty[] {
                        new CompositeCustomPropertyGetter.ComplexCustomProperty(){
                            Property = new ProgressionRankGetter(){
                                Progression = BloodragerArcaneBloodline.ToReference<BlueprintProgressionReference>(),
                                UseMax = true,
                                Max = 20
                            }
                        },
                        new CompositeCustomPropertyGetter.ComplexCustomProperty(){
                            Property = new ClassLevelGetter(){
                                m_Class = BloodragerClass.ToReference<BlueprintCharacterClassReference>()
                            }
                        }
                    };
                });
            });

            var ProtectionFromArrowsArcaneBloodragerBuff = Helpers.CreateBuff("ProtectionFromArrowsArcaneBloodrageBuff", bp => {
                bp.m_DisplayName = ProtectionFromArrows.m_DisplayName;
                bp.m_Description = ProtectionFromArrows.m_Description;
                bp.m_DescriptionShort = ProtectionFromArrows.m_DescriptionShort;
                bp.m_Icon = ProtectionFromArrowsBuff.m_Icon;
                bp.AddComponent<AddDamageResistancePhysical>(c => {
                    c.m_WeaponType = BlueprintReferenceBase.CreateTyped<BlueprintWeaponTypeReference>(null);
                    c.Or = true;
                    c.Material = Kingmaker.Enums.Damage.PhysicalDamageMaterial.Adamantite;
                    c.BypassedByMagic = true;
                    c.MinEnhancementBonus = 1;
                    c.Alignment = Kingmaker.Enums.Damage.DamageAlignment.Good;
                    c.Reality = Kingmaker.Enums.Damage.DamageRealityType.Ghost;
                    c.BypassedByMeleeWeapon = true;
                    c.m_CheckedFactMythic = BlueprintReferenceBase.CreateTyped<BlueprintUnitFactReference>(null);
                    c.Value = new ContextValue() {
                        Value = 10
                    };
                    c.UsePool = true;
                    c.Pool = new ContextValue() {
                        ValueType = ContextValueType.Rank
                    };
                });
                var crc = Helpers.CreateContextRankConfig();
                crc.m_BaseValueType = ContextRankBaseValueType.ClassLevel;
                crc.m_Progression = ContextRankProgression.MultiplyByModifier;
                crc.m_StepLevel = 10;
                crc.m_UseMax = true;
                crc.m_Max = 100;
                crc.m_Class = new BlueprintCharacterClassReference[] {
                        BloodragerClass.ToReference<BlueprintCharacterClassReference>()
                    };
                bp.AddComponent(crc);
            });

            var ResistFire = Resources.GetBlueprint<BlueprintAbility>("ddfb4ac970225f34dbff98a10a4a8844");
            var ResistFireBuff = Resources.GetBlueprint<BlueprintBuff>("468877871a8e3ba41813a9697ec4eb4e");

            var ResistCold = Resources.GetBlueprint<BlueprintAbility>("5368cecec375e1845ae07f48cdc09dd1");
            var ResistColdBuff = Resources.GetBlueprint<BlueprintBuff>("dfedc0bf1d93f024d85546314c42b56a");

            var ResistElectricity = Resources.GetBlueprint<BlueprintAbility>("90987584f54ab7a459c56c2d2f22cee2");
            var ResistElectricityBuff = Resources.GetBlueprint<BlueprintBuff>("17aee23103aee674082ff9891c82ae2f");

            var ResistAcid = Resources.GetBlueprint<BlueprintAbility>("fedc77de9b7aad54ebcc43b4daf8decd");
            var ResistAcidBuff = Resources.GetBlueprint<BlueprintBuff>("8d8f20391422c0e41a1650e7a9b7a21f");

            var ResistSonic = Resources.GetBlueprint<BlueprintAbility>("8d3b10f92387c84429ced317b06ad001");
            var ResistSonicBuff = Resources.GetBlueprint<BlueprintBuff>("c0f3b16ff3f79b749b121905d659a2d4");

            BlueprintBuff BloodragerArcaneSpellBlurSwitchBuff = BloodlineTools.CreateArcaneBloodrageSwitchBuff(
                "BloodragerArcaneSpellBlurSwitchBuff",
                BloodragerArcaneSpellAbility,
                BloodragerStandartRageBuff,
                BlurBuff,
                bp => bp.SetName("Arcane Bloodrage: Blur"));

            BlueprintBuff BloodragerArcaneSpellProtectionFromArrowsSwitchBuff = BloodlineTools.CreateArcaneBloodrageSwitchBuff(
                "BloodragerArcaneSpellProtectionFromArrowsSwitchBuff",
                BloodragerArcaneSpellAbility,
                BloodragerStandartRageBuff,
                ProtectionFromArrowsArcaneBloodragerBuff,
                bp => bp.SetName("Arcane Bloodrage: Protection From Arrows"));

            BlueprintBuff BloodragerArcaneSpellResistFireSwitchBuff = BloodlineTools.CreateArcaneBloodrageSwitchBuff(
                "BloodragerArcaneSpellResistFireSwitchBuff",
                BloodragerArcaneSpellAbility,
                BloodragerStandartRageBuff,
                ResistFireBuff,
                bp => bp.SetName("Arcane Bloodrage: Resist Fire"));

            BlueprintBuff BloodragerArcaneSpellResistColdSwitchBuff = BloodlineTools.CreateArcaneBloodrageSwitchBuff(
                "BloodragerArcaneSpellResistColdSwitchBuff",
                BloodragerArcaneSpellAbility,
                BloodragerStandartRageBuff,
                ResistColdBuff,
                bp => bp.SetName("Arcane Bloodrage: Resist Cold"));

            BlueprintBuff BloodragerArcaneSpellResistElectricitySwitchBuff = BloodlineTools.CreateArcaneBloodrageSwitchBuff(
                "BloodragerArcaneSpellResistElectricitySwitchBuff",
                BloodragerArcaneSpellAbility,
                BloodragerStandartRageBuff,
                ResistElectricityBuff,
                bp => bp.SetName("Arcane Bloodrage: Resist Electricity"));

            BlueprintBuff BloodragerArcaneSpellResistAcidSwitchBuff = BloodlineTools.CreateArcaneBloodrageSwitchBuff(
                "BloodragerArcaneSpellResistAcidSwitchBuff",
                BloodragerArcaneSpellAbility,
                BloodragerStandartRageBuff,
                ResistAcidBuff,
                bp => bp.SetName("Arcane Bloodrage: Resist Acid"));

            BlueprintBuff BloodragerArcaneSpellResistSonicSwitchBuff = BloodlineTools.CreateArcaneBloodrageSwitchBuff(
                "BloodragerArcaneSpellResistSonicSwitchBuff",
                BloodragerArcaneSpellAbility,
                BloodragerStandartRageBuff,
                ResistSonicBuff,
                bp => bp.SetName("Arcane Bloodrage: Resist Sonic"));

            var AllBloodragerArcaneSpellSwitchBuffs = new List<BlueprintBuff>() {
                    BloodragerArcaneSpellBlurSwitchBuff,
                    BloodragerArcaneSpellProtectionFromArrowsSwitchBuff,
                    BloodragerArcaneSpellResistFireSwitchBuff,
                    BloodragerArcaneSpellResistColdSwitchBuff,
                    BloodragerArcaneSpellResistElectricitySwitchBuff,
                    BloodragerArcaneSpellResistAcidSwitchBuff,
                    BloodragerArcaneSpellResistSonicSwitchBuff
                };

            BlueprintAbility BloodragerArcaneSpellBlurToggle = BloodlineTools.CreateArcaneBloodrageToggle(
                "BloodragerArcaneSpellBlurToggle",
                Blur,
                BloodragerArcaneSpellAbility,
                BloodragerArcaneSpellBlurSwitchBuff,
                "BloodragerArcaneSpell",
                AllBloodragerArcaneSpellSwitchBuffs,
                BloodragerArcaneProgressionProperty);

            BlueprintAbility BloodragerArcaneSpellProtectionFromArrowsToggle = BloodlineTools.CreateArcaneBloodrageToggle(
                "BloodragerArcaneSpellProtectionFromArrowsToggle",
                ProtectionFromArrows,
                BloodragerArcaneSpellAbility,
                BloodragerArcaneSpellProtectionFromArrowsSwitchBuff,
                "BloodragerArcaneSpell",
                AllBloodragerArcaneSpellSwitchBuffs,
                BloodragerArcaneProgressionProperty);

            BlueprintAbility BloodragerArcaneSpellResistFireToggle = BloodlineTools.CreateArcaneBloodrageToggle(
                "BloodragerArcaneSpellResistFireToggle",
                ResistFire,
                BloodragerArcaneSpellAbility,
                BloodragerArcaneSpellResistFireSwitchBuff,
                "BloodragerArcaneSpell",
                AllBloodragerArcaneSpellSwitchBuffs,
                BloodragerArcaneProgressionProperty);

            BlueprintAbility BloodragerArcaneSpellResistColdToggle = BloodlineTools.CreateArcaneBloodrageToggle(
                "BloodragerArcaneSpellResistColdToggle",
                ResistCold,
                BloodragerArcaneSpellAbility,
                BloodragerArcaneSpellResistColdSwitchBuff,
                "BloodragerArcaneSpell",
                AllBloodragerArcaneSpellSwitchBuffs,
                BloodragerArcaneProgressionProperty);

            BlueprintAbility BloodragerArcaneSpellResistElectricityToggle = BloodlineTools.CreateArcaneBloodrageToggle(
                "BloodragerArcaneSpellResistElectricityToggle",
                ResistElectricity,
                BloodragerArcaneSpellAbility,
                BloodragerArcaneSpellResistElectricitySwitchBuff,
                "BloodragerArcaneSpell",
                AllBloodragerArcaneSpellSwitchBuffs,
                BloodragerArcaneProgressionProperty);

            BlueprintAbility BloodragerArcaneSpellResistAcidToggle = BloodlineTools.CreateArcaneBloodrageToggle(
                "BloodragerArcaneSpellResistAcidToggle",
                ResistAcid,
                BloodragerArcaneSpellAbility,
                BloodragerArcaneSpellResistAcidSwitchBuff,
                "BloodragerArcaneSpell",
                AllBloodragerArcaneSpellSwitchBuffs,
                BloodragerArcaneProgressionProperty);

            BlueprintAbility BloodragerArcaneSpellResistSonicToggle = BloodlineTools.CreateArcaneBloodrageToggle(
                "BloodragerArcaneSpellResistSonicToggle",
                ResistSonic,
                BloodragerArcaneSpellAbility,
                BloodragerArcaneSpellResistSonicSwitchBuff,
                "BloodragerArcaneSpell",
                AllBloodragerArcaneSpellSwitchBuffs,
                BloodragerArcaneProgressionProperty);

            // Greater Arcane Bloodrage
            var BloodragerArcaneGreaterSpell = Resources.GetBlueprint<BlueprintAbility>("31dbadf586920494b87e8e95452af998");

            var Displacement = Resources.GetBlueprint<BlueprintAbility>("903092f6488f9ce45a80943923576ab3");
            var DisplacementBuff = Resources.GetBlueprint<BlueprintBuff>("00402bae4442a854081264e498e7a833");

            var Haste = Resources.GetBlueprint<BlueprintAbility>("486eaff58293f6441a5c2759c4872f98");
            var HasteBuff = Resources.GetBlueprint<BlueprintBuff>("03464790f40c3c24aa684b57155f3280");
            var SlowBuff = Resources.GetBlueprint<BlueprintBuff>("0bc608c3f2b548b44b7146b7530613ac");

            BlueprintBuff BloodragerArcaneGreaterSpellHasteActivationBuff = Helpers.CreateBuff("BloodragerArcaneGreaterSpellHasteActivationBuff", bp => {
                bp.m_Flags = BlueprintBuff.Flags.HiddenInUi;
                bp.IsClassFeature = true;
                bp.SetName("Greater Arcane Bloodrage: Haste");
                bp.m_Description = HasteBuff.m_Description;
                bp.m_Icon = Haste.m_Icon;
                bp.AddComponent<AddFactContextActions>(c => {
                    c.Activated = new ActionList() {
                        Actions = new GameAction[] {
                                new Conditional() {
                                    name = "HasteActivationBuffCondition",
                                    Comment = "",
                                    ConditionsChecker = new ConditionsChecker() {
                                        Conditions = new Condition[] {
                                            new ContextConditionHasBuff {
                                                m_Buff = SlowBuff.ToReference<BlueprintBuffReference>()
                                            }
                                        }
                                    },
                                    IfTrue = new ActionList() {
                                        Actions = new GameAction[] {
                                            new ContextActionRemoveBuff() {
                                                m_Buff = SlowBuff.ToReference<BlueprintBuffReference>()
                                            }
                                        }
                                    },
                                    IfFalse = new ActionList() {
                                        Actions = new GameAction[] {
                                            new ContextActionApplyBuff() {
                                                m_Buff = HasteBuff.ToReference<BlueprintBuffReference>(),
                                                Permanent = true,
                                                AsChild = true,
                                                DurationValue = new ContextDurationValue(),
                                                IsFromSpell = false
                                            }
                                        }
                                    }
                                }
                            }
                    };
                    c.Deactivated = new ActionList();
                    c.NewRound = new ActionList();
                });
            });

            BlueprintBuff BloodragerArcaneGreaterSpellDisplacementSwitchBuff = BloodlineTools.CreateArcaneBloodrageSwitchBuff(
                "BloodragerArcaneGreaterSpellDisplacementSwitchBuff",
                BloodragerArcaneGreaterSpell,
                BloodragerStandartRageBuff,
                DisplacementBuff);

            BlueprintBuff BloodragerArcaneGreaterSpellHasteSwitchBuff = BloodlineTools.CreateArcaneBloodrageSwitchBuff(
                "BloodragerArcaneGreaterSpellHasteSwitchBuff",
                BloodragerArcaneGreaterSpell,
                BloodragerStandartRageBuff,
                BloodragerArcaneGreaterSpellHasteActivationBuff);

            var AllBloodragerArcaneGreaterSpellSwitchBuffs = new List<BlueprintBuff>() {
                    BloodragerArcaneGreaterSpellDisplacementSwitchBuff,
                    BloodragerArcaneGreaterSpellHasteSwitchBuff
                };


            BlueprintAbility BloodragerArcaneSpellGreaterDisplacementToggle = BloodlineTools.CreateArcaneBloodrageToggle(
                "BloodragerArcaneSpellGreaterDisplacementToggle",
                Displacement,
                BloodragerArcaneGreaterSpell,
                BloodragerArcaneGreaterSpellDisplacementSwitchBuff,
                "BloodragerArcaneSpellGreater",
                AllBloodragerArcaneGreaterSpellSwitchBuffs,
                BloodragerArcaneProgressionProperty);

            BlueprintAbility BloodragerArcaneSpellGreaterHasteToggle = BloodlineTools.CreateArcaneBloodrageToggle(
                "BloodragerArcaneSpellGreaterHasteToggle",
                Haste,
                BloodragerArcaneGreaterSpell,
                BloodragerArcaneGreaterSpellHasteSwitchBuff,
                "BloodragerArcaneSpellGreater",
                AllBloodragerArcaneGreaterSpellSwitchBuffs,
                BloodragerArcaneProgressionProperty);

            // True Arcane Bloodrage
            var BloodragerArcaneTrueSpellAbility = Resources.GetBlueprint<BlueprintAbility>("9d4d7f56d2d87f643b5ef990ef481094");

            var BeastShapeIVShamblingMoundAbility = Resources.GetBlueprint<BlueprintAbility>("b140c323981ba0a45a3bee5a1a57f493");
            var BeastShapeIVShamblingMoundBuff = Resources.GetBlueprint<BlueprintBuff>("50ab9c820eb9cf94d8efba3632ad5ce2");
            var BloodragerArcaneTrueSpellBeastShapIVShamblingMoundActivationBuff = BloodlineTools.CreateBloodragerTrueArcaneSpellRagePolymorphActivationBuff(
                "BloodragerArcaneTrueSpellBeastShapIVShamblingMoundActivationBuff",
                BeastShapeIVShamblingMoundBuff,
                bp => bp.SetName("True Arcane Bloodrage: Beast Shape IV (Shambling Mound)"));

            var BeastShapeIVSmilodonAbility = Resources.GetBlueprint<BlueprintAbility>("502cd7fd8953ac74bb3a3df7e84818ae");
            var BeastShapeIVSmilodonBuff = Resources.GetBlueprint<BlueprintBuff>("c38def68f6ce13b4b8f5e5e0c6e68d08");
            var BloodragerArcaneTrueSpellBeastShapIVSmilodonActivationBuff = BloodlineTools.CreateBloodragerTrueArcaneSpellRagePolymorphActivationBuff(
                "BloodragerArcaneTrueSpellBeastShapIVSmilodonActivationBuff",
                BeastShapeIVSmilodonBuff,
                bp => bp.SetName("True Arcane Bloodrage: Beast Shape IV (Smilodon)"));

            var BeastShapeIVWyvernAbility = Resources.GetBlueprint<BlueprintAbility>("3fa892e5e3efa364fb3d2692738a7c15");
            var BeastShapeIVWyvernBuff = Resources.GetBlueprint<BlueprintBuff>("dae2d173d9bd5b14dbeb4a1d9d9b0edc");
            var BloodragerArcaneTrueSpellBeastShapIVWyvernActivationBuff = BloodlineTools.CreateBloodragerTrueArcaneSpellRagePolymorphActivationBuff(
                "BloodragerArcaneTrueSpellBeastShapIVWyvernActivationBuff",
                BeastShapeIVWyvernBuff,
                bp => bp.SetName("True Arcane Bloodrage: Beast Shape IV (Wyvern)"));

            var FormOfTheDragonIAbility = Resources.GetBlueprint<BlueprintAbility>("f767399367df54645ac620ef7b2062bb");

            var FormOfTheDragonIBlackAbility = Resources.GetBlueprint<BlueprintAbility>("baeb1c45b53de864ca0c10784ce447f0");
            var FormOfTheDragonIBlackBuff = Resources.GetBlueprint<BlueprintBuff>("268fafac0a5b78c42a58bd9c1ae78bcf");
            var BloodragerArcaneTrueSpellFormOfTheDragonIBlackActivationBuff = BloodlineTools.CreateBloodragerTrueArcaneSpellRagePolymorphActivationBuff(
                "BloodragerArcaneTrueSpellFormOfTheDragonIBlackActivationBuff",
                FormOfTheDragonIBlackBuff,
                bp => bp.SetName("True Arcane Bloodrage: Dragonkind I (Black)"));

            var FormOfTheDragonIBlueAbility = Resources.GetBlueprint<BlueprintAbility>("7e889430ba65f724c81702101346e39a");
            var FormOfTheDragonIBlueBuff = Resources.GetBlueprint<BlueprintBuff>("b117bc8b41735924dba3fb23318f39ff");
            var BloodragerArcaneTrueSpellFormOfTheDragonIBlueActivationBuff = BloodlineTools.CreateBloodragerTrueArcaneSpellRagePolymorphActivationBuff(
                "BloodragerArcaneTrueSpellFormOfTheDragonIBlueActivationBuff",
                FormOfTheDragonIBlueBuff,
                bp => bp.SetName("True Arcane Bloodrage: Dragonkind I (Blue)"));

            var FormOfTheDragonIBrassAbility = Resources.GetBlueprint<BlueprintAbility>("2271bc6960317164aa61363ebe7c0228");
            var FormOfTheDragonIBrassBuff = Resources.GetBlueprint<BlueprintBuff>("17d330af03f5b3042a4417ab1d45e484");
            var BloodragerArcaneTrueSpellFormOfTheDragonIBrassActivationBuff = BloodlineTools.CreateBloodragerTrueArcaneSpellRagePolymorphActivationBuff(
                "BloodragerArcaneTrueSpellFormOfTheDragonIBrassActivationBuff",
                FormOfTheDragonIBrassBuff,
                bp => bp.SetName("True Arcane Bloodrage: Dragonkind I (Brass)"));

            var FormOfTheDragonIBronzeAbility = Resources.GetBlueprint<BlueprintAbility>("f1103c097be761e489ee27a8d49a373b");
            var FormOfTheDragonIBronzeBuff = Resources.GetBlueprint<BlueprintBuff>("1032d4ffb1c56444ca5bfce2c778614d");
            var BloodragerArcaneTrueSpellFormOfTheDragonIBronzeActivationBuff = BloodlineTools.CreateBloodragerTrueArcaneSpellRagePolymorphActivationBuff(
                "BloodragerArcaneTrueSpellFormOfTheDragonIBronzeActivationBuff",
                FormOfTheDragonIBronzeBuff,
                bp => bp.SetName("True Arcane Bloodrage: Dragonkind I (Bronze)"));

            var FormOfTheDragonICopperAbility = Resources.GetBlueprint<BlueprintAbility>("7ecab895312f8b541a712f965ee7afdb");
            var FormOfTheDragonICopperBuff = Resources.GetBlueprint<BlueprintBuff>("a4cc7169fb7e64a4a8f53bdc774341b1");
            var BloodragerArcaneTrueSpellFormOfTheDragonICopperActivationBuff = BloodlineTools.CreateBloodragerTrueArcaneSpellRagePolymorphActivationBuff(
                "BloodragerArcaneTrueSpellFormOfTheDragonICopperActivationBuff",
                FormOfTheDragonICopperBuff,
                bp => bp.SetName("True Arcane Bloodrage: Dragonkind I (Copper)"));

            var FormOfTheDragonIGoldAbility = Resources.GetBlueprint<BlueprintAbility>("12e6785ca0f97a145a7c02a5f0fd155c");
            var FormOfTheDragonIGoldBuff = Resources.GetBlueprint<BlueprintBuff>("89669cfba3d9c15448c23b79dd604c41");
            var BloodragerArcaneTrueSpellFormOfTheDragonIGoldActivationBuff = BloodlineTools.CreateBloodragerTrueArcaneSpellRagePolymorphActivationBuff(
                "BloodragerArcaneTrueSpellFormOfTheDragonIGoldActivationBuff",
                FormOfTheDragonIGoldBuff,
                bp => bp.SetName("True Arcane Bloodrage: Dragonkind I (Gold)"));

            var FormOfTheDragonIGreenAbility = Resources.GetBlueprint<BlueprintAbility>("9d649b9e77bcd3d4ea0f91b8512a3744");
            var FormOfTheDragonIGreenBuff = Resources.GetBlueprint<BlueprintBuff>("02611a12f38bed340920d1d427865917");
            var BloodragerArcaneTrueSpellFormOfTheDragonIGreenActivationBuff = BloodlineTools.CreateBloodragerTrueArcaneSpellRagePolymorphActivationBuff(
                "BloodragerArcaneTrueSpellFormOfTheDragonIGreenActivationBuff",
                FormOfTheDragonIGreenBuff,
                bp => bp.SetName("True Arcane Bloodrage: Dragonkind I (Green)"));

            var TransformationAbility = Resources.GetBlueprint<BlueprintAbility>("27203d62eb3d4184c9aced94f22e1806");
            var TransformationBuff = Resources.GetBlueprint<BlueprintBuff>("287682389d2011b41b5a65195d9cbc84");

            // SwitchBuffs
            BlueprintBuff BloodragerArcaneTrueSpellBeastShapeIVShamblingMoundSwitchBuff = BloodlineTools.CreateArcaneBloodrageSwitchBuff(
                "BloodragerArcaneTrueSpellBeastShapeIVShamblingMoundSwitchBuff",
                BloodragerArcaneTrueSpellAbility,
                BloodragerStandartRageBuff,
                BloodragerArcaneTrueSpellBeastShapIVShamblingMoundActivationBuff);

            BlueprintBuff BloodragerArcaneTrueSpellBeastShapeIVSmilodonSwitchBuff = BloodlineTools.CreateArcaneBloodrageSwitchBuff(
                "BloodragerArcaneTrueSpellBeastShapeIVSmilodonSwitchBuff",
                BloodragerArcaneTrueSpellAbility,
                BloodragerStandartRageBuff,
                BloodragerArcaneTrueSpellBeastShapIVSmilodonActivationBuff);

            BlueprintBuff BloodragerArcaneTrueSpellBeastShapeIVWyvernSwitchBuff = BloodlineTools.CreateArcaneBloodrageSwitchBuff(
                "BloodragerArcaneTrueSpellBeastShapeIVWyvernSwitchBuff",
                BloodragerArcaneTrueSpellAbility,
                BloodragerStandartRageBuff,
                BloodragerArcaneTrueSpellBeastShapIVWyvernActivationBuff);

            BlueprintBuff BloodragerArcaneTrueSpellFormOfTheDragonIBlackSwitchBuff = BloodlineTools.CreateArcaneBloodrageSwitchBuff(
                "BloodragerArcaneTrueSpellFormOfTheDragonIBlackSwitchBuff",
                BloodragerArcaneTrueSpellAbility,
                BloodragerStandartRageBuff,
                BloodragerArcaneTrueSpellFormOfTheDragonIBlackActivationBuff);

            BlueprintBuff BloodragerArcaneTrueSpellFormOfTheDragonIBlueSwitchBuff = BloodlineTools.CreateArcaneBloodrageSwitchBuff(
                "BloodragerArcaneTrueSpellFormOfTheDragonIBlueSwitchBuff",
                BloodragerArcaneTrueSpellAbility,
                BloodragerStandartRageBuff,
                BloodragerArcaneTrueSpellFormOfTheDragonIBlueActivationBuff);

            BlueprintBuff BloodragerArcaneTrueSpellFormOfTheDragonIBrassSwitchBuff = BloodlineTools.CreateArcaneBloodrageSwitchBuff(
                "BloodragerArcaneTrueSpellFormOfTheDragonIBrassSwitchBuff",
                BloodragerArcaneTrueSpellAbility,
                BloodragerStandartRageBuff,
                BloodragerArcaneTrueSpellFormOfTheDragonIBrassActivationBuff);

            BlueprintBuff BloodragerArcaneTrueSpellFormOfTheDragonIBronzeSwitchBuff = BloodlineTools.CreateArcaneBloodrageSwitchBuff(
                "BloodragerArcaneTrueSpellFormOfTheDragonIBronzeSwitchBuff",
                BloodragerArcaneTrueSpellAbility,
                BloodragerStandartRageBuff,
                BloodragerArcaneTrueSpellFormOfTheDragonIBronzeActivationBuff);

            BlueprintBuff BloodragerArcaneTrueSpellFormOfTheDragonICopperSwitchBuff = BloodlineTools.CreateArcaneBloodrageSwitchBuff(
                "BloodragerArcaneTrueSpellFormOfTheDragonICopperSwitchBuff",
                BloodragerArcaneTrueSpellAbility,
                BloodragerStandartRageBuff,
                BloodragerArcaneTrueSpellFormOfTheDragonICopperActivationBuff);

            BlueprintBuff BloodragerArcaneTrueSpellFormOfTheDragonIGoldSwitchBuff = BloodlineTools.CreateArcaneBloodrageSwitchBuff(
                "BloodragerArcaneTrueSpellFormOfTheDragonIGoldSwitchBuff",
                BloodragerArcaneTrueSpellAbility,
                BloodragerStandartRageBuff,
                BloodragerArcaneTrueSpellFormOfTheDragonIGoldActivationBuff);

            BlueprintBuff BloodragerArcaneTrueSpellFormOfTheDragonIGreenSwitchBuff = BloodlineTools.CreateArcaneBloodrageSwitchBuff(
                "BloodragerArcaneTrueSpellFormOfTheDragonIGreenSwitchBuff",
                BloodragerArcaneTrueSpellAbility,
                BloodragerStandartRageBuff,
                BloodragerArcaneTrueSpellFormOfTheDragonIGreenActivationBuff);

            BlueprintBuff BloodragerArcaneTrueSpellTransformationSwitchBuff = BloodlineTools.CreateArcaneBloodrageSwitchBuff(
                "BloodragerArcaneTrueSpellTransformationSwitchBuff",
                BloodragerArcaneTrueSpellAbility,
                BloodragerStandartRageBuff,
                TransformationBuff);

            var AllBloodragerArcaneTrueSpellSwitchBuffs = new List<BlueprintBuff>() {
                    BloodragerArcaneTrueSpellBeastShapeIVShamblingMoundSwitchBuff,
                    BloodragerArcaneTrueSpellBeastShapeIVSmilodonSwitchBuff,
                    BloodragerArcaneTrueSpellBeastShapeIVWyvernSwitchBuff,
                    BloodragerArcaneTrueSpellFormOfTheDragonIBlackSwitchBuff,
                    BloodragerArcaneTrueSpellFormOfTheDragonIBlueSwitchBuff,
                    BloodragerArcaneTrueSpellFormOfTheDragonIBrassSwitchBuff,
                    BloodragerArcaneTrueSpellFormOfTheDragonIBronzeSwitchBuff,
                    BloodragerArcaneTrueSpellFormOfTheDragonICopperSwitchBuff,
                    BloodragerArcaneTrueSpellFormOfTheDragonIGoldSwitchBuff,
                    BloodragerArcaneTrueSpellFormOfTheDragonIGreenSwitchBuff,
                    BloodragerArcaneTrueSpellTransformationSwitchBuff
                };

            // Toggles
            BlueprintAbility BloodragerArcaneSpellTrueBeastShapeIVShamblingMoundToggle = BloodlineTools.CreateArcaneBloodrageToggle(
                "BloodragerArcaneSpellTrueBeastShapeIVShamblingMoundToggle",
                BeastShapeIVShamblingMoundAbility,
                BloodragerArcaneTrueSpellAbility,
                BloodragerArcaneTrueSpellBeastShapeIVShamblingMoundSwitchBuff,
                "BloodragerArcaneTrueSpell",
                AllBloodragerArcaneTrueSpellSwitchBuffs,
                BloodragerArcaneProgressionProperty);

            BlueprintAbility BloodragerArcaneSpellTrueBeastShapeIVSmilodonToggle = BloodlineTools.CreateArcaneBloodrageToggle(
                "BloodragerArcaneSpellTrueBeastShapeIVSmilodonToggle",
                BeastShapeIVSmilodonAbility,
                BloodragerArcaneTrueSpellAbility,
                BloodragerArcaneTrueSpellBeastShapeIVSmilodonSwitchBuff,
                "BloodragerArcaneTrueSpell",
                AllBloodragerArcaneTrueSpellSwitchBuffs,
                BloodragerArcaneProgressionProperty);

            BlueprintAbility BloodragerArcaneSpellTrueBeastShapeIVWyvernToggle = BloodlineTools.CreateArcaneBloodrageToggle(
                "BloodragerArcaneSpellTrueBeastShapeIVWyvernToggle",
                BeastShapeIVWyvernAbility,
                BloodragerArcaneTrueSpellAbility,
                BloodragerArcaneTrueSpellBeastShapeIVWyvernSwitchBuff,
                "BloodragerArcaneTrueSpell",
                AllBloodragerArcaneTrueSpellSwitchBuffs,
                BloodragerArcaneProgressionProperty);

            BlueprintAbility BloodragerArcaneSpellTrueFormOfTheDragonIBlackToggle = BloodlineTools.CreateArcaneBloodrageToggle(
                "BloodragerArcaneSpellTrueFormOfTheDragonIBlackToggle",
                FormOfTheDragonIBlackAbility,
                BloodragerArcaneTrueSpellAbility,
                BloodragerArcaneTrueSpellFormOfTheDragonIBlackSwitchBuff,
                "BloodragerArcaneTrueSpell",
                AllBloodragerArcaneTrueSpellSwitchBuffs,
                BloodragerArcaneProgressionProperty);

            BlueprintAbility BloodragerArcaneSpellTrueFormOfTheDragonIBlueToggle = BloodlineTools.CreateArcaneBloodrageToggle(
                "BloodragerArcaneSpellTrueFormOfTheDragonIBlueToggle",
                FormOfTheDragonIBlueAbility,
                BloodragerArcaneTrueSpellAbility,
                BloodragerArcaneTrueSpellFormOfTheDragonIBlueSwitchBuff,
                "BloodragerArcaneTrueSpell",
                AllBloodragerArcaneTrueSpellSwitchBuffs,
                BloodragerArcaneProgressionProperty);

            BlueprintAbility BloodragerArcaneSpellTrueFormOfTheDragonIBrassToggle = BloodlineTools.CreateArcaneBloodrageToggle(
                "BloodragerArcaneSpellTrueFormOfTheDragonIBrassToggle",
                FormOfTheDragonIBrassAbility,
                BloodragerArcaneTrueSpellAbility,
                BloodragerArcaneTrueSpellFormOfTheDragonIBrassSwitchBuff,
                "BloodragerArcaneTrueSpell",
                AllBloodragerArcaneTrueSpellSwitchBuffs,
                BloodragerArcaneProgressionProperty);

            BlueprintAbility BloodragerArcaneSpellTrueFormOfTheDragonIBronzeToggle = BloodlineTools.CreateArcaneBloodrageToggle(
                "BloodragerArcaneSpellTrueFormOfTheDragonIBronzeToggle",
                FormOfTheDragonIBronzeAbility,
                BloodragerArcaneTrueSpellAbility,
                BloodragerArcaneTrueSpellFormOfTheDragonIBronzeSwitchBuff,
                "BloodragerArcaneTrueSpell",
                AllBloodragerArcaneTrueSpellSwitchBuffs,
                BloodragerArcaneProgressionProperty);

            BlueprintAbility BloodragerArcaneSpellTrueFormOfTheDragonICopperToggle = BloodlineTools.CreateArcaneBloodrageToggle(
                "BloodragerArcaneSpellTrueFormOfTheDragonICopperToggle",
                FormOfTheDragonICopperAbility,
                BloodragerArcaneTrueSpellAbility,
                BloodragerArcaneTrueSpellFormOfTheDragonICopperSwitchBuff,
                "BloodragerArcaneTrueSpell",
                AllBloodragerArcaneTrueSpellSwitchBuffs,
                BloodragerArcaneProgressionProperty);

            BlueprintAbility BloodragerArcaneSpellTrueFormOfTheDragonIGoldToggle = BloodlineTools.CreateArcaneBloodrageToggle(
                "BloodragerArcaneSpellTrueFormOfTheDragonIGoldToggle",
                FormOfTheDragonIGoldAbility,
                BloodragerArcaneTrueSpellAbility,
                BloodragerArcaneTrueSpellFormOfTheDragonIGoldSwitchBuff,
                "BloodragerArcaneTrueSpell",
                AllBloodragerArcaneTrueSpellSwitchBuffs,
                BloodragerArcaneProgressionProperty);

            BlueprintAbility BloodragerArcaneSpellTrueFormOfTheDragonIGreenToggle = BloodlineTools.CreateArcaneBloodrageToggle(
                "BloodragerArcaneSpellTrueFormOfTheDragonIGreenToggle",
                FormOfTheDragonIGreenAbility,
                BloodragerArcaneTrueSpellAbility,
                BloodragerArcaneTrueSpellFormOfTheDragonIGreenSwitchBuff,
                "BloodragerArcaneTrueSpell",
                AllBloodragerArcaneTrueSpellSwitchBuffs,
                BloodragerArcaneProgressionProperty);

            BlueprintAbility BloodragerArcaneSpellTrueTransformationToggle = BloodlineTools.CreateArcaneBloodrageToggle(
                "BloodragerArcaneSpellTrueTransformationToggle",
                TransformationAbility,
                BloodragerArcaneTrueSpellAbility,
                BloodragerArcaneTrueSpellTransformationSwitchBuff,
                "BloodragerArcaneTrueSpell",
                AllBloodragerArcaneTrueSpellSwitchBuffs,
                BloodragerArcaneProgressionProperty);
        }
    }
}
