using GPUInstancingRender;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
namespace GPUInstancingRender
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Rendering;

    namespace GPUInstancingRender
    {
        public class InstancedRenderGroupComponent : MonoBehaviour, IInstancingRenderGroup
        {
            [Header("Common settings for this group")]
            [SerializeField] private Mesh _commonMesh;           // ������� ���, ���� ��������������������� ��� ��� ��������
            [SerializeField] private Material _commonMaterial;   // ������� � ��������� GPU Instancing
            [SerializeField] private int _layer = 0;               // ��������� ���

            // ���������� ��������� � DrawMeshInstanced ������� �������� 1023 �������� �� ������
            private const int BatchSize = 1023;
            // ������ ��� ��'����, �� ����������� ��� ���� �����
            private readonly List<IInstancingData> _instances = new List<IInstancingData>();

            /// <summary>
            /// ������ ��'��� � ������ ��� instancing.
            /// </summary>
            public void Register(IInstancingData data)
            {
                if (!_instances.Contains(data))
                    _instances.Add(data);
            }
            /// <summary>
            /// ��������� ��������� ����� instancing.
            /// </summary>
            public void SetSettings(Mesh commonMesh, Material commonMaterial, int layer)
            {
                _commonMesh = commonMesh;
                _commonMaterial = commonMaterial;
                _layer = layer;
            }
            /// <summary>
            /// ������� ��'��� �� �����.
            /// </summary>
            public void Unregister(IInstancingData data)
            {
                _instances.Remove(data);
            }

            // ��������� ��������� ��������� �����
            void Update()
            {
                RenderInstances();
            }

            /// <summary>
            /// ����� ��� ��� ������������� ��'���� � ������ instanced rendering.
            /// </summary>
            private void RenderInstances()
            {
                int count = _instances.Count;
                if (count == 0 || _commonMesh == null || _commonMaterial == null)
                    return;

                int batches = Mathf.CeilToInt((float)count / BatchSize);

                Camera cam = Camera.main;
                if (cam != null)
                {
                    Debug.LogFormat("Camera pos=({0:F2}, {1:F2}, {2:F2}), rot=({3:F2}, {4:F2}, {5:F2}), near={6}, far={7}",
                        cam.transform.position.x, cam.transform.position.y, cam.transform.position.z,
                        cam.transform.eulerAngles.x, cam.transform.eulerAngles.y, cam.transform.eulerAngles.z,
                        cam.nearClipPlane, cam.farClipPlane
                    );
                }


                // �������� ���� ����� bounds, ��� �����������, �� �� �������� ����������� � �����
                Bounds bigBounds = new Bounds(Vector3.zero, new Vector3(10000, 10000, 10000));

                for (int b = 0; b < batches; b++)
                {
                    int start = b * BatchSize;
                    int currentBatch = Mathf.Min(BatchSize, count - start);

                    Matrix4x4[] matrices = new Matrix4x4[currentBatch];
                    Vector4[] pivots = new Vector4[currentBatch];

                    for (int i = 0; i < currentBatch; i++)
                    {
                        IInstancingData data = _instances[start + i];
                        matrices[i] = data.InstancingMatrix;
                        pivots[i] = data.Pivot;

                        // ��� ��� ������� ����������:
                        Debug.LogFormat(
                            "Instance {0}: \n" +
                            "Pivot = {1}, " +
                            "Matrix pos=({2:F2}, {3:F2}, {4:F2}), \n" +
                            "Matrix forward=({5:F2}, {6:F2}, {7:F2})",
                            i,
                            pivots[i],
                            matrices[i].m03, matrices[i].m13, matrices[i].m23,
                            matrices[i].m02, matrices[i].m12, matrices[i].m22
                        );
                    }
                    var propertyBlock = new MaterialPropertyBlock();
                    propertyBlock.SetVectorArray("_Pivot", pivots);

                    // �� pivots[i] = ����� Vector4( pivotX, pivotY, pivotZ, 1 );
                    Graphics.DrawMeshInstanced(
                        _commonMesh,
                        0,
                        _commonMaterial,
                        matrices,
                        currentBatch,
                        propertyBlock,
                        ShadowCastingMode.Off,
                        false,
                        _layer,
                        Camera.main
                    );

                }
            }

        }

    }
}
