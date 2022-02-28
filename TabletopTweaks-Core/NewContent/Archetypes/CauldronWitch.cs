﻿using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using System.Linq;
using static TabletopTweaks.Core.Main;

namespace TabletopTweaks.Core.NewContent.Archetypes {
    static class CauldronWitch {
        public static void AddCauldrenWitch() {
            if (TTTContext.AddedContent.Archetypes.IsDisabled("CauldronWitchArchetype")) { return; }
            var CauldronWitchArchetype = Resources.GetBlueprint<BlueprintArchetype>("e0012a7015774e140be217f4a1480b6f");
            var WitchClass = Resources.GetBlueprint<BlueprintCharacterClass>("1b9873f1e7bfe5449bc84d03e9c8e3cc");
            WitchClass.m_Archetypes = WitchClass.m_Archetypes.AddItem(CauldronWitchArchetype.ToReference<BlueprintArchetypeReference>()).ToArray();
            TTTContext.Logger.LogPatch("Added", CauldronWitchArchetype);
        }
    }
}
