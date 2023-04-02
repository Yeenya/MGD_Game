using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainManager : MonoBehaviour
{
    private static MainManager _instance;

    public List<GameObject> buildings;
    public List<GameObject> enemies;
    public List<GameObject> allies;

    public float soundVolume = 0.5f;
    public float musicVolume = 0.5f;

    public AudioClip[] clips;

    private AudioSource music;

    private void Awake()
    {
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

    IEnumerator LateBuildingsLoad()
    {
        yield return new WaitForSeconds(3);
        buildings = new List<GameObject>(GameObject.FindGameObjectsWithTag("Building"));
    }
}
