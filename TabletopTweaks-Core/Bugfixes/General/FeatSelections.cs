﻿using HarmonyLib;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Utility;
using System.Linq;
using TabletopTweaks.Core.Utilities;
using static TabletopTweaks.Core.Main;

namespace TabletopTweaks.Core.Bugfixes.General {
    static class FeatSelections {
        [HarmonyPatch(typeof(BlueprintsCache), "Init")]
        [HarmonyPriority(Priority.Last)]
        static class BlueprintsCache_Init_Patch {
            static bool Initialized;

            static void Postfix() {
                if (Initialized) return;
                Initialized = true;
                FixFeatSelections();

                static void FixFeatSelections() {
                    if (Main.TTTContext.Fixes.BaseFixes.IsDisabled("FeatSelections")) { return; }

                    TTTContext.Logger.LogHeader("Patching Feat Selections");
                    var allFeats = FeatTools.Selections.BasicFeatSelection.m_AllFeatures;
                    foreach (var feat in allFeats) {
                        FeatTools.Selections.FeatSelections
                            .Where(selection => feat.Get().HasGroup(selection.Group) || feat.Get().HasGroup(selection.Group2))
                            .ForEach(selection => selection.AddFeatures(feat));
                    }
                    var ArcaneDiscoverySelection = Resources.GetModBlueprint<BlueprintFeatureSelection>(modContext: TTTContext, "ArcaneDiscoverySelection");
                    FeatTools.Selections.LoremasterWizardFeatSelection.RemoveFeatures(ArcaneDiscoverySelection);
                }
            }
        }
    }
}
