using UnityEngine;

public static class PlayerProfile
{
    private const string KEY_PLAYER_NAME = "PRU_PlayerName";

    public static bool HasPlayerName()
        => !string.IsNullOrWhiteSpace(PlayerPrefs.GetString(KEY_PLAYER_NAME, ""));

    public static string GetPlayerName()
        => PlayerPrefs.GetString(KEY_PLAYER_NAME, "Player");

    public static bool TrySetPlayerName(string rawName)
    {
        if (rawName == null) return false;
        string name = rawName.Trim();
        if (name.Length == 0) return false;
        if (name.Length > 20) name = name.Substring(0, 20);

        PlayerPrefs.SetString(KEY_PLAYER_NAME, name);
        PlayerPrefs.Save();
        return true;
    }
}

