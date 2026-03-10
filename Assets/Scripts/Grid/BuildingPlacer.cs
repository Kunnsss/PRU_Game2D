using UnityEngine;

public class BuildingPlacer : MonoBehaviour
{
    public static BuildingPlacer Instance;

    [Header("Building đang được chọn để đặt")]
    public BuildingData selectedBuilding;

    [Header("Chế độ xóa")]
    public bool isDeleteMode = false;

    [Header("Chế độ mở ô")]
    public bool isUnlockMode = false;

    private GameObject _previewObject;
    private SpriteRenderer _previewSr;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        CreatePreview();
    }

    void CreatePreview()
    {
        if (_previewObject != null) Destroy(_previewObject);
        _previewObject = new GameObject("Preview");
        _previewSr = _previewObject.AddComponent<SpriteRenderer>();
        _previewSr.sortingOrder = 10;
        _previewObject.transform.localScale = new Vector3(0.9f, 0.9f, 1f);
        _previewObject.SetActive(false);
    }

    void Update()
    {
        // Dừng khi GameOver
        if (GameManager.Instance.State == GameManager.GameState.GameOver)
        {
            if (_previewObject != null) _previewObject.SetActive(false);
            return;
        }

        // Chế độ mở ô đất
        if (isUnlockMode)
        {
            HandleUnlockMode();
            return;
        }

        // Chế độ xóa
        if (isDeleteMode)
        {
            HandleDeleteMode();
            return;
        }

        // Không có building được chọn
        if (selectedBuilding == null)
        {
            if (_previewObject != null) _previewObject.SetActive(false);
            return;
        }

        // Lấy vị trí chuột
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;

        int x = Mathf.FloorToInt(mouseWorld.x);
        int y = Mathf.FloorToInt(mouseWorld.y);

        // Hiện preview
        _previewObject.SetActive(true);
        _previewObject.transform.position = new Vector3(x + 0.5f, y + 0.5f, 0);

        bool canPlace = GridManager.Instance.IsEmpty(x, y)
                     && x >= 0 && x < 6
                     && y >= 0 && y < 4
                     && GridManager.Instance.IsSlotUnlocked(x, y);

        _previewSr.color = canPlace
            ? new Color(0f, 1f, 0f, 0.4f)
            : new Color(1f, 0f, 0f, 0.4f);

        _previewSr.sprite = selectedBuilding.sprite;

        if (Input.GetMouseButtonDown(0) && canPlace)
            PlaceBuilding(x, y);

        if (Input.GetKeyDown(KeyCode.Escape))
            CancelPlacement();
    }

    void PlaceBuilding(int x, int y)
    {
        if (!GridManager.Instance.IsSlotUnlocked(x, y))
        {
            Debug.Log($"O ({x},{y}) chua mo khoa!");
            return;
        }

        if (selectedBuilding.unlockCost > 0)
        {
            if (!GoldManager.Instance.SpendGold(selectedBuilding.unlockCost))
            {
                Debug.Log($"Khong du vang! Can {selectedBuilding.unlockCost}");
                return;
            }
        }

        Vector3 pos = new Vector3(x + 0.5f, y + 0.5f, 0);
        GameObject obj = Instantiate(selectedBuilding.prefab, pos, Quaternion.identity);

        BuildingController bc = obj.GetComponent<BuildingController>();
        if (bc != null)
            bc.data = selectedBuilding;

        GridManager.Instance.PlaceBuilding(bc, x, y);
        Debug.Log($"Dat {selectedBuilding.buildingName} tai ({x},{y})");

        CancelPlacement();
    }

    void HandleDeleteMode()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;

        int x = Mathf.FloorToInt(mouseWorld.x);
        int y = Mathf.FloorToInt(mouseWorld.y);

        bool hasBuilding = x >= 0 && x < 6 && y >= 0 && y < 4
                        && !GridManager.Instance.IsEmpty(x, y);

        _previewObject.SetActive(hasBuilding);
        if (hasBuilding)
        {
            _previewObject.transform.position = new Vector3(x + 0.5f, y + 0.5f, 0);
            _previewSr.color = new Color(1f, 0f, 0f, 0.5f);
        }

        if (Input.GetMouseButtonDown(0) && hasBuilding)
        {
            bool result = GridManager.Instance.RemoveBuilding(x, y);
            Debug.Log(result ? $"Xoa thanh cong tai ({x},{y})" : "Xoa that bai!");
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            SetDeleteMode(false);
    }

    void HandleUnlockMode()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;

        int x = Mathf.FloorToInt(mouseWorld.x);
        int y = Mathf.FloorToInt(mouseWorld.y);

        bool isLocked = x >= 0 && x < 6 && y >= 0 && y < 4
                     && !GridManager.Instance.IsSlotUnlocked(x, y);

        _previewObject.SetActive(isLocked);
        if (isLocked)
        {
            _previewObject.transform.position = new Vector3(x + 0.5f, y + 0.5f, 0);
            _previewSr.color = new Color(1f, 0.8f, 0f, 0.5f);
        }

        if (Input.GetMouseButtonDown(0) && isLocked)
        {
            if (GridManager.Instance.UnlockSlot(x, y))
            {
                GridManager.Instance.RefreshLockedVisuals();
                Debug.Log($"Mo o ({x},{y}) thanh cong!");
            }
            else
            {
                Debug.Log("Khong du vang!");
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetUnlockMode(false);
            FindFirstObjectByType<UnlockSlotButton>()?.TurnOff();
        }
    }

    public void SelectBuilding(BuildingData data)
    {
        isDeleteMode = false;
        isUnlockMode = false;
        selectedBuilding = data;
        CreatePreview();
        if (data.sprite != null)
            _previewSr.sprite = data.sprite;
        Debug.Log($"Da chon: {data.buildingName}");
    }

    public void CancelPlacement()
    {
        selectedBuilding = null;
        if (_previewObject != null)
        {
            _previewObject.SetActive(false);
            if (_previewSr != null) _previewSr.color = Color.white;
        }
    }

    public void SetDeleteMode(bool active)
    {
        isDeleteMode = active;
        isUnlockMode = false;
        selectedBuilding = null;
        if (_previewObject != null) _previewObject.SetActive(false);
    }

    public void SetUnlockMode(bool active)
    {
        isUnlockMode = active;
        isDeleteMode = false;
        selectedBuilding = null;
        if (_previewObject != null) _previewObject.SetActive(false);
        Debug.Log(active ? "Che do mo o: BAT" : "Che do mo o: TAT");
    }
}