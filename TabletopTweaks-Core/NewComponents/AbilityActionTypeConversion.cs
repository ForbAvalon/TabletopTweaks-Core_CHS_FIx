﻿using HarmonyLib;
using JetBrains.Annotations;
using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components.Base;
using Kingmaker.UnitLogic.Commands.Base;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using TabletopTweaks.Core.NewEvents;

namespace TabletopTweaks.Core.NewComponents {
    [TypeId("d2cf98b4e2854b90b4d451059cc581c0")]
    public class AbilityActionTypeConversion : UnitFactComponentDelegate, ISpontaneousConversionHandler {

        private ReferenceArrayProxy<BlueprintAbility, BlueprintAbilityReference> Abilities {
            get {
                return this.m_Abilities;
            }
        }

        public void HandleGetConversions(AbilityData ability, ref IEnumerable<AbilityData> conversions) {
            var conversionList = conversions.ToList();
            var MatchesDescriptors = CheckDescriptors && Descriptors.HasAnyFlag(ability.Blueprint.SpellDescriptor);
            if (Abilities.Contains(ability.Blueprint) || (MatchesDescriptors && (RequireAoE ? ability.IsAOE : !ability.IsAOE))) {
                if (ApplyToVariants && ability.Blueprint.HasVariants) {
                    Main.TTTContext.Logger.Log("Entered Swift Conversion");
                    foreach (var variant in ability.Blueprint.AbilityVariants.Variants) {
                        Main.TTTContext.Logger.Log("Hit Variant");
                        var variantAbilityData = conversions.First(c => c.Blueprint == variant);
                        if (variantAbilityData == null) { continue; }
                        CustomSpeedAbilityData swiftAbility = new CustomSpeedAbilityData(variantAbilityData, null) {
                            MetamagicData = variantAbilityData.MetamagicData ?? new MetamagicData(),
                            OverridenResourceLogic = new CustomSpeedResourceOverride() {
                                m_RequiredResource = variantAbilityData.ResourceLogic.RequiredResource.ToReference<BlueprintAbilityResourceReference>(),
                                Multiplier = ResourceMultiplier
                            },
                            CustomActionType = ActionType
                        };
                        AbilityData.AddAbilityUnique(ref conversionList, swiftAbility);
                        Main.TTTContext.Logger.Log("Added Swift Conversion");
                    }
                } else {
                    CustomSpeedAbilityData swiftAbility = new CustomSpeedAbilityData(ability, null) {
                        MetamagicData = ability.MetamagicData ?? new MetamagicData(),
                        OverridenResourceLogic = new CustomSpeedResourceOverride() {
                            m_RequiredResource = ability.ResourceLogic.RequiredResource.ToReference<BlueprintAbilityResourceReference>(),
                            Multiplier = ResourceMultiplier
                        },
                        CustomActionType = ActionType
                    };
                    AbilityData.AddAbilityUnique(ref conversionList, swiftAbility);
                }
                conversions = conversionList;
            }
        }

        public UnitCommand.CommandType ActionType = UnitCommand.CommandType.Swift;
        public int ResourceMultiplier = 2;
        public BlueprintAbilityReference[] m_Abilities = new BlueprintAbilityReference[0];
        public SpellDescriptorWrapper Descriptors;
        public bool CheckDescriptors;
        public bool RequireAoE;
        public bool ApplyToVariants;

        [HarmonyPatch(typeof(AbilityData), nameof(AbilityData.RuntimeActionType), MethodType.Getter)]
        static class AbilityData_RuntimeActionType_QuickChannel_Patch {
            static void Postfix(AbilityData __instance, ref UnitCommand.CommandType __result) {
                switch (__instance) {
                    case CustomSpeedAbilityData abilityData:
                        __result = abilityData.RuntimeActionType;
                        break;
                }
            }
        }
        [HarmonyPatch(typeof(AbilityData), nameof(AbilityData.ActionType), MethodType.Getter)]
        static class AbilityData_ActionType_QuickChannel_Patch {
            static void Postfix(AbilityData __instance, ref UnitCommand.CommandType __result) {
                switch (__instance) {
                    case CustomSpeedAbilityData abilityData:
                        __result = abilityData.ActionType;
                        break;
                }
            }
        }

        private class CustomSpeedAbilityData : AbilityData {

            public CustomSpeedAbilityData() : base() { }
            public CustomSpeedAbilityData(
                BlueprintAbility blueprint,
                UnitDescriptor caster,
                [CanBeNull] Ability fact,
                [CanBeNull] BlueprintSpellbook spellbookBlueprint) : base(blueprint, caster, fact, spellbookBlueprint) {
            }

            public CustomSpeedAbilityData(AbilityData other, BlueprintAbility replaceBlueprint) : this(replaceBlueprint ?? other.Blueprint, other.Caster, other.Fact, other.SpellbookBlueprint) {
                MetamagicData metamagicData = other.MetamagicData;
                this.MetamagicData = (metamagicData != null) ? metamagicData.Clone() : null;
                this.m_ConvertedFrom = other;
            }

            public new UnitCommand.CommandType RuntimeActionType {
                get {
                    return CustomActionType;
                }
            }
            public new UnitCommand.CommandType ActionType {
                get {
                    return CustomActionType;
                }
            }

            [JsonProperty]
            public UnitCommand.CommandType CustomActionType;
        }
        private class CustomSpeedResourceOverride : IAbilityResourceLogic {
            public CustomSpeedResourceOverride() : base() { }

            public BlueprintAbilityResource RequiredResource => m_RequiredResource.Get();

            public bool IsSpendResource => true;

            public int CalculateCost(AbilityData ability) {
                var BaseCost = ability.ConvertedFrom?.ResourceCost ?? 0;
                return BaseCost * Multiplier;
            }

            public void Spend(AbilityData ability) {
                UnitEntityData unit = ability.Caster.Unit;
                if (unit == null) {
                    PFLog.Default.Error("Caster is missing", Array.Empty<object>());
                    return;
                }
                if (unit.Blueprint.IsCheater) {
                    return;
                }
                unit.Descriptor.Resources.Spend(this.RequiredResource, CalculateCost(ability));
            }

            [JsonProperty]
            public BlueprintAbilityResourceReference m_RequiredResource;
            [JsonProperty]
            public int Multiplier;
        }
    }
}
