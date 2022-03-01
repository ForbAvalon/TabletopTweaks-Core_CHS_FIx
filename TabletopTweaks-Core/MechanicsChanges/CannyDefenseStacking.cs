﻿using HarmonyLib;
using Kingmaker.Designers.Mechanics.Buffs;
using Kingmaker.EntitySystem;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using static TabletopTweaks.Core.Main;
using static TabletopTweaks.Core.MechanicsChanges.AdditionalModifierDescriptors;

namespace TabletopTweaks.Core.MechanicsChanges {
    internal class CannyDefenseStacking {
        [HarmonyPatch(typeof(CannyDefensePermanent), "ActivateModifier")]
        static class CannyDefensePermanent_Stacking_Patch {
            static readonly MethodInfo Modifier_AddModifierUnique = AccessTools.Method(typeof(ModifiableValue), "AddModifierUnique", new Type[] {
                typeof(int),
                typeof(EntityFactComponent),
                typeof(ModifierDescriptor)
            });
            //Change bonus descriptor to Dodge.Intelligence instead of Dodge
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
                if (TTTContext.Fixes.BaseFixes.IsDisabled("DisableCannyDefenseStacking")) { return instructions; }

                var codes = new List<CodeInstruction>(instructions);
                int target = FindInsertionTarget(codes);
                //Utilities.ILUtils.LogIL(codes);
                codes[target] = new CodeInstruction(OpCodes.Ldc_I4, (int)Dodge.Intelligence);
                //Utilities.ILUtils.LogIL(codes);
                return codes.AsEnumerable();
            }
            private static int FindInsertionTarget(List<CodeInstruction> codes) {
                for (int i = 0; i < codes.Count; i++) {
                    //AddModifierUnique is called only once directly after the descriptor is loaded onto the stack
                    if (codes[i].opcode == OpCodes.Callvirt && codes[i].Calls(Modifier_AddModifierUnique)) {
                        return i - 1;
                    }
                }
                TTTContext.Logger.Log("CANNY DEFENSE PATCH: COULD NOT FIND TARGET");
                return -1;
            }
        }
    }
}
