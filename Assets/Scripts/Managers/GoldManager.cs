using UnityEngine;
using UnityEngine.Events;

public class GoldManager : MonoBehaviour
{
    public static GoldManager Instance;

    [Header("Cấu Hình Balance")]
    public int startingGold = 0;
    public int ext1MinCost = 50;   // gia hạn +1 phút
    public int ext3MinCost = 120;  // gia hạn +3 phút
    public int ext5MinCost = 180;  // gia hạn +5 phút

    [Header("Events")]
    public UnityEvent<int> onGoldChanged;

    private int _gold;

    void Awake()
    {
        // Singleton
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        _gold = startingGold;
    }

    public void AddGold(int amount)
    {
        _gold += amount;
        onGoldChanged?.Invoke(_gold);
    }

    public bool SpendGold(int amount)
    {
        if (_gold < amount) return false;
        _gold -= amount;
        onGoldChanged?.Invoke(_gold);
        return true;
    }

    public void ResetGold()
    {
        _gold = 0;
        onGoldChanged?.Invoke(_gold);
    }

    public int GetGold() => _gold;
}