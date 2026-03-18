using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class LeaderboardService
{
    private const string FILE_NAME = "leaderboard.json";
    private static LeaderboardData _data;
    private static bool _loaded;

    private static string FilePath
        => Path.Combine(Application.persistentDataPath, FILE_NAME);

    public static void EnsureLoaded()
    {
        if (_loaded) return;
        _loaded = true;
        _data = LoadFromDisk() ?? new LeaderboardData();
        if (_data.entries == null) _data.entries = new List<LeaderboardEntry>();
    }

    public static void UpsertBestScore(string playerName, int score)
    {
        EnsureLoaded();

        string name = (playerName ?? "").Trim();
        if (name.Length == 0) name = "Player";

        var existing = _data.entries.FirstOrDefault(e =>
            string.Equals(e.playerName, name, StringComparison.OrdinalIgnoreCase));

        if (existing == null)
        {
            _data.entries.Add(new LeaderboardEntry
            {
                playerName = name,
                bestScore = score,
                updatedAtUnixMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            });
            SaveToDisk();
            return;
        }

        if (score > existing.bestScore)
        {
            existing.bestScore = score;
            existing.updatedAtUnixMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            SaveToDisk();
        }
    }

    public static IReadOnlyList<LeaderboardEntry> GetTop10()
    {
        EnsureLoaded();
        return _data.entries
            .OrderByDescending(e => e.bestScore)
            .ThenBy(e => e.playerName, StringComparer.OrdinalIgnoreCase)
            .Take(10)
            .Select(e => new LeaderboardEntry
            {
                playerName = e.playerName,
                bestScore = e.bestScore,
                updatedAtUnixMs = e.updatedAtUnixMs
            })
            .ToList();
    }

    private static LeaderboardData LoadFromDisk()
    {
        try
        {
            if (!File.Exists(FilePath)) return null;
            string json = File.ReadAllText(FilePath);
            if (string.IsNullOrWhiteSpace(json)) return null;
            return JsonUtility.FromJson<LeaderboardData>(json);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Leaderboard load failed: {e.Message}");
            return new LeaderboardData();
        }
    }

    private static void SaveToDisk()
    {
        try
        {
            Directory.CreateDirectory(Application.persistentDataPath);
            string json = JsonUtility.ToJson(_data, true);
            File.WriteAllText(FilePath, json);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Leaderboard save failed: {e.Message}");
        }
    }
}

