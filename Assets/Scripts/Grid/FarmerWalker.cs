using UnityEngine;

public class FarmerWalker : MonoBehaviour
{
    [Header("Di chuyển")]
    public float moveSpeed = 2f;

    [Header("Giới hạn map")]
    public float minX = 0.5f;
    public float maxX = 5.5f;
    public float minY = 0.5f;
    public float maxY = 2.5f; // không đi vào hàng locked

    private Animator _anim;
    private Vector2 _targetPos;
    private Vector2 _moveDir;
    private float _waitTimer = 0f;
    private bool _isWaiting = false;

    void Start()
    {
        _anim = GetComponent<Animator>();
        _targetPos = transform.position;
        PickNewTarget(); // chọn điểm đi đầu tiên
    }

    void Update()
    {
        // Đứng yên nếu game chưa chạy
        if (GameManager.Instance.State != GameManager.GameState.Playing)
        {
            _anim.SetBool("isMoving", false);
            return;
        }

        if (_isWaiting)
        {
            _waitTimer -= Time.deltaTime;
            _anim.SetBool("isMoving", false);
            if (_waitTimer <= 0f)
            {
                _isWaiting = false;
                PickNewTarget();
            }
            return;
        }

        // Di chuyển về target
        Vector2 current = transform.position;
        float dist = Vector2.Distance(current, _targetPos);

        if (dist < 0.05f)
        {
            // Đến nơi → đứng chờ rồi đi tiếp
            transform.position = new Vector3(_targetPos.x, _targetPos.y, 0);
            _isWaiting = true;
            _waitTimer = Random.Range(0.5f, 2f);
            _anim.SetBool("isMoving", false);
            return;
        }

        // Move
        _moveDir = (_targetPos - current).normalized;
        transform.position = Vector2.MoveTowards(current, _targetPos,
            moveSpeed * Time.deltaTime);

        // Cập nhật Animator
        _anim.SetFloat("moveX", _moveDir.x);
        _anim.SetFloat("moveY", _moveDir.y);
        _anim.SetBool("isMoving", true);
    }

    void PickNewTarget()
    {
        // Chọn ngẫu nhiên điểm trong map (tránh đè building)
        float x = Random.Range(minX, maxX);
        float y = Random.Range(minY, maxY);
        _targetPos = new Vector2(
            Mathf.Round(x * 2) / 2f,  // snap 0.5 grid
            Mathf.Round(y * 2) / 2f
        );
    }
}
