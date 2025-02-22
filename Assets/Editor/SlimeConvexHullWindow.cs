using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using MeshProcess; // Підключаємо ваш namespace з VHACD.cs

public class SlimeVHACDWithParamsWindow : EditorWindow
{
    // Основні поля
    private Mesh sourceMesh;                // Вихідний меш
    private bool mergeAllHulls = false;     // Зливати всі оболонки чи брати одну
    private float offsetDistance = 0.1f;    // Offset

    private bool doRemesh = false;          // Чи робити ремеш
    private int remeshDetail = 2;           // "Деталь" ремешу (кількість ітерацій subdivision)

    // ---- Нове: обираємо метод генерації
    private enum GenerateMethod
    {
        VHACD_ConvexOrMultiHull,
        SDF_MarchingCubes // non-convex
    }
    private GenerateMethod generateMethod = GenerateMethod.VHACD_ConvexOrMultiHull;

    // VHACD Parameters
    private bool showVHACDParams = false;   // Згортання/розгортання блоку в UI

    // Локальний екземпляр параметрів
    private VHACD.Parameters vhacdParams = new VHACD.Parameters();

    [MenuItem("Tools/SlimeFactory Generator (VHACD Params)")]
    public static void ShowWindow()
    {
        var wnd = GetWindow<SlimeVHACDWithParamsWindow>("SlimeFactory VHACD Generator");
        wnd.InitializeDefaultParams();
    }

    private void InitializeDefaultParams()
    {
        // Початкові (рекомендовані) значення VHACD:
        vhacdParams.Init(); // Викликаємо ваш .Init()
        vhacdParams.m_maxConvexHulls = 1;
        vhacdParams.m_resolution = 200000;
        vhacdParams.m_concavity = 0.01;
        vhacdParams.m_planeDownsampling = 4;
        vhacdParams.m_convexhullDownsampling = 4;
        vhacdParams.m_alpha = 0.05;
        vhacdParams.m_beta = 0.05;
        vhacdParams.m_minVolumePerCH = 0.0001;
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("SlimeFactory Mesh Generator", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // Вибір вихідного меша
        sourceMesh = (Mesh)EditorGUILayout.ObjectField("Source Mesh", sourceMesh, typeof(Mesh), false);

        // Вибираємо метод: VHACD або SDF+MarchingCubes
        generateMethod = (GenerateMethod)EditorGUILayout.EnumPopup("Generate Method", generateMethod);

        // Якщо VHACD
        if (generateMethod == GenerateMethod.VHACD_ConvexOrMultiHull)
        {
            mergeAllHulls = EditorGUILayout.Toggle("Merge all hulls?", mergeAllHulls);
        }

        offsetDistance = EditorGUILayout.Slider("Offset Distance", offsetDistance, 0f, 1f);

        doRemesh = EditorGUILayout.Toggle("Remesh after offset?", doRemesh);
        if (doRemesh)
        {
            remeshDetail = EditorGUILayout.IntSlider("Remesh Detail (subdiv iterations)", remeshDetail, 1, 5);
        }

        // VHACD ПАРАМЕТРИ (розгортаємо/згортаємо)
        if (generateMethod == GenerateMethod.VHACD_ConvexOrMultiHull)
        {
            showVHACDParams = EditorGUILayout.Foldout(showVHACDParams, "VHACD Parameters");
            if (showVHACDParams)
            {
                EditorGUI.indentLevel++;
                // resolution
                vhacdParams.m_resolution = (uint)EditorGUILayout.IntSlider("Resolution",
                    (int)vhacdParams.m_resolution, 20000, 500000);

                // concavity
                double newConcavity = EditorGUILayout.Slider("Concavity",
                    (float)vhacdParams.m_concavity, 0f, 0.1f);
                vhacdParams.m_concavity = newConcavity;

                // planeDownsampling
                vhacdParams.m_planeDownsampling = (uint)EditorGUILayout.IntSlider("Plane Downsampling",
                    (int)vhacdParams.m_planeDownsampling, 1, 16);

                // convexhullDownsampling
                vhacdParams.m_convexhullDownsampling = (uint)EditorGUILayout.IntSlider("ConvexHull Downsampling",
                    (int)vhacdParams.m_convexhullDownsampling, 1, 16);

                // alpha
                vhacdParams.m_alpha = EditorGUILayout.Slider("Alpha", (float)vhacdParams.m_alpha, 0f, 1f);

                // beta
                vhacdParams.m_beta = EditorGUILayout.Slider("Beta", (float)vhacdParams.m_beta, 0f, 1f);

                // minVolumePerCH
                vhacdParams.m_minVolumePerCH = EditorGUILayout.Slider("Min Volume Per CH",
                    (float)vhacdParams.m_minVolumePerCH, 0f, 0.01f);

                // maxConvexHulls
                vhacdParams.m_maxConvexHulls = (uint)EditorGUILayout.IntSlider("Max Convex Hulls",
                    (int)vhacdParams.m_maxConvexHulls, 1, 32);

                // projectHullVertices
                vhacdParams.m_projectHullVertices = EditorGUILayout.Toggle("Project Hull Vertices",
                    vhacdParams.m_projectHullVertices);

                EditorGUI.indentLevel--;
            }
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Generate SlimeFactory Mesh"))
        {
            GenerateSlimeMesh();
        }
    }

    private void GenerateSlimeMesh()
    {
        if (!sourceMesh)
        {
            Debug.LogError("No source mesh assigned!");
            return;
        }

        // В залежності від обраного методу:
        if (generateMethod == GenerateMethod.VHACD_ConvexOrMultiHull)
        {
            GenerateViaVHACD();
        }
        else
        {
            GenerateViaSDFMarchingCubes();
        }
    }

    // ================== 1) VHACD MultiHull Method ====================
    private void GenerateViaVHACD()
    {
        // Створюємо тимчасовий GameObject для VHACD
        GameObject tempGO = new GameObject("VHACD_Temp");
        VHACD vhacd = tempGO.AddComponent<VHACD>();

        // Присвоюємо параметри
        vhacd.m_parameters = vhacdParams;

        // Генеруємо hulls
        List<Mesh> hulls = vhacd.GenerateConvexMeshes(sourceMesh);
        if (hulls == null || hulls.Count == 0)
        {
            Debug.LogError("VHACD returned no hulls!");
            DestroyImmediate(tempGO);
            return;
        }
        Debug.Log($"VHACD returned {hulls.Count} hull(s).");

        // Обираємо: злиття чи найбільший
        Mesh combinedMesh = null;
        if (mergeAllHulls && hulls.Count > 1)
        {
            combinedMesh = MergeHulls(hulls);
            // Додаємо Welding після Merge
            combinedMesh = WeldVertices(combinedMesh, 1e-4f);
        }
        else if (hulls.Count == 1)
        {
            combinedMesh = hulls[0];
        }
        else
        {
            // Якщо багато, але mergeAllHulls=false, беремо найбільший
            combinedMesh = hulls.OrderByDescending(m => m.vertexCount).First();
        }

        // Offset
        if (offsetDistance > 0f)
        {
            combinedMesh = MeshOffsetter.OffsetMesh(combinedMesh, offsetDistance);
        }

        // Remesh (Subdivide)
        Mesh finalMesh = combinedMesh;
        if (doRemesh)
        {
            finalMesh = RemeshMesh(combinedMesh, remeshDetail);
            if (finalMesh == null)
            {
                Debug.LogError("Remesh failed!");
                DestroyImmediate(tempGO);
                return;
            }
        }

        // Зберігаємо
        SaveMeshAsset(finalMesh, "VHACD_SlimeMesh");

        DestroyImmediate(tempGO);
    }

    // ================== 2) SDF + Marching Cubes Method (placeholder) ====================
    private void GenerateViaSDFMarchingCubes()
    {
        // Тут ми показуємо псевдокод. У реальній реалізації треба:
        // 1) Voxelize (Compute SDF) 
        // 2) Викликати Marching Cubes
        // 3) (Опціонально) Offset SDF, якщо треба інфляція
        // 4) Згенерувати Mesh

        Debug.Log("GenerateViaSDFMarchingCubes: This is a placeholder for a real SDF+MarchingCubes approach.");

        Mesh sdfMesh = GenerateMeshViaSDF(sourceMesh, offsetDistance);

        if (doRemesh)
        {
            sdfMesh = RemeshMesh(sdfMesh, remeshDetail);
        }

        SaveMeshAsset(sdfMesh, "SDF_SlimeMesh");
    }

    // Приклад заглушки для SDF+MarchingCubes
    private Mesh GenerateMeshViaSDF(Mesh src, float offset)
    {
        // 1) Voxelize + Compute SDF
        //    - Використовуємо готову бібліотеку або свій код. 
        //    - Зберігаємо distance до поверхні (neg=inside, pos=outside).
        // 2) Якщо треба offset => SDF(x) = SDF(x) - offset
        // 3) MarchingCubes(SDF) -> генеруємо вершини/трикутники

        // Заглушка: повернемо просто копію:
        Mesh dummy = Object.Instantiate(src);
        // In real approach, you'd produce a new mesh from SDF
        dummy.name = "SDF_Marching_Result";
        return dummy;
    }

    // ================== Допоміжні методи ====================

    private void SaveMeshAsset(Mesh mesh, string suffix)
    {
        string folderPath = "Assets/Resources/GeneratedMeshes";
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        string newAssetName = sourceMesh.name + "_" + suffix + ".asset";
        string assetPath = Path.Combine(folderPath, newAssetName);

        AssetDatabase.CreateAsset(mesh, assetPath);
        AssetDatabase.SaveAssets();

        Debug.Log($"SlimeFactory mesh generated and saved at: {assetPath}");
    }

    /// <summary>
    /// Зливає декілька мешів у один.
    /// </summary>
    private Mesh MergeHulls(List<Mesh> hulls)
    {
        int totalVerts = hulls.Sum(h => h.vertexCount);
        int totalTris = hulls.Sum(h => h.triangles.Length);

        Vector3[] mergedVerts = new Vector3[totalVerts];
        int[] mergedIndices = new int[totalTris];

        int vertOffset = 0;
        int triOffset = 0;

        for (int i = 0; i < hulls.Count; i++)
        {
            var hv = hulls[i].vertices;
            var ht = hulls[i].triangles;

            for (int v = 0; v < hv.Length; v++)
            {
                mergedVerts[vertOffset + v] = hv[v];
            }
            for (int t = 0; t < ht.Length; t++)
            {
                mergedIndices[triOffset + t] = ht[t] + vertOffset;
            }

            vertOffset += hv.Length;
            triOffset += ht.Length;
        }

        Mesh merged = new Mesh();
        merged.vertices = mergedVerts;
        merged.triangles = mergedIndices;
        merged.RecalculateNormals();
        merged.RecalculateBounds();
        return merged;
    }

    /// <summary>
    /// Зварює вершини, які збігаються в просторі з точністю epsilon,
    /// оновлює індекси трикутників, щоб уникнути "швів" між кількома hulls.
    /// </summary>
    private Mesh WeldVertices(Mesh mesh, float epsilon)
    {
        Vector3[] verts = mesh.vertices;
        int[] tris = mesh.triangles;

        // Використовуємо словник: (roundedX, roundedY, roundedZ) -> newIndex
        Dictionary<Vector3, int> map = new Dictionary<Vector3, int>();
        List<Vector3> newVerts = new List<Vector3>();
        int[] newTris = new int[tris.Length];

        for (int i = 0; i < verts.Length; i++)
        {
            Vector3 v = verts[i];
            // Округлюємо до сітки epsilon
            Vector3 key = new Vector3(
                Mathf.Round(v.x / epsilon),
                Mathf.Round(v.y / epsilon),
                Mathf.Round(v.z / epsilon)
            );

            if (!map.TryGetValue(key, out int index))
            {
                index = newVerts.Count;
                newVerts.Add(v);
                map[key] = index;
            }
        }

        // Перебираємо трикутники
        for (int t = 0; t < tris.Length; t++)
        {
            Vector3 oldVert = verts[tris[t]];
            Vector3 oldKey = new Vector3(
                Mathf.Round(oldVert.x / epsilon),
                Mathf.Round(oldVert.y / epsilon),
                Mathf.Round(oldVert.z / epsilon)
            );
            newTris[t] = map[oldKey];
        }

        Mesh welded = new Mesh();
        welded.vertices = newVerts.ToArray();
        welded.triangles = newTris;
        welded.RecalculateNormals();
        welded.RecalculateBounds();
        return welded;
    }

    /// <summary>
    /// Простий Offset (Inflation) вздовж нормалей.
    /// </summary>
    public static class MeshOffsetter
    {
        public static Mesh OffsetMesh(Mesh inputMesh, float offsetDistance)
        {
            if (offsetDistance == 0f) return inputMesh;

            Mesh mesh = Object.Instantiate(inputMesh);
            Vector3[] vertices = mesh.vertices;
            mesh.RecalculateNormals();
            Vector3[] normals = mesh.normals;

            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] += normals[i] * offsetDistance;
            }
            mesh.vertices = vertices;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }
    }

    /// <summary>
    /// Наївний subdivision (поділ кожного трикутника на 4).
    /// detailLevel = кількість ітерацій.
    /// </summary>
    private Mesh RemeshMesh(Mesh input, int detailLevel)
    {
        Mesh result = Object.Instantiate(input);
        for (int i = 0; i < detailLevel; i++)
        {
            result = SubdivideOneIteration(result);
        }
        return result;
    }

    /// <summary>
    /// Дуже спрощений subdivision: кожен трикутник -> 4 трикутники, додаємо midpoints на ребрах.
    /// Без обробки UV, нормалей тощо (після цього RecalculateNormals).
    /// </summary>
    private Mesh SubdivideOneIteration(Mesh src)
    {
        src.RecalculateNormals();
        Vector3[] oldVerts = src.vertices;
        int[] oldTris = src.triangles;

        List<Vector3> newVerts = new List<Vector3>();
        List<int> newTris = new List<int>();

        // Щоб шукати midpoint ребра, використаємо словник (key=unordered pair (i0,i1))
        Dictionary<long, int> midpointMap = new Dictionary<long, int>();

        // helper function
        System.Func<int, int, long> pairKey = (a, b) => {
            if (a > b) { int tmp = a; a = b; b = tmp; }
            return ((long)a << 32) + (long)b;
        };

        newVerts.AddRange(oldVerts);

        System.Func<int, int, int> getMidpointIndex = (i0, i1) => {
            long key = pairKey(i0, i1);
            if (midpointMap.ContainsKey(key))
                return midpointMap[key];

            Vector3 v0 = oldVerts[i0];
            Vector3 v1 = oldVerts[i1];
            Vector3 mid = (v0 + v1) * 0.5f;
            int idx = newVerts.Count;
            newVerts.Add(mid);
            midpointMap[key] = idx;
            return idx;
        };

        for (int t = 0; t < oldTris.Length; t += 3)
        {
            int i0 = oldTris[t + 0];
            int i1 = oldTris[t + 1];
            int i2 = oldTris[t + 2];

            int m01 = getMidpointIndex(i0, i1);
            int m12 = getMidpointIndex(i1, i2);
            int m20 = getMidpointIndex(i2, i0);

            // tri1
            newTris.Add(i0);
            newTris.Add(m01);
            newTris.Add(m20);

            // tri2
            newTris.Add(i1);
            newTris.Add(m12);
            newTris.Add(m01);

            // tri3
            newTris.Add(i2);
            newTris.Add(m20);
            newTris.Add(m12);

            // tri4
            newTris.Add(m01);
            newTris.Add(m12);
            newTris.Add(m20);
        }

        Mesh subdivMesh = new Mesh();
        subdivMesh.vertices = newVerts.ToArray();
        subdivMesh.triangles = newTris.ToArray();
        subdivMesh.RecalculateNormals();
        subdivMesh.RecalculateBounds();

        return subdivMesh;
    }
}
