using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI")]
    public NamePromptPanelController namePromptPanel;

    public enum GameState { Menu, Playing, GameOver }
    public GameState State { get; private set; }

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        // Lắng nghe sự kiện hết giờ
        TimeSystem.Instance.onTimeUp.AddListener(OnTimeUp);
        EnsurePlayerNameAndStart();
    }

    public void StartGame()
    {
        State = GameState.Playing;
        BuildingUnlockSystem.Instance.Reset();
        GoldManager.Instance.ResetGold();      // ← reset vàng về 0
        InventorySystem.Instance.Clear();      // ← xóa inventory
        TimeSystem.Instance.StartGame();
        Debug.Log("Game bat dau!");
    }

    void EnsurePlayerNameAndStart()
    {
        if (!PlayerProfile.HasPlayerName())
        {
            State = GameState.Menu;
            if (namePromptPanel != null)
            {
                namePromptPanel.Show(StartGame);
            }
            else
            {
                Debug.LogWarning("NamePromptPanelController chưa được gán - sẽ start game luôn để tránh kẹt.");
                StartGame();
            }
            return;
        }

        StartGame();
    }

    void OnTimeUp()
    {
        State = GameState.GameOver;

        // Dừng tất cả building đang sản xuất
        BuildingController[] allBuildings =
            FindObjectsByType<BuildingController>(FindObjectsSortMode.None);
        foreach (var b in allBuildings)
            b.StopProduction();

        int finalGold = GoldManager.Instance.GetGold();
        ScoreSystem.Instance.SaveScore(finalGold);
        LeaderboardService.UpsertBestScore(PlayerProfile.GetPlayerName(), finalGold);

        if (HUDManager.Instance != null)
            HUDManager.Instance.ShowResultScreen(finalGold);
        // Hủy placement nếu đang chọn building
        if (BuildingPlacer.Instance != null)
            BuildingPlacer.Instance.CancelPlacement();

        Debug.Log($"Het gio! Tong vang: {finalGold}");
    }

    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}