using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;

namespace EthicalCompany;

[BepInPlugin(ModGUID, ModName, ModVersion)]
[BepInProcess("Lethal Company.exe")]
public class Plugin : BaseUnityPlugin {
    public const string ModGUID = "TofuTofuMe.EthicalCompany";
    public const string ModName = "EthicalCompany";
    public const string ModVersion = "1.0.0";
    private readonly Harmony harmony = new Harmony(ModGUID);
    public static ManualLogSource mls;
    public static ConfigEntry<bool> isCompanyEthical;
    public static ConfigEntry<bool> supplyOnCompany;
    public static ConfigEntry<int> freeWeaponChance;
    public static ConfigEntry<int> freeCredits;
    public static ConfigEntry<string> supplyEquipment;
    public static ConfigEntry<string> supplyWeapons;
    public static bool isPlayerHost;
    public static bool freeSupplyClaimed;

    private void Awake() {
        mls = BepInEx.Logging.Logger.CreateLogSource(ModGUID);
        mls.LogInfo($"Loading {ModGUID}...");

        SetConfig();

        harmony.PatchAll(typeof(Plugin));

        mls.LogInfo($"{ModGUID} loaded.");
        mls.LogInfo("The company just became ethical!");
    }

    private void SetConfig() {
        isCompanyEthical = Config.Bind("Settings", "isCompanyEthical", true, "Is the company ethical? This toggles the mod.");
        supplyOnCompany = Config.Bind("Settings", "supplyOnCompany", true, "Should free goods only be given on the Company moon.");
        freeWeaponChance = Config.Bind("Settings", "freeWeaponChance", 10, "Percent chance for the Company to give out arms. Set to 0 to disable.");
        freeCredits = Config.Bind("Settings", "freeCreditsAmount", 60, "Free credits given when arriving at the Company. Set to 0 to disable.");
        supplyEquipment = Config.Bind("Supplies", "supplyEquipment", "0, 1", "IDs of equipment supplied by the Company.");
        supplyWeapons = Config.Bind("Supplies", "supplyWeapons", "2", "IDs of weapons supplied by the Company.");
    }

    [HarmonyPatch(typeof(RoundManager), "Start")]
    [HarmonyPostfix]
    static void CheckIfHost() {
        isPlayerHost = RoundManager.Instance.NetworkManager.IsHost;
        mls.LogInfo($"Is the player host? {(isPlayerHost ? "Yes" : "No")}");
    }

    [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.openingDoorsSequence))]
    [HarmonyPostfix]
    static void SupplyGoods() {
        if (isPlayerHost && isCompanyEthical.Value) {
            Terminal terminal = FindObjectOfType<Terminal>();
            ItemDropship dropship = FindObjectOfType<ItemDropship>();
            List<int> supplyIDList = new List<int>();
            List<string> supplyList = new List<string>();

            mls.LogInfo("Ship is landing!");
            if (supplyOnCompany.Value && StartOfRound.Instance.currentLevelID == 3 && freeSupplyClaimed == false) {
                supplyList.AddRange(supplyEquipment.Value.Split(','));
                if (freeWeaponChance.Value > 0 && Random.Range(0, 100) < freeWeaponChance.Value) {
                    supplyList.AddRange(supplyWeapons.Value.Split(','));
                    HUDManager.Instance.AddTextToChatOnServer("<color=white>The Company thanks your service with lights and firepower.</color>");
                } else {
                    HUDManager.Instance.AddTextToChatOnServer("<color=white>The Company thanks your service with equipment.</color>");
                }
                for (int i = 0; i < StartOfRound.Instance.livingPlayers; i++) {
                    foreach (string supply in supplyList) {
                        supplyIDList.Add(int.Parse(supply.Trim()));
                    }
                    terminal.BuyItemsServerRpc(supplyIDList.ToArray(), terminal.groupCredits, 0);
                }
                if (freeCredits.Value > 0) {
                    terminal.groupCredits += freeCredits.Value;
                    terminal.SyncGroupCreditsServerRpc(terminal.groupCredits, terminal.numberOfItemsInDropship);
                    HUDManager.Instance.AddTextToChatOnServer($"<color=white>The Company deposited {freeCredits.Value} to your bank account.</color>");
                }
                Traverse.Create(dropship).Method("LandShipOnServer").GetValue();
                freeSupplyClaimed = true;

            } else if (supplyOnCompany.Value && StartOfRound.Instance.currentLevelID != 3) {
                mls.LogInfo("No free supplies on moons.");
                freeSupplyClaimed = false;
            } else if (!supplyOnCompany.Value) {
                supplyList.AddRange(supplyEquipment.Value.Split(','));
                if (freeWeaponChance.Value > 0 && Random.Range(0, 100) < freeWeaponChance.Value) {
                    supplyList.AddRange(supplyWeapons.Value.Split(','));
                    HUDManager.Instance.AddTextToChatOnServer("<color=white>The Company thanks your service with lights and firepower.</color>");
                } else {
                    HUDManager.Instance.AddTextToChatOnServer("<color=white>The Company thanks your service with equipment.</color>");
                }
                for (int i = 0; i < StartOfRound.Instance.livingPlayers; i++) {
                    foreach (string supply in supplyList) {
                        supplyIDList.Add(int.Parse(supply.Trim()));
                    }
                    terminal.BuyItemsServerRpc(supplyIDList.ToArray(), terminal.groupCredits, 0);
                }
                if (freeCredits.Value > 0) {
                    terminal.groupCredits += freeCredits.Value;
                    terminal.SyncGroupCreditsServerRpc(terminal.groupCredits, terminal.numberOfItemsInDropship);
                    HUDManager.Instance.AddTextToChatOnServer($"<color=white>The Company deposited {freeCredits.Value} to your bank account.</color>");
                }
                Traverse.Create(dropship).Method("LandShipOnServer").GetValue();
                freeSupplyClaimed = true;
            }
        }
    }
}
