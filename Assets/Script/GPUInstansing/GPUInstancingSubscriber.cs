using GPUInstancingRender;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(MeshRenderer))]
public class GPUInstancingSubscriber : MonoBehaviour, IInstancingData
{
    [Tooltip("������ ����� instancing ��� ����� ��'����")]
    public GPUInstancingGroupType GroupType;

    private MeshRenderer _meshRenderer;
    private IInstancingRenderGroup _instancingGroup;

    [Inject]
    private InstancedRenderGroupManager _groupManager;

    public Matrix4x4 InstancingMatrix => transform.localToWorldMatrix;

    public Vector4 Pivot
    {
        get
        {
            var mf = GetComponent<MeshFilter>();
            if (mf != null && mf.sharedMesh != null)
            {
                // ������������� ����� bounds �� pivot (� ��������� �����������)
                Vector3 localPivot = mf.sharedMesh.bounds.center;
                // ������������ � ������� ������
                Vector3 worldPivot = transform.TransformPoint(localPivot);
                return new Vector4(worldPivot.x, worldPivot.y, worldPivot.z, 1f);
            }
            return new Vector4(transform.position.x, transform.position.y, transform.position.z, 1f);
        }
    }



    void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        // �������� ������ ����������, ��� �� ���� ��������� ���������
        if (_meshRenderer != null)
            _meshRenderer.enabled = false;
    }

    void OnEnable()
    {
        _instancingGroup = _groupManager.GetGroup(GroupType);
        if (_instancingGroup != null)
            _instancingGroup.Register(this);
    }

    void OnDisable()
    {
        if (_instancingGroup != null)
            _instancingGroup.Unregister(this);
    }
}
