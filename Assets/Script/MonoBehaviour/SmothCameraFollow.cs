using UnityEngine;
using Zenject;

namespace WannaBe
{
    public class OrbitalCameraController : MonoBehaviour
    {
        [Inject] private ICameraInput _cameraInput;  // �������� ����� ��� ������

        [Header("��������� �� ����")]
        public Transform target;

        [Header("������������ ������")]
        [Tooltip("Գ������� ������ ������ (���������� Y).")]
        public float fixedCameraHeight = 5f;
        [Tooltip("³������ �� ��� (����� �����).")]
        public float distance = 10f;
        [Tooltip("�������� ��������� ������ ������� �� Y.")]
        public float rotationSpeed = 100f;
        [Tooltip("��� ���������� ���� ������.")]
        public float followSmoothTime = 0.2f;

        private float _currentYAngle;
        private Vector3 _currentVelocity = Vector3.zero;
        private void Awake()
        {
            transform.parent = null;
        }
        void Start()
        {
            _currentYAngle = transform.eulerAngles.y;

            Camera cam = GetComponent<Camera>();
            if (cam != null)
                cam.orthographic = true;

            // Գ����� ��� ������ �� X �� 45�
            Vector3 startRotation = transform.eulerAngles;
            startRotation.x = 20f;
            transform.eulerAngles = startRotation;
        }

        void LateUpdate()
        {
            if (target == null)
                return;

            // ������ �������� ������� ���� ����� ��������
            Vector2 rightStickValue = _cameraInput.GetRightStickValue();
            float rotationInput = rightStickValue.x;
            _currentYAngle += rotationInput * rotationSpeed * Time.deltaTime;

            Quaternion desiredRotation = Quaternion.Euler(20f, _currentYAngle, 0f);
            Vector3 offset = new Vector3(0f, 0f, -distance);
            Vector3 targetXZ = new Vector3(target.position.x, 0f, target.position.z);
            Vector3 desiredPosition = targetXZ + desiredRotation * offset;
            desiredPosition.y = fixedCameraHeight;

            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref _currentVelocity, followSmoothTime);
            transform.rotation = desiredRotation;
        }
    }
}