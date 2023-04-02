using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

#if UNITY_EDITOR
[CustomEditor(typeof(Spawner))]
public class SpawnerEditor : Editor
{
    private SerializedProperty enemiesProperty;
    private SerializedProperty numberToSpawnProperty;
    private SerializedProperty spawnCooldownProperty;
    private SerializedProperty wavesProperty;

    private List<Spawner> selectedSpawners = new List<Spawner>();

    private void OnEnable()
    {
        enemiesProperty = serializedObject.FindProperty("enemies");
        numberToSpawnProperty = serializedObject.FindProperty("numberToSpawn");
        spawnCooldownProperty = serializedObject.FindProperty("spawnCooldown");
        wavesProperty = serializedObject.FindProperty("waves");
    }

    private void OnDisable()
    {
        selectedSpawners.Clear();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(enemiesProperty, true);

        if (GUILayout.Button("Add Enemy"))
        {
            GameObject newEnemy = new GameObject();
            newEnemy.name = "New Enemy";
            newEnemy.AddComponent<Enemy>();
            enemiesProperty.InsertArrayElementAtIndex(enemiesProperty.arraySize);
            enemiesProperty.GetArrayElementAtIndex(enemiesProperty.arraySize - 1).objectReferenceValue = newEnemy;
        }

        if (GUILayout.Button("Remove Enemy") && enemiesProperty.arraySize > 0)
        {
            enemiesProperty.DeleteArrayElementAtIndex(enemiesProperty.arraySize - 1);
        }

        EditorGUILayout.PropertyField(numberToSpawnProperty);
        EditorGUILayout.PropertyField(spawnCooldownProperty);
        EditorGUILayout.PropertyField(wavesProperty);

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
