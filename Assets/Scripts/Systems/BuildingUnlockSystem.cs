using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BuildingUnlockSystem : MonoBehaviour
{
    public static BuildingUnlockSystem Instance;

    [Header("Events")]
    public UnityEvent onUnlockChanged;  // gọi khi có building được mở khóa

    // Lưu danh sách building đã unlock
    private HashSet<BuildingData> _unlockedBuildings = new();

    [Header("Building có sẵn từ đầu (unlockCost = 0)")]
    public BuildingData[] defaultBuildings;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        // Unlock tất cả building miễn phí ngay từ đầu
        foreach (var b in defaultBuildings)
            if (b != null) _unlockedBuildings.Add(b);
    }

    /// <summary>
    /// Mua unlock building. Trả về:
    ///  0 = thành công
    /// -1 = đã unlock rồi
    /// -2 = không đủ vàng
    /// </summary>
    public int UnlockBuilding(BuildingData data)
    {
        if (IsUnlocked(data)) return -1;

        if (!GoldManager.Instance.SpendGold(data.unlockCost)) return -2;

        _unlockedBuildings.Add(data);
        onUnlockChanged?.Invoke();

        Debug.Log($"🔓 Mở khóa {data.buildingName}!");
        return 0;
    }

    public bool IsUnlocked(BuildingData data)
        => _unlockedBuildings.Contains(data);

    // Gọi khi restart game
    public void Reset()
    {
        _unlockedBuildings.Clear();
        foreach (var b in defaultBuildings)
            if (b != null) _unlockedBuildings.Add(b);
    }
}