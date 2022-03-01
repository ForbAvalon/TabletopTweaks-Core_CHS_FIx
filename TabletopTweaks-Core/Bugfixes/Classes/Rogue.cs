﻿using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Prerequisites;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.Utility;
using System.Collections.Generic;
using System.Linq;
using TabletopTweaks.Core.Utilities;
using TabletopTweaks.Core.Wrappers;
using static TabletopTweaks.Core.Main;

namespace TabletopTweaks.Core.Bugfixes.Clases {
    class Rogue {
        [HarmonyPatch(typeof(BlueprintsCache), "Init")]
        static class BlueprintsCache_Init_Patch {
            static bool Initialized;

            static void Postfix() {
                if (Initialized) return;
                Initialized = true;
                TTTContext.Logger.LogHeader("Patching Rogue");

                PatchBase();
                PatchEldritchScoundrel();
                PatchSylvanTrickster();
            }
            static void PatchBase() {
                PatchTrapfinding();
                PatchRogueTalentSelection();
                PatchSlipperyMind();

                void PatchTrapfinding() {
                    if (Main.TTTContext.Fixes.Rogue.Base.IsDisabled("Trapfinding")) { return; }
                    var Trapfinding = Resources.GetBlueprint<BlueprintFeature>("dbb6b3bffe6db3547b31c3711653838e");
                    Trapfinding.AddComponent(Helpers.Create<AddContextStatBonus>(c => {
                        c.Stat = StatType.SkillThievery;
                        c.Multiplier = 1;
                        c.Value = new ContextValue() {
                            ValueType = ContextValueType.Rank
                        };
                    }));
                    Trapfinding.SetDescription("A rogue adds 1/2 her level on Perception checks and Trickery checks.");
                    TTTContext.Logger.LogPatch("Patched", Trapfinding);
                }
                void PatchRogueTalentSelection() {
                    if (Main.TTTContext.Fixes.Rogue.Base.IsDisabled("RogueTalentSelection")) { return; }
                    var RogueTalentSelection = Resources.GetBlueprint<BlueprintFeatureSelection>("c074a5d615200494b8f2a9c845799d93");
                    RogueTalentSelection.AllFeatures.ForEach(feature => {
                        if (!feature.HasGroup(FeatureGroup.Feat) && feature.HasGroup(FeatureGroup.RogueTalent)) {
                            feature.AddPrerequisite<PrerequisiteNoFeature>(p => {
                                p.m_Feature = feature.ToReference<BlueprintFeatureReference>();
                                p.Group = Prerequisite.GroupType.All;
                            });
                        }
                    });
                    //RogueTalentSelection.Mode = SelectionMode.OnlyNew;
                    TTTContext.Logger.LogPatch("Patched", RogueTalentSelection);
                }
                void PatchSlipperyMind() {
                    if (Main.TTTContext.Fixes.Rogue.Base.IsDisabled("SlipperyMind")) { return; }
                    var AdvanceTalents = Resources.GetBlueprint<BlueprintFeature>("a33b99f95322d6741af83e9381b2391c");
                    var SlipperyMind = Resources.GetBlueprint<BlueprintFeature>("a14e8c1801911334f96d410f10eab7bf");
                    SlipperyMind.AddComponent(Helpers.Create<RecalculateOnStatChange>(c => {
                        c.Stat = StatType.Dexterity;
                    }));
                    SlipperyMind.AddPrerequisiteFeature(AdvanceTalents);
                    TTTContext.Logger.LogPatch("Patched", SlipperyMind);
                }
            }
            static void PatchEldritchScoundrel() {
                PatchSneakAttackProgression();
                PatchRogueTalentProgression();

                void PatchSneakAttackProgression() {
                    if (Main.TTTContext.Fixes.Rogue.Archetypes["EldritchScoundrel"].IsDisabled("SneakAttackProgression")) { return; }
                    var EldritchScoundrelArchetype = Resources.GetBlueprint<BlueprintArchetype>("57f93dd8423c97c49989501281296c4a");
                    var SneakAttack = Resources.GetBlueprint<BlueprintFeature>("9b9eac6709e1c084cb18c3a366e0ec87");
                    EldritchScoundrelArchetype.RemoveFeatures = EldritchScoundrelArchetype.RemoveFeatures.AppendToArray(Helpers.CreateLevelEntry(1, SneakAttack));

                    TTTContext.Logger.LogPatch("Patched", EldritchScoundrelArchetype);
                }
                void PatchRogueTalentProgression() {
                    if (Main.TTTContext.Fixes.Rogue.Archetypes["EldritchScoundrel"].IsDisabled("RogueTalentProgression")) { return; }
                    var EldritchScoundrelArchetype = Resources.GetBlueprint<BlueprintArchetype>("57f93dd8423c97c49989501281296c4a");
                    var SneakAttack = Resources.GetBlueprint<BlueprintFeature>("9b9eac6709e1c084cb18c3a366e0ec87");
                    var RogueTalentSelection = Resources.GetBlueprint<BlueprintFeatureSelection>("c074a5d615200494b8f2a9c845799d93");
                    var UncannyDodgeChecker = Resources.GetBlueprint<BlueprintFeature>("8f800ed6ce8c42e8a01fd8f3e990c459");

                    EldritchScoundrelArchetype.RemoveFeatures = EldritchScoundrelArchetype.RemoveFeatures
                        .Where(entry => entry.Level != 4)
                        .AddItem(new LevelEntry() {
                            m_Features = new List<BlueprintFeatureBaseReference>() { RogueTalentSelection.ToReference<BlueprintFeatureBaseReference>() },
                            Level = 2
                        })
                        .AddItem(new LevelEntry() {
                            m_Features = new List<BlueprintFeatureBaseReference>() { UncannyDodgeChecker.ToReference<BlueprintFeatureBaseReference>() },
                            Level = 4
                        }).ToArray();
                    TTTContext.Logger.LogPatch("Patched", EldritchScoundrelArchetype);
                }
            }
            static void PatchSylvanTrickster() {
                PatchRogueTalentSelection();

                void PatchRogueTalentSelection() {
                    if (Main.TTTContext.Fixes.Rogue.Archetypes["SylvanTrickster"].IsDisabled("FeyTricks")) { return; }
                    var RogueTalentSelection = Resources.GetBlueprint<BlueprintFeatureSelection>("c074a5d615200494b8f2a9c845799d93");
                    var SylvanTricksterTalentSelection = Resources.GetBlueprint<BlueprintFeatureSelection>("290bbcc3c3bb92144b853fd8fb8ff452");
                    SylvanTricksterTalentSelection.AddFeatures(RogueTalentSelection.m_AllFeatures);

                    TTTContext.Logger.LogPatch("Patched", SylvanTricksterTalentSelection);
                }
            }
        }
    }
}
