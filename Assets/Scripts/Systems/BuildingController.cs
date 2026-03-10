using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class BuildingController : MonoBehaviour
{
    [Header("Dữ Liệu Building")]
    public BuildingData data;

    [Header("Events")]
    public UnityEvent<ItemData> onProductionComplete;

    private bool _isRunning = false;
    private SpriteRenderer _sr;

    void Start()
    {
        _sr = GetComponent<SpriteRenderer>();

        if (data != null)
        {
            _sr.color = data.buildingColor;
            if (data.sprite != null) _sr.sprite = data.sprite;
        }

        // Lắng nghe inventory thay đổi → thử sản xuất lại
        InventorySystem.Instance.onInventoryChanged
            .AddListener(OnInventoryChanged);

        TryStartProduction();
    }

    void OnDestroy()
    {
        // Hủy listener khi building bị xóa
        if (InventorySystem.Instance != null)
            InventorySystem.Instance.onInventoryChanged
                .RemoveListener(OnInventoryChanged);
    }

    void OnInventoryChanged()
    {
        // Khi inventory thay đổi → thử chạy lại nếu đang idle
        if (!_isRunning)
            TryStartProduction();
    }

    public void TryStartProduction()
    {
        if (_isRunning) return;
        if (data == null || data.outputItem == null) return;

        // Kiểm tra game có đang chạy không
        if (GameManager.Instance.State != GameManager.GameState.Playing)
            return;

        var ingredients = data.outputItem.ingredients;

        // Không cần nguyên liệu → canh đồng, chuồng gà
        if (ingredients == null || ingredients.Length == 0)
        {
            StartCoroutine(ProductionLoop());
            return;
        }

        // Cần nguyên liệu → kiểm tra và tiêu thụ
        if (InventorySystem.Instance.ConsumeIngredients(ingredients))
            StartCoroutine(ProductionLoop());
    }

    IEnumerator ProductionLoop()
    {
        _isRunning = true;

        // Màu tối hơn = đang sản xuất
        if (_sr && data != null)
            _sr.color = new Color(
                data.buildingColor.r * 0.7f,
                data.buildingColor.g * 0.7f,
                data.buildingColor.b * 0.7f);

        yield return new WaitForSeconds(data.productionTime);

        // Kiểm tra game vẫn đang chạy
        if (GameManager.Instance.State != GameManager.GameState.Playing)
        {
            _isRunning = false;
            if (_sr && data != null) _sr.color = data.buildingColor;
            yield break;  // dừng coroutine nếu game over
        }

        // Thêm item vào inventory
        InventorySystem.Instance.AddItem(data.outputItem);
        onProductionComplete?.Invoke(data.outputItem);
        Debug.Log($"+ {data.buildingName} san xuat {data.outputItem.itemName}");

        // Trả về màu gốc
        if (_sr && data != null)
            _sr.color = data.buildingColor;

        _isRunning = false;

        // Thử vòng tiếp theo
        TryStartProduction();
    }

    public void StopProduction()
    {
        StopAllCoroutines();
        _isRunning = false;
        if (_sr && data != null)
            _sr.color = data.buildingColor;
    }

    public bool IsRunning => _isRunning;
}