﻿using BlueprintCore.Actions.Builder;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Abilities;
using BlueprintCore.Blueprints.CustomConfigurators.UnitLogic.Buffs;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils.Types;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Buffs;
using Kingmaker.UnitLogic.Parts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingmaker.Craft;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using BlueprintCore.Actions.Builder.ContextEx;
using static Kingmaker.Visual.Animation.Kingmaker.Actions.UnitAnimationActionCastSpell;
using BlueprintCore.Blueprints.Configurators.Items.Weapons;
using BlueprintCore.Blueprints.Configurators.Items.Ecnchantments;
using Kingmaker.Blueprints.Items.Weapons;
using Kingmaker.Blueprints;
using Kingmaker.ElementsSystem;
using Kingmaker.Items.Slots;
using Kingmaker.Items;
using Kingmaker.PubSubSystem;
using Kingmaker.UnitLogic.Buffs.Components;
using Kingmaker.UnitLogic.Class.Kineticist;
using Kingmaker.UnitLogic;
using Kingmaker;
using Owlcat.QA.Validation;
using UnityEngine.Serialization;
using UnityEngine;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.RuleSystem.Rules;
using BlueprintCore.Utils;
using HarmonyLib;
using Kingmaker.Blueprints.Items.Ecnchantments;

namespace PrestigePlus.Blueprint.Spell
{
    internal class EmblemGreed
    {
        private const string EmblemGreedAbility = "NewSpell.UseEmblemGreed";
        public static readonly string EmblemGreedAbilityGuid = "{B7D3605D-22A7-4D31-B6FF-52C3BDE43CCA}";

        private const string EmblemGreedBuff = "NewSpell.EmblemGreedBuff";
        public static readonly string EmblemGreedBuffGuid = "{1034AF48-D063-43E1-935C-959D39484484}";

        internal const string DisplayName = "NewSpellEmblemGreed.Name";
        private const string Description = "NewSpellEmblemGreed.Description";

        private const string MaximName = "EmblemGreedWeapon";
        public static readonly string MaximGuid = "{3E39268B-E1E6-404B-8021-E5C03D613C6B}";

        private const string MaximName2 = "EmblemGreedWeapon2";
        public static readonly string MaximGuid2 = "{A3B0F724-6567-47FE-8EC0-3BDC3D2D1341}";

        private const string MaximName3 = "EmblemGreedWeapon3";
        public static readonly string MaximGuid3 = "{408E8A43-D3BF-46DD-9259-989DB3D464F3}";
        public static void Configure()
        {
            var icon = ActivatableAbilityRefs.KineticBladeBlueFlameBlastAbility.Reference.Get().Icon;

            var glaive = ItemWeaponRefs.GlaiveFlamingPlus1.Reference.Get();

            var maxim = ItemWeaponConfigurator.New(MaximName, MaximGuid)
                .SetDisplayNameText(DisplayName)
                .SetDescriptionText(Description)
                .SetFlavorText(Description)
                .SetIcon(glaive.Icon)
                .SetVisualParameters(glaive.m_VisualParameters)
                .SetDC(1)
                .SetType(WeaponTypeRefs.Glaive.ToString())
                .AddToEnchantments(WeaponEnchantmentRefs.Flaming.ToString())
                .AddToEnchantments(WeaponEnchantmentRefs.Enhancement1.ToString())
                .Configure();

            var maxim2 = ItemWeaponConfigurator.New(MaximName2, MaximGuid2)
                .SetDisplayNameText(DisplayName)
                .SetDescriptionText(Description)
                .SetFlavorText(Description)
                .SetIcon(glaive.Icon)
                .SetVisualParameters(glaive.m_VisualParameters)
                .SetDC(1)
                .SetType(WeaponTypeRefs.Glaive.ToString())
                .AddToEnchantments(WeaponEnchantmentRefs.Flaming.ToString())
                .AddToEnchantments(WeaponEnchantmentRefs.Enhancement2.ToString())
                .Configure();

            var maxim3 = ItemWeaponConfigurator.New(MaximName3, MaximGuid3)
                .SetDisplayNameText(DisplayName)
                .SetDescriptionText(Description)
                .SetFlavorText(Description)
                .SetIcon(glaive.Icon)
                .SetVisualParameters(glaive.m_VisualParameters)
                .SetDC(1)
                .SetType(WeaponTypeRefs.Glaive.ToString())
                .AddToEnchantments(WeaponEnchantmentRefs.Flaming.ToString())
                .AddToEnchantments(WeaponEnchantmentRefs.Enhancement3.ToString())
                .Configure();

            var buff = BuffConfigurator.New(EmblemGreedBuff, EmblemGreedBuffGuid)
              .SetDisplayName(DisplayName)
              .SetDescription(Description)
              .SetIcon(icon)
              .AddComponent<AddGreedBlade>(c => { c.Blade1 = maxim; c.Blade2 = maxim2; c.Blade3 = maxim3; })
              .AddToFlags(Kingmaker.UnitLogic.Buffs.Blueprints.BlueprintBuff.Flags.StayOnDeath)
              .Configure();

            AbilityConfigurator.NewSpell(EmblemGreedAbility, EmblemGreedAbilityGuid, SpellSchool.Transmutation, canSpecialize: true)
                .SetDisplayName(DisplayName)
                .SetDescription(Description)
              .SetIcon(icon)
              .SetAnimation(CastAnimationStyle.SelfTouch)
              .SetRange(AbilityRange.Personal)
              .SetLocalizedDuration(AbilityRefs.EnlargePerson.Reference.Get().LocalizedDuration)
              .SetAvailableMetamagic(Metamagic.CompletelyNormal, Metamagic.Heighten, Metamagic.Extend, Metamagic.Quicken)
              .AddToSpellLists(level: 6, SpellList.Cleric)
              .AddToSpellLists(level: 6, SpellList.Inquisitor)
              .AddToSpellLists(level: 6, SpellList.Warpriest)
              .AddToSpellLists(level: 7, SpellList.Shaman)
              .AddToSpellLists(level: 6, SpellList.Wizard)
              .AddToSpellLists(level: 6, SpellList.Magus)
              .AddAbilityEffectRunAction(
                actions: ActionsBuilder.New()
                  .ApplyBuff(buff, ContextDuration.Variable(ContextValues.Rank(), Kingmaker.UnitLogic.Mechanics.DurationRate.Minutes), isFromSpell: true)
                  .Build())
              .AddSpellDescriptorComponent(SpellDescriptor.Polymorph)
              .Configure();
        }
    }

    public class AddGreedBlade : UnitBuffComponentDelegate<AddKineticistBladeData>, IAreaActivationHandler, IGlobalSubscriber, ISubscriber, IInitiatorRulebookHandler<RuleCalculateAttackBonusWithoutTarget>, IRulebookHandler<RuleCalculateAttackBonusWithoutTarget>
    {
        public BlueprintItemWeapon Blade1;
        public BlueprintItemWeapon Blade2;
        public BlueprintItemWeapon Blade3;

        public BlueprintItemWeapon Blade 
        {
            get
            {
                if (Buff.Context.Params.CasterLevel < 15) return Blade1;
                if (Buff.Context.Params.CasterLevel < 19) return Blade2;
                return Blade3;
            }
        }
        void IRulebookHandler<RuleCalculateAttackBonusWithoutTarget>.OnEventAboutToTrigger(RuleCalculateAttackBonusWithoutTarget evt)
        {
            if (evt.Initiator == Owner && evt.Weapon?.Blueprint == Blade)
            {
                int num = Buff.Context.Params.CasterLevel - Owner.Stats.BaseAttackBonus;
                ModifierDescriptor des = ModifierDescriptor.UntypedStackable;
                if (num < 0)
                {
                    des = ModifierDescriptor.Penalty;
                }
                evt.AddModifier(num, base.Fact, des);
            }
        }

        void IRulebookHandler<RuleCalculateAttackBonusWithoutTarget>.OnEventDidTrigger(RuleCalculateAttackBonusWithoutTarget evt)
        {

        }
        public override void OnActivate()
        {
            base.OnActivate();
            base.Owner.MarkNotOptimizableInSave(true);
            base.Data.Applied = this.Blade.CreateEntity<ItemEntityWeapon>();
            base.Data.Applied.MakeNotLootable();
            HandSlot handSlot = Owner.Body.PrimaryHand;
            if (!handSlot.CanInsertItem(base.Data.Applied, false))
            {
                base.Data.Applied = null;
                PFLog.Default.Error("Can't insert kineticist weapon to target hand", Array.Empty<object>());
                return;
            }
            using (ContextData<ItemsCollection.SuppressEvents>.Request())
            {
                handSlot.InsertItem(base.Data.Applied);
            }
        }

        // Token: 0x0600CC05 RID: 52229 RVA: 0x00350E00 File Offset: 0x0034F000
        public override void OnDeactivate()
        {
            base.OnDeactivate();
            if (base.Data.Applied != null)
            {
                ItemSlot holdingSlot = base.Data.Applied.HoldingSlot;
                if (holdingSlot != null)
                {
                    holdingSlot.RemoveItem(true);
                }
                using (ContextData<ItemsCollection.SuppressEvents>.Request())
                {
                    ItemsCollection collection = base.Data.Applied.Collection;
                    if (collection != null)
                    {
                        collection.Remove(base.Data.Applied);
                    }
                }
                base.Data.Applied = null;
            }
        }
        public override void ApplyValidation(ValidationContext context, int parentIndex)
        {
            base.ApplyValidation(context, parentIndex);
        }

        // Token: 0x0600CC07 RID: 52231 RVA: 0x00350ECF File Offset: 0x0034F0CF
        public override void OnTurnOn()
        {
            ItemEntityWeapon applied = base.Data.Applied;
            if (applied == null)
            {
                return;
            }
            ItemSlot holdingSlot = applied.HoldingSlot;
            if (holdingSlot == null)
            {
                return;
            }
            holdingSlot.Lock.Retain();
        }

        // Token: 0x0600CC08 RID: 52232 RVA: 0x00350EF5 File Offset: 0x0034F0F5
        public override void OnTurnOff()
        {
            ItemEntityWeapon applied = base.Data.Applied;
            if (applied == null)
            {
                return;
            }
            ItemSlot holdingSlot = applied.HoldingSlot;
            if (holdingSlot == null)
            {
                return;
            }
            holdingSlot.Lock.Release();
        }

        // Token: 0x0600CC09 RID: 52233 RVA: 0x00350F1B File Offset: 0x0034F11B
        public void OnAreaActivated()
        {
            if (base.Data.Applied == null)
            {
                this.OnActivate();
                this.OnTurnOn();
            }
        }
    }

    [HarmonyPatch(typeof(ItemEntity), nameof(ItemEntity.AddEnchantment))]
    internal class EmblemCantEnchantFix
    {
        static void Prefix(ref ItemEntity __instance, ref BlueprintItemEnchantment blueprint)
        {
            try
            {
                if (__instance.Blueprint == Wep.Get() || __instance.Blueprint == Wep2.Get() || __instance.Blueprint == Wep3.Get())
                {
                    blueprint = WeaponEnchantmentRefs.Enhancement1.Reference;
                }
            }
            catch (Exception ex) { Main.Logger.Error("Failed to EmblemCantEnchantFix.", ex); }
        }
        private static BlueprintItemWeaponReference Wep = BlueprintTool.GetRef<BlueprintItemWeaponReference>(EmblemGreed.MaximGuid);
        private static BlueprintItemWeaponReference Wep2 = BlueprintTool.GetRef<BlueprintItemWeaponReference>(EmblemGreed.MaximGuid2);
        private static BlueprintItemWeaponReference Wep3 = BlueprintTool.GetRef<BlueprintItemWeaponReference>(EmblemGreed.MaximGuid3);
    }

    [HarmonyPatch(typeof(ItemEntity), nameof(ItemEntity.CanBeEquippedBy))]
    internal class EmblemCantEnchantFix2
    {
        static void Postfix(ref ItemEntity __instance, ref bool __result)
        {
            try
            {
                if (__instance.Blueprint == Wep.Get() || __instance.Blueprint == Wep2.Get() || __instance.Blueprint == Wep3.Get())
                {
                    __result = true;
                }
            }
            catch (Exception ex) { Main.Logger.Error("Failed to EmblemCantEnchantFix2.", ex); }
        }
        private static BlueprintItemWeaponReference Wep = BlueprintTool.GetRef<BlueprintItemWeaponReference>(EmblemGreed.MaximGuid);
        private static BlueprintItemWeaponReference Wep2 = BlueprintTool.GetRef<BlueprintItemWeaponReference>(EmblemGreed.MaximGuid2);
        private static BlueprintItemWeaponReference Wep3 = BlueprintTool.GetRef<BlueprintItemWeaponReference>(EmblemGreed.MaximGuid3);
    }
}
