using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public Button startButton;
    public Button optionsButton;
    public Button exitButton;
    public Button backButton;

    public VisualElement buttonsMenu;
    public VisualElement optionsMenu;

    public Slider soundSlider;
    public Slider musicSlider;

    private MainManager manager;

    void Start()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        startButton = root.Q<Button>("Play");
        optionsButton = root.Q<Button>("Options");
        exitButton = root.Q<Button>("Exit");
        backButton = root.Q<Button>("Back");

        buttonsMenu = root.Q<VisualElement>("ButtonsMenu");
        optionsMenu = root.Q<VisualElement>("OptionsMenu");

        soundSlider = root.Q<Slider>("SoundVolume");
        musicSlider = root.Q<Slider>("MusicVolume");
        soundSlider.RegisterValueChangedCallback(OnSoundSliderValueChanged);
        musicSlider.RegisterValueChangedCallback(OnMusicSliderValueChanged);
        print(soundSlider + " " + musicSlider);

        startButton.clicked += StartButtonPressed;
        optionsButton.clicked += OptionsButtonPressed;
        exitButton.clicked += ExitButtonPressed;
        backButton.clicked += BackButtonPressed;

        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<MainManager>();
    }

    void StartButtonPressed()
    {
        manager.ChangeScene("MainScene");
        //SceneManager.LoadScene("MainScene");
    }

    void OptionsButtonPressed()
    {
        buttonsMenu.style.display = DisplayStyle.None;
        optionsMenu.style.display = DisplayStyle.Flex;
    }

    void ExitButtonPressed()
    {
        Application.Quit();
    }

    void BackButtonPressed()
    {
        buttonsMenu.style.display = DisplayStyle.Flex;
        optionsMenu.style.display = DisplayStyle.None;
    }

    void OnSoundSliderValueChanged(ChangeEvent<float> evt)
    {
        manager.soundVolume = evt.newValue;
    }

    void OnMusicSliderValueChanged(ChangeEvent<float> evt)
    {
        manager.musicVolume = evt.newValue;
    }
}
