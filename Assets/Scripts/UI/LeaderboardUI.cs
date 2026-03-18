using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LeaderboardUI : MonoBehaviour
{
    private static LeaderboardUI _instance;

    private Canvas _canvas;
    private TMP_Text[] _rows;

    public static void ShowTop10()
    {
        if (_instance == null)
        {
            var go = new GameObject("LeaderboardUI");
            _instance = go.AddComponent<LeaderboardUI>();
            DontDestroyOnLoad(go);
        }

        _instance.Show();
    }

    void Show()
    {
        EnsureUI();
        Refresh();
        _canvas.enabled = true;
    }

    void Hide()
    {
        if (_canvas != null) _canvas.enabled = false;
    }

    void EnsureUI()
    {
        if (_canvas != null) return;
        UIEventSystemBootstrap.EnsureEventSystem();

        var canvasGo = new GameObject("Canvas");
        canvasGo.transform.SetParent(transform, false);
        _canvas = canvasGo.AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGo.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasGo.AddComponent<GraphicRaycaster>();

        var bg = new GameObject("Backdrop");
        bg.transform.SetParent(canvasGo.transform, false);
        var bgImg = bg.AddComponent<Image>();
        bgImg.color = new Color(0f, 0f, 0f, 0.65f);
        var bgRt = bg.GetComponent<RectTransform>();
        bgRt.anchorMin = Vector2.zero;
        bgRt.anchorMax = Vector2.one;
        bgRt.offsetMin = Vector2.zero;
        bgRt.offsetMax = Vector2.zero;
        // Cho phép click bất kỳ chỗ nào ngoài panel để đóng leaderboard
        var bgBtn = bg.AddComponent<Button>();
        bgBtn.transition = Selectable.Transition.None;
        bgBtn.onClick.AddListener(Hide);

        var panel = new GameObject("Panel");
        panel.transform.SetParent(canvasGo.transform, false);
        var panelImg = panel.AddComponent<Image>();
        panelImg.color = new Color(0.10f, 0.10f, 0.10f, 0.96f);
        var prt = panel.GetComponent<RectTransform>();
        prt.anchorMin = new Vector2(0.5f, 0.5f);
        prt.anchorMax = new Vector2(0.5f, 0.5f);
        // Panel nhỏ gọn hơn, không che quá nhiều màn hình
        prt.sizeDelta = new Vector2(420, 320);
        prt.anchoredPosition = Vector2.zero;

        var v = panel.AddComponent<VerticalLayoutGroup>();
        v.padding = new RectOffset(24, 24, 24, 24);
        v.spacing = 10;
        v.childAlignment = TextAnchor.UpperCenter;

        var title = CreateText(panel.transform, "🏆 Top 10", 24, FontStyles.Bold);
        title.alignment = TextAlignmentOptions.Center;

        var header = CreateText(panel.transform, "Rank   Name              Score", 18, FontStyles.Bold);
        header.color = new Color(0.95f, 0.95f, 0.95f, 1f);
        header.alignment = TextAlignmentOptions.Left;

        _rows = new TMP_Text[10];
        for (int i = 0; i < 10; i++)
        {
            var row = CreateText(panel.transform, "", 16, FontStyles.Normal);
            row.alignment = TextAlignmentOptions.Left;
            row.color = i == 0 ? new Color(1f, 0.85f, 0.25f, 1f) : Color.white;
            _rows[i] = row;
        }

        var btnRow = new GameObject("Buttons");
        btnRow.transform.SetParent(panel.transform, false);
        var h = btnRow.AddComponent<HorizontalLayoutGroup>();
        h.spacing = 12;
        h.childAlignment = TextAnchor.MiddleCenter;

        var closeBtn = CreateButton(btnRow.transform, "Đóng");
        closeBtn.onClick.AddListener(Hide);

        _canvas.enabled = false;
    }

    void Refresh()
    {
        IReadOnlyList<LeaderboardEntry> top = LeaderboardService.GetTop10();
        for (int i = 0; i < 10; i++)
        {
            if (i < top.Count)
            {
                var e = top[i];
                int rank = i + 1;
                _rows[i].text = $"{rank, -4}  {Truncate(e.playerName, 26), -28}  {e.bestScore, 6}";
            }
            else
            {
                _rows[i].text = $"{i + 1, -4}  -                           -";
            }
        }
    }

    static string Truncate(string s, int max)
    {
        if (string.IsNullOrEmpty(s)) return "";
        return s.Length <= max ? s : s.Substring(0, max - 1) + "…";
    }

    static TMP_Text CreateText(Transform parent, string text, int fontSize, FontStyles style)
    {
        var go = new GameObject("Text");
        go.transform.SetParent(parent, false);
        var t = go.AddComponent<TextMeshProUGUI>();
        t.text = text;
        t.fontSize = fontSize;
        t.fontStyle = style;
        t.color = Color.white;
        t.enableWordWrapping = false;
        var rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(0, 0);
        return t;
    }

    static Button CreateButton(Transform parent, string label)
    {
        var go = new GameObject("Button");
        go.transform.SetParent(parent, false);
        var img = go.AddComponent<Image>();
        img.color = new Color(0.25f, 0.55f, 0.95f, 1f);
        var btn = go.AddComponent<Button>();

        var rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(180, 48);

        var textGo = new GameObject("Label");
        textGo.transform.SetParent(go.transform, false);
        var t = textGo.AddComponent<TextMeshProUGUI>();
        t.text = label;
        t.fontSize = 22;
        t.fontStyle = FontStyles.Bold;
        t.color = Color.white;
        t.alignment = TextAlignmentOptions.Center;

        var trt = textGo.GetComponent<RectTransform>();
        trt.anchorMin = Vector2.zero;
        trt.anchorMax = Vector2.one;
        trt.offsetMin = Vector2.zero;
        trt.offsetMax = Vector2.zero;

        return btn;
    }
}

