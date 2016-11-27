using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIManagerScript : MonoBehaviour {
    public Button exitButton;

	// Use this for initialization
	void Start () {
        exitButton.onClick.AddListener(exit);
	}
	
	// Update is called once per frame
	void Update () {

    }

    void exit()
    {
        Application.Quit();
    }
}
