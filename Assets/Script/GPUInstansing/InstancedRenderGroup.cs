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
                const int NUM_LAYERS = 1000; // ������ ���� ������� �������������
                List<List<IInstancingData>> layers = new List<List<IInstancingData>>(NUM_LAYERS);

                // ���������� ������ ��� ������� ����
                for (int i = 0; i < NUM_LAYERS; i++)
                    layers.Add(new List<IInstancingData>());

                // 1) ����������� �ᒺ��� �� �������� �� ������
                Camera cam = Camera.main;
                if (cam == null) return;

                foreach (var instance in _instances)
                {
                    float distance = Vector3.Distance(instance.Pivot, cam.transform.position);
                    int layerIndex = Mathf.Clamp((int)(distance / 10f), 0, NUM_LAYERS - 1);
                    layers[layerIndex].Add(instance);
                }

                // 2) ������� ��'���� �� ������
                for (int layerIndex = 0; layerIndex < NUM_LAYERS; layerIndex++)
                {
                    var layer = layers[layerIndex];
                    if (layer.Count == 0) continue;

                    int currentBatch = layer.Count;
                    Matrix4x4[] matrices = new Matrix4x4[currentBatch];
                    Vector4[] pivots = new Vector4[currentBatch];
                    float[] depthLevels = new float[currentBatch];

                    for (int i = 0; i < currentBatch; i++)
                    {
                        IInstancingData data = layer[i];
                        matrices[i] = data.InstancingMatrix;
                        pivots[i] = data.Pivot;
                        depthLevels[i] = (float)layerIndex / (float)NUM_LAYERS; // ���������� ����� �������
                    }

                    var propertyBlock = new MaterialPropertyBlock();
                    propertyBlock.SetVectorArray("_Pivot", pivots);
                    propertyBlock.SetFloatArray("_DepthLevel", depthLevels); // �������� ����� � ������

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
                        cam
                    );
                }

            }

        }

    }
}
