using System;
using System.Collections.Generic;

[Serializable]
public class LeaderboardEntry
{
    public string playerName;
    public int bestScore;
    public long updatedAtUnixMs;
}

[Serializable]
public class LeaderboardData
{
    public List<LeaderboardEntry> entries = new();
}

