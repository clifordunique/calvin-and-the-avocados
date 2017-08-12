using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class Menu : MonoBehaviour {

    public string scene;
    public Button start;
    public Button quit;

	List<Button> buttons;
    int current;
    bool checkAxes = false;

    // Use this for initialization
    void Start () {

        buttons = new List<Button>();

        Time.timeScale = 1f;

        // restart button
        start = start.GetComponent<Button>();
        start.onClick.AddListener(StartManager);
        buttons.Add(start);

        // quit button
        quit = quit.GetComponent<Button>();
        quit.onClick.AddListener(QuitManager);
        buttons.Add(quit);

        current = -1;
	}
	
	// Update is called once per frame
	void Update () {
        InputMap();
	}

    /// <summary>
    /// Input when in pause menu mode
    /// </summary>
    private void InputMap ()
    {

        if (Input.GetAxisRaw ("Vertical") == 1 && checkAxes)
        {
            Debug.Log("input up " + current);
            current = (current <= 0) ? 0 : --current;
            buttons[current].Select();
            checkAxes = false;
        }

        if (Input.GetAxisRaw ("Vertical") == -1 && checkAxes)
        {
            Debug.Log("input down before " + current);
            int count = buttons.Count - 1;
            current = (current == count) ? count : ++current;
            buttons[current].Select();
            Debug.Log("input down after " + current);
            checkAxes = false;
        }

        if (Input.GetAxisRaw ("Vertical") == 0 && !checkAxes)
        {
            checkAxes = true;
        }

    }

    /// <summary>
    /// Load level one
    /// </summary>
    private void StartManager()
    {
		SceneManager.LoadScene (scene);
    }

    /// <summary>
    /// Quit application
    /// </summary>
    private void QuitManager()
    {
        Application.Quit();
    }
}
