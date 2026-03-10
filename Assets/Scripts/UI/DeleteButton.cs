using UnityEngine;
using UnityEngine.UI;

public class DeleteButton : MonoBehaviour
{
    private Button _btn;
    private bool _deleteMode = false;

    void Start()
    {
        _btn = GetComponent<Button>();
        _btn.onClick.AddListener(ToggleDeleteMode);
    }

    void ToggleDeleteMode()
    {
        _deleteMode = !_deleteMode;
        BuildingPlacer.Instance.SetDeleteMode(_deleteMode);

        // Đổi màu nút để biết đang bật/tắt
        var img = _btn.GetComponent<Image>();
        if (img != null)
            img.color = _deleteMode
                ? new Color(0.9f, 0.2f, 0.2f)  // đỏ = đang xóa
                : new Color(0.3f, 0.3f, 0.3f);  // xám = tắt
    }
}