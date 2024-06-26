﻿using BlueprintCore.Utils;
using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Items.Weapons;
using Kingmaker.DialogSystem.Blueprints;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Commands;
using Kingmaker.UnitLogic.Commands.Base;
using Kingmaker.UnitLogic.Parts;
using Kingmaker.Utility;
using PrestigePlus.Blueprint.Feat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ChromaSDK.ChromaAnimationAPI;

namespace PrestigePlus.HarmonyFix
{
    [HarmonyPatch(typeof(UnitDescriptor), nameof(UnitDescriptor.GetWeaponRange))]
    internal class AAdistanceFix
    {
        static void Postfix(ref Feet __result, ref UnitDescriptor __instance, ref BlueprintItemWeapon weapon)
        {
            try
            {
                if (weapon == null) { return; }
                if (__instance.HasFact(Arcane) && weapon.IsRanged && !__instance.State.Features.PointBlankRange)
                {
                    __result += weapon.AttackRange;
                }
                if (__instance.HasFact(Noc) && weapon.Category == Kingmaker.Enums.WeaponCategory.Dagger)
                {
                    __result += 5.Feet();
                }
                if (__instance.HasFact(Buff) && weapon.Category == Kingmaker.Enums.WeaponCategory.UnarmedStrike)
                {
                    if (__instance.HasFact(Buff2))
                    {
                        __result = 40.Feet();
                    }
                    else
                    {
                        __result = 30.Feet();
                    }
                }
            }
            catch (Exception ex) { Logger.Error("Failed to AAdistanceFix.", ex); }
        }
        private static readonly LogWrapper Logger = LogWrapper.Get("PrestigePlus");
        private static BlueprintFeatureReference Arcane = BlueprintTool.GetRef<BlueprintFeatureReference>("{1DA01231-4001-46F3-8A31-E748AE8DFBF3}");
        private static BlueprintFeatureReference Noc = BlueprintTool.GetRef<BlueprintFeatureReference>(DeificObedience.Nocticula2Guid);
        private static BlueprintBuffReference Buff = BlueprintTool.GetRef<BlueprintBuffReference>(MageHandTrick.ThrowPunchbuffGuid);
        private static BlueprintBuffReference Buff2 = BlueprintTool.GetRef<BlueprintBuffReference>(MageHandTrick.ReachingHandbuffGuid);
    }
}
