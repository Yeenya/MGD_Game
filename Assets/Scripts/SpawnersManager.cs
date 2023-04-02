using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.SceneManagement;

#if UNITY_EDITOR
public class SpawnerManager : EditorWindow
{
    private List<Spawner> spawners = new List<Spawner>();

    [SerializeField]
    private List<GameObject> enemyPrefabs = new List<GameObject>();

    private int selectedSpawnerIndex = -1;

    [MenuItem("Window/Spawner Manager")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(SpawnerManager));
    }

    private void Refresh()
    {
        spawners = new List<Spawner>(FindObjectsOfType<Spawner>());

        var folderPath = "Assets/EnemyPrefabs";

        var prefabGUIDs = AssetDatabase.FindAssets("t:GameObject", new[] { folderPath });

        foreach (var prefabGUID in prefabGUIDs)
        {
            var prefabPath = AssetDatabase.GUIDToAssetPath(prefabGUID);
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (prefab != null && prefab.CompareTag("Enemy") && !enemyPrefabs.Contains(prefab))
            {
                enemyPrefabs.Add(prefab);
            }
        }
    }

    private void OnGUI()
    {

        if (GUILayout.Button("Refresh"))
        {
            Refresh();
        }

        GUILayout.Label("Spawners", EditorStyles.boldLabel);

        EditorGUILayout.Space();

        for (int i = 0; i < spawners.Count; i++)
        {
            Spawner spawner = spawners[i];

            EditorGUILayout.BeginHorizontal();

            bool foldoutOpen = EditorGUILayout.Foldout(i == selectedSpawnerIndex, spawner.name);
            if (foldoutOpen && i != selectedSpawnerIndex)
            {
                selectedSpawnerIndex = i;
            }
            else if (!foldoutOpen && i == selectedSpawnerIndex)
            {
                selectedSpawnerIndex = -1;
            }

            if (GUILayout.Button("Edit"))
            {
                Selection.activeGameObject = spawner.gameObject;
            }

            if (GUILayout.Button("Remove"))
            {
                if (selectedSpawnerIndex == i)
                {
                    selectedSpawnerIndex = -1;
                }

                spawners.RemoveAt(i);
                i--;
            }

            EditorGUILayout.EndHorizontal();

            if (i == selectedSpawnerIndex)
            {
                EditorGUI.indentLevel++;

                EditorGUI.BeginChangeCheck();
                spawner.numberToSpawn = EditorGUILayout.IntField("Number to Spawn", spawner.numberToSpawn);
                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(spawner);
                }

                EditorGUI.BeginChangeCheck();
                spawner.spawnCooldown = EditorGUILayout.IntField("Spawn Cooldown", spawner.spawnCooldown);
                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(spawner);
                }

                EditorGUI.indentLevel--;
            }
        }

        if (GUILayout.Button("Add Spawner"))
        {
            GameObject newSpawnerObj = new GameObject("Spawner");
            newSpawnerObj.AddComponent<Spawner>();
            spawners.Add(newSpawnerObj.GetComponent<Spawner>());
        }

        if (GUILayout.Button("Remove Last Spawner"))
        {
            if (spawners.Count > 0)
            {
                Spawner spawnerToRemove = spawners[spawners.Count - 1];
                spawners.Remove(spawnerToRemove);
                DestroyImmediate(spawnerToRemove.gameObject);
            }
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Enemy Prefabs", EditorStyles.boldLabel);

        for (int i = 0; i < enemyPrefabs.Count; i++)
        {
            GUILayout.BeginHorizontal();

            EditorGUILayout.ObjectField(enemyPrefabs[i], typeof(GameObject), false);

            if (GUILayout.Button("Edit Enemy", GUILayout.Width(100)))
            {
                var enemyPrefab = enemyPrefabs[i];
                Selection.activeGameObject = enemyPrefab.gameObject;
                PrefabStageUtility.OpenPrefab("Assets/EnemyPrefabs/" +enemyPrefab.name + ".prefab");
            }

            GUILayout.EndHorizontal();
        }
    }
}
#endif
