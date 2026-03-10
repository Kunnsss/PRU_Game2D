using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    [Header("Kích Thước Grid")]
    public int width = 6;
    public int height = 4;
    public float cellSize = 1f;

    [Header("Mở Khóa Ô Đất")]
    public int unlockCostPerSlot = 40;

    [Header("Visual")]
    public GameObject lockedSlotPrefab;

    private BuildingController[,] _grid;
    private bool[,] _unlockedSlots;
    private GameObject[,] _lockedVisuals;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;

        _grid = new BuildingController[width, height];
        _unlockedSlots = new bool[width, height];

        // Mở sẵn 3 hàng đầu (18 ô), khóa hàng trên cùng (6 ô)
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                _unlockedSlots[x, y] = (y < 3);
    }

    void Start()
    {
        RefreshLockedVisuals();
    }

    // ── GRID ──────────────────────────────────
    public bool IsEmpty(int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height) return false;
        return _grid[x, y] == null;
    }

    public bool PlaceBuilding(BuildingController building, int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height) return false;
        if (!IsEmpty(x, y)) return false;

        if (!IsSlotUnlocked(x, y))
        {
            Debug.Log($"O ({x},{y}) chua mo khoa! Can {unlockCostPerSlot} vang");
            return false;
        }

        _grid[x, y] = building;
        building.transform.position = GridToWorld(x, y);
        return true;
    }

    public bool RemoveBuilding(int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height) return false;
        if (_grid[x, y] == null) return false;

        Destroy(_grid[x, y].gameObject);
        _grid[x, y] = null;
        return true;
    }

    // ── MỞ KHÓA Ô ─────────────────────────────
    public bool IsSlotUnlocked(int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height) return false;
        return _unlockedSlots[x, y];
    }

    public bool UnlockSlot(int x, int y)
    {
        if (IsSlotUnlocked(x, y)) return false;
        if (!GoldManager.Instance.SpendGold(unlockCostPerSlot)) return false;

        _unlockedSlots[x, y] = true;
        Debug.Log($"Mo o dat ({x},{y}) thanh cong!");
        return true;
    }

    // ── VISUAL Ô KHÓA ─────────────────────────
    public void RefreshLockedVisuals()
    {
        // Xóa visuals cũ
        if (_lockedVisuals != null)
            foreach (var v in _lockedVisuals)
                if (v != null) Destroy(v);

        _lockedVisuals = new GameObject[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (!_unlockedSlots[x, y])
                {
                    // Dùng SpriteRenderer thay vì Quad
                    var obj = new GameObject($"LockedSlot_{x}_{y}");
                    obj.transform.position = GridToWorld(x, y);
                    obj.transform.localScale = new Vector3(0.9f, 0.9f, 1f);

                    var sr = obj.AddComponent<SpriteRenderer>();
                    sr.sprite = GetWhiteSprite();
                    sr.color = new Color(0.1f, 0.1f, 0.1f, 0.85f); // tối
                    sr.sortingOrder = 1; // trên GridBackground nhưng dưới building

                    // Thêm text hiện giá
                    var child = new GameObject("PriceLabel");
                    child.transform.SetParent(obj.transform);
                    child.transform.localPosition = Vector3.zero;
                    child.transform.localScale = new Vector3(0.4f, 0.4f, 1f);

                    _lockedVisuals[x, y] = obj;
                }
            }
        }
    }

    // Tạo sprite trắng 1x1 pixel
    Sprite GetWhiteSprite()
    {
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        return Sprite.Create(tex,
            new Rect(0, 0, 1, 1),
            new Vector2(0.5f, 0.5f), 1f);
    }

    // ── TIỆN ÍCH ──────────────────────────────
    public Vector3 GridToWorld(int x, int y)
        => new Vector3(x * cellSize + cellSize / 2f,
                       y * cellSize + cellSize / 2f, 0f);

    public bool WorldToGrid(Vector3 worldPos, out int x, out int y)
    {
        x = Mathf.FloorToInt(worldPos.x / cellSize);
        y = Mathf.FloorToInt(worldPos.y / cellSize);
        return x >= 0 && x < width && y >= 0 && y < height;
    }
}