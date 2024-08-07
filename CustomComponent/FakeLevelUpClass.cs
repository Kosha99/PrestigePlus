﻿using BlueprintCore.Utils;
using Kingmaker.Blueprints;
using Kingmaker;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Enums;
using Kingmaker.PubSubSystem;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Class.LevelUp;
using Kingmaker.UnitLogic.Class.LevelUp.Actions;
using PrestigePlus.CustomComponent.PrestigeClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Kingmaker.UI.CanvasScalerWorkaround;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.View;
using PrestigePlus.Blueprint.PrestigeClass;
using Kingmaker.UnitLogic.Class.Kineticist;
using BlueprintCore.Blueprints.References;
using PrestigePlus.Blueprint.CombatStyle;
using HarmonyLib;
using Kingmaker.Controllers.Combat;
using Kingmaker.Controllers.Projectiles;
using Kingmaker.Designers;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.Utility;
using PrestigePlus.Blueprint.Archetype;
using Kingmaker.UI.MVVM._VM.ServiceWindows.CharacterInfo.Sections.Progression.Main;
using UniRx;

namespace PrestigePlus.CustomComponent
{
    [TypeId("{EECCF91C-0225-4F5A-96B7-1B33E9CB4CD0}")]
    internal class FakeLevelUpClass : UnitFactComponentDelegate, IUnitSubscriber, ISubscriber
    {
        //private static readonly LogWrapper Logger = LogWrapper.Get("PrestigePlus");
        public override void OnActivate()
        {
            LevelUpController controller = Game.Instance?.LevelUpController;
            if (controller == null) { return; }
            var realclazz = BlueprintTool.GetRef<BlueprintCharacterClassReference>(clazz)?.Get();
            var data = Owner.Progression.GetClassData(realclazz);
            if (data == null || data.Level >= 20) { return; }
            ClassData classData = Owner.Progression.SureClassData(FakeClass);
            classData.Level--;
            Owner.Progression.m_ClassesOrder.Add(FakeClass);
            data.Level += 1;
            LevelUpHelper.UpdateProgression(controller.State, Owner, realclazz.Progression);
            ApplySpell(controller.State, Owner, data.Level, realclazz);
            List<Feature> features = new();
            foreach (var progress in Owner.Progression.Features)
            {
                features.Add(progress);
            }
            foreach (var progress in features)
            {
                if (progress.Blueprint is BlueprintProgression pro)
                {
                    LevelUpHelper.UpdateProgression(controller.State, Owner, pro);
                }
            }
        }

        public string clazz;

        public void ApplySpell(LevelUpState state, UnitDescriptor unit, int level, BlueprintCharacterClass realclazz)
        {
            if (realclazz == null)
            {
                return;
            }
            SkipLevelsForSpellProgression component = realclazz.GetComponent<SkipLevelsForSpellProgression>();
            if (component != null && component.Levels.Contains(level))
            {
                return;
            }
            ClassData classData = unit.Progression.GetClassData(realclazz);
            if (classData.Spellbook != null)
            {
                Spellbook spellbook = unit.DemandSpellbook(classData.Spellbook);
                if (realclazz.Spellbook && realclazz.Spellbook != classData.Spellbook)
                {
                    Spellbook spellbook2 = unit.Spellbooks.FirstOrDefault((Spellbook s) => s.Blueprint == realclazz.Spellbook);
                    if (spellbook2 != null)
                    {
                        foreach (AbilityData abilityData in spellbook2.GetAllKnownSpells())
                        {
                            spellbook.AddKnown(abilityData.SpellLevel, abilityData.Blueprint, true);
                        }
                        unit.DeleteSpellbook(realclazz.Spellbook);
                    }
                }
                int num = classData.CharacterClass.IsMythic ? spellbook.CasterLevel : spellbook.BaseLevel;
                spellbook.AddLevelFromClass(classData.CharacterClass);
                int classLevel = classData.CharacterClass.IsMythic ? spellbook.CasterLevel : spellbook.BaseLevel;
                bool flag = num == 0;
                SpellSelectionData spellSelectionData = state.DemandSpellSelection(spellbook.Blueprint, spellbook.Blueprint.SpellList);
                if (spellbook.Blueprint.SpellsKnown != null)
                {
                    BlueprintSpellsTable blueprintSpellsTable = spellbook.Blueprint.SpellsKnown;
                    if (classData.CharacterClass.IsMythic)
                    {
                        UnitFact unitFact = unit.Facts.Get((UnitFact x) => x.Blueprint is BlueprintFeatureSelectMythicSpellbook);
                        if ((unitFact?.Blueprint) is BlueprintFeatureSelectMythicSpellbook blueprintFeatureSelectMythicSpellbook)
                        {
                            blueprintSpellsTable = blueprintFeatureSelectMythicSpellbook.SpellKnownForSpontaneous;
                            if (blueprintSpellsTable != null)
                            {
                                num = spellbook.MythicLevel - 1;
                                classLevel = spellbook.MythicLevel;
                            }
                        }
                    }
                    for (int i = 0; i <= 10; i++)
                    {
                        int num2 = Math.Max(0, blueprintSpellsTable.GetCount(num, i));
                        int num3 = Math.Max(0, blueprintSpellsTable.GetCount(classLevel, i));
                        spellSelectionData.SetLevelSpells(i, Math.Max(0, num3 - num2));
                    }
                }
                int maxSpellLevel = spellbook.MaxSpellLevel;
                if (spellbook.Blueprint.SpellsPerLevel > 0)
                {
                    if (flag)
                    {
                        spellSelectionData.SetExtraSpells(0, maxSpellLevel);
                        spellSelectionData.ExtraByStat = true;
                        spellSelectionData.UpdateMaxLevelSpells(unit);
                    }
                    else
                    {
                        spellSelectionData.SetExtraSpells(spellbook.Blueprint.SpellsPerLevel, maxSpellLevel);
                    }
                }
                foreach (AddCustomSpells customSpells in spellbook.Blueprint.GetComponents<AddCustomSpells>())
                {
                    ApplySpellbook.TryApplyCustomSpells(spellbook, customSpells, state, unit);
                }
            }
        }

        private static BlueprintCharacterClassReference FakeClass = BlueprintTool.GetRef<BlueprintCharacterClassReference>(FakeAlignedClass.ArchetypeGuid);
    }

    [HarmonyPatch(typeof(ClassProgressionVM), nameof(ClassProgressionVM.AddAdditionalProgressions))]
    internal class FixNoToybox
    {
        static void Postfix(ref ClassProgressionVM __instance)
        {
            if (__instance.Level.Value < 0 && !__instance.ProgressionVms.Any())
            {
                __instance.ProgressionVms = [null];
            }
        }
    }

    [HarmonyPatch(typeof(ClassProgressionVM), nameof(ClassProgressionVM.DisposeImplementation))]
    internal class FixNoToybox2
    {
        static void Prefix(ref ClassProgressionVM __instance)
        {
            if (__instance.ProgressionVms.First() == null)
            {
                __instance.ProgressionVms = [];
            }
        }
    }
}
