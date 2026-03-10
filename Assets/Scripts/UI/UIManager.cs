using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("HUD")]
    public TMP_Text goldText;
    public TMP_Text timerText;
    public TMP_Text extensionsLeftText;

    [Header("Result Screen")]
    public GameObject resultPanel;
    public TMP_Text finalGoldText;
    public TMP_Text highScoreText;
    public TMP_Text rankText;
    public TMP_Text deltaText;

    [Header("Time Extension UI")]
    public Button ext1Button;   // +1 phút
    public Button ext3Button;   // +3 phút
    public Button ext5Button;   // +5 phút

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        resultPanel.SetActive(false);
    }

    void Start()
    {
        // Gắn events
        GoldManager.Instance.onGoldChanged.AddListener(UpdateGoldUI);
        TimeSystem.Instance.onTimeUpdated.AddListener(UpdateTimerUI);
        TimeSystem.Instance.onWarning.AddListener(OnWarning);

        // Gắn nút gia hạn
        ext1Button.onClick.AddListener(() => TryExtend(1));
        ext3Button.onClick.AddListener(() => TryExtend(3));
        ext5Button.onClick.AddListener(() => TryExtend(5));

        UpdateGoldUI(GoldManager.Instance.GetGold());
    }

    void UpdateGoldUI(int gold)
    {
        goldText.text = $"🪙 {gold}";
    }

    void UpdateTimerUI(float timeLeft)
    {
        int min = Mathf.FloorToInt(timeLeft / 60f);
        int sec = Mathf.FloorToInt(timeLeft % 60f);
        timerText.text = $"{min:00}:{sec:00}";

        // Đổi màu đỏ khi còn < 30 giây
        timerText.color = timeLeft < 30f ? Color.red : Color.white;

        // Cập nhật số lần gia hạn còn lại
        extensionsLeftText.text = $"Gia hạn: {TimeSystem.Instance.GetExtensionsLeft()} lần";
    }

    void OnWarning()
    {
        Debug.Log("⚠️ Còn dưới 30 giây!");
        // TODO: thêm hiệu ứng rung/nhấp nháy
    }

    void TryExtend(int minutes)
    {
        int result = TimeSystem.Instance.BuyExtension(minutes);
        switch (result)
        {
            case 0:
                Debug.Log($"✅ Đã gia hạn +{minutes} phút!");
                break;
            case -1:
                Debug.Log("❌ Hết lượt gia hạn!");
                break;
            case -2:
                Debug.Log("❌ Không đủ vàng!");
                break;
        }
    }

    public void ShowResultScreen(int finalGold)
    {
        resultPanel.SetActive(true);
        int best = ScoreSystem.Instance.GetHighScore();
        int delta = finalGold - best;

        finalGoldText.text = $"Vàng kiếm được: {finalGold}";
        highScoreText.text = $"Kỷ lục: {best}";
        rankText.text = ScoreSystem.Instance.GetRank(finalGold);
        deltaText.text = delta > 0 ? $"+{delta} 🎉 Kỷ lục mới!"
                            : delta == 0 ? "Bằng kỷ lục!"
                            : $"{delta} so với kỷ lục";
        deltaText.color = delta > 0 ? Color.yellow : Color.white;
    }
}