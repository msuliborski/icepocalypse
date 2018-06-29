using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Scripts.Variables;

public class TimerController : MonoBehaviour {

    // Use this for initialization
    int _secondsElapsed;
    Text _text;
    public IntVariable PlayTime;

    private void Start()
    {
        _text = GetComponent<Text>();
    }
    public void OnGameStarted()
    {
        StartCoroutine(Timer());
    }

    public void OnPlayerDeath()
    {
        StopAllCoroutines();
    }

    public void OnPlayerWin()
    {
        Debug.Log("no i tera to jo");
        StopAllCoroutines();
        PlayTime.Value = _secondsElapsed;

    }

    IEnumerator Timer()
    {
        _secondsElapsed = 0;
        while (true)
        {
            yield return new WaitForSeconds(1.0f);
            _secondsElapsed += 1;
            UpdateTime();
        }
    }

    void UpdateTime()
    {
        int minutes = _secondsElapsed / 60;
        int seconds = _secondsElapsed % 60;
        string time = minutes.ToString("D2") + ":" + seconds.ToString("D2");
        _text.text = time;
    }
}
