﻿using BlueprintCore.Blueprints.CustomConfigurators.Classes.Selection;
using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.References;
using Kingmaker.Blueprints.Classes;
using PrestigePlus.Blueprint.Archetype;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingmaker.Enums;
using Kingmaker.Blueprints.Classes.Spells;
using BlueprintCore.Actions.Builder;
using PrestigePlus.CustomAction.OtherFeatRelated;
using Kingmaker.Items;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UnitLogic.ActivatableAbilities.Restrictions;
using Kingmaker.UnitLogic;

namespace PrestigePlus.Blueprint.Feat
{
    internal class RacialHeritage
    {
        private const string RacialHeritageFeat = "RacialHeritage.RacialHeritage";
        public static readonly string RacialHeritageGuid = "{240032CB-F8E6-4D08-8A43-F0003D3669A6}";

        internal const string RacialHeritageDisplayName = "RacialHeritage.Name";
        private const string RacialHeritageDescription = "RacialHeritage.Description";
        public static void RacialHeritageConfigure()
        {
            var icon = FeatureSelectionRefs.BloodlineAscendance.Reference.Get().Icon;

            var feat = FeatureSelectionConfigurator.New(RacialHeritageFeat, RacialHeritageGuid)
              .SetDisplayName(RacialHeritageDisplayName)
              .SetDescription(RacialHeritageDescription)
              .SetIcon(icon)
              .SetIgnorePrerequisites(false)
              .SetObligatory(false)
              .AddToAllFeatures(MultiArmedFeat())
              .AddToAllFeatures(MagicKitsuneFeat())
              .AddToAllFeatures(BloodDrinkFeat())
              .AddToAllFeatures(DarkPowersFeat())
              .AddToAllFeatures(CelestialCrusaderFeat())
              .Configure();

            FeatureSelectionConfigurator.For(FeatureSelectionRefs.MythicFeatSelection)
                .AddToAllFeatures(feat)
                .Configure();

            try { 
            FeatureSelectionConfigurator.For("0d3a3619-9d99-47af-8e47-cb6cc4d26821") //ttt
                .AddToAllFeatures(feat)
                .Configure();
            } catch(Exception e) { Main.Logger.Error("Failed to edit ttt mythic feat.", e); }

        }

        private const string MultiArmed = "RacialHeritage.MultiArmed";
        public static readonly string MultiArmedGuid = "{025D261A-1C63-4F66-ADB6-3F768D3FB47C}";

        internal const string MultiArmedDisplayName = "RacialHeritageMultiArmed.Name";
        private const string MultiArmedDescription = "RacialHeritageMultiArmed.Description";

        public static BlueprintFeature MultiArmedFeat()
        {
            var icon = FeatureRefs.TwoWeaponFightingGreater.Reference.Get().Icon;

            return FeatureConfigurator.New(MultiArmed, MultiArmedGuid)
              .SetDisplayName(MultiArmedDisplayName)
              .SetDescription(MultiArmedDescription)
              .SetIcon(icon)
              .AddComponent<FourWeaponFightingDamagePenalty>()
              .AddInitiatorAttackWithWeaponTrigger(ActionsBuilder.New().Add<KasathaExtraAttack>().Build(),
                    onlyOnFullAttack: true, onlyOnFirstAttack: true, onlyHit: false)
              .Configure();
        }

        private const string MagicKitsune = "RacialHeritage.MagicKitsune";
        public static readonly string MagicKitsuneGuid = "{162A55BA-6153-4AFD-B72D-CD357F331245}";

        internal const string MagicKitsuneDisplayName = "RacialHeritageMagicKitsune.Name";
        private const string MagicKitsuneDescription = "RacialHeritageMagicKitsune.Description";

        public static BlueprintFeature MagicKitsuneFeat()
        {
            var icon = FeatureRefs.KitsuneMagic.Reference.Get().Icon;

            return FeatureConfigurator.New(MagicKitsune, MagicKitsuneGuid)
              .SetDisplayName(MagicKitsuneDisplayName)
              .SetDescription(MagicKitsuneDescription)
              .SetIcon(icon)
              .AddPrerequisiteNoFeature(RaceRefs.KitsuneRace.ToString())
              .AddFacts(new() { FeatureRefs.KitsuneMagic.ToString() })
              .Configure();
        }

        private const string CelestialCrusader = "RacialHeritage.CelestialCrusader";
        public static readonly string CelestialCrusaderGuid = "{2677E30E-E2EC-4FC9-907F-C5C852647A1E}";

        internal const string CelestialCrusaderDisplayName = "RacialHeritageCelestialCrusader.Name";
        private const string CelestialCrusaderDescription = "RacialHeritageCelestialCrusader.Description";

        public static BlueprintFeature CelestialCrusaderFeat()
        {
            var icon = FeatureRefs.TorturedCrusaderFeature.Reference.Get().Icon;

            return FeatureConfigurator.New(CelestialCrusader, CelestialCrusaderGuid)
              .SetDisplayName(CelestialCrusaderDisplayName)
              .SetDescription(CelestialCrusaderDescription)
              .SetIcon(icon)
              .AddPrerequisiteNoFeature(RaceRefs.AasimarRace.ToString())
              .AddAttackBonusAgainstFactOwner(1, null, FeatureRefs.SubtypeEvil.ToString(), ModifierDescriptor.Insight)
              .AddACBonusAgainstFactOwner(null, 1, FeatureRefs.SubtypeEvil.ToString(), ModifierDescriptor.Insight)
              .Configure();
        }

        private const string BloodDrink = "RacialHeritage.BloodDrink";
        public static readonly string BloodDrinkGuid = "{BEFE3D55-7856-44D0-83D5-599BAEA1F345}";

        internal const string BloodDrinkDisplayName = "RacialHeritageBloodDrink.Name";
        private const string BloodDrinkDescription = "RacialHeritageBloodDrink.Description";

        public static BlueprintFeature BloodDrinkFeat()
        {
            var icon = FeatureRefs.NegativeEnergyAffinityDhampir.Reference.Get().Icon;

            return FeatureConfigurator.New(BloodDrink, BloodDrinkGuid)
              .SetDisplayName(BloodDrinkDisplayName)
              .SetDescription(BloodDrinkDescription)
              .SetIcon(icon)
              .AddPrerequisiteNoFeature(RaceRefs.DhampirRace.ToString())
              .AddFacts(new() { FeatureRefs.NegativeEnergyAffinityDhampir.ToString(), FeatureRefs.BloodDrinker.ToString() })
              .Configure();
        }

        private const string DarkPowers = "RacialHeritage.DarkPowers";
        public static readonly string DarkPowersGuid = "{88A85C3D-789A-41D3-83D2-D18B54E804E5}";

        internal const string DarkPowersDisplayName = "RacialHeritageDarkPowers.Name";
        private const string DarkPowersDescription = "RacialHeritageDarkPowers.Description";

        public static BlueprintFeature DarkPowersFeat()
        {
            var icon = AbilityRefs.InflictCriticalWoundsMass.Reference.Get().Icon;

            return FeatureConfigurator.New(DarkPowers, DarkPowersGuid)
              .SetDisplayName(DarkPowersDisplayName)
              .SetDescription(DarkPowersDescription)
              .SetIcon(icon)
              .AddIncreaseSpellDescriptorDC(2, SpellDescriptor.Evil, spellsOnly: true)
              .Configure();
        }
    }
    public class FourWeaponFightingDamagePenalty : UnitFactComponentDelegate, IInitiatorRulebookHandler<RuleCalculateWeaponStats>, IRulebookHandler<RuleCalculateWeaponStats>, ISubscriber, IInitiatorRulebookSubscriber
    {
        // Token: 0x0600EC06 RID: 60422 RVA: 0x003C95A4 File Offset: 0x003C77A4
        public void OnEventAboutToTrigger(RuleCalculateWeaponStats evt)
        {
            ItemEntityWeapon maybeWeapon = evt.Initiator.Body.PrimaryHand.MaybeWeapon;
            if (evt.Weapon == null || maybeWeapon == evt.Weapon || evt.Initiator.Descriptor.State.Features.DoubleSlice)
            {
                return;
            }
            evt.HalfDamageBonus = true;
        }

        // Token: 0x0600EC07 RID: 60423 RVA: 0x003C9610 File Offset: 0x003C7810
        public void OnEventDidTrigger(RuleCalculateWeaponStats evt)
        {
        }
    }
}
