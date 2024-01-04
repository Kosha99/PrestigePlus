﻿using BlueprintCore.Blueprints.CustomConfigurators.Classes;
using BlueprintCore.Blueprints.References;
using PrestigePlus.Blueprint.PrestigeClass;
using PrestigePlus.Feats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrestigePlus.Blueprint.MythicFeat
{
    internal class BondGenie
    {
        private static readonly string FeatName = "BondGenie";
        private static readonly string FeatGuid = "{BDF243F1-A851-4C3F-9F6B-64E4848A7AE3}";

        private static readonly string DisplayName = "BondGenie.Name";
        private static readonly string Description = "BondGenie.Description";

        public static void Configure()
        {
            var icon = FeatureRefs.DruidNatureBond.Reference.Get().Icon;

            FeatureConfigurator.New(FeatName, FeatGuid, Kingmaker.Blueprints.Classes.FeatureGroup.MythicAbility)
                    .SetDisplayName(DisplayName)
                    .SetDescription(Description)
                    .SetIcon(icon)
                    .AddPrerequisiteFeature(Asavir.ShaitanGuid)
                    .Configure();
        }
    }
}
