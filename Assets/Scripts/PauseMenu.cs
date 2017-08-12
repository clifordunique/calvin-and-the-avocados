using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class PauseMenu : MonoBehaviour {

    private bool isPaused = false;
    public GameObject pauseMenu;
    public Button resume;
    public Button restart;
    public Button quit;
	Player player;

	List<Button> buttons;
    int current;
    bool checkAxes = false;

    // On awake
    private void Awake()
    {
        pauseMenu.SetActive(false);
        isPaused = false;
    }

    // Use this for initialization
    void Start () {

        player = GetComponent<Player>();
        buttons = new List<Button>();

        player.inputEnable = true;
        Time.timeScale = 1f;

        // resume button
        resume = resume.GetComponent<Button>();
        resume.onClick.AddListener(PauseManager);
        buttons.Add(resume);

        // restart button
        restart = restart.GetComponent<Button>();
        restart.onClick.AddListener(RestartManager);
        buttons.Add(restart);

        // quit button
        quit = quit.GetComponent<Button>();
        quit.onClick.AddListener(QuitManager);
        buttons.Add(quit);

        current = -1;
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetButtonDown("Pause"))
        {
            PauseManager();
        }

        if (!player.inputEnable)
        {
            InputMap();
        }
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
    /// Disable player input when we are in pause menu mode
    /// </summary>
    private void DisabledPlayerInput ()
    {
        player.inputEnable = !isPaused;
    }

    /// <summary>
    /// Pause/unpause game
    /// </summary>
    private void PauseManager()
    {
        isPaused = !isPaused;
        Time.timeScale = (isPaused) ? 0f : 1f;
        pauseMenu.SetActive(isPaused);
        DisabledPlayerInput();
    }

    /// <summary>
    /// Restart level
    /// </summary>
    private void RestartManager()
    {
		string scene = SceneManager.GetActiveScene ().name;
		SceneManager.LoadScene (scene);
    }

    /// <summary>
    /// Load main menu
    /// </summary>
    private void QuitManager()
    {
		SceneManager.LoadScene ("menu");
    }
}
