using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button exitButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button newGame;
    // Start is called before the first frame update
    void Start()
    {
        continueButton.gameObject.SetActive(PlayerPrefs.HasKey("Save"));
        continueButton.onClick.AddListener(() => SceneManager.LoadScene(1));
        exitButton.onClick.AddListener(Application.Quit);
        newGame.onClick.AddListener(() =>
        {
            PlayerPrefs.DeleteKey("Save");
            SceneManager.LoadScene(1);
        });
    }

}
