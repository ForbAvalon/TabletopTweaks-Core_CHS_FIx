﻿using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.Items.Ecnchantments;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Designers.EventConditionActionSystem.Actions;
using Kingmaker.Designers.Mechanics.Buffs;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.ElementsSystem;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.Enums.Damage;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules.Damage;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.Abilities.Components.AreaEffects;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Actions;
using Kingmaker.UnitLogic.Mechanics.Components;
using Kingmaker.UnitLogic.Mechanics.Conditions;
using Kingmaker.UnitLogic.Mechanics.Properties;
using Kingmaker.Utility;
using System.Collections.Generic;
using System.Linq;
using static TabletopTweaks.Core.Main;
using TabletopTweaks.Core.NewActions;
using TabletopTweaks.Core.NewComponents;
using TabletopTweaks.Core.NewComponents.AbilitySpecific;
using TabletopTweaks.Core.NewComponents.OwlcatReplacements;
using TabletopTweaks.Core.Utilities;
using static TabletopTweaks.Core.NewContent.MechanicsChanges.MetamagicExtention;
using static TabletopTweaks.Core.Main;

namespace TabletopTweaks.Core.Bugfixes.Abilities {
    class Spells {
        [HarmonyPatch(typeof(BlueprintsCache), "Init")]
        static class BlueprintsCache_Init_Patch {
            static bool Initialized;

            static void Postfix() {
                if (Initialized) return;
                Initialized = true;
                TTTContext.Logger.LogHeader("Patching Spells");
                PatchAbyssalStorm();
                PatchAcidMaw();
                PatchBelieveInYourself();
                PatchBestowCurseGreater();
                PatchBreakEnchantment();
                PatchChainLightning();
                PatchCrusadersEdge();
                PatchDispelMagicGreater();
                PatchEyeOfTheSun();
                PatchFirebrand();
                PatchFlamestrike();
                PatchFrightfulAspect();
                PatchGeniekind();
                PatchHellfireRay();
                PatchMagicalVestment();
                PatchMagicWeaponGreater();
                PatchMicroscopicProportions();
                PatchPerfectForm();
                PatchRemoveFear();
                PatchRemoveSickness();
                PatchShadowConjuration();
                PatchShadowEvocation();
                PatchShadowEvocationGreater();
                PatchStarlight();
                PatchSunForm();
                PatchSupernova();
                PatchUnbreakableHeart();
                PatchWindsOfFall();
                PatchWrachingRay();
                PatchVampiricBlade();
                PatchFromSpellFlags();
            }

            static void PatchAbyssalStorm() {
                if (Main.TTTContext.Fixes.Spells.IsDisabled("AbyssalStorm")) { return; }

                var AbyssalStorm = Resources.GetBlueprint<BlueprintAbility>("58e9e2883bca1574e9c932e72fd361f9");
                AbyssalStorm.GetComponent<AbilityEffectRunAction>().SavingThrowType = SavingThrowType.Unknown;
                AbyssalStorm.FlattenAllActions().OfType<ContextActionDealDamage>().ForEach(a => {
                    if (a.WriteResultToSharedValue) {
                        a.WriteRawResultToSharedValue = true;
                    } else {
                        a.ReadPreRolledFromSharedValue = true;
                    }
                    a.Value.DiceType = DiceType.D6;
                    a.Value.DiceCountValue = new ContextValue() {
                        ValueType = ContextValueType.Rank
                    };
                    a.Value.BonusValue = new ContextValue();
                    a.HalfIfSaved = false;
                    a.IsAoE = true;
                });
                AbyssalStorm.GetComponent<AbilityTargetsAround>().TemporaryContext(c => {
                    c.m_Condition = new ConditionsChecker() {
                        Conditions = new Condition[] {
                            new ContextConditionIsCaster(){
                                Not = true
                            }
                        }
                    };
                });
                TTTContext.Logger.LogPatch("Patched", AbyssalStorm);
            }

            static void PatchAcidMaw() {
                if (Main.TTTContext.Fixes.Spells.IsDisabled("AcidMaw")) { return; }

                var AcidMawBuff = Resources.GetBlueprint<BlueprintBuff>("f1a6799b05a40144d9acdbdca1d7c228");

                var EnergyType = AcidMawBuff.FlattenAllActions().OfType<ContextActionDealDamage>().First().DamageType.Energy;
                var AttackWithWeaponTrigger = AcidMawBuff.GetComponent<AddInitiatorAttackWithWeaponTrigger>();
                AttackWithWeaponTrigger.Action.Actions = AttackWithWeaponTrigger.Action.Actions.OfType<ContextActionApplyBuff>().ToArray();
                AcidMawBuff.AddComponent<AddAdditionalWeaponDamage>(c => {
                    c.Value = new ContextDiceValue() {
                        DiceType = DiceType.D4,
                        DiceCountValue = 1,
                        BonusValue = 0
                    };
                    c.DamageType = new DamageTypeDescription() {
                        Type = DamageType.Energy,
                        Energy = EnergyType
                    };
                    c.CheckWeaponCatergoy = true;
                    c.Category = WeaponCategory.Bite;
                });
                TTTContext.Logger.LogPatch("Patched", AcidMawBuff);
            }

            static void PatchBelieveInYourself() {
                if (Main.TTTContext.Fixes.Spells.IsDisabled("BelieveInYourself")) { return; }

                BlueprintAbility BelieveInYourself = Resources.GetBlueprint<BlueprintAbility>("3ed3cef7c267cb847bfd44ed4708b726");
                BlueprintAbilityReference[] BelieveInYourselfVariants = BelieveInYourself
                    .GetComponent<AbilityVariants>()
                    .Variants;
                foreach (BlueprintAbility Variant in BelieveInYourselfVariants) {
                    Variant.FlattenAllActions()
                        .OfType<ContextActionApplyBuff>()
                        .ForEach(b => {
                            var ContextRankConfig = b.Buff.GetComponent<ContextRankConfig>();
                            ContextRankConfig.m_BaseValueType = ContextRankBaseValueType.CasterLevel;
                            ContextRankConfig.m_Progression = ContextRankProgression.DivStep;
                            ContextRankConfig.m_StepLevel = 4;
                            TTTContext.Logger.LogPatch("Patched", b.Buff);
                        });
                }
            }

            static void PatchBestowCurseGreater() {
                if (Main.TTTContext.Fixes.Spells.IsDisabled("BestowCurseGreater")) { return; }

                var BestowCurseGreaterDeterioration = Resources.GetBlueprint<BlueprintAbility>("71196d7e6d6645247a058a3c3c9bb5fd");
                var BestowCurseGreaterFeebleBody = Resources.GetBlueprint<BlueprintAbility>("c74a7dfebd7b1004a80f7e59689dfadd");
                var BestowCurseGreaterIdiocy = Resources.GetBlueprint<BlueprintAbility>("f7739a453e2138b46978e9098a29b3fb");
                var BestowCurseGreaterWeakness = Resources.GetBlueprint<BlueprintAbility>("abb2d42dd9219eb41848ec56a8726d58");

                var BestowCurseGreaterDeteriorationCast = Resources.GetBlueprint<BlueprintAbility>("54606d540f5d3684d9f7d6e2e2be9b63");
                var BestowCurseGreaterFeebleBodyCast = Resources.GetBlueprint<BlueprintAbility>("292d630a5abae64499bb18057aaa24b4");
                var BestowCurseGreaterIdiocyCast = Resources.GetBlueprint<BlueprintAbility>("e0212142d2a426f43926edd4202996bb");
                var BestowCurseGreaterWeaknessCast = Resources.GetBlueprint<BlueprintAbility>("1168f36fac0bad64f965928206df7b86");

                var BestowCurseGreaterDeteriorationBuff = Resources.GetBlueprint<BlueprintBuff>("8f8835d083f31c547a39ebc26ae42159");
                var BestowCurseGreaterFeebleBodyBuff = Resources.GetBlueprint<BlueprintBuff>("28c9db77dfb1aa54a94e8a7413b1840a");
                var BestowCurseGreaterIdiocyBuff = Resources.GetBlueprint<BlueprintBuff>("493dcc29a21abd94d9adb579e1f40318");
                var BestowCurseGreaterWeaknessBuff = Resources.GetBlueprint<BlueprintBuff>("0493a9d25687d7e4682e250ae3ccb187");

                RebuildCurse(
                    BestowCurseGreaterDeterioration,
                    BestowCurseGreaterDeteriorationCast,
                    BestowCurseGreaterDeteriorationBuff);
                RebuildCurse(
                    BestowCurseGreaterFeebleBody,
                    BestowCurseGreaterFeebleBodyCast,
                    BestowCurseGreaterFeebleBodyBuff);
                RebuildCurse(
                    BestowCurseGreaterIdiocy,
                    BestowCurseGreaterIdiocyCast,
                    BestowCurseGreaterIdiocyBuff);
                RebuildCurse(
                    BestowCurseGreaterWeakness,
                    BestowCurseGreaterWeaknessCast,
                    BestowCurseGreaterWeaknessBuff);

                void RebuildCurse(BlueprintAbility curse, BlueprintAbility curseCast, BlueprintBuff curseBuff) {
                    curseCast.GetComponent<AbilityEffectStickyTouch>().m_TouchDeliveryAbility = curse.ToReference<BlueprintAbilityReference>();
                    TTTContext.Logger.LogPatch("Patched", curseCast);
                    curse.GetComponent<AbilityEffectRunAction>()
                        .Actions.Actions.OfType<ContextActionConditionalSaved>().First()
                        .Failed.Actions.OfType<ContextActionApplyBuff>().First()
                        .m_Buff = curseBuff.ToReference<BlueprintBuffReference>();
                    TTTContext.Logger.LogPatch("Patched", curse);
                    curseBuff.m_Icon = curse.m_Icon;
                    TTTContext.Logger.LogPatch("Patched", curseBuff);
                }
            }

            static void PatchBreakEnchantment() {
                if (Main.TTTContext.Fixes.Spells.IsDisabled("BreakEnchantment")) { return; }

                var BreakEnchantment = Resources.GetBlueprint<BlueprintAbility>("7792da00c85b9e042a0fdfc2b66ec9a8");
                BreakEnchantment
                    .FlattenAllActions()
                    .OfType<ContextActionDispelMagic>()
                    .ForEach(dispel => {
                        dispel.OnlyTargetEnemyBuffs = true;
                        //dispel.m_MaxSpellLevel.Value = 10;
                    });
                TTTContext.Logger.LogPatch("Patched", BreakEnchantment);
            }

            static void PatchChainLightning() {
                if (Main.TTTContext.Fixes.Spells.IsDisabled("ChainLightning")) { return; }

                var ChainLightning = Resources.GetBlueprint<BlueprintAbility>("645558d63604747428d55f0dd3a4cb58");
                ChainLightning
                    .FlattenAllActions()
                    .OfType<ContextActionDealDamage>()
                    .ForEach(damage => {
                        damage.Value.DiceCountValue.ValueRank = AbilityRankType.DamageDice;
                    });
                TTTContext.Logger.LogPatch("Patched", ChainLightning);
            }

            static void PatchCrusadersEdge() {
                if (Main.TTTContext.Fixes.Spells.IsDisabled("CrusadersEdge")) { return; }

                BlueprintBuff CrusadersEdgeBuff = Resources.GetBlueprint<BlueprintBuff>("7ca348639a91ae042967f796098e3bc3");
                CrusadersEdgeBuff.GetComponent<AddInitiatorAttackWithWeaponTrigger>().CriticalHit = true;
                TTTContext.Logger.LogPatch("Patched", CrusadersEdgeBuff);
            }

            static void PatchDispelMagicGreater() {
                if (Main.TTTContext.Fixes.Spells.IsDisabled("DispelMagicGreater")) { return; }

                var DispelMagicGreaterTarget = Resources.GetBlueprint<BlueprintAbility>("6d490c80598f1d34bb277735b52d52c1");
                DispelMagicGreaterTarget.SetDescription("This functions as a targeted dispel magic, but it can dispel one spell for every four caster " +
                    "levels you possess, starting with the highest level spells and proceeding to lower level spells.\n" +
                    "Targeted Dispel: One object, creature, or spell is the target of the dispel magic spell. You make one dispel check (1d20 + your caster level)" +
                    " and compare that to the spell with highest caster level (DC = 11 + the spell’s caster level). If successful, that spell ends. " +
                    "If not, compare the same result to the spell with the next highest caster level. Repeat this process until you have dispelled " +
                    "one spell affecting the target, or you have failed to dispel every spell.");
                TTTContext.Logger.LogPatch("Patched", DispelMagicGreaterTarget);
            }

            static void PatchEyeOfTheSun() {
                if (Main.TTTContext.Fixes.Spells.IsDisabled("EyeOfTheSun")) { return; }

                var AngelEyeOfTheSun = Resources.GetBlueprint<BlueprintAbility>("a948e10ecf1fa674dbae5eaae7f25a7f");
                AngelEyeOfTheSun.FlattenAllActions().OfType<ContextActionDealDamage>().ForEach(a => {
                    if (a.WriteResultToSharedValue) {
                        a.WriteRawResultToSharedValue = true;
                    } else {
                        a.Half = true;
                        a.AlreadyHalved = false;
                        a.ReadPreRolledFromSharedValue = true;
                    }
                });
                TTTContext.Logger.LogPatch("Patched", AngelEyeOfTheSun);
            }

            static void PatchFirebrand() {
                if (Main.TTTContext.Fixes.Spells.IsDisabled("Firebrand")) { return; }

                var FirebrandBuff = Resources.GetBlueprint<BlueprintBuff>("c6cc1c5356db4674dbd2be20ea205c86");

                FirebrandBuff.RemoveComponents<AddInitiatorAttackWithWeaponTrigger>();
                FirebrandBuff.AddComponent<AddAdditionalWeaponDamage>(c => {
                    c.Value = new ContextDiceValue() {
                        DiceType = DiceType.D6,
                        DiceCountValue = 1,
                        BonusValue = 0
                    };
                    c.DamageType = new DamageTypeDescription() {
                        Type = DamageType.Energy,
                        Energy = DamageEnergyType.Fire
                    };
                });
                TTTContext.Logger.LogPatch("Patched", FirebrandBuff);
            }

            static void PatchFlamestrike() {
                if (Main.TTTContext.Fixes.Spells.IsDisabled("FlameStrike")) { return; }

                var FlameStrike = Resources.GetBlueprint<BlueprintAbility>("f9910c76efc34af41b6e43d5d8752f0f");
                FlameStrike.FlattenAllActions().OfType<ContextActionDealDamage>().ForEach(a => {
                    a.Half = true;
                    a.HalfIfSaved = true;
                });
                TTTContext.Logger.LogPatch("Patched", FlameStrike);
            }

            static void PatchFrightfulAspect() {
                if (Main.TTTContext.Fixes.Spells.IsDisabled("FrightfulAspect")) { return; }

                var FrightfulAspectArea = Resources.GetBlueprint<BlueprintAbilityAreaEffect>("b69066531acd0b94184d03e010c92e94");
                var FrightfulAspectShakenBuff = Resources.GetBlueprintReference<BlueprintBuffReference>("522164167a56d804bb4fa5abf36b4fbc");
                FrightfulAspectArea.GetComponent<AbilityAreaEffectBuff>().m_Buff = FrightfulAspectShakenBuff;
                TTTContext.Logger.LogPatch(FrightfulAspectArea);
            }

            static void PatchGeniekind() {
                if (Main.TTTContext.Fixes.Spells.IsDisabled("Geniekind")) { return; }

                var GeniekindDjinniBuff = Resources.GetBlueprint<BlueprintBuff>("082caf8c1005f114ba6375a867f638cf");
                var GeniekindEfreetiBuff = Resources.GetBlueprint<BlueprintBuff>("d47f45f29c4cfc0469f3734d02545e0b");
                var GeniekindMaridBuff = Resources.GetBlueprint<BlueprintBuff>("4f37fc07fe2cf7f4f8076e79a0a3bfe9");
                var GeniekindShaitanBuff = Resources.GetBlueprint<BlueprintBuff>("1d498104f8e35e246b5d8180b0faed43");

                ReplaceComponents(GeniekindDjinniBuff);
                ReplaceComponents(GeniekindEfreetiBuff);
                ReplaceComponents(GeniekindMaridBuff);
                ReplaceComponents(GeniekindShaitanBuff);

                void ReplaceComponents(BlueprintBuff GeniekindBuff) {
                    var EnergyType = GeniekindBuff.FlattenAllActions().OfType<ContextActionDealDamage>().First().DamageType.Energy;
                    GeniekindBuff.RemoveComponents<AddInitiatorAttackWithWeaponTrigger>();
                    GeniekindBuff.AddComponent<AddAdditionalWeaponDamage>(c => {
                        c.CheckWeaponRangeType = true;
                        c.RangeType = WeaponRangeType.Melee;
                        c.Value = new ContextDiceValue() {
                            DiceType = DiceType.D6,
                            DiceCountValue = 1,
                            BonusValue = 0
                        };
                        c.DamageType = new DamageTypeDescription() {
                            Type = DamageType.Energy,
                            Energy = EnergyType
                        };
                    });
                    TTTContext.Logger.LogPatch("Patched", GeniekindBuff);
                }
            }

            static void PatchHellfireRay() {
                if (Main.TTTContext.Fixes.Spells.IsDisabled("HellfireRay")) { return; }

                var HellfireRay = Resources.GetBlueprint<BlueprintAbility>("700cfcbd0cb2975419bcab7dbb8c6210");
                HellfireRay.GetComponent<SpellDescriptorComponent>().Descriptor = SpellDescriptor.Evil;
                HellfireRay.AvailableMetamagic &= (Metamagic)~(CustomMetamagic.Flaring | CustomMetamagic.Burning);
                HellfireRay.FlattenAllActions().OfType<ContextActionDealDamage>().ForEach(a => {
                    a.Half = true;
                    a.Value.DiceType = DiceType.D6;
                    a.Value.DiceCountValue.ValueType = ContextValueType.Rank;
                    a.Value.BonusValue = new ContextValue();
                });
                TTTContext.Logger.LogPatch("Patched", HellfireRay);
            }

            static void PatchMagicalVestment() {
                if (Main.TTTContext.Fixes.Spells.IsDisabled("MagicalVestment")) { return; }

                PatchMagicalVestmentArmor();
                PatchMagicalVestmentShield();

                void PatchMagicalVestmentShield() {
                    var MagicalVestmentShield = Resources.GetBlueprint<BlueprintAbility>("adcda176d1756eb45bd5ec9592073b09");
                    var MagicalVestmentShieldBuff = Resources.GetBlueprint<BlueprintBuff>("2e8446f820936a44f951b50d70a82b16");

                    MagicalVestmentShieldBuff.SetComponents();
                    MagicalVestmentShieldBuff.AddComponent<MagicalVestmentComponent>(c => {
                        c.Shield = true;
                        c.EnchantLevel = new ContextValue() {
                            ValueType = ContextValueType.Rank
                        };
                        c.m_EnchantmentBlueprints = new BlueprintItemEnchantmentReference[] {
                            Resources.GetBlueprintReference<BlueprintItemEnchantmentReference>("1d9b60d57afb45c4f9bb0a3c21bb3b98"), // TemporaryArmorEnhancementBonus1
                            Resources.GetBlueprintReference<BlueprintItemEnchantmentReference>("d45bfd838c541bb40bde7b0bf0e1b684"), // TemporaryArmorEnhancementBonus2
                            Resources.GetBlueprintReference<BlueprintItemEnchantmentReference>("51c51d841e9f16046a169729c13c4d4f"), // TemporaryArmorEnhancementBonus3
                            Resources.GetBlueprintReference<BlueprintItemEnchantmentReference>("a23bcee56c9fcf64d863dafedb369387"), // TemporaryArmorEnhancementBonus4
                            Resources.GetBlueprintReference<BlueprintItemEnchantmentReference>("15d7d6cbbf56bd744b37bbf9225ea83b")  // TemporaryArmorEnhancementBonus5
                        };
                    });
                    MagicalVestmentShieldBuff.AddContextRankConfig(c => {
                        c.m_BaseValueType = ContextRankBaseValueType.CasterLevel;
                        c.m_Progression = ContextRankProgression.DivStep;
                        c.m_StepLevel = 4;
                        c.m_Min = 1;
                        c.m_Max = 5;
                    });
                    TTTContext.Logger.LogPatch("Patched", MagicalVestmentShieldBuff);
                }
                void PatchMagicalVestmentArmor() {
                    var MagicalVestmentArmor = Resources.GetBlueprint<BlueprintAbility>("956309af83352714aa7ee89fb4ecf201");
                    var MagicalVestmentArmorBuff = Resources.GetBlueprint<BlueprintBuff>("9e265139cf6c07c4fb8298cb8b646de9");

                    MagicalVestmentArmorBuff.SetComponents();
                    MagicalVestmentArmorBuff.AddComponent<MagicalVestmentComponent>(c => {
                        c.EnchantLevel = new ContextValue() {
                            ValueType = ContextValueType.Rank
                        };
                        c.m_EnchantmentBlueprints = new BlueprintItemEnchantmentReference[] {
                            Resources.GetBlueprintReference<BlueprintItemEnchantmentReference>("1d9b60d57afb45c4f9bb0a3c21bb3b98"), // TemporaryArmorEnhancementBonus1
                            Resources.GetBlueprintReference<BlueprintItemEnchantmentReference>("d45bfd838c541bb40bde7b0bf0e1b684"), // TemporaryArmorEnhancementBonus2
                            Resources.GetBlueprintReference<BlueprintItemEnchantmentReference>("51c51d841e9f16046a169729c13c4d4f"), // TemporaryArmorEnhancementBonus3
                            Resources.GetBlueprintReference<BlueprintItemEnchantmentReference>("a23bcee56c9fcf64d863dafedb369387"), // TemporaryArmorEnhancementBonus4
                            Resources.GetBlueprintReference<BlueprintItemEnchantmentReference>("15d7d6cbbf56bd744b37bbf9225ea83b")  // TemporaryArmorEnhancementBonus5
                        };
                    });
                    MagicalVestmentArmorBuff.AddContextRankConfig(c => {
                        c.m_BaseValueType = ContextRankBaseValueType.CasterLevel;
                        c.m_Progression = ContextRankProgression.DivStep;
                        c.m_StepLevel = 4;
                        c.m_Min = 1;
                        c.m_Max = 5;
                    });
                    TTTContext.Logger.LogPatch("Patched", MagicalVestmentArmorBuff);
                }
            }

            static void PatchMagicWeaponGreater() {
                if (Main.TTTContext.Fixes.Spells.IsDisabled("GreaterMagicWeapon")) { return; }

                var MagicWeaponGreaterPrimary = Resources.GetBlueprint<BlueprintAbility>("a3fe23711486ee9489af1dadd6906149");
                var MagicWeaponGreaterSecondary = Resources.GetBlueprint<BlueprintAbility>("89c13df989e5e624692134d55195121a");
                var newEnhancements = new BlueprintItemEnchantmentReference[] {
                    Resources.GetModBlueprint<BlueprintWeaponEnchantment>("TemporaryEnhancement1NonStacking").ToReference<BlueprintItemEnchantmentReference>(),
                    Resources.GetModBlueprint<BlueprintWeaponEnchantment>("TemporaryEnhancement2NonStacking").ToReference<BlueprintItemEnchantmentReference>(),
                    Resources.GetModBlueprint<BlueprintWeaponEnchantment>("TemporaryEnhancement3NonStacking").ToReference<BlueprintItemEnchantmentReference>(),
                    Resources.GetModBlueprint<BlueprintWeaponEnchantment>("TemporaryEnhancement4NonStacking").ToReference<BlueprintItemEnchantmentReference>(),
                    Resources.GetModBlueprint<BlueprintWeaponEnchantment>("TemporaryEnhancement5NonStacking").ToReference<BlueprintItemEnchantmentReference>(),
                };

                MagicWeaponGreaterPrimary.FlattenAllActions().OfType<EnhanceWeapon>().ForEach(c => c.m_Enchantment = newEnhancements);
                MagicWeaponGreaterSecondary.FlattenAllActions().OfType<EnhanceWeapon>().ForEach(c => c.m_Enchantment = newEnhancements);

                TTTContext.Logger.LogPatch("Patched", MagicWeaponGreaterPrimary);
                TTTContext.Logger.LogPatch("Patched", MagicWeaponGreaterSecondary);
            }
            static void PatchMicroscopicProportions() {
                if (Main.TTTContext.Fixes.Spells.IsDisabled("MicroscopicProportions")) { return; }

                var TricksterMicroscopicProportionsBuff = Resources.GetBlueprint<BlueprintBuff>("1dfc2f933e7833f41922411962e1d58a");
                TricksterMicroscopicProportionsBuff
                    .GetComponents<AddContextStatBonus>()
                    .ForEach(c => c.Descriptor = ModifierDescriptor.Size);

                TTTContext.Logger.LogPatch("Patched", TricksterMicroscopicProportionsBuff);
            }
            static void PatchPerfectForm() {
                if (Main.TTTContext.Fixes.Spells.IsDisabled("PerfectForm")) { return; }

                var PerfectForm = Resources.GetBlueprint<BlueprintAbility>("91d04f9180e94065ac768959323d2002");
                var perfectFormBuffs = new BlueprintBuffReference[] {
                        Resources.GetBlueprintReference<BlueprintBuffReference>("ccd363424d954c668a81b0024012a66a"),    // PerfectFormEqualToCharismaBuff
                        Resources.GetBlueprintReference<BlueprintBuffReference>("774567a0bcf54a83807f7387d5dd9c23"),    // PerfectFormEqualToConstitutionBuff
                        Resources.GetBlueprintReference<BlueprintBuffReference>("23b87498fac14465bc9c22cc3366e6e7"),    // PerfectFormEqualToDexterityBuff
                        Resources.GetBlueprintReference<BlueprintBuffReference>("3bc3d8660ddc467aabea43b070fcd10b"),    // PerfectFormEqualToIntelligenceBuff
                        Resources.GetBlueprintReference<BlueprintBuffReference>("149e7d34927146a8804404087bf9703f"),    // PerfectFormEqualToStrengthBuff
                        Resources.GetBlueprintReference<BlueprintBuffReference>("06785b5665264ad1b257fa3e724ed68f")     // PerfectFormEqualToWisdomBuff
  
                    };
                PerfectForm
                    .GetComponent<AbilityEffectRunAction>()
                    .TemporaryContext(c => {
                        c.Actions = Helpers.CreateActionList(
                            CreateRemoveBuff(perfectFormBuffs)
                                .Concat(c.Actions.Actions)
                                .ToArray()
                        );
                    });
                IEnumerable<GameAction> CreateRemoveBuff(BlueprintBuffReference[] buffs) {
                    foreach (var buff in buffs) {
                        var removeBuff = new ContextActionRemoveBuff() {
                            m_Buff = buff,
                        };
                        var conditional = new Conditional() {
                            ConditionsChecker = new ConditionsChecker() {
                                Conditions = new Condition[] {
                                new ContextConditionHasBuff() {
                                    m_Buff = buff
                                }
                            }
                            },
                            IfTrue = Helpers.CreateActionList(removeBuff),
                            IfFalse = Helpers.CreateActionList()
                        };
                        yield return conditional;
                    }
                }
                TTTContext.Logger.LogPatch(PerfectForm);
            }
            static void PatchRemoveFear() {
                if (Main.TTTContext.Fixes.Spells.IsDisabled("RemoveFear")) { return; }

                var RemoveFearBuff = Resources.GetBlueprint<BlueprintBuff>("c5c86809a1c834e42a2eb33133e90a28");
                RemoveFearBuff.RemoveComponents<AddConditionImmunity>();
                QuickFixTools.ReplaceSuppression(RemoveFearBuff, TTTContext);
            }

            static void PatchRemoveSickness() {
                if (Main.TTTContext.Fixes.Spells.IsDisabled("RemoveSickness")) { return; }

                var RemoveSicknessBuff = Resources.GetBlueprint<BlueprintBuff>("91e09b2d99bb71243a97565af8b282e9");
                RemoveSicknessBuff.RemoveComponents<AddConditionImmunity>();
                RemoveSicknessBuff.AddComponent<SuppressBuffsTTT>(c => {
                    c.Descriptor = SpellDescriptor.Sickened | SpellDescriptor.Nauseated;
                });
                TTTContext.Logger.LogPatch("Patched", RemoveSicknessBuff);
            }

            static void PatchShadowConjuration() {
                if (Main.TTTContext.Fixes.Spells.IsDisabled("ShadowConjuration")) { return; }

                var ShadowConjuration = Resources.GetBlueprint<BlueprintAbility>("caac251ca7601324bbe000372a0a1005");
                ShadowConjuration.AddToSpellList(SpellTools.SpellList.WizardSpellList, 4);
                TTTContext.Logger.LogPatch("Patched", ShadowConjuration);
            }

            static void PatchShadowEvocation() {
                if (Main.TTTContext.Fixes.Spells.IsDisabled("ShadowEvocation")) { return; }

                var ShadowEvocation = Resources.GetBlueprint<BlueprintAbility>("237427308e48c3341b3d532b9d3a001f");
                ShadowEvocation.AvailableMetamagic |= Metamagic.Empower
                    | Metamagic.Maximize
                    | Metamagic.Quicken
                    | Metamagic.Heighten
                    | Metamagic.Reach
                    | Metamagic.CompletelyNormal
                    | Metamagic.Persistent
                    | Metamagic.Selective
                    | Metamagic.Bolstered;
                TTTContext.Logger.LogPatch("Patched", ShadowEvocation);
            }

            static void PatchShadowEvocationGreater() {
                if (Main.TTTContext.Fixes.Spells.IsDisabled("ShadowEvocationGreater")) { return; }

                var ShadowEvocationGreaterProperty = Resources.GetBlueprint<BlueprintUnitProperty>("0f813eb338594c5bb840c5583fd29c3d");
                var ShadowEvocationGreater = Resources.GetBlueprint<BlueprintAbility>("3c4a2d4181482e84d9cd752ef8edc3b6");
                ShadowEvocationGreater.AvailableMetamagic |= Metamagic.Empower
                    | Metamagic.Maximize
                    | Metamagic.Quicken
                    | Metamagic.Heighten
                    | Metamagic.Reach
                    | Metamagic.CompletelyNormal
                    | Metamagic.Persistent
                    | Metamagic.Selective
                    | Metamagic.Bolstered;
                ShadowEvocationGreaterProperty.BaseValue = 60;
                TTTContext.Logger.LogPatch("Patched", ShadowEvocationGreater);
            }

            static void PatchStarlight() {
                if (Main.TTTContext.Fixes.Spells.IsDisabled("Starlight")) { return; }

                var StarlightAllyBuff = Resources.GetBlueprint<BlueprintBuff>("f4ead47adc2ca2744a00efd4e088ecb2");
                StarlightAllyBuff.GetComponent<AddConcealment>().Descriptor = ConcealmentDescriptor.InitiatorIsBlind;
                TTTContext.Logger.LogPatch("Patched", StarlightAllyBuff);
            }

            static void PatchSunForm() {
                if (Main.TTTContext.Fixes.Spells.IsDisabled("SunForm")) { return; }

                var AngelSunFormRay = Resources.GetBlueprint<BlueprintAbility>("d0d8811bf5a8e2942b6b7d77d9691eb9");
                AngelSunFormRay.FlattenAllActions().OfType<ContextActionDealDamage>().ForEach(a => {
                    if (a.WriteResultToSharedValue) {
                        a.WriteRawResultToSharedValue = true;
                        a.Half = true;
                    } else {
                        a.Half = true;
                        a.AlreadyHalved = false;
                        a.ReadPreRolledFromSharedValue = true;
                    }
                    a.Value.DiceType = DiceType.D6;
                    a.Value.DiceCountValue = new ContextValue() {
                        ValueType = ContextValueType.Rank
                    };
                    a.Value.BonusValue = new ContextValue();
                });
                TTTContext.Logger.LogPatch("Patched", AngelSunFormRay);
            }

            static void PatchSupernova() {
                if (Main.TTTContext.Fixes.Spells.IsDisabled("Supernova")) { return; }
                var Supernova = Resources.GetBlueprint<BlueprintAbility>("1325e698f4a3f224b880e3b83a551228");
                var SupernovaArea = Resources.GetBlueprint<BlueprintAbilityAreaEffect>("165a01a3597c0bf44a8b333ac6dd631a");
                var BlindnessBuff = Resources.GetBlueprintReference<BlueprintBuffReference>("187f88d96a0ef464280706b63635f2af");

                Supernova.AvailableMetamagic |= Metamagic.Empower | Metamagic.Maximize | Metamagic.Bolstered;
                SupernovaArea.RemoveComponents<AbilityAreaEffectRunAction>();
                SupernovaArea.AddComponent<AbilityAreaEffectRunAction>(c => {
                    c.UnitEnter = Helpers.CreateActionList(CreateBlindnessSave(), CreateDamageSave());
                    c.UnitExit = Helpers.CreateActionList();
                    c.UnitMove = Helpers.CreateActionList();
                    c.Round = Helpers.CreateActionList(CreateDamageSave());
                });
                TTTContext.Logger.LogPatch("Patched", SupernovaArea);

                ContextActionSavingThrow CreateBlindnessSave() {
                    return Helpers.Create<ContextActionSavingThrow>(save => {
                        save.Type = SavingThrowType.Fortitude;
                        save.m_ConditionalDCIncrease = new ContextActionSavingThrow.ConditionalDCIncrease[0];
                        save.CustomDC = new ContextValue();
                        save.Actions = Helpers.CreateActionList(
                            Helpers.Create<ContextActionConditionalSaved>(c => {
                                c.Succeed = Helpers.CreateActionList();
                                c.Failed = Helpers.CreateActionList(
                                    Helpers.Create<ContextActionApplyBuff>(a => {
                                        a.m_Buff = BlindnessBuff;
                                        a.DurationValue = new ContextDurationValue() {
                                            m_IsExtendable = true,
                                            DiceCountValue = new ContextValue(),
                                            BonusValue = new ContextValue() {
                                                ValueType = ContextValueType.Rank
                                            }
                                        };
                                        a.AsChild = true;
                                    })
                                );
                            })
                        );
                    });
                }
                ContextActionSavingThrow CreateDamageSave() {
                    return Helpers.Create<ContextActionSavingThrow>(save => {
                        save.Type = SavingThrowType.Reflex;
                        save.m_ConditionalDCIncrease = new ContextActionSavingThrow.ConditionalDCIncrease[0];
                        save.CustomDC = new ContextValue();
                        save.Actions = Helpers.CreateActionList(
                            Helpers.Create<ContextActionDealDamage>(c => {
                                c.DamageType = new DamageTypeDescription() {
                                    Type = DamageType.Energy,
                                    Common = new DamageTypeDescription.CommomData(),
                                    Physical = new DamageTypeDescription.PhysicalData(),
                                    Energy = DamageEnergyType.Fire
                                };
                                c.Duration = new ContextDurationValue() {
                                    m_IsExtendable = true,
                                    DiceCountValue = new ContextValue(),
                                    BonusValue = new ContextValue()
                                };
                                c.Value = new ContextDiceValue() {
                                    DiceType = DiceType.D6,
                                    DiceCountValue = 2,
                                    BonusValue = new ContextValue() {
                                        ValueType = ContextValueType.Rank
                                    }
                                };
                                c.IsAoE = true;
                                c.HalfIfSaved = true;
                                c.WriteResultToSharedValue = true;
                                c.WriteRawResultToSharedValue = true;
                            }),
                            Helpers.Create<ContextActionDealDamage>(c => {
                                c.DamageType = new DamageTypeDescription() {
                                    Type = DamageType.Energy,
                                    Common = new DamageTypeDescription.CommomData(),
                                    Physical = new DamageTypeDescription.PhysicalData(),
                                    Energy = DamageEnergyType.Divine
                                };
                                c.Duration = new ContextDurationValue() {
                                    m_IsExtendable = true,
                                    DiceCountValue = new ContextValue(),
                                    BonusValue = new ContextValue()
                                };
                                c.Value = new ContextDiceValue() {
                                    DiceType = DiceType.D6,
                                    DiceCountValue = 2,
                                    BonusValue = new ContextValue() {
                                        ValueType = ContextValueType.Rank
                                    }
                                };
                                c.IsAoE = true;
                                c.HalfIfSaved = true;
                                c.ReadPreRolledFromSharedValue = true;
                            })
                        );
                    });
                }
            }

            static void PatchUnbreakableHeart() {
                if (Main.TTTContext.Fixes.Spells.IsDisabled("UnbreakableHeart")) { return; }

                var UnbreakableHeartBuff = Resources.GetBlueprint<BlueprintBuff>("6603b27034f694e44a407a9cdf77c67e");
                QuickFixTools.ReplaceSuppression(UnbreakableHeartBuff, TTTContext);
            }

            static void PatchWindsOfFall() {
                if (Main.TTTContext.Fixes.Spells.IsDisabled("WindsOfFall")) { return; }

                var WindsOfTheFall = Resources.GetBlueprint<BlueprintAbility>("af2ed41c7894b934c9a9ca5048af3f58");
                var WindsOfTheFallBuff = Resources.GetBlueprint<BlueprintBuff>("b90339c580288eb48b7fea4abba0507e");

                WindsOfTheFall.TemporaryContext(bp => {
                    bp.Range = AbilityRange.Projectile;
                    bp.CanTargetEnemies = true;
                    bp.CanTargetFriends = true;
                    bp.CanTargetPoint = true;
                });
                WindsOfTheFallBuff.TemporaryContext(bp => {
                    bp.SetComponents();
                    bp.AddComponent<ModifyD20>(c => {
                        c.Rule = RuleType.All;
                        c.RollsAmount = 1;
                        c.TakeBest = false;
                        c.m_SavingThrowType = ModifyD20.InnerSavingThrowType.All;
                        c.m_TandemTripFeature = new BlueprintFeatureReference();
                        c.RollResult = new ContextValue();
                        c.Bonus = new ContextValue();
                        c.Chance = new ContextValue();
                        c.ValueToCompareRoll = new ContextValue();
                        c.Skill = new StatType[0];
                        c.Value = new ContextValue();
                    });
                });
                TTTContext.Logger.LogPatch(WindsOfTheFall);
                TTTContext.Logger.LogPatch(WindsOfTheFallBuff);
            }

            static void PatchWrachingRay() {
                if (Main.TTTContext.Fixes.Spells.IsDisabled("WrackingRay")) { return; }

                var WrackingRay = Resources.GetBlueprint<BlueprintAbility>("1cde0691195feae45bab5b83ea3f221e");
                foreach (AbilityEffectRunAction component in WrackingRay.GetComponents<AbilityEffectRunAction>()) {
                    foreach (ContextActionDealDamage action in component.Actions.Actions.OfType<ContextActionDealDamage>()) {
                        action.Value.DiceType = Kingmaker.RuleSystem.DiceType.D4;
                    }
                }
                TTTContext.Logger.LogPatch("Patched", WrackingRay);
            }

            static void PatchVampiricBlade() {
                if (Main.TTTContext.Fixes.Spells.IsDisabled("VampiricBlade")) { return; }

                var VampiricBladeBuff = Resources.GetBlueprint<BlueprintBuff>("f6007b38909c3b248a8a77b316f5bc2d");

                VampiricBladeBuff.GetComponents<AddInitiatorAttackWithWeaponTrigger>()
                    .Where(c => c.ActionsOnInitiator == false)
                    .FirstOrDefault()
                    .TemporaryContext(c => {
                        c.Action = Helpers.CreateActionList(
                            Helpers.Create<ContextActionDealDamageTTT>(a => {
                                a.DamageType = new DamageTypeDescription() {
                                    Type = DamageType.Energy,
                                    Energy = DamageEnergyType.Unholy,
                                    Common = new DamageTypeDescription.CommomData(),
                                    Physical = new DamageTypeDescription.PhysicalData()
                                };
                                a.Duration = new ContextDurationValue() {
                                    m_IsExtendable = true,
                                    DiceCountValue = 0,
                                    BonusValue = 0
                                };
                                a.Value = new ContextDiceValue() {
                                    DiceType = DiceType.D6,
                                    DiceCountValue = 1,
                                    BonusValue = new ContextValue() {
                                        ValueType = ContextValueType.Rank
                                    }
                                };
                                a.WriteResultToSharedValue = true;
                                a.ResultSharedValue = AbilitySharedValue.Heal;
                                a.IgnoreWeapon = true;
                            })
                        );
                    });
                TTTContext.Logger.LogPatch("Patched", VampiricBladeBuff);
            }

            static void PatchFromSpellFlags() {
                if (Main.TTTContext.Fixes.Spells.IsDisabled("FixSpellFlags")) { return; }

                TTTContext.Logger.Log("Updating Spell Flags");
                SpellTools.SpellList.AllSpellLists
                    .SelectMany(list => list.SpellsByLevel)
                    .Where(spellList => spellList.SpellLevel != 0)
                    .SelectMany(level => level.Spells)
                    .Distinct()
                    .SelectMany(spell => spell.AbilityAndVariants())
                    .OrderBy(spell => spell.Name)
                    .SelectMany(a => a.FlattenAllActions())
                    .OfType<ContextActionApplyBuff>()
                    .Distinct()
                    .Select(a => a.Buff)
                    .Distinct()
                    .OrderBy(buff => buff.name)
                    .ForEach(buff => {
                        if (buff.GetComponent<AddCondition>() == null
                        && buff.GetComponent<BuffStatusCondition>() == null
                        && buff.GetComponent<BuffPoisonStatDamage>() == null
                        && (buff.SpellDescriptor & SpellDescriptor.Bleed) == 0) {
                            buff.m_Flags |= BlueprintBuff.Flags.IsFromSpell;
                            TTTContext.Logger.LogPatch("Patched", buff);
                        }
                    });
                TTTContext.Logger.Log("Finished Spell Flags");
            }
        }
    }
}