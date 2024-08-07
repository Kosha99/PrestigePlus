﻿using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.CustomConfigurators.Classes.Selection;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils;
using BlueprintCore.Utils.Types;
using Kingmaker.Blueprints;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.Utility;
using PrestigePlus.Blueprint.PrestigeClass;
using PrestigePlus.Grapple;
using PrestigePlus.Patch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrestigePlus.Blueprint.Feat
{
    internal class RippleWater
    {
        private static readonly string FeatNamePro = "RippleWaterPro";
        public static readonly string FeatGuidPro = "{F2675BE8-0E12-4D43-B485-708DA87A972F}";

        private static readonly string FeatNamePro2 = "RippleWaterPro2";
        public static readonly string FeatGuidPro2 = "{D9DBF5C4-642C-475B-9C9A-C99361E172BF}";

        private static readonly string DisplayName = "RippleWater.Name";
        private static readonly string Description = "RippleWater.Description";

        public static void Configure()
        {
            var icon = AbilityRefs.DeathWard.Reference.Get().Icon;

            var featreal = FeatureConfigurator.New(FeatNamePro2, FeatGuidPro2)
                    .SetDisplayName(DisplayName)
                    .SetDescription(Description)
                    .SetIcon(icon)
                    .AddComponent<KineticProgressionContinue>()
                    .SetRanks(20)
                    .SetHideInUI(true)
                    .Configure();

            ProgressionConfigurator.For(ProgressionRefs.KineticBlastProgression)
                .AddToFeaturesRankIncrease(featreal)
                .SetGiveFeaturesForPreviousLevels(true)
                .Configure();

            var pro = ProgressionConfigurator.New(FeatNamePro, FeatGuidPro)
              .SetDisplayName(DisplayName)
              .SetDescription(Description)
              .SetIcon(icon)
              .AddPrerequisiteClassLevel(CharacterClassRefs.KineticistClass.ToString(), 1)
              .AddPrerequisiteClassLevel(CharacterClassRefs.MonkClass.ToString(), 3)
              .AddPointBlankMaster(Kingmaker.Enums.WeaponCategory.KineticBlast)
              .AddIncreaseResourceAmountBySharedValue(false, AbilityResourceRefs.KiPowerResource.ToString(), ContextValues.Rank())
              .AddIncreaseResourceAmountBySharedValue(false, "fd01f3f969a04febab7877a17aebb812", ContextValues.Rank())
              .AddContextRankConfig(ContextRankConfigs.ClassLevel(new string[] { CharacterClassRefs.KineticistClass.ToString() }).WithOnePlusDiv2Progression())
              .SetClasses(BlueprintTool.GetRef<BlueprintCharacterClassReference>(CharacterClassRefs.MonkClass.ToString()))
              .SetGiveFeaturesForPreviousLevels(true)
              .AddToLevelEntry(1, featreal)
              .AddToLevelEntry(2, featreal)
              .AddToLevelEntry(3, featreal)
              .AddToLevelEntry(4, featreal)
              .AddToLevelEntry(5, featreal)
              .AddToLevelEntry(6, featreal)
              .AddToLevelEntry(7, featreal)
              .AddToLevelEntry(8, featreal)
              .AddToLevelEntry(9, featreal)
              .AddToLevelEntry(10, featreal)
              .AddToLevelEntry(11, featreal)
              .AddToLevelEntry(12, featreal)
              .AddToLevelEntry(13, featreal)
              .AddToLevelEntry(14, featreal)
              .AddToLevelEntry(15, featreal)
              .AddToLevelEntry(16, featreal)
              .AddToLevelEntry(17, featreal)
              .AddToLevelEntry(18, featreal)
              .AddToLevelEntry(19, featreal)
              .AddToLevelEntry(20, featreal)
              .Configure();

            FeatureSelectionConfigurator.For(FeatureSelectionRefs.BasicFeatSelection)
                .AddToAllFeatures(pro)
                .Configure();
            FeatureSelectionConfigurator.For(FeatureSelectionRefs.DragonLevel2FeatSelection)
                .AddToAllFeatures(pro)
                .Configure();
            FeatureSelectionConfigurator.For(FeatureSelectionRefs.FighterFeatSelection)
                .AddToAllFeatures(pro)
                .Configure();
            FeatureSelectionConfigurator.For(FeatureSelectionRefs.CombatTrick)
                .AddToAllFeatures(pro)
                .Configure();
            FeatureSelectionConfigurator.For(FeatureSelectionRefs.LoremasterCombatFeatSelection)
                .AddToAllFeatures(pro)
                .Configure();
            FeatureSelectionConfigurator.For(FeatureSelectionRefs.LoremasterTricksterCombatFeatSelection)
                .AddToAllFeatures(pro)
                .Configure();
            FeatureSelectionConfigurator.For(FeatureSelectionRefs.StudentOfWarCombatFeatSelection)
                .AddToAllFeatures(pro)
                .Configure();
            FeatureSelectionConfigurator.For(FeatureSelectionRefs.EldritchKnightFeatSelection)
                .AddToAllFeatures(pro)
                .Configure();
        }
    }
}
