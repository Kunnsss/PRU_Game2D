using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MarketUI : MonoBehaviour
{
    [System.Serializable]
    public class ItemButton
    {
        public ItemData data;
        public Button button;
        public TMP_Text nameText;
        public TMP_Text countText;
        public TMP_Text priceText;
    }

    [Header("Danh sách item có thể bán")]
    public ItemButton[] items;

    void Start()
    {
        // Gắn sự kiện click bán
        foreach (var item in items)
        {
            var captured = item;
            item.button.onClick.AddListener(() => SellItem(captured));

            // Điền tên và giá
            if (item.nameText) item.nameText.text = item.data.itemName;
            if (item.priceText) item.priceText.text = $"+{item.data.sellPrice}🪙";
        }

        // Cập nhật UI khi inventory thay đổi
        InventorySystem.Instance.onInventoryChanged.AddListener(RefreshUI);
        RefreshUI();
    }

    void SellItem(ItemButton item)
    {
        int count = InventorySystem.Instance.GetCount(item.data);
        if (count <= 0)
        {
            Debug.Log($"Không có {item.data.itemName} để bán!");
            return;
        }

        // Bán 1 item → cộng vàng
        InventorySystem.Instance.RemoveItem(item.data, 1);
        GoldManager.Instance.AddGold(item.data.sellPrice);

        Debug.Log($"💰 Bán {item.data.itemName} → +{item.data.sellPrice} vàng");
    }

    void RefreshUI()
    {
        foreach (var item in items)
        {
            int count = InventorySystem.Instance.GetCount(item.data);

            // Cập nhật số lượng
            if (item.countText)
                item.countText.text = $"x{count}";

            // Disable nút nếu không có hàng
            item.button.interactable = count > 0 && item.data.sellPrice > 0;

            // Đổi màu số lượng
            if (item.countText)
                item.countText.color = count > 0 ? Color.white : Color.gray;
        }
    }
}