﻿using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UnitLogic.Mechanics.Actions;
using Kingmaker.Visual.Animation.Kingmaker;
using Kingmaker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using Kingmaker.UnitLogic;
using Kingmaker.Items;
using Kingmaker.Items.Slots;
using Kingmaker.Enums;
using Kingmaker.Blueprints;
using PrestigePlus.Blueprint.Feat;
using Kingmaker.Utility;
using Kingmaker.UnitLogic.Buffs;
using static Kingmaker.UnitLogic.Abilities.Components.AbilityCustomVitalStrike;

namespace PrestigePlus.CustomAction.OtherFeatRelated
{
    internal class KasathaExtraAttack : ContextAction
    {
        public override string GetCaption()
        {
            return "KasathaExtraAttack";
        }
        private static readonly LogWrapper Logger = LogWrapper.Get("PrestigePlus");
        // Token: 0x0600CBFF RID: 52223 RVA: 0x0034ECD0 File Offset: 0x0034CED0
        public override void RunAction()
        {
            try
            {
                UnitEntityData unit = Target.Unit;
                if (unit == null)
                {
                    PFLog.Default.Error("Target unit is missing", Array.Empty<object>());
                    return;
                }
                UnitEntityData maybeCaster = Context.MaybeCaster;
                if (maybeCaster == null)
                {
                    PFLog.Default.Error("Caster is missing", Array.Empty<object>());
                    return;
                }
                if (maybeCaster.Body.IsPolymorphed)
                {
                    return;
                }
                var weapons = new List<ItemEntityWeapon> { };
                foreach (var slot in maybeCaster.Body.EquipmentSlots)
                {
                    if ((slot == maybeCaster.Body.HandsEquipmentSets[0].PrimaryHand || slot == maybeCaster.Body.HandsEquipmentSets[0].SecondaryHand) && slot != maybeCaster.Body.PrimaryHand && slot != maybeCaster.Body.SecondaryHand)
                    {
                        if (slot.HasItem && slot.Item is ItemEntityWeapon weapon)
                        {
                            weapons.Add(weapon);
                            Logger.Info(weapon.Name);
                        }
                    }
                }
                if (!weapons.Any()) return;
                RunAttackRule(maybeCaster, unit, weapons[0]);
                if (weapons[0].Blueprint.Double == true)
                {
                    RunAttackRule(maybeCaster, unit, weapons[0]);
                    return;
                }
                if (weapons.Count() < 2) return;
                RunAttackRule(maybeCaster, unit, weapons[1]);
            }
            catch (Exception ex) { Logger.Error("Failed to storm.", ex); }
        }
        private static void RunAttackRule(UnitEntityData maybeCaster, UnitEntityData unit, ItemEntityWeapon weapon)
        {
            if (weapon != null)// && (!weapon.Blueprint.IsTwoHanded || weapon.Blueprint.Double))
            {
                int penalty = 8;
                if (maybeCaster.HasFact(FeatureRefs.TwoWeaponFighting.Reference))
                {
                    penalty = 2;
                }
                if (maybeCaster.HasFact(FeatureRefs.TwoWeaponFightingMythicFeat.Reference))
                {
                    penalty = 0;
                }
                if (!weapon.Blueprint.IsLight && !weapon.Blueprint.Double && !maybeCaster.State.Features.EffortlessDualWielding)
                {
                    penalty += 2;
                }
                if (unit.HasFact(Mantis) && weapon.Blueprint.Category == WeaponCategory.SawtoothSabre)
                {
                    penalty = 0;
                }
                RuleAttackWithWeapon ruleAttackWithWeapon = new(maybeCaster, unit, weapon, penalty)
                {
                    Reason = maybeCaster.Context,
                    AutoHit = false,
                    AutoCriticalThreat = false,
                    AutoCriticalConfirmation = false,
                    ExtraAttack = true,
                    IsFullAttack = true,
                    AttackNumber = 0,
                    AttacksCount = 1
                };
                maybeCaster.Context.TriggerRule(ruleAttackWithWeapon);
            }
        }

        private static readonly BlueprintFeatureReference Mantis = BlueprintTool.GetRef<BlueprintFeatureReference>(DeificObedience.Achaekek3Guid);
    }
}
