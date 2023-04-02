using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainManager : MonoBehaviour
{
    // Manager for the whole general game

    private static MainManager _instance;

    [Tooltip("All buildings in the scene, added automatically at Start")]
    public List<GameObject> buildings;
    [Tooltip("All enemies in the scene, updated at every Update")]
    public List<GameObject> enemies;
    [Tooltip("All allies in the scene, updated at every Update")]
    public List<GameObject> allies;

    [Tooltip("Volume of enemies, fireball etc.")]
    public float soundVolume = 0.5f;
    public float musicVolume = 0.5f;

    public AudioClip[] clips;

    private AudioSource music;

    private void Awake()
    {
        // Singleton principle
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        buildings = new List<GameObject>(GameObject.FindGameObjectsWithTag("Building"));
        music = GetComponent<AudioSource>();
        music.clip = clips[0];
        music.Play();
    }

    // Update is called once per frame
    void Update()
    {
        enemies = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));
        foreach(GameObject enemy in enemies)
        {
            if (enemy.GetComponent<AudioSource>().volume != soundVolume)
            {
                enemy.GetComponent<AudioSource>().volume = soundVolume;
            }
        }
        allies = new List<GameObject>(GameObject.FindGameObjectsWithTag("Ally"));
        if (music.volume != musicVolume)
        {
            music.volume = musicVolume;
        }
            
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);

        // Play music according to the current scene
        if (sceneName == "MainMenu")
        {
            PlayClip(0);
        }
        else
        {
            PlayClip(1);
            StartCoroutine(LateBuildingsLoad());
        }
    }

    private void PlayClip(int index)
    {
        music.clip = clips[index];
        music.Play();
    }

    IEnumerator LateBuildingsLoad() // Sometimes the buildings didn't load properly if searched for them instantly at Start, so I added a coroutine
    {
        yield return new WaitForSeconds(3);
        buildings = new List<GameObject>(GameObject.FindGameObjectsWithTag("Building"));
    }
}
