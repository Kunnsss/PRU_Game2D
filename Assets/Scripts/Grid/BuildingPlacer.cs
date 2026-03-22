using UnityEngine;

public class BuildingPlacer : MonoBehaviour
{
    public static BuildingPlacer Instance;

    [Header("Building đang được chọn để đặt")]
    public BuildingData selectedBuilding;

    [Header("Chế độ xóa")]
    public bool isDeleteMode;

    [Header("Chế độ mở ô")]
    public bool isUnlockMode;

    private GameObject previewObject;
    private SpriteRenderer previewRenderer;

    private const int GRID_WIDTH = 6;
    private const int GRID_HEIGHT = 4;

    #region Unity Lifecycle

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        CreatePreviewObject();
    }

    private void Update()
    {
        if (IsGameOver()) return;

        if (HandleSpecialModes()) return;

        HandlePlacementMode();
    }

    #endregion

    #region Preview

    private void CreatePreviewObject()
    {
        if (previewObject != null)
            Destroy(previewObject);

        previewObject = new GameObject("Preview");

        previewRenderer = previewObject.AddComponent<SpriteRenderer>();
        previewRenderer.sortingOrder = 10;

        previewObject.transform.localScale = new Vector3(0.9f, 0.9f, 1f);
        previewObject.SetActive(false);
    }

    private void UpdatePreview(Vector3 position, Color color, Sprite sprite)
    {
        previewObject.SetActive(true);
        previewObject.transform.position = position;
        previewRenderer.color = color;
        previewRenderer.sprite = sprite;
    }

    private void HidePreview()
    {
        if (previewObject != null)
            previewObject.SetActive(false);
    }

    #endregion

    #region Update Logic

    private bool IsGameOver()
    {
        if (GameManager.Instance.State != GameManager.GameState.GameOver)
            return false;

        HidePreview();
        return true;
    }

    private bool HandleSpecialModes()
    {
        if (isUnlockMode)
        {
            HandleUnlockMode();
            return true;
        }

        if (isDeleteMode)
        {
            HandleDeleteMode();
            return true;
        }

        return false;
    }

    private void HandlePlacementMode()
    {
        if (selectedBuilding == null)
        {
            HidePreview();
            return;
        }

        Vector3 mousePos = GetMouseWorldPosition();
        Vector2Int gridPos = GetGridPosition(mousePos);

        bool canPlace = CanPlaceBuilding(gridPos);

        UpdatePreview(
            GetCenteredPosition(gridPos),
            canPlace ? new Color(0f, 1f, 0f, 0.4f) : new Color(1f, 0f, 0f, 0.4f),
            selectedBuilding.sprite
        );

        if (Input.GetMouseButtonDown(0) && canPlace)
            PlaceBuilding(gridPos);

        if (Input.GetKeyDown(KeyCode.Escape))
            CancelPlacement();
    }

    #endregion

    #region Placement

    private void PlaceBuilding(Vector2Int gridPos)
    {
        int x = gridPos.x;
        int y = gridPos.y;

        if (!GridManager.Instance.IsSlotUnlocked(x, y))
        {
            Debug.Log($"O ({x},{y}) chua mo khoa!");
            return;
        }

        if (selectedBuilding.unlockCost > 0 &&
            !GoldManager.Instance.SpendGold(selectedBuilding.unlockCost))
        {
            Debug.Log($"Khong du vang! Can {selectedBuilding.unlockCost}");
            return;
        }

        GameObject obj = Instantiate(
            selectedBuilding.prefab,
            GetCenteredPosition(gridPos),
            Quaternion.identity
        );

        BuildingController controller = obj.GetComponent<BuildingController>();
        if (controller != null)
            controller.data = selectedBuilding;

        GridManager.Instance.PlaceBuilding(controller, x, y);

        Debug.Log($"Dat {selectedBuilding.buildingName} tai ({x},{y})");

        CancelPlacement();
    }

    private bool CanPlaceBuilding(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < GRID_WIDTH &&
               pos.y >= 0 && pos.y < GRID_HEIGHT &&
               GridManager.Instance.IsEmpty(pos.x, pos.y) &&
               GridManager.Instance.IsSlotUnlocked(pos.x, pos.y);
    }

    #endregion

    #region Delete Mode

    private void HandleDeleteMode()
    {
        Vector2Int gridPos = GetGridPosition(GetMouseWorldPosition());

        bool hasBuilding = IsInsideGrid(gridPos) &&
                           !GridManager.Instance.IsEmpty(gridPos.x, gridPos.y);

        previewObject.SetActive(hasBuilding);

        if (hasBuilding)
        {
            UpdatePreview(
                GetCenteredPosition(gridPos),
                new Color(1f, 0f, 0f, 0.5f),
                null
            );
        }

        if (Input.GetMouseButtonDown(0) && hasBuilding)
        {
            bool result = GridManager.Instance.RemoveBuilding(gridPos.x, gridPos.y);
            Debug.Log(result ? $"Xoa thanh cong tai ({gridPos.x},{gridPos.y})" : "Xoa that bai!");
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            SetDeleteMode(false);
    }

    #endregion

    #region Unlock Mode

    private void HandleUnlockMode()
    {
        Vector2Int gridPos = GetGridPosition(GetMouseWorldPosition());

        bool isLocked = IsInsideGrid(gridPos) &&
                        !GridManager.Instance.IsSlotUnlocked(gridPos.x, gridPos.y);

        previewObject.SetActive(isLocked);

        if (isLocked)
        {
            UpdatePreview(
                GetCenteredPosition(gridPos),
                new Color(1f, 0.8f, 0f, 0.5f),
                null
            );
        }

        if (Input.GetMouseButtonDown(0) && isLocked)
        {
            if (GridManager.Instance.UnlockSlot(gridPos.x, gridPos.y))
            {
                GridManager.Instance.RefreshLockedVisuals();
                Debug.Log($"Mo o ({gridPos.x},{gridPos.y}) thanh cong!");
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

    #endregion

    #region Helpers

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0;
        return pos;
    }

    private Vector2Int GetGridPosition(Vector3 worldPos)
    {
        return new Vector2Int(
            Mathf.FloorToInt(worldPos.x),
            Mathf.FloorToInt(worldPos.y)
        );
    }

    private Vector3 GetCenteredPosition(Vector2Int gridPos)
    {
        return new Vector3(gridPos.x + 0.5f, gridPos.y + 0.5f, 0);
    }

    private bool IsInsideGrid(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < GRID_WIDTH &&
               pos.y >= 0 && pos.y < GRID_HEIGHT;
    }

    #endregion

    #region Public API

    public void SelectBuilding(BuildingData data)
    {
        isDeleteMode = false;
        isUnlockMode = false;

        selectedBuilding = data;

        CreatePreviewObject();

        if (data.sprite != null)
            previewRenderer.sprite = data.sprite;

        Debug.Log($"Da chon: {data.buildingName}");
    }

    public void CancelPlacement()
    {
        selectedBuilding = null;

        if (previewObject != null)
        {
            previewObject.SetActive(false);
            if (previewRenderer != null)
                previewRenderer.color = Color.white;
        }
    }

    public void SetDeleteMode(bool active)
    {
        isDeleteMode = active;
        isUnlockMode = false;
        selectedBuilding = null;

        HidePreview();
    }

    public void SetUnlockMode(bool active)
    {
        isUnlockMode = active;
        isDeleteMode = false;
        selectedBuilding = null;

        HidePreview();

        Debug.Log(active ? "Che do mo o: BAT" : "Che do mo o: TAT");
    }

    #endregion
}