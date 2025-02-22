using UnityEngine;
using Zenject;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class SlimeJumpController : MonoBehaviour
{
    [Inject] private ISlimeInput _slimeInput;  // Стратегія вводу для слайма

    [Header("=== СЛАЙМ: Рух та стрибки ===")]
    [Tooltip("Сила стрибка по вертикалі.")]
    public float jumpForce = 5f;
    [Tooltip("Сила горизонтальної складової стрибка.")]
    public float horizontalJumpForce = 5f;
    [Tooltip("Швидкість руху в повітрі.")]
    public float moveSpeed = 5f;
    [Tooltip("Множник для корекції швидкості в повітрі.")]
    public float airControlForce = 2f;

    [Header("=== Перевірка на землю ===")]
    [Tooltip("Додаткова відстань для перевірки землі.")]
    public float extraGroundCheckHeight = 0.1f;
    [Tooltip("Шар, який визначає землю.")]
    public LayerMask groundLayer;

    [Header("=== Камера ===")]
    [Tooltip("Посилання на камеру, відносно якої обчислюється рух.")]
    [SerializeField] private Transform _cameraTransform;

    private Rigidbody _rb;
    private Collider _col;
    private bool _isGrounded;
    private bool _hasJumped;
    private Vector2 _moveInput;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _col = GetComponent<Collider>();
    }

    void Update()
    {
        // Зчитуємо значення руху через стратегію
        _moveInput = _slimeInput.GetMoveValue();

        // Обчислюємо напрямок руху відносно камери:
        // Беремо forward та right камери, ігноруючи вертикальну складову.
        Vector3 cameraForward = _cameraTransform.forward;
        cameraForward.y = 0f;
        cameraForward.Normalize();

        Vector3 cameraRight = _cameraTransform.right;
        cameraRight.y = 0f;
        cameraRight.Normalize();

        // Обчислюємо світовий напрямок руху із врахуванням вводу
        Vector3 inputDir = (cameraRight * _moveInput.x + cameraForward * _moveInput.y).normalized;

        // Автоматично виконуємо стрибок, якщо слайм на землі, є рух і ще не стрибнув
        if (_isGrounded && _moveInput.sqrMagnitude > 0.01f && !_hasJumped)
        {
            // Стрибок у напрямку, обчисленому відносно камери
            Vector3 jumpVector = inputDir * horizontalJumpForce + Vector3.up * jumpForce;

            // Очищаємо горизонтальну швидкість, зберігаючи вертикальну
            Vector3 currentVel = _rb.linearVelocity;
            _rb.linearVelocity = new Vector3(0f, currentVel.y, 0f);

            _rb.AddForce(jumpVector, ForceMode.Impulse);
            _hasJumped = true;
        }
    }

    void FixedUpdate()
    {
        CheckGrounded();

        if (!_isGrounded)
            ApplyAirControl();
        else
            _hasJumped = false;
    }

    private void ApplyAirControl()
    {
        // Обчислюємо напрямок руху відносно камери (аналогічно Update)
        Vector3 cameraForward = _cameraTransform.forward;
        cameraForward.y = 0f;
        cameraForward.Normalize();

        Vector3 cameraRight = _cameraTransform.right;
        cameraRight.y = 0f;
        cameraRight.Normalize();

        Vector3 inputDir = (cameraRight * _moveInput.x + cameraForward * _moveInput.y).normalized;
        Vector3 desiredVelocity = inputDir * moveSpeed;
        Vector3 currentHorizontal = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
        Vector3 newHorizontal = Vector3.Lerp(currentHorizontal, desiredVelocity, airControlForce * Time.fixedDeltaTime);
        _rb.linearVelocity = new Vector3(newHorizontal.x, _rb.linearVelocity.y, newHorizontal.z);
    }

    private void CheckGrounded()
    {
        float rayLength = _col.bounds.extents.y + extraGroundCheckHeight;
        Vector3 rayOrigin = transform.position;
        _isGrounded = Physics.Raycast(rayOrigin, Vector3.down, rayLength, groundLayer);
    }
}
