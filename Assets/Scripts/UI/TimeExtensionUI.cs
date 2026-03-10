using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimeExtensionUI : MonoBehaviour
{
    [System.Serializable]
    public class ExtensionButton
    {
        public int minutes;
        public Button button;
        public TMP_Text labelText;
        public TMP_Text costText;
    }

    [Header("3 nút gia hạn")]
    public ExtensionButton[] buttons;

    [Header("Thông báo kết quả")]
    public TMP_Text feedbackText;

    void Start()
    {
        foreach (var b in buttons)
        {
            if (b.labelText)
                b.labelText.text = $"+{b.minutes} phút";
            // Sửa
            if (b.costText)
                b.costText.text = $"{TimeSystem.Instance.GetExtensionCost(b.minutes)} vang";

            var captured = b;
            b.button.onClick.AddListener(() => OnClickExtend(captured));
        }

        GoldManager.Instance.onGoldChanged.AddListener(_ => RefreshButtons());
        TimeSystem.Instance.onExtensionUsed.AddListener(_ => RefreshButtons());

        if (feedbackText)
        {
            feedbackText.text = "";
            feedbackText.gameObject.SetActive(true);
        }

        RefreshButtons();
    }

    void OnClickExtend(ExtensionButton b)
    {
        // Kiểm tra thủ công TRƯỚC khi gọi BuyExtension
        // để hiện đúng thông báo lỗi
        if (TimeSystem.Instance.GetExtensionsLeft() <= 0)
        {
            ShowFeedback(" Hết lượt gia hạn!", Color.red);
            return;
        }

        int cost = TimeSystem.Instance.GetExtensionCost(b.minutes);
        if (GoldManager.Instance.GetGold() < cost)
        {
            ShowFeedback($" Cần {cost}🪙", Color.red);
            return;
        }

        // Đủ điều kiện → mua
        int result = TimeSystem.Instance.BuyExtension(b.minutes);
        if (result == 0)
            ShowFeedback($" +{b.minutes} phút!", Color.green);

        RefreshButtons();
    }

    void RefreshButtons()
    {
        int extensionsLeft = TimeSystem.Instance.GetExtensionsLeft();
        int currentGold = GoldManager.Instance.GetGold();

        foreach (var b in buttons)
        {
            int cost = TimeSystem.Instance.GetExtensionCost(b.minutes);
            bool canBuy = extensionsLeft > 0 && currentGold >= cost;

            b.button.interactable = canBuy;

            if (b.costText)
                b.costText.color = currentGold >= cost
                    ? new Color(0.98f, 0.79f, 0.14f)
                    : Color.red;
        }
    }

    void ShowFeedback(string message, Color color)
    {
        if (feedbackText == null) return;

        feedbackText.gameObject.SetActive(true);
        feedbackText.text = message;
        feedbackText.color = color;
        feedbackText.fontSize = 20;

        CancelInvoke(nameof(ClearFeedback));
        Invoke(nameof(ClearFeedback), 3f);
    }

    void ClearFeedback()
    {
        if (feedbackText != null)
            feedbackText.text = "";
    }
}