using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance;

    [Header("Events")]
    public UnityEvent onInventoryChanged;

    // key = ItemData, value = số lượng
    private Dictionary<ItemData, int> _inventory = new();

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    // ─────────────────────────────────────────
    // THÊM / XÓA item
    // ─────────────────────────────────────────

    public void AddItem(ItemData item, int amount = 1)
    {
        if (!_inventory.ContainsKey(item)) _inventory[item] = 0;
        _inventory[item] += amount;
        onInventoryChanged?.Invoke();
    }

    public bool RemoveItem(ItemData item, int amount = 1)
    {
        if (GetCount(item) < amount) return false;
        _inventory[item] -= amount;
        onInventoryChanged?.Invoke();
        return true;
    }

    // ─────────────────────────────────────────
    // KIỂM TRA & TIÊU THỤ NGUYÊN LIỆU (dùng ItemIngredient[])
    // ─────────────────────────────────────────

    /// <summary>Kiểm tra có đủ nguyên liệu theo công thức không</summary>
    public bool HasIngredients(ItemIngredient[] ingredients)
    {
        if (ingredients == null || ingredients.Length == 0) return true;
        foreach (var ing in ingredients)
            if (GetCount(ing.item) < ing.amount) return false;
        return true;
    }

    /// <summary>Tiêu thụ nguyên liệu — trả về false nếu không đủ</summary>
    public bool ConsumeIngredients(ItemIngredient[] ingredients)
    {
        if (!HasIngredients(ingredients)) return false;
        foreach (var ing in ingredients)
            RemoveItem(ing.item, ing.amount);
        return true;
    }

    // ─────────────────────────────────────────
    // API CŨ — GIỮ LẠI để không lỗi code khác
    // ─────────────────────────────────────────

    /// @deprecated Dùng HasIngredients(ItemIngredient[]) thay thế
    public bool HasItems(ItemData[] items, int amountEach)
    {
        foreach (var item in items)
            if (GetCount(item) < amountEach) return false;
        return true;
    }

    /// @deprecated Dùng ConsumeIngredients(ItemIngredient[]) thay thế
    public bool ConsumeInputs(ItemData[] items, int amountEach)
    {
        if (!HasItems(items, amountEach)) return false;
        foreach (var item in items) RemoveItem(item, amountEach);
        return true;
    }

    // ─────────────────────────────────────────
    // TIỆN ÍCH
    // ─────────────────────────────────────────

    public int GetCount(ItemData item)
    {
        return _inventory.TryGetValue(item, out int count) ? count : 0;
    }

    /// <summary>Lấy toàn bộ inventory — dùng cho UI hiển thị</summary>
    public Dictionary<ItemData, int> GetAll() => _inventory;

    /// <summary>Xóa sạch inventory — gọi khi restart game</summary>
    public void Clear()
    {
        _inventory.Clear();
        onInventoryChanged?.Invoke();
    }
}