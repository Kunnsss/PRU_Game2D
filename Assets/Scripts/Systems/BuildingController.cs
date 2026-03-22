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
    private Animator _animator;

    void Start()
    {
        _sr = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();

        if (data != null)
        {
            _sr.color = data.buildingColor;
            if (data.sprite != null) _sr.sprite = data.sprite;
        }

        InventorySystem.Instance.onInventoryChanged
            .AddListener(OnInventoryChanged);

        TryStartProduction();
    }

    void OnDestroy()
    {
        if (InventorySystem.Instance != null)
            InventorySystem.Instance.onInventoryChanged
                .RemoveListener(OnInventoryChanged);
    }

    void OnInventoryChanged()
    {
        if (!_isRunning)
            TryStartProduction();
    }

    public void TryStartProduction()
    {
        if (_isRunning) return;
        if (data == null || data.outputItem == null) return;
        if (GameManager.Instance.State != GameManager.GameState.Playing)
            return;

        var ingredients = data.outputItem.ingredients;

        if (ingredients == null || ingredients.Length == 0)
        {
            StartCoroutine(ProductionLoop());
            return;
        }

        if (InventorySystem.Instance.ConsumeIngredients(ingredients))
            StartCoroutine(ProductionLoop());
    }

    IEnumerator ProductionLoop()
    {
        _isRunning = true;

        if (_sr && data != null)
            _sr.color = new Color(
                data.buildingColor.r * 0.7f,
                data.buildingColor.g * 0.7f,
                data.buildingColor.b * 0.7f);

        float stageDuration = data.productionTime / 3f;

        // Giai đoạn 0: Seedling
        _animator?.SetInteger("Stage", 0);
        Debug.Log($"[{data.buildingName}] Stage 0 - Seedling ({stageDuration}s)");
        yield return new WaitForSeconds(stageDuration);

        // Giai đoạn 1: HalfRipe
        _animator?.SetInteger("Stage", 1);
        Debug.Log($"[{data.buildingName}] Stage 1 - HalfRipe ({stageDuration}s)");
        yield return new WaitForSeconds(stageDuration);

        // Giai đoạn 2: Ripe
        _animator?.SetInteger("Stage", 2);
        Debug.Log($"[{data.buildingName}] Stage 2 - Ripe ({stageDuration}s)");
        yield return new WaitForSeconds(stageDuration);

        // Kiểm tra game vẫn đang chạy
        if (GameManager.Instance.State != GameManager.GameState.Playing)
        {
            _isRunning = false;
            _animator?.SetInteger("Stage", 0);
            if (_sr && data != null) _sr.color = data.buildingColor;
            yield break;
        }

        // Thu hoạch vào inventory
        Debug.Log($"[{data.buildingName}] Thu hoạch!");
        InventorySystem.Instance.AddItem(data.outputItem);
        onProductionComplete?.Invoke(data.outputItem);

        if (_sr && data != null)
            _sr.color = data.buildingColor;

        _isRunning = false;

        // Vòng tiếp theo
        TryStartProduction();
    }

    public void StopProduction()
    {
        StopAllCoroutines();
        _isRunning = false;
        _animator?.SetInteger("Stage", 0);
        if (_sr && data != null)
            _sr.color = data.buildingColor;
    }

    public bool IsRunning => _isRunning;
}