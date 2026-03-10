using UnityEngine;
using UnityEngine.Events;

public class TimeSystem : MonoBehaviour
{
    public static TimeSystem Instance;

    [Header("Cấu Hình")]
    public float totalTime = 300f;   // 5 phút
    public int maxExtensions = 2;     // tối đa 2 lần

    [Header("Events")]
    public UnityEvent<float> onTimeUpdated;
    public UnityEvent onTimeUp;
    public UnityEvent onWarning;
    public UnityEvent<int> onExtensionUsed;   // ← mới: broadcast còn mấy lần

    private float _timeLeft;
    private int _extensionCount = 0;
    private bool _running = false;
    private bool _warningSent = false;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void StartGame()
    {
        _timeLeft = totalTime;
        _extensionCount = 0;
        _running = true;
        _warningSent = false;
    }

    void Update()
    {
        if (!_running) return;

        _timeLeft -= Time.deltaTime;
        onTimeUpdated?.Invoke(_timeLeft);

        if (!_warningSent && _timeLeft <= 30f)
        {
            _warningSent = true;
            onWarning?.Invoke();
        }

        if (_timeLeft <= 0f)
        {
            _timeLeft = 0f;
            _running = false;
            onTimeUp?.Invoke();
        }
    }

    /// <summary>
    /// Mua gia hạn. Trả về:
    ///  0 = thành công
    /// -1 = hết lượt gia hạn
    /// -2 = không đủ vàng
    /// </summary>
    public int BuyExtension(int minutes)
    {
        if (_extensionCount >= maxExtensions) return -1;

        int cost = GetExtensionCost(minutes);
        if (!GoldManager.Instance.SpendGold(cost)) return -2;

        _timeLeft += minutes * 60f;
        _extensionCount++;

        // Nếu đã gần hết giờ → reset warning
        if (_timeLeft > 30f) _warningSent = false;

        onExtensionUsed?.Invoke(GetExtensionsLeft());
        Debug.Log($"⏰ Gia hạn +{minutes} phút! Còn {GetExtensionsLeft()} lần");
        return 0;
    }

    public int GetExtensionCost(int minutes)
    {
        return minutes switch
        {
            1 => GoldManager.Instance.ext1MinCost,
            3 => GoldManager.Instance.ext3MinCost,
            5 => GoldManager.Instance.ext5MinCost,
            _ => 999
        };
    }

    public int GetExtensionsLeft() => maxExtensions - _extensionCount;
    public float GetTimeLeft() => _timeLeft;
    public bool IsRunning() => _running;
}