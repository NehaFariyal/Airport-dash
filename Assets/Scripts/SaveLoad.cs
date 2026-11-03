using UnityEngine;

/// <summary>
/// Handles persistent storage of coins, gems, and distance run using Unity PlayerPrefs.
/// Keys specified in Capstone specification: COINSAVE, GEMSAVE, DISTANCESAVE.
/// Author: Sk. Neha Fariyal - 2303A51598 (Airport Dash)
/// </summary>
public static class SaveLoad
{
    public const string COIN_KEY = "COINSAVE";
    public const string GEM_KEY = "GEMSAVE";
    public const string DISTANCE_KEY = "DISTANCESAVE";

    public static int GetSavedCoins()
    {
        return PlayerPrefs.GetInt(COIN_KEY, 0);
    }

    public static int GetSavedGems()
    {
        return PlayerPrefs.GetInt(GEM_KEY, 0);
    }

    public static int GetSavedDistance()
    {
        return PlayerPrefs.GetInt(DISTANCE_KEY, 0);
    }

    public static void SaveRunData(int coinsEarned, int gemsEarned, int distanceReached)
    {
        int totalCoins = GetSavedCoins() + coinsEarned;
        int totalGems = GetSavedGems() + gemsEarned;
        int bestDistance = Mathf.Max(GetSavedDistance(), distanceReached);

        PlayerPrefs.SetInt(COIN_KEY, totalCoins);
        PlayerPrefs.SetInt(GEM_KEY, totalGems);
        PlayerPrefs.SetInt(DISTANCE_KEY, bestDistance);
        PlayerPrefs.Save();

        Debug.Log($"[SaveLoad] Progress Saved! Total Coins: {totalCoins}, Total Gems: {totalGems}, Best Distance: {bestDistance}m");
    }

    public static void ResetData()
    {
        PlayerPrefs.DeleteKey(COIN_KEY);
        PlayerPrefs.DeleteKey(GEM_KEY);
        PlayerPrefs.DeleteKey(DISTANCE_KEY);
        PlayerPrefs.Save();
        Debug.Log("[SaveLoad] Saved progress reset.");
    }
}

