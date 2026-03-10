using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnlockSlotButton : MonoBehaviour
{
    [Header("UI")]
    public Button button;
    public TMP_Text labelText;
    public TMP_Text costText;

    private bool _unlockMode = false;

    void Start()
    {
        button.onClick.AddListener(ToggleUnlockMode);
        GoldManager.Instance.onGoldChanged.AddListener(_ => RefreshUI());
        RefreshUI();
    }

    void ToggleUnlockMode()
    {
        _unlockMode = !_unlockMode;

        if (_unlockMode)
        {
            // Tắt các mode khác
            BuildingPlacer.Instance.CancelPlacement();
            BuildingPlacer.Instance.SetDeleteMode(false);
        }

        BuildingPlacer.Instance.SetUnlockMode(_unlockMode);
        RefreshUI();
    }

    void RefreshUI()
    {
        int cost = GridManager.Instance.unlockCostPerSlot;
        int currentGold = GoldManager.Instance.GetGold();

        if (costText)
        {
            costText.text = $"{cost} vang";
            costText.color = currentGold >= cost ? Color.yellow : Color.red;
        }

        // Đổi màu nút khi bật/tắt mode
        var img = button.GetComponent<Image>();
        if (img != null)
            img.color = _unlockMode
                ? new Color(0.9f, 0.7f, 0.1f)  // vàng = đang bật
                : new Color(0.3f, 0.3f, 0.3f);  // xám = tắt
    }

    public void TurnOff()
    {
        _unlockMode = false;
        RefreshUI();
    }
}