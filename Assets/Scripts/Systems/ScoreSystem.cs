using UnityEngine;
using UnityEngine.Events;

public class ScoreSystem : MonoBehaviour
{
    public static ScoreSystem Instance;

    // Keys cho PlayerPrefs
    private const string KEY_HIGH_SCORE = "NTV_HighScore";
    private const string KEY_TOTAL_RUNS = "NTV_TotalRuns";
    private const string KEY_LAST_SCORE = "NTV_LastScore";

    [Header("Events")]
    public UnityEvent<int> onHighScoreBeaten;  // khi phá kỷ lục

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    // ── LƯU ĐIỂM ──────────────────────────────
    public void SaveScore(int gold)
    {
        // Lưu điểm lần này
        PlayerPrefs.SetInt(KEY_LAST_SCORE, gold);

        // Tăng số lần chơi
        int runs = GetTotalRuns() + 1;
        PlayerPrefs.SetInt(KEY_TOTAL_RUNS, runs);

        // Cập nhật high score nếu phá kỷ lục
        if (gold > GetHighScore())
        {
            PlayerPrefs.SetInt(KEY_HIGH_SCORE, gold);
            onHighScoreBeaten?.Invoke(gold);
            Debug.Log($"Ky luc moi: {gold} vang!");
        }

        PlayerPrefs.Save(); // ghi xuống disk ngay
        Debug.Log($"Da luu diem: {gold} vang | Ky luc: {GetHighScore()}");
    }

    // ── ĐỌC DỮ LIỆU ───────────────────────────
    public int GetHighScore()
        => PlayerPrefs.GetInt(KEY_HIGH_SCORE, 0);

    public int GetLastScore()
        => PlayerPrefs.GetInt(KEY_LAST_SCORE, 0);

    public int GetTotalRuns()
        => PlayerPrefs.GetInt(KEY_TOTAL_RUNS, 0);

    // ── XÓA DỮ LIỆU ───────────────────────────
    public void ResetAllData()
    {
        PlayerPrefs.DeleteKey(KEY_HIGH_SCORE);
        PlayerPrefs.DeleteKey(KEY_LAST_SCORE);
        PlayerPrefs.DeleteKey(KEY_TOTAL_RUNS);
        PlayerPrefs.Save();
        Debug.Log("Da xoa toan bo du lieu!");
    }

    // ── XẾP HẠNG ──────────────────────────────
    public string GetRank(int gold)
    {
        if (gold >= 500) return "Huyen Thoai";
        if (gold >= 300) return "Xuat Sac";
        if (gold >= 150) return "Gioi";
        return "Tap Su";
    }

    public string GetRankIcon(int gold)
    {
        if (gold >= 500) return "★★★★★";
        if (gold >= 300) return "★★★★☆";
        if (gold >= 150) return "★★★☆☆";
        return "★☆☆☆☆";
    }
}