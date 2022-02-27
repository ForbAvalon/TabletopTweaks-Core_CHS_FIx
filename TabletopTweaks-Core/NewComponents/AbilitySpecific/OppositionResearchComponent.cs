﻿using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.UnitLogic;

namespace TabletopTweaks.Core.NewComponents.AbilitySpecific {
    /// <summary>
    /// Removes the specified school as an opposition school.
    /// </summary>
    [AllowMultipleComponents]
    [AllowedOn(typeof(BlueprintFeature), false)]
    [TypeId("12a7be82e1c945239fce42ad3410521f")]
    public class OppositionResearchComponent : UnitFactComponentDelegate {
        public override void OnActivate() {
            foreach (Spellbook spellbook in base.Owner.Spellbooks) {
                if (spellbook.OppositionSchools.Contains(School)) {
                    spellbook.ExOppositionSchools.Add(School);
                }
                spellbook.OppositionSchools.RemoveAll(s => s == School);
            }
        }
        public override void OnDeactivate() {
            foreach (Spellbook spellbook in base.Owner.Spellbooks) {
                if (spellbook.ExOppositionSchools.Contains(School)) {
                    spellbook.OppositionSchools.Add(School);
                }
                spellbook.ExOppositionSchools.RemoveAll(s => s == School);
            }
        }
        /// <summary>
        /// School to remove as opposition school.
        /// </summary>
        public SpellSchool School;
    }
}
