using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Scripts.Variables;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Scripts.Variables;

public class UIController : MonoBehaviour
{
    public GameObject Player;

    public GameObject DeathPanel;
    public GameObject GameTopPanel;
    public GameObject GameControlsPanel;
    public GameObject StartPanel;
    public GameObject WinPanel;

    public Transform PlayerStartPosition;

    public GameEvent GameStartedEvent;
    public IntVariable PlayTime;



    private Vector2 _startPosition;
    private bool _touching = false;
    private PlayerControllerExperimental _playerControllerExperimental;
    private FightSystem _fightSystem;

    bool _rightArrow = false;
    bool _leftArrow = false;

    public void Start()
    {
        _playerControllerExperimental = Player.GetComponent<PlayerControllerExperimental>();
        _fightSystem = Player.GetComponent<FightSystem>();
    }

    public void OnLeftExit()
    {
        _leftArrow = false;
        _playerControllerExperimental.OnLeftDirectionEnd();
        if (!_rightArrow) 
        {
            _playerControllerExperimental.StopMovement();
        }
    }

    public void OnRightExit()
    {
        _rightArrow = false;
        _playerControllerExperimental.OnRightDirectionEnd();
        if (!_leftArrow) 
        {
            _playerControllerExperimental.StopMovement();
        }
    }

	public void OnLeft()
	{
        _leftArrow = true;
        _rightArrow = false;
		_playerControllerExperimental.OnLeftDirection();
	}

	public void OnRight()
	{
        _rightArrow = true;
        _leftArrow = false;
		_playerControllerExperimental.OnRightDirection();
	}

    public void OnTop() {
        _playerControllerExperimental.OnTopDirection();

    }

    public void OnDown() 
    {
        _playerControllerExperimental.OnDownDirection();
    }

    public void OnAttack()
    {
        _playerControllerExperimental.StopMovement();
        _fightSystem.ClickedAttack = true;
    }


    public void OnGameRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //StartCoroutine(GameRestart());
    }

    public void OnGameStart()
    {
        StartCoroutine(GameStart());
    }

    public void OnPlayerDeath()
    {
        StartCoroutine(PlayerDeath());

    }

    public void OnPlayerWin()
    {
        // order matters
        int currTime = PlayTime.Value;
        WinPanel.GetComponent<WinPanelController>().UpdatePanel();
        if (currTime < PlayerPrefs.GetInt("bestTime", 500))
        {
            PlayerPrefs.SetInt("bestTime", currTime);
        }
        StartCoroutine(PlayerWin());

    }

    IEnumerator PlayerWin()
    {
        CanvasGroup winCanvasGroup = WinPanel.GetComponent<CanvasGroup>();
        CanvasGroup gameTopCanvasGroup = GameTopPanel.GetComponent<CanvasGroup>();
        CanvasGroup gameControlsCanvasGroup = GameControlsPanel.GetComponent<CanvasGroup>();

        WinPanel.SetActive(true);
        winCanvasGroup.alpha = 0;
        StartCoroutine(FadeInCanvas(winCanvasGroup, 0.5f));
        StartCoroutine(FadeOutCanvas(gameTopCanvasGroup, 0.3f));
        StartCoroutine(FadeOutCanvas(gameControlsCanvasGroup, 0.3f));

        yield return new WaitForSeconds(0.5f);
        gameTopCanvasGroup.alpha = 0;
        gameControlsCanvasGroup.alpha = 0;
        yield return null;
    }

    IEnumerator PlayerDeath()
    {
        CanvasGroup deathCanvasGroup = DeathPanel.GetComponent<CanvasGroup>();
        CanvasGroup gameTopCanvasGroup = GameTopPanel.GetComponent<CanvasGroup>();
        CanvasGroup gameControlsCanvasGroup = GameControlsPanel.GetComponent<CanvasGroup>();

        DeathPanel.SetActive(true);
        deathCanvasGroup.alpha = 0;
        StartCoroutine(FadeInCanvas(deathCanvasGroup, 0.5f));
        StartCoroutine(FadeOutCanvas(gameTopCanvasGroup, 0.3f));
        StartCoroutine(FadeOutCanvas(gameControlsCanvasGroup, 0.3f));

        yield return new WaitForSeconds(0.5f);
        gameTopCanvasGroup.alpha = 0;
        gameControlsCanvasGroup.alpha = 0;
        yield return null;
    }

    IEnumerator GameStart()
    {
        CanvasGroup startCanvasGroup = StartPanel.GetComponent<CanvasGroup>();
        CanvasGroup gameTopCanvasGroup = GameTopPanel.GetComponent<CanvasGroup>();
        CanvasGroup gameControlsCanvasGroup = GameControlsPanel.GetComponent<CanvasGroup>();


        // 0.5f -> first 0.2 only  fading out StartPanel
        //      -> then 0.3 fade out DeathPanel + fade in Controls and HP Bar
        StartPanel.SetActive(true);
        StartCoroutine(FadeOutCanvas(startCanvasGroup, 0.5f));
        yield return new WaitForSeconds(0.2f);

        GameTopPanel.SetActive(true);
        GameControlsPanel.SetActive(true);
        gameTopCanvasGroup.alpha = 0;
        gameControlsCanvasGroup.alpha = 0;

        StartCoroutine(FadeInCanvas(gameTopCanvasGroup, 0.3f));
        StartCoroutine(FadeInCanvas(gameControlsCanvasGroup, 0.3f));
        yield return new WaitForSeconds(0.3f);

        StartPanel.SetActive(false);

        GameStartedEvent.Raise();

        yield return null;
    }


    IEnumerator GameRestart()
    {
        CanvasGroup deathCanvasGroup = DeathPanel.GetComponent<CanvasGroup>();
        CanvasGroup gameTopCanvasGroup = GameTopPanel.GetComponent<CanvasGroup>();
        CanvasGroup gameControlsCanvasGroup = GameControlsPanel.GetComponent<CanvasGroup>();


        // 0.5f -> first 0.2 only  fading out DeathScreen
        //      -> then 0.3 fade out DeathScren + fade in Controls and HP Bar
        DeathPanel.SetActive(true);
        StartCoroutine(FadeOutCanvas(deathCanvasGroup, 0.5f));
        yield return new WaitForSeconds(0.2f);

        GameTopPanel.SetActive(true);
        GameControlsPanel.SetActive(true);
        gameTopCanvasGroup.alpha = 0;
        gameControlsCanvasGroup.alpha = 0;

        StartCoroutine(FadeInCanvas(gameTopCanvasGroup, 0.3f));
        StartCoroutine(FadeInCanvas(gameControlsCanvasGroup, 0.3f));
        yield return new WaitForSeconds(0.3f);

        DeathPanel.SetActive(false);

        GameStartedEvent.Raise(); 

        yield return null;
    }

    IEnumerator FadeOutCanvas(CanvasGroup canvasGroup, float fadeTime)
    {
        for (float time = fadeTime; time > 0; time -= Time.unscaledDeltaTime)
        {
            canvasGroup.alpha = (time / fadeTime);
            yield return null;
        }
        yield return null;
    }

    IEnumerator FadeInCanvas(CanvasGroup canvasGroup, float fadeTime)
    {
        for (float time = fadeTime; time > 0; time -= Time.unscaledDeltaTime)
        {
            canvasGroup.alpha = 1.0f - (time / fadeTime);
            yield return null;
        }
        yield return null;
    }

}
