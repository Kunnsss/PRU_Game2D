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

    private BuildingController[,] grid;
    private bool[,] unlockedSlots;
    private GameObject[,] lockedVisuals;

    #region Unity Lifecycle

    private void Awake()
    {
        InitializeSingleton();
        InitializeGrid();
    }

    private void Start()
    {
        RefreshLockedVisuals();
    }

    #endregion

    #region Initialization

    private void InitializeSingleton()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void InitializeGrid()
    {
        grid = new BuildingController[width, height];
        unlockedSlots = new bool[width, height];

        // Mở 3 hàng đầu
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                unlockedSlots[x, y] = (y < 3);
            }
        }
    }

    #endregion

    #region Grid Logic

    public bool IsEmpty(int x, int y)
    {
        if (!IsValidPosition(x, y)) return false;
        return grid[x, y] == null;
    }

    public bool PlaceBuilding(BuildingController building, int x, int y)
    {
        if (!IsValidPosition(x, y)) return false;
        if (!IsEmpty(x, y)) return false;

        if (!IsSlotUnlocked(x, y))
        {
            Debug.Log($"O ({x},{y}) chua mo khoa! Can {unlockCostPerSlot} vang");
            return false;
        }

        grid[x, y] = building;
        building.transform.position = GridToWorld(x, y);

        return true;
    }

    public bool RemoveBuilding(int x, int y)
    {
        if (!IsValidPosition(x, y)) return false;
        if (grid[x, y] == null) return false;

        Destroy(grid[x, y].gameObject);
        grid[x, y] = null;

        return true;
    }

    #endregion

    #region Unlock System

    public bool IsSlotUnlocked(int x, int y)
    {
        if (!IsValidPosition(x, y)) return false;
        return unlockedSlots[x, y];
    }

    public bool UnlockSlot(int x, int y)
    {
        if (IsSlotUnlocked(x, y)) return false;

        if (!GoldManager.Instance.SpendGold(unlockCostPerSlot))
            return false;

        unlockedSlots[x, y] = true;

        Debug.Log($"Mo o dat ({x},{y}) thanh cong!");

        return true;
    }

    #endregion

    #region Visuals

    public void RefreshLockedVisuals()
    {
        ClearOldVisuals();

        lockedVisuals = new GameObject[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (!unlockedSlots[x, y])
                {
                    lockedVisuals[x, y] = CreateLockedSlotVisual(x, y);
                }
            }
        }
    }

    private void ClearOldVisuals()
    {
        if (lockedVisuals == null) return;

        foreach (var v in lockedVisuals)
        {
            if (v != null)
                Destroy(v);
        }
    }

    private GameObject CreateLockedSlotVisual(int x, int y)
    {
        GameObject obj = new GameObject($"LockedSlot_{x}_{y}");

        obj.transform.position = GridToWorld(x, y);
        obj.transform.localScale = new Vector3(0.9f, 0.9f, 1f);

        SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
        sr.sprite = GetWhiteSprite();
        sr.color = new Color(0.1f, 0.1f, 0.1f, 0.85f);
        sr.sortingOrder = 1;

        CreatePriceLabel(obj);

        return obj;
    }

    private void CreatePriceLabel(GameObject parent)
    {
        GameObject child = new GameObject("PriceLabel");
        child.transform.SetParent(parent.transform);
        child.transform.localPosition = Vector3.zero;
        child.transform.localScale = new Vector3(0.4f, 0.4f, 1f);
    }

    private Sprite GetWhiteSprite()
    {
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();

        return Sprite.Create(
            tex,
            new Rect(0, 0, 1, 1),
            new Vector2(0.5f, 0.5f),
            1f
        );
    }

    #endregion

    #region Utilities

    public Vector3 GridToWorld(int x, int y)
    {
        return new Vector3(
            x * cellSize + cellSize / 2f,
            y * cellSize + cellSize / 2f,
            0f
        );
    }

    public bool WorldToGrid(Vector3 worldPos, out int x, out int y)
    {
        x = Mathf.FloorToInt(worldPos.x / cellSize);
        y = Mathf.FloorToInt(worldPos.y / cellSize);

        return IsValidPosition(x, y);
    }

    private bool IsValidPosition(int x, int y)
    {
        return x >= 0 && x < width &&
               y >= 0 && y < height;
    }

    #endregion
}