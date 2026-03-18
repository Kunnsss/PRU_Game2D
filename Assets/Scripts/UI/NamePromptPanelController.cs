using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NamePromptPanelController : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField playerNameInput;
    public TMP_Text errorText;
    public Button startButton;

    private Action _onDone;

    void Awake()
    {
        if (errorText != null)
            errorText.text = "";

        if (startButton != null)
        {
            startButton.onClick.RemoveListener(OnClickStart);
            startButton.onClick.AddListener(OnClickStart);
        }
    }

    void OnEnable()
    {
        if (errorText != null)
            errorText.text = "";

        if (playerNameInput != null)
        {
            playerNameInput.text = "";
            playerNameInput.Select();
            playerNameInput.ActivateInputField();
        }
    }

    public void Show(Action onDone)
    {
        _onDone = onDone;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void OnClickStart()
    {
        string raw = playerNameInput != null ? playerNameInput.text : "";
        Debug.Log("[NamePrompt] OnClickStart, text = " + raw);

        if (!PlayerProfile.TrySetPlayerName(raw))
        {
            if (errorText != null)
                errorText.text = "Tên không hợp lệ. Vui lòng nhập lại (1-20 ký tự).";
            Debug.Log("[NamePrompt] Name invalid");
            return;
        }

        Debug.Log("[NamePrompt] Name accepted, starting game");
        Hide();
        _onDone?.Invoke();
        _onDone = null;
    }
}

