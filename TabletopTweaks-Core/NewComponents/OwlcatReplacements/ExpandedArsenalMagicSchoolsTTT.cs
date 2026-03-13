using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.Enums;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem.Rules.Abilities;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Parts;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static TabletopTweaks.Core.MechanicsChanges.AdditionalModifierDescriptors;


namespace TabletopTweaks.Core.NewComponents.OwlcatReplacements {
    [AllowedOn(typeof(BlueprintParametrizedFeature), false)]
    [TypeId("70f053af93104dcbab7ee6580a85de63")]
    public class ExpandedArsenalMagicSchoolsTTT : UnitFactComponentDelegate, IInitiatorRulebookHandler<RuleCalculateAbilityParams>, IRulebookHandler<RuleCalculateAbilityParams>, ISubscriber, IInitiatorRulebookSubscriber {

        public override void OnTurnOn() {
            base.OnTurnOn();
            SpellSchool valueOrDefault = base.Param.SpellSchool.GetValueOrDefault();
            if (valueOrDefault == SpellSchool.None) {
                return;
            }
            base.Owner.Ensure<UnitPartExpandedArsenal>().AddSpellSchoolEntry(valueOrDefault, base.Fact);
        }

        public override void OnTurnOff() {
            base.OnTurnOff();
            base.Owner.Ensure<UnitPartExpandedArsenal>().RemoveSpellSchoolEntry(base.Fact);
        }

        public void OnEventAboutToTrigger(RuleCalculateAbilityParams evt) {
            AbilityData abilityData = evt.AbilityData;
            SpellSchool applliedSpellSchool = base.Param.SpellSchool.GetValueOrDefault();
            if (applliedSpellSchool == SpellSchool.None) { return; }

            SpellSchool abilitySchool = SpellSchool.None;

            if (abilityData == null) {
                BlueprintAbility spell = evt.Spell;
                if (spell == null) {
                    abilitySchool = SpellSchool.None; ;
                } else {
                    SpellComponent component = spell.GetComponent<SpellComponent>();
                    abilitySchool = ((component != null) ? component.School : SpellSchool.None);
                }
            } else {
                abilitySchool = abilityData.SpellSchool;
            }
            if (abilitySchool == SpellSchool.None) { return; }
            if (abilitySchool != applliedSpellSchool) { return; }

            var SpellFocusFeatures = base.Owner
                .Progression
                .Features
                .Enumerable
                .Where(feature => m_Focuses.Any((BlueprintParametrizedFeatureReference f) => f.Get() == feature.Blueprint))
                .ToList();
            var SchoolMasteryFeatures = base.Owner
                .Progression
                .Features
                .Enumerable
                .Where(feature => m_SchoolMastery.Any((BlueprintParametrizedFeatureReference f) => f.Get() == feature.Blueprint))
                .ToList();

            Dictionary<ModifierDescriptor, int> DCBonuses = new Dictionary<ModifierDescriptor, int>();
            foreach (Feature feature in SpellFocusFeatures) {
                var bonusDC = ((SpellFocusParametrized)feature.BlueprintComponents.Find((BlueprintComponent x) => x is SpellFocusParametrized)).GetBonusDC(evt);
                var descriptor = ((SpellFocusParametrized)feature.BlueprintComponents.Find((BlueprintComponent x) => x is SpellFocusParametrized)).Descriptor;
                if (DCBonuses.ContainsKey(descriptor)) {
                    if (DCBonuses[descriptor] < bonusDC) {
                        DCBonuses[descriptor] = bonusDC;
                    }
                } else {
                    DCBonuses.Add(descriptor, bonusDC);
                }
            }
            foreach (var entry in DCBonuses) {
                evt.AddBonusDC(entry.Value, entry.Key);
            }

            Dictionary<ModifierDescriptor, int> CLBonuses = new Dictionary<ModifierDescriptor, int>();
            foreach (Feature feature in SchoolMasteryFeatures) {
                var bonusDC = 0;
                var descriptor = ModifierDescriptor.None;

                if (feature.GetComponent<SchoolMasteryParametrized>()) {
                    bonusDC = feature.GetComponent<SchoolMasteryParametrized>().GetBonus(evt);
                    descriptor = (ModifierDescriptor)Untyped.SchoolMastery;
                } else if(feature.GetComponent<BonusCasterLevelParametrized>()) {
                    bonusDC = feature.GetComponent<BonusCasterLevelParametrized>().Bonus.Calculate(base.Context);
                    descriptor = feature.GetComponent<BonusCasterLevelParametrized>().Descriptor;
                }
                if (bonusDC != 0 && CLBonuses.ContainsKey(descriptor)) {
                    if (CLBonuses[descriptor] < bonusDC) {
                        CLBonuses[descriptor] = bonusDC;
                    }
                } else {
                    CLBonuses.Add(descriptor, bonusDC);
                }
            }
            foreach (var entry in CLBonuses) {
                evt.AddBonusDC(entry.Value, entry.Key);
            }
        }

        public void OnEventDidTrigger(RuleCalculateAbilityParams evt) {
        }

        [SerializeField]
        public List<BlueprintParametrizedFeatureReference> m_Focuses = new List<BlueprintParametrizedFeatureReference>();

        [SerializeField]
        public List<BlueprintParametrizedFeatureReference> m_SchoolMastery = new List<BlueprintParametrizedFeatureReference>();
    }
}
