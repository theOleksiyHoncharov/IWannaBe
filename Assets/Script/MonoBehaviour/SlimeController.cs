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

    [Header("=== Перевірка на землю (ручні параметри) ===")]
    [Tooltip("Радіус нижньої частини сфери для перевірки землі.")]
    public float groundCheckRadius = 0.5f;
    [Tooltip("Відступ від нижньої точки об’єкта до центру області перевірки (наприклад, половина радіусу).")]
    public float groundCheckOffsetY = 0.25f;
    [Tooltip("Додатковий запас для перевірки землі.")]
    public float groundCheckExtraHeight = 0.1f;

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

    // Отримуємо вхідні дані в Update для більшої реакції
    void Update()
    {
        _moveInput = _slimeInput.GetMoveValue();
    }

    void FixedUpdate()
    {
        CheckGrounded();

        // Обчислюємо напрямок руху відносно камери
        Vector3 cameraForward = _cameraTransform.forward;
        cameraForward.y = 0f;
        cameraForward.Normalize();

        Vector3 cameraRight = _cameraTransform.right;
        cameraRight.y = 0f;
        cameraRight.Normalize();

        Vector3 inputDir = (cameraRight * _moveInput.x + cameraForward * _moveInput.y).normalized;

        // Якщо слайм на землі, є ввід і ще не стрибав – виконуємо стрибок
        if (_isGrounded && _moveInput.sqrMagnitude > 0.01f && !_hasJumped)
        {
            // Очищаємо горизонтальну швидкість, зберігаючи вертикальну
            Vector3 currentVel = _rb.linearVelocity;
            _rb.linearVelocity = new Vector3(0f, currentVel.y, 0f);

            // Обчислюємо вектор стрибка: горизонтальна і вертикальна складові
            Vector3 jumpVector = inputDir * horizontalJumpForce + Vector3.up * jumpForce;
            _rb.AddForce(jumpVector, ForceMode.Impulse);

            _hasJumped = true;
        }

        // Якщо слайм у повітрі – застосовуємо контроль в повітрі
        if (!_isGrounded)
            ApplyAirControl();
        else
            _hasJumped = false;
    }

    private void ApplyAirControl()
    {
        // Обчислюємо напрямок руху відносно камери (аналогічно)
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
        // Центр нижньої половини для перевірки
        Vector3 lowerHemisphereCenter = transform.position + Vector3.up * groundCheckOffsetY;
        // Радіус перевірки – базовий радіус з додатковим запасом
        float checkRadius = groundCheckRadius + groundCheckExtraHeight;

        Collider[] overlaps = Physics.OverlapSphere(lowerHemisphereCenter, checkRadius, groundLayer, QueryTriggerInteraction.Ignore);
        _isGrounded = overlaps.Length > 0;
    }
}
