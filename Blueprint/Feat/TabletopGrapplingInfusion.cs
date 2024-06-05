﻿using BlueprintCore.Blueprints.Configurators.UnitLogic.ActivatableAbilities;
using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils;
using Kingmaker.Blueprints;
using Kingmaker;
using Kingmaker.Blueprints.Classes;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UnitLogic.ActivatableAbilities;
using Kingmaker.UnitLogic.Mechanics.Actions;
using PrestigePlus.Blueprint.PrestigeClass;
using PrestigePlus.CustomAction.OtherManeuver;
using PrestigePlus.CustomComponent.Feat;
using PrestigePlus.Grapple;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Class.Kineticist;
using Kingmaker.Designers;
using BlueprintCore.Actions.Builder;
using BlueprintCore.Conditions.Builder;
using BlueprintCore.Conditions.Builder.ContextEx;
using Kingmaker.UnitLogic.Mechanics.Components;
using Kingmaker.Enums;
using Kingmaker.PubSubSystem;
using UnityEngine;
using Kingmaker.Controllers.Units;
using static Kingmaker.UI.CanvasScalerWorkaround;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.UI.Common;

namespace PrestigePlus.Blueprint.Feat
{
    internal class TabletopGrapplingInfusion
    {
        private static readonly string TabletopGrapplingDisplayName = "InfusionTabletopGrappling.Name";
        private static readonly string TabletopGrapplingDescription = "InfusionTabletopGrappling.Description";

        private const string TabletopGrapplingbuff = "InfusionTrick.TabletopGrapplingbuff";
        public static readonly string TabletopGrapplingbuffGuid = "{C391227D-DB3D-482C-A305-67A22F3E182B}";

        private const string TabletopGrapplingActivatableAbility = "InfusionTrick.TabletopGrapplingActivatableAbility";
        private static readonly string TabletopGrapplingActivatableAbilityGuid = "{D0872A09-DE1A-4B22-83D4-A883F1A4DCE0}";

        private const string TabletopGrapplingbuff2 = "InfusionTrick.TabletopGrapplingbuff2";
        public static readonly string TabletopGrapplingbuff2Guid = "{7FFC0AEE-F313-4065-B6E9-8FAC57D0F2E0}";

        public static void ConfigureTabletopGrappling()
        {
            var icon = AbilityRefs.ArmyShifterGrabAbility.Reference.Get().Icon;

            var Buff = BuffConfigurator.New(TabletopGrapplingbuff, TabletopGrapplingbuffGuid)
              .SetDisplayName(TabletopGrapplingDisplayName)
              .SetDescription(TabletopGrapplingDescription)
              .AddToFlags(Kingmaker.UnitLogic.Buffs.Blueprints.BlueprintBuff.Flags.StayOnDeath)
              .AddToFlags(Kingmaker.UnitLogic.Buffs.Blueprints.BlueprintBuff.Flags.HiddenInUi)
              .Configure();

            var Buff2 = BuffConfigurator.New(TabletopGrapplingbuff2, TabletopGrapplingbuff2Guid)
              .SetDisplayName(TabletopGrapplingDisplayName)
              .SetDescription(TabletopGrapplingDescription)
              .SetIcon(icon)
              .AddCondition(UnitCondition.MovementBan)
              .AddCondition(UnitCondition.DisableAttacksOfOpportunity)
              .AddComponent<PPGrabInfusionBuff>()
              .Configure();

            var ability = ActivatableAbilityConfigurator.New(TabletopGrapplingActivatableAbility, TabletopGrapplingActivatableAbilityGuid)
                .SetDisplayName(TabletopGrapplingDisplayName)
                .SetDescription(TabletopGrapplingDescription)
                .SetIcon(icon)
                .SetBuff(Buff)
                .SetDeactivateImmediately(true)
                .SetActivationType(AbilityActivationType.Immediately)
                .SetIsOnByDefault(true)
                .Configure();

            FeatureConfigurator.For(FeatureRefs.GrapplingInfusionFeature)
                .AddFacts(new() { ability })
                .Configure();

            var action = ActionsBuilder.New().Add<InfusionGrapple>(c => { c.buff = Buff2; }).Build();
            var cond = ConditionsBuilder.New().CasterHasFact(Buff).HasFact(BuffRefs.GrapplingInfusionEffectBuff.ToString(), true).Build();
            var oldaction = BuffRefs.GrapplingInfusionBuff.Reference.Get().GetComponent<AddKineticistInfusionDamageTrigger>().Actions;
            var newaction = ActionsBuilder.New()
                .Conditional(cond, action, oldaction)
                .Build();

            BuffRefs.GrapplingInfusionBuff.Reference.Get().GetComponent<AddKineticistInfusionDamageTrigger>().Actions = newaction;
        }
    }

    internal class InfusionGrapple : ContextAction
    {
        public override string GetCaption()
        {
            return "Infusion Grapple";
        }

        public void RunGrapple()
        {
            UnitEntityData unit = Target.Unit;
            if (unit == null)
            {
                PFLog.Default.Error("Target unit is missing", Array.Empty<object>());
                return;
            }
            UnitEntityData maybeCaster = Context.MaybeCaster;
            if (maybeCaster?.Get<UnitPartKineticist>() == null)
            {
                PFLog.Default.Error("Caster is missing", Array.Empty<object>());
                return;
            }
            if (unit == maybeCaster)
            {
                PFLog.Default.Error("Unit can't grapple themselves", Array.Empty<object>());
                return;
            }
            int sizebonus = maybeCaster.State.Size.GetModifiers().CMDAndCMD;
            var AttackBonusRule = new RuleCalculateAttackBonus(maybeCaster, unit, maybeCaster.Body.EmptyHandWeapon, 0) { };
            AttackBonusRule.AddModifier(2 - sizebonus, descriptor: Kingmaker.Enums.ModifierDescriptor.UntypedStackable);
            ContextActionCombatTrickery.TriggerMRule(ref AttackBonusRule);
            RuleCombatManeuver ruleCombatManeuver = new(maybeCaster, unit, CombatManeuver.Grapple, AttackBonusRule)
            {
                ReplaceAttackBonus = maybeCaster.Progression.GetClassLevel(CharacterClassRefs.KineticistClass.Reference),
                ReplaceBaseStat = maybeCaster.Get<UnitPartKineticist>().MainStatType
            };
            ruleCombatManeuver = (Context?.TriggerRule(ruleCombatManeuver)) ?? Rulebook.Trigger(ruleCombatManeuver);
            if (ruleCombatManeuver.Success)
            {
                GameHelper.ApplyBuff(unit, buff);
            }
        }

        public override void RunAction()
        {
            RunGrapple();
        }

        public BlueprintBuff buff;
    }

    internal class PPGrabInfusionBuff : PPGrabBuffBase, ITickEachRound
    {
        void ITickEachRound.OnNewRound()
        {
            var maybeCaster = Buff.Context.MaybeCaster;
            var unit = Owner;
            if (maybeCaster == null) { return; }
            int sizebonus = maybeCaster.State.Size.GetModifiers().CMDAndCMD;
            var AttackBonusRule = new RuleCalculateAttackBonus(maybeCaster, unit, maybeCaster.Body.EmptyHandWeapon, 0) { };
            AttackBonusRule.AddModifier(7 - sizebonus, descriptor: ModifierDescriptor.UntypedStackable);
            ContextActionCombatTrickery.TriggerMRule(ref AttackBonusRule);
            RuleCombatManeuver ruleCombatManeuver = new(maybeCaster, unit, CombatManeuver.Grapple, AttackBonusRule)
            {
                ReplaceAttackBonus = maybeCaster.Progression.GetClassLevel(CharacterClassRefs.KineticistClass.Reference),
                ReplaceBaseStat = maybeCaster.Get<UnitPartKineticist>().MainStatType
            };
            ruleCombatManeuver = (Context?.TriggerRule(ruleCombatManeuver)) ?? Rulebook.Trigger(ruleCombatManeuver);
            if (!ruleCombatManeuver.Success)
            {
                UIUtility.SendWarning(maybeCaster.CharacterName + "'s blast fails to maintain grapple.");
                Buff.Remove();
                return;
            }
            UIUtility.SendWarning(maybeCaster.CharacterName + "'s blast maintains grapple.");
            unit.SpendAction(Kingmaker.UnitLogic.Commands.Base.UnitCommand.CommandType.Standard, false, 0);
            int dc = ruleCombatManeuver.CMBRule.Result + 5;
            if (TryBreakFree(unit, dc))
            {
                Buff.Remove();
                return;
            }
        }

        public static bool TryBreakFree(UnitEntityData unit, int dc)
        {
            var context2 = unit.Context;
            StatType statType = StatType.SkillMobility;
            if (unit.Stats.SkillMobility < unit.Stats.SkillAthletics)
            {
                statType = StatType.SkillAthletics;
            }
            return GameHelper.TriggerSkillCheck(new RuleSkillCheck(unit, statType, dc)
            {
                IgnoreDifficultyBonusToDC = unit.IsPlayersEnemy
            }, context2, true).Success;
        }
    }
}
