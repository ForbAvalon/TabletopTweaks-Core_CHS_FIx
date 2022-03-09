﻿using Kingmaker.EntitySystem.Entities;
using Kingmaker.UnitLogic;
using Kingmaker.Utility;
using System.Collections.Generic;
using static TabletopTweaks.Core.NewUnitParts.UnitPartCustomMechanicsFeatures;

namespace TabletopTweaks.Core.NewUnitParts {
    public class UnitPartCustomMechanicsFeatures : OldStyleUnitPart {

        public void AddMechanicsFeature(CustomMechanicsFeature type) {
            CountableFlag MechanicsFeature = GetMechanicsFeature(type);
            MechanicsFeature.Retain();
        }

        public void RemoveMechanicsFeature(CustomMechanicsFeature type) {
            CountableFlag MechanicsFeature = GetMechanicsFeature(type);
            MechanicsFeature.Release();
        }

        public void ClearMechanicsFeature(CustomMechanicsFeature type) {
            CountableFlag MechanicsFeature = GetMechanicsFeature(type);
            MechanicsFeature.ReleaseAll();
        }

        public CountableFlag GetMechanicsFeature(CustomMechanicsFeature type) {
            CountableFlag MechanicsFeature;
            MechanicsFeatures.TryGetValue(type, out MechanicsFeature);
            if (MechanicsFeature == null) {
                MechanicsFeature = new CountableFlag();
                MechanicsFeatures[type] = MechanicsFeature;
            }
            return MechanicsFeature;
        }

        private readonly Dictionary<CustomMechanicsFeature, CountableFlag> MechanicsFeatures = new Dictionary<CustomMechanicsFeature, CountableFlag>();

        //If you want to extend this externally please use something > 1000
        public enum CustomMechanicsFeature : int {
            QuickDraw,
            UseWeaponOneHanded,
            UndersizedMount,
            MountedSkirmisher,
            ManyshotMythic,
            FavoriteMetamagicPersistent,
            FavoriteMetamagicSelective,
            FavoriteMetamagicIntensified,
            FavoriteMetamagicRime,
            FavoriteMetamagicBurning,
            FavoriteMetamagicFlaring,
            FavoriteMetamagicPiercing,
            FavoriteMetamagicSolidShadows,
            IdealizeDiscovery,
            IdealizeDiscoveryUpgrade
        }
    }
    public static class CustomMechanicsFeaturesExtentions {
        public static CountableFlag CustomMechanicsFeature(this UnitDescriptor unit, CustomMechanicsFeature type) {
            var mechanicsFeatures = unit.Ensure<UnitPartCustomMechanicsFeatures>();
            return mechanicsFeatures.GetMechanicsFeature(type);
        }

        public static CountableFlag CustomMechanicsFeature(this UnitEntityData unit, CustomMechanicsFeature type) {
            return unit.Descriptor.CustomMechanicsFeature(type);
        }
    }
}
