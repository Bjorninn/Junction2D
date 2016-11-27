using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameOverScreenHandler : MonoBehaviour {

    public Button restartButton;
    public Button exitButton;

	// Use this for initialization
	void Start () {
        restartButton.onClick.AddListener(restart);
        exitButton.onClick.AddListener(restart);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void restart()
    {
        SceneManager.LoadScene("Level1");
    }

    void exit()
    {
        Application.Quit();
    }
}
