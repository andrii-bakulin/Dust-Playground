using System.Collections.Generic;
using UnityEngine;

namespace DustEngine.DustEditor.Playground.UI
{
    public static class Icons
    {
        internal const string ACTION_DELETE = "UI/Action-Delete";
        internal const string ACTION_ADD_DEFORMER = "UI/Add-Deformer";
        internal const string ACTION_ADD_FACTORY_MACHINE = "UI/Add-Factory-Machine";
        internal const string ACTION_ADD_FIELD = "UI/Add-Field";

        internal const string STATE_ENABLED = "UI/State-Enabled";
        internal const string STATE_DISABLED = "UI/State-Disabled";

        private static readonly Dictionary<string, Texture> classIconsCache = new Dictionary<string, Texture>();

        private static readonly string[] resourcePaths =
        {
            "Components/Deformers/",

            "Components/Factory/",
            "Components/FactoryMachines/",
            
            "Components/Fields/",
            
            "Components/Gizmos/",
        };

        //--------------------------------------------------------------------------------------------------------------

        public static Texture GetTextureByComponent(Component component)
            => GetTextureByComponent(component, "");

        public static Texture GetTextureByComponent(Component component, string suffix)
        {
            if (Dust.IsNull(component))
                return null;

            return GetTextureByClassName(component.GetType().ToString(), suffix);
        }

        public static Texture GetTextureByClassName(string className)
            => GetTextureByClassName(className, "");

        public static Texture GetTextureByClassName(string className, string suffix)
        {
            string classNameId = className + "::" + suffix;

            if (!classIconsCache.ContainsKey(classNameId))
            {
                string resourceFilename = className;

                if (suffix != "")
                    resourceFilename += "-" + suffix;

                foreach (var resourcePath in resourcePaths)
                {
                    var resourceId = resourcePath + resourceFilename;

                    classIconsCache[className] = Resources.Load(resourceId) as Texture;

                    if (Dust.IsNotNull(classIconsCache[className]))
                        break;
                }
            }

            return classIconsCache[className];
        }
    }
}
