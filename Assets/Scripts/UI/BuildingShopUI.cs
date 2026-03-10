using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingShopUI : MonoBehaviour
{
    [System.Serializable]
    public class BuildingButton
    {
        public BuildingData data;
        public Button button;
        public TMP_Text nameText;
        public TMP_Text costText;
        public GameObject lockIcon;   // icon ổ khóa (tùy chọn)
    }

    [Header("Danh sách building")]
    public BuildingButton[] buildings;

    void Start()
    {
        foreach (var b in buildings)
        {
            if (b.nameText) b.nameText.text = b.data.buildingName;

            var captured = b;
            b.button.onClick.AddListener(() => OnClickBuilding(captured));
        }

        GoldManager.Instance.onGoldChanged
            .AddListener(_ => RefreshButtons());
        BuildingUnlockSystem.Instance.onUnlockChanged
            .AddListener(RefreshButtons);

        // Delay 1 frame để đảm bảo tất cả Awake/Start đã chạy xong
        Invoke(nameof(RefreshButtons), 0.1f);
    }

    void OnClickBuilding(BuildingButton b)
    {
        // Đã unlock → chọn để đặt
        if (BuildingUnlockSystem.Instance.IsUnlocked(b.data))
        {
            BuildingPlacer.Instance.SelectBuilding(b.data);
            return;
        }

        // Chưa unlock → thử mua unlock
        int result = BuildingUnlockSystem.Instance.UnlockBuilding(b.data);

        switch (result)
        {
            case 0:
                Debug.Log($"Mua thanh cong {b.data.buildingName}!");
                RefreshButtons();
                // Tự động chọn luôn sau khi mở khóa
                BuildingPlacer.Instance.SelectBuilding(b.data);
                break;
            case -1:
                Debug.Log("Da unlock roi!");
                break;
            case -2:
                Debug.Log($"Can {b.data.unlockCost} vang de mo {b.data.buildingName}!");
                break;
        }
    }

    void RefreshButtons()
    {
        int currentGold = GoldManager.Instance.GetGold();

        foreach (var b in buildings)
        {
            bool isUnlocked = BuildingUnlockSystem.Instance.IsUnlocked(b.data);
            bool canAfford = currentGold >= b.data.unlockCost;

            // Luôn cho bấm để hiện thông báo
            b.button.interactable = true;

            // Đổi màu nút
            var img = b.button.GetComponent<Image>();
            if (img != null)
            {
                if (isUnlocked)
                    img.color = new Color(0.2f, 0.6f, 0.2f);  // xanh = đã mở
                else if (canAfford)
                    img.color = new Color(0.8f, 0.6f, 0.1f);  // vàng = đủ tiền mua
                else
                    img.color = new Color(0.4f, 0.4f, 0.4f);  // xám = chưa đủ tiền
            }

            // Hiện giá hoặc trạng thái
            if (b.costText)
            {
                if (isUnlocked)
                {
                    b.costText.text = "San sang";
                    b.costText.color = Color.white;
                }
                else
                {
                    b.costText.text = $"Mo: {b.data.unlockCost} vang";
                    b.costText.color = canAfford ? Color.yellow : Color.red;
                }
            }

            // Ẩn/hiện lock icon nếu có
            if (b.lockIcon)
                b.lockIcon.SetActive(!isUnlocked);
        }
    }
}