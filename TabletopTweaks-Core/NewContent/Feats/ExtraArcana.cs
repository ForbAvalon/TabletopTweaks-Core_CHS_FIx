﻿using Kingmaker.Blueprints.Classes.Selection;
using TabletopTweaks.Core.Utilities;
using TabletopTweaks.Core.Wrappers;
using static TabletopTweaks.Core.Main;

namespace TabletopTweaks.Core.NewContent.Feats {
    static class ExtraArcana {
        public static void AddExtraArcana() {
            var MagusArcanaSelection = BlueprintTools.GetBlueprint<BlueprintFeatureSelection>("e9dc4dfc73eaaf94aae27e0ed6cc9ada");
            var HexcrafterMagusHexArcanaSelection = BlueprintTools.GetBlueprint<BlueprintFeatureSelection>("ad6b9cecb5286d841a66e23cea3ef7bf");
            var EldritchMagusArcanaSelection = BlueprintTools.GetBlueprint<BlueprintFeatureSelection>("d4b54d9db4932454ab2899f931c2042c");

            var ExtraArcana = FeatTools.CreateExtraSelectionFeat(modContext: TTTContext, "ExtraArcana", MagusArcanaSelection, bp => {
                bp.SetName("Extra Arcana");
                bp.SetDescription("You gain one additional magus arcana. You must meet all the prerequisites for this magus arcana." +
                    "\nYou can gain this feat multiple times. Its effects stack, granting a new arcana each time you gain this feat.");
            });
            var ExtraArcanaHexcrafter = FeatTools.CreateExtraSelectionFeat(modContext: TTTContext, "ExtraArcanaHexcrafter", HexcrafterMagusHexArcanaSelection, bp => {
                bp.SetName("Extra Arcana (Hexcrafter)");
                bp.SetDescription("You gain one additional magus arcana. You must meet all the prerequisites for this magus arcana." +
                    "\nYou can gain this feat multiple times. Its effects stack, granting a new arcana each time you gain this feat.");
            });
            var ExtraArcanaEldritchScion = FeatTools.CreateExtraSelectionFeat(modContext: TTTContext, "ExtraArcanaEldritchScion", EldritchMagusArcanaSelection, bp => {
                bp.SetName("Extra Arcana (Eldritch Scion)");
                bp.SetDescription("You gain one additional magus arcana. You must meet all the prerequisites for this magus arcana." +
                    "\nYou can gain this feat multiple times. Its effects stack, granting a new arcana each time you gain this feat.");
            });
            if (TTTContext.AddedContent.Feats.IsDisabled("ExtraArcana")) { return; }
            FeatTools.AddAsFeat(ExtraArcana);
            FeatTools.AddAsFeat(ExtraArcanaHexcrafter);
            FeatTools.AddAsFeat(ExtraArcanaEldritchScion);
        }
    }
}
