﻿using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Designers.Mechanics.Buffs;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UnitLogic.Buffs;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.Utility;
using System.Collections.Generic;
using System.Linq;
using static TabletopTweaks.Core.Main;
using TabletopTweaks.Core.NewUnitParts;
using TabletopTweaks.Core.Utilities;

namespace TabletopTweaks.Core.MechanicsChanges {
    class PolymorphStacking {

        private static class PolymorphMechanics {
            private static bool Initialized = false;
            [PostPatchInitialize]
            public static void Initalize() {
                if (Initialized) { return; }
                if (Main.TTTContext.Fixes.BaseFixes.IsEnabled("DisablePolymorphStacking")) {
                    EventBus.Subscribe(PolymorphStackingRules.Instance);
                }
                if (Main.TTTContext.Fixes.BaseFixes.IsEnabled("DisablePolymorphSizeStacking")) {
                    EventBus.Subscribe(PolymorphSizeRules.PolymorphBuffApply.Instance);
                }
                EventBus.Subscribe(PolymorphSizeRules.PolymorphBuffRemove.Instance);
                if (Main.TTTContext.Fixes.BaseFixes.IsEnabled("DisableSizeStacking")) {
                    EventBus.Subscribe(SizeStackingRules.SizeBuffApply.Instance);
                }
                EventBus.Subscribe(SizeStackingRules.SizeBuffRemove.Instance);
                Initialized = true;
            }
            private class PolymorphStackingRules : IAfterRulebookEventTriggerHandler<RuleCanApplyBuff>, IGlobalSubscriber {
                public static PolymorphStackingRules Instance = new();
                private PolymorphStackingRules() { }
                public void OnAfterRulebookEventTrigger(RuleCanApplyBuff evt) {
                    var Descriptor = evt.Blueprint.GetComponent<SpellDescriptorComponent>();
                    if (Descriptor == null) { return; }
                    if (!Descriptor.Descriptor.HasAnyFlag(SpellDescriptor.Polymorph)) { return; }
                    if (evt.CanApply && (evt.Context.MaybeCaster.Faction == evt.Initiator.Faction)) {
                        evt.Initiator
                            .Buffs
                            .Enumerable
                            .Where(buff => buff.Context.SpellDescriptor.HasAnyFlag(SpellDescriptor.Polymorph))
                            .ForEach(buff => buff.Remove());
                    }
                }
            }
            private class PolymorphSizeRules {
                public class PolymorphBuffApply : IUnitBuffHandler, IGlobalSubscriber, ISubscriber {
                    public static PolymorphBuffApply Instance = new();
                    private PolymorphBuffApply() { }
                    public void HandleBuffDidAdded(Buff buff) {
                        if (!buff.Context.SpellDescriptor.HasAnyFlag(SpellDescriptor.Polymorph)) { return; }
                        var owner = buff.Owner;
                        var suppressionPart = owner?.Ensure<UnitPartBuffSupressTTT>();
                        if (suppressionPart == null) { return; }
                        suppressionPart.AddContinuousPolymorphEntry(buff);
                    }

                    public void HandleBuffDidRemoved(Buff buff) {
                    }
                }
                public class PolymorphBuffRemove : IUnitBuffHandler, IGlobalSubscriber, ISubscriber {
                    public static PolymorphBuffRemove Instance = new();
                    private PolymorphBuffRemove() { }
                    public void HandleBuffDidAdded(Buff buff) {
                    }

                    public void HandleBuffDidRemoved(Buff buff) {
                        if (!buff.Context.SpellDescriptor.HasAnyFlag(SpellDescriptor.Polymorph)) { return; }
                        var owner = buff.Owner;
                        var suppressionPart = owner?.Get<UnitPartBuffSupressTTT>();
                        if (suppressionPart == null) { return; }
                        suppressionPart.RemoveEntry(buff);
                    }
                }
            }
            private class SizeStackingRules {
                public class SizeBuffApply : IUnitBuffHandler, IGlobalSubscriber, ISubscriber {
                    public static SizeBuffApply Instance = new();
                    private SizeBuffApply() { }
                    public void HandleBuffDidAdded(Buff buff) {
                        if (buff.Context.SpellDescriptor.HasAnyFlag(SpellDescriptor.Polymorph) || !buff.GetComponent<ChangeUnitSize>()) { return; }
                        var owner = buff.Owner;
                        var suppressionPart = owner?.Ensure<UnitPartBuffSupressTTT>();
                        if (suppressionPart == null) { return; }
                        suppressionPart.AddSizeEntry(buff);
                    }

                    public void HandleBuffDidRemoved(Buff buff) {
                    }
                }
                public class SizeBuffRemove : IUnitBuffHandler, IGlobalSubscriber, ISubscriber {
                    public static SizeBuffRemove Instance = new();
                    private SizeBuffRemove() { }
                    public void HandleBuffDidAdded(Buff buff) {
                    }

                    public void HandleBuffDidRemoved(Buff buff) {
                        if (buff.Context.SpellDescriptor.HasAnyFlag(SpellDescriptor.Polymorph) || !buff.GetComponent<ChangeUnitSize>()) { return; }
                        var owner = buff.Owner;
                        var suppressionPart = owner?.Get<UnitPartBuffSupressTTT>();
                        if (suppressionPart == null) { return; }
                        suppressionPart.RemoveEntry(buff);
                    }
                }
            }
        }
        [HarmonyPatch(typeof(BlueprintsCache), "Init")]
        static class BlueprintsCache_Init_Patch {
            static bool Initialized;

            static void Postfix() {
                if (Initialized) return;
                Initialized = true;
                if (Main.TTTContext.Fixes.BaseFixes.IsDisabled("DisablePolymorphStacking")) { return; }
                TTTContext.Logger.LogHeader("Patching Polymorph Effects");
                AddModifers();
                RemoveModifiers();

            }
            static void AddModifers() {
                IEnumerable<BlueprintBuff> polymorphBuffs = new List<BlueprintBuff>() {
                    Resources.GetBlueprint<BlueprintBuff>("082caf8c1005f114ba6375a867f638cf"), //GeniekindDjinniBuff  
                    Resources.GetBlueprint<BlueprintBuff>("d47f45f29c4cfc0469f3734d02545e0b"), //GeniekindEfreetiBuff  
                    Resources.GetBlueprint<BlueprintBuff>("4f37fc07fe2cf7f4f8076e79a0a3bfe9"), //GeniekindMaridBuff  
                    Resources.GetBlueprint<BlueprintBuff>("1d498104f8e35e246b5d8180b0faed43"), //GeniekindShaitanBuff  
                };
                polymorphBuffs
                    .OrderBy(buff => buff.name)
                    .ForEach(buff => {
                        var originalComponent = buff.GetComponent<SpellDescriptorComponent>();
                        if (originalComponent) {
                            originalComponent.Descriptor |= SpellDescriptor.Polymorph;
                        } else {
                            buff.AddComponent(Helpers.Create<SpellDescriptorComponent>(c => {
                                c.Descriptor = SpellDescriptor.Polymorph;
                            }));
                        }
                        TTTContext.Logger.LogPatch("Patched", buff);
                    });
            }
            static void RemoveModifiers() {
                IEnumerable<BlueprintBuff> polymorphBuffs = new List<BlueprintBuff>() {
                    Resources.GetBlueprint<BlueprintBuff>("ae5d58afe8b9aa14cae977d36ff090c8"), //FormOfTheDragonISilverBreathWeaponBuff  
                    Resources.GetBlueprint<BlueprintBuff>("43868c29760f7204f996dcc99ec93b39"), //FormOfTheDragonIISilverBreathWeaponBuff  
                    Resources.GetBlueprint<BlueprintBuff>("b78d21189e7f6e943920236f009d30e3"), //FormOfTheDragonIIIBreathWeaponCooldownBuff  
                    Resources.GetBlueprint<BlueprintBuff>("5effa97644bb7394d8532c216ec0f216"), //FormOfTheDragonIIGreenBreathWeaponBuff  
                    Resources.GetBlueprint<BlueprintBuff>("81e3330720af3d04eb65d9a2e7d92abb"), //FormOfTheDragonIIGoldBreathWeaponBuff  
                    Resources.GetBlueprint<BlueprintBuff>("a7a5d1143490dae49b8603810866cf4d"), //FormOfTheDragonIIBreathWeaponCooldownBuff  
                    Resources.GetBlueprint<BlueprintBuff>("a81c1b775d3da144ea8d3c43c5b349a2"), //FormOfTheDragonIIBrassBreathWeaponBuff  
                    Resources.GetBlueprint<BlueprintBuff>("ae5c9458a570b334d8dac0774f62efa1"), //FormOfTheDragonIIBlueBreathWeaponBuff  
                    Resources.GetBlueprint<BlueprintBuff>("7c21715c4c4938b40ac2996a1330eeb2"), //FormOfTheDragonIIBlackBreathWeaponBuff  
                    Resources.GetBlueprint<BlueprintBuff>("3980fce6bbc14604e99fcd77b326220e"), //FormOfTheDragonIGreenBreathWeaponBuff  
                    Resources.GetBlueprint<BlueprintBuff>("3eebb97861a3405418396e1b9866be72"), //FormOfTheDragonIGoldBreathWeaponBuff  
                    Resources.GetBlueprint<BlueprintBuff>("e8307c93669e05c4ea235a70bf5c8f98"), //FormOfTheDragonIBrassBreathWeaponBuff  
                    Resources.GetBlueprint<BlueprintBuff>("93e27994169df9c43885394dc68f137f"), //FormOfTheDragonIBlueBreathWeaponBuff  
                    Resources.GetBlueprint<BlueprintBuff>("5d7089b61f459204993a1292d6f158f8"), //FormOfTheDragonIBlackBreathWeaponBuff  
                    Resources.GetBlueprint<BlueprintBuff>("ee6c7f5437a57ad48aaf47320129df33"), //KitsunePolymorphBuff  
                    Resources.GetBlueprint<BlueprintBuff>("a13e2e71485901045b1722824019d6f5")  //KitsunePolymorphBuff_Nenio  
                };
                polymorphBuffs
                    .OrderBy(buff => buff.name)
                    .ForEach(buff => {
                        buff.RemoveComponents<SpellDescriptorComponent>(c => c.Descriptor.HasAnyFlag(SpellDescriptor.Polymorph));
                        TTTContext.Logger.LogPatch("Patched", buff);
                    });
            }
        }
    }
}
