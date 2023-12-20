using System;
using Assets.Scripts;
using Assets.Scripts.Voxel;
using Assets.Scripts.Inventory;
using HarmonyLib;

using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

using Assets.Scripts.Objects.Items;
using Assets.Scripts.Objects;

namespace DeepMineMod
{

    /// <summary>
    /// Alter ore drop quantities based on their world position
    /// </summary>
    [HarmonyPatch(typeof(Minables), MethodType.Constructor, new Type[] { typeof(Minables), typeof(Vector3), typeof(Asteroid)})]
    public class Minables_Constructor
    {
        static void Postfix(Minables __instance, Minables masterInstance, Vector3 position, Asteroid parentAsteroid)
        {
            int commonDifference = (int)DeepMinePlugin.BedrockDepth / (5 - 1);

            if (position.y > 40)
            {
                __instance.MinDropQuantity = DeepMinePlugin.OreMinDropQuantity * (int)Math.Pow(DeepMinePlugin.LevelMultiplier, 2 - 1);
                __instance.MaxDropQuantity = DeepMinePlugin.OreMaxDropQuantity * (int)Math.Pow(DeepMinePlugin.LevelMultiplier, 2 - 1);
            }
            else if (position.y > -10)
            {
                __instance.MinDropQuantity = DeepMinePlugin.OreMinDropQuantity;
                __instance.MaxDropQuantity = DeepMinePlugin.OreMaxDropQuantity;
            }
            else if(position.y > (1 * commonDifference))
            {
                __instance.MinDropQuantity = DeepMinePlugin.OreMinDropQuantity * (int)Math.Pow(DeepMinePlugin.LevelMultiplier, 2 - 1);
                __instance.MaxDropQuantity = DeepMinePlugin.OreMaxDropQuantity * (int)Math.Pow(DeepMinePlugin.LevelMultiplier, 2 - 1);
            }
            else if (position.y > (2 * commonDifference))
            {
                __instance.MinDropQuantity = DeepMinePlugin.OreMinDropQuantity * (int)Math.Pow(DeepMinePlugin.LevelMultiplier, 3 - 1);
                __instance.MaxDropQuantity = DeepMinePlugin.OreMaxDropQuantity * (int)Math.Pow(DeepMinePlugin.LevelMultiplier, 3 - 1);
            }
            else if (position.y > (3 * commonDifference))
            {
                __instance.MinDropQuantity = DeepMinePlugin.OreMinDropQuantity * (int)Math.Pow(DeepMinePlugin.LevelMultiplier, 4 - 1);
                __instance.MaxDropQuantity = DeepMinePlugin.OreMaxDropQuantity * (int)Math.Pow(DeepMinePlugin.LevelMultiplier, 4 - 1);
            }
            else
            {
                __instance.MinDropQuantity = DeepMinePlugin.OreMinDropQuantity * (int)Math.Pow(DeepMinePlugin.LevelMultiplier, 5 - 1);
                __instance.MaxDropQuantity = DeepMinePlugin.OreMaxDropQuantity * (int)Math.Pow(DeepMinePlugin.LevelMultiplier, 5 - 1);
            }

            if (DeepMinePlugin.DebugMode)
            {
                DeepMinePlugin.ModLog("yLevel=" + position.y);
                DeepMinePlugin.ModLog("MinDQ=" + __instance.MinDropQuantity);
                DeepMinePlugin.ModLog("MaxDQ=" + __instance.MaxDropQuantity);
            }
        }
    }

    /// <summary>
    /// Alter ore minin drill speed based on altitude
    /// </summary>
    [HarmonyPatch(typeof(MiningDrill), "OnUsePrimary")]
    public class MiningDrill_OnUsePrimary
    {
        static void Postfix(MiningDrill __instance, Vector3 targetLocation)
        {
            int commonDifference = (int)DeepMinePlugin.BedrockDepth / (5 - 1);

            if (targetLocation.y > 40)
            {
                __instance.MineCompletionTime = 0.15f;
            }
            else if (targetLocation.y > -10)
            {
                __instance.MineCompletionTime = 0.12f;
            }
            else if (targetLocation.y > (1 * commonDifference))
            {
                __instance.MineCompletionTime = 0.18f;
            }
            else if (targetLocation.y > (2 * commonDifference))
            {
                __instance.MineCompletionTime = 0.22f;
            }
            else if (targetLocation.y > (3 * commonDifference))
            {
                __instance.MineCompletionTime = 0.28f;
            }
            else
            {
                __instance.MineCompletionTime = 0.35f;
            }

            if (DeepMinePlugin.DebugMode)
            {
                DeepMinePlugin.ModLog("Y=" + targetLocation.y);
                DeepMinePlugin.ModLog("MCT=" + __instance.MineCompletionTime);
            }
        }
    }


    /// <summary>
    /// Placeholder for larger mining tools
    /// </summary>
    [HarmonyPatch(typeof(CursorVoxel), MethodType.Constructor)]
    public class CursorVoxel_Constructor
    {
        static void Postfix(CursorVoxel __instance)
        {
            Type typeBoxCollider = __instance.GameObject.GetComponent("BoxCollider").GetType();
            PropertyInfo prop = typeBoxCollider.GetProperty("size");
        }
    }

    /// <summary>
    /// Prevents lava bedrock texture from spawning, potentially has insidious effect but unclear from initial tests
    /// </summary>
    [HarmonyPatch(typeof(TerrainGeneration), "SetUpChunk", new Type[] { typeof(ChunkObject) })]
    public class TerrainGeneration_SetUpChunk
    {
        static void Postfix(TerrainGeneration __instance, ref ChunkObject chunk)
        {
            chunk.MeshRenderer.sharedMaterial.SetVector("_WorldOrigin", WorldManager.OriginPositionLoading + new Vector3(0, 150, 0));
        }
    }

    /// <summary>
    /// Increase the bedrock level
    /// </summary>
    [HarmonyPatch(typeof(TerrainGeneration), "BuildAsteroidsStream", new Type[] { typeof(Vector3), typeof(int), typeof(int), typeof(bool) })]
    public class WorldManager_SetWorldEnvironments
    {
        static FieldInfo SizeOfWorld = AccessTools.Field(typeof(WorldManager), "SizeOfWorld");
        static FieldInfo HalfSizeOfWorld = AccessTools.Field(typeof(WorldManager), "HalfSizeOfWorld");
        static FieldInfo BedrockLevel = AccessTools.Field(typeof(WorldManager), "BedrockLevel");

        static void Prefix(TerrainGeneration __instance)
        {
            WorldManager.BedrockLevel = DeepMinePlugin.BedrockDepth;
            WorldManager.LavaLevel = DeepMinePlugin.BedrockDepth;
        }
    }

    /// <summary>
    /// Enforcing new bedrock level during the world creation process
    /// </summary>
    [HarmonyPatch(typeof(Asteroid), "GenerateChunk", new Type[] { typeof(IReadOnlyCollection<Vector4>), typeof(uint), typeof(bool) })]
    public class Asteroid_GenerateChunk
    {
        static void Prefix(Asteroid __instance)
        {
            WorldManager.BedrockLevel = DeepMinePlugin.BedrockDepth;
        }
    }

    /// <summary>
    /// Alter GPR Range
    /// </summary>
    [HarmonyPatch(typeof(PortableGPR), "Awake")]
    public class PortableGPR_Awake
    {
        static void Prefix(PortableGPR __instance)
        {
            __instance.Resolution = (int)Math.Floor(Math.Abs(DeepMinePlugin.BedrockDepth) * 0.4);
        }
    }

    /// <summary>
    /// Increasing drill radius
    /// </summary>
    [HarmonyPatch(typeof(MiningDrill), "Awake")]
    public class MiningDrill_Awake
    {
        static void Prefix(MiningDrill __instance)
        {
            //__instance.MineCompletionTime = DeepMinePlugin.MineCompletionTime;
            __instance.MineAmount = DeepMinePlugin.MineAmount;
        }
    }

    /// <summary>
    /// Alter ore max stack size based on layer multiplier
    /// </summary>
    [HarmonyPatch(typeof(Prefab), "LoadCorePrefabs")]
    public class Prefab_LoadCorePrefabs
    {
        static void Postfix()
        {
            double roundedNumber = Math.Ceiling(((DeepMinePlugin.OreMaxDropQuantity * (double)Math.Pow(DeepMinePlugin.LevelMultiplier, 5 - 1)) * 2) / 10) * 10;
            if (roundedNumber < 50) { roundedNumber = 50; }

            foreach (var orePrefab in Ore.AllOrePrefabs)
            {              
                orePrefab.MaxQuantity = (int)roundedNumber;
            }
        }

    }
}
