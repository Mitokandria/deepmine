using BepInEx;
using HarmonyLib;
using BepInEx.Configuration;
using UnityEngine;

namespace DeepMineMod
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class DeepMinePlugin : BaseUnityPlugin
    {
        private ConfigEntry<float> configBedrockDepth;
        //private ConfigEntry<int> configLevelMultiplier;
        private ConfigEntry<int> configOreMinDropQuantity;
        private ConfigEntry<int> configOreMaxDropQuantity;
        //private ConfigEntry<float> configMineCompletionTime;
        private ConfigEntry<float> configMineAmount;
        private ConfigEntry<bool> configDebugMode;

        public static float BedrockDepth;
        public static double LevelMultiplier = 2;
        public static int OreMinDropQuantity;
        public static int OreMaxDropQuantity;
        //public static float MineCompletionTime;
        public static float MineAmount;
        public static bool DebugMode;

        public static double drillMultiplier = 1;

        public static void ModLog(string text)
        {
            Debug.Log($"{PluginInfo.PLUGIN_NAME}: " + text);
        }

        // Awake is called once when both the game and the plug-in are loaded
        void Awake()
        {
            HandleConfig();

            Debug.Log($"Plugin {PluginInfo.PLUGIN_NAME} {PluginInfo.PLUGIN_VERSION} is loaded!");
            var harmony = new Harmony($"{PluginInfo.PLUGIN_GUID}");
            harmony.PatchAll();
            Debug.Log($"{PluginInfo.PLUGIN_NAME} Patching complete!");
        }

        void HandleConfig()
        {
            configBedrockDepth = Config.Bind("General",   // The section under which the option is shown
                                     "BedrockDepth",  // The key of the configuration option in the configuration file
                                     -120f, // The default value
                                     "The depth of the bedrock layer, vanilla is -40. Portable GPR range is 40% of the depth of the bedrock layer.\nThe mod divides the depth of the bedrock by 4, creating 4 layers to spread the ores in different ammounts"); // Description of the option to show in the config file
            BedrockDepth = configBedrockDepth.Value;

            /*configLevelMultiplier = Config.Bind("Mining Level",   // The section under which the option is shown
                                     "LevelMultiplier",  // The key of the configuration option in the configuration file
                                     2, // The default value
                                     "Multiplier to calculate how drop quantity is increased in each layer"); // Description of the option to show in the config file
            LevelMultiplier = configLevelMultiplier.Value;*/

            configOreMinDropQuantity = Config.Bind("Mining Level",   // The section under which the option is shown
                                     "OreMinDropQuantity",  // The key of the configuration option in the configuration file
                                     1, // The default value
                                     "Minimum drop quantity for the first layer (the first layer is the top most layer of the terrain)"); // Description of the option to show in the config file
            OreMinDropQuantity = configOreMinDropQuantity.Value;

            configOreMaxDropQuantity = Config.Bind("Mining Level",   // The section under which the option is shown
                                     "OreMaxDropQuantity",  // The key of the configuration option in the configuration file
                                     3, // The default value
                                     "Maximum drop quantity for the first layer (the first layer is the top most layer of the terrain)"); // Description of the option to show in the config file
            OreMaxDropQuantity = configOreMaxDropQuantity.Value;


            /*configMineCompletionTime = Config.Bind("Mining Tool",   // The section under which the option is shown
                                     "MineCompletionTime",  // The key of the configuration option in the configuration file
                                     0.12f, // The default value
                                     "Time to complete mining when using the tool. Smaller is faster drilling. Vanilla is 0.12"); // Description of the option to show in the config file
            MineCompletionTime = configMineCompletionTime.Value;*/

            configMineAmount = Config.Bind("Mining Tool",   // The section under which the option is shown 
                         "MineAmount",  // The key of the configuration option in the configuration file
                         0.4f, // The default value
                         "How much of the voxel to mine at a time. Larger is faster drilling. Vanilla is 0.2"); // Description of the option to show in the config file
            MineAmount = configMineAmount.Value;

            configDebugMode = Config.Bind("Debug",   // The section under which the option is shown
                                     "DebugMode",  // The key of the configuration option in the configuration file
                                     false, // The default value
                                     "Turns debug mode"); // Description of the option to show in the config file
            DebugMode = configDebugMode.Value;
        }

    }
}