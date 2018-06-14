using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBarController : MonoBehaviour {
    
    public List<Image> _bars = new List<Image>();
    private int _lastBar;

    void Start()
    {
        foreach (Transform child in transform)
        {
            _bars.Add(child.gameObject.GetComponent<Image>());
        }

        _lastBar = _bars.Count - 1;
    }



    IEnumerator HideBar(Image bar)
    {
        if (bar.enabled == false)
        {
            yield return null;
        }
        bar.CrossFadeAlpha(0, 0.5f, true);
        yield return new WaitForSeconds(0.5f);
        bar.enabled = false;
        bar.color = new Color(255, 255, 255, 255);
        yield return null;
    }

    void ResetBars()
    {
        foreach (Image bar in _bars)
        {
            bar.enabled = true;
        }
        _lastBar = _bars.Count - 1;
    }

    public void OnPlayerDamaged()
    {
        StartCoroutine(HideBar(_bars[_lastBar]));
        _lastBar -= 1;
        if (_lastBar < 0)
        {
            _lastBar = _bars.Count - 1;
        }
    }

    public void OnGameStarted()
    {
        StopAllCoroutines();
        ResetBars();
    }
}
