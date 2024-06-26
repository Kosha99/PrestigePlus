﻿using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils;
using Kingmaker.Blueprints;
using Kingmaker.Designers;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UnitLogic.Mechanics.Actions;
using Kingmaker.Visual.Animation.Kingmaker;
using Kingmaker;
using PrestigePlus.Blueprint.Archetype;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using Kingmaker.UnitLogic;
using PrestigePlus.CustomAction.OtherManeuver;

namespace PrestigePlus.CustomAction.OtherFeatRelated
{
    internal class StaggeringAssaultAttack : ContextAction
    {
        public override string GetCaption()
        {
            return "StaggeringAssaultAttack";
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
                RunAttackRule(maybeCaster, unit);
            }
            catch (Exception ex) { Logger.Error("Failed to storm.", ex); }
        }
        private void RunAttackRule(UnitEntityData maybeCaster, UnitEntityData unit)
        {
            var weapon = maybeCaster.GetThreatHand()?.Weapon ?? maybeCaster.Body.EmptyHandWeapon;
            if (weapon != null)
            {
                var attackAnimation = maybeCaster.View.AnimationManager.CreateHandle(UnitAnimationType.SpecialAttack);
                maybeCaster.View.AnimationManager.Execute(attackAnimation);
                RuleAttackWithWeapon ruleAttackWithWeapon = new(maybeCaster, unit, weapon, 0)
                {
                    Reason = maybeCaster.Context,
                    AutoHit = false,
                    AutoCriticalThreat = false,
                    AutoCriticalConfirmation = false,
                    ExtraAttack = true,
                    IsFullAttack = false,
                    AttackNumber = 0,
                    AttacksCount = 1
                };
                var rule = maybeCaster.Context.TriggerRule(ruleAttackWithWeapon);
                if (rule.AttackRoll?.IsHit == true)
                {
                    var AttackBonusRule = new RuleCalculateAttackBonus(maybeCaster, unit, weapon, 0) { };
                    if (rule.MeleeDamage?.Result > 0)
                    {
                        AttackBonusRule.AddModifier(rule.MeleeDamage.Result / 2, descriptor: Kingmaker.Enums.ModifierDescriptor.UntypedStackable);
                    }
                    ContextActionCombatTrickery.TriggerMRule(ref AttackBonusRule);
                    RuleCombatManeuver ruleCombatManeuver = new RuleCombatManeuver(maybeCaster, unit, CombatManeuver.BullRush, AttackBonusRule);
                    maybeCaster.Context.TriggerRule(ruleCombatManeuver);
                }
            }
        }
    }
}