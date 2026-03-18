using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance;

    [Header("HUD Elements")]
    public TMP_Text goldText;
    public TMP_Text timerText;
    public TMP_Text extensionText;

    [Header("Result Screen")]
    public GameObject resultPanel;
    public TMP_Text finalGoldText;
    public TMP_Text highScoreText;
    public TMP_Text rankText;
    public TMP_Text deltaText;
    public TMP_Text totalRunsText;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        // Ẩn result panel lúc đầu
        resultPanel.SetActive(false);
        // Thu nhỏ panel kết quả để không che quá nhiều màn hình
        if (resultPanel != null)
            resultPanel.transform.localScale = new Vector3(0.8f, 0.8f, 1f);

        // Lắng nghe events
        GoldManager.Instance.onGoldChanged.AddListener(UpdateGold);
        TimeSystem.Instance.onTimeUpdated.AddListener(UpdateTimer);
        TimeSystem.Instance.onWarning.AddListener(OnWarning);
        //TimeSystem.Instance.onTimeUp.AddListener(OnTimeUp);

        // Hiện giá trị ban đầu
        UpdateGold(GoldManager.Instance.GetGold());
    }

    // ── GOLD ──────────────────────────────
    void UpdateGold(int gold)
    {
        if (goldText)
            goldText.text = $"🪙 {gold}";
    }

    // ── TIMER ─────────────────────────────
    void UpdateTimer(float timeLeft)
    {
        if (!timerText) return;

        int min = Mathf.FloorToInt(timeLeft / 60f);
        int sec = Mathf.FloorToInt(timeLeft % 60f);
        timerText.text = $"{min:00}:{sec:00}";

        // Đỏ khi còn < 30 giây
        timerText.color = timeLeft < 30f ? Color.red : Color.white;

        // Cập nhật số lần gia hạn còn lại
        if (extensionText)
            extensionText.text =
                $"Gia hạn: {TimeSystem.Instance.GetExtensionsLeft()} lần";
    }

    void OnWarning()
    {
        Debug.Log("⚠️ Còn dưới 30 giây!");
    }

    // ── RESULT SCREEN ─────────────────────
    void OnTimeUp()
    {
        // Điểm và số lần chơi đã được lưu trong GameManager.OnTimeUp()
        int finalGold = GoldManager.Instance.GetGold();
        int totalRuns = ScoreSystem.Instance.GetTotalRuns();
        int newBest = ScoreSystem.Instance.GetHighScore();
        int delta = finalGold - newBest;

        // Hiện result panel
        resultPanel.SetActive(true);

        if (finalGoldText)
            finalGoldText.text = $"Vang kiem duoc: {finalGold}";

        if (highScoreText)
            highScoreText.text = $"Ky luc: {newBest}";

        if (rankText)
        {
            string rank = ScoreSystem.Instance.GetRank(finalGold);
            string rankIcon = ScoreSystem.Instance.GetRankIcon(finalGold);
            rankText.text = $"{rankIcon}\n{rank}";
        }

        if (deltaText)
        {
            if (delta > 0)
            {
                deltaText.text = $"+{delta} Ky luc moi!";
                deltaText.color = Color.yellow;
            }
            else if (delta == 0)
            {
                deltaText.text = "Bang ky luc!";
                deltaText.color = Color.white;
            }
            else
            {
                deltaText.text = $"{delta} so voi ky luc";
                deltaText.color = Color.gray;
            }

        }
        if (totalRunsText)
            totalRunsText.text = $"Lan choi thu {totalRuns}";

        Debug.Log($"Lan choi thu {totalRuns} | Vang: {finalGold} | Ky luc: {newBest}");

        // Auto hiển thị leaderboard top 10
        LeaderboardUI.ShowTop10();
    }

    public void ShowResultScreen(int finalGold) => OnTimeUp();

    void OnEnable()
    {
        // Khi game over, bảng xếp hạng sẽ tự hiện (Top 10)
        // (đảm bảo service đã lưu điểm ở GameManager.OnTimeUp)
    }

    public void ShowLeaderboardTop10()
    {
        LeaderboardUI.ShowTop10();
    }
}