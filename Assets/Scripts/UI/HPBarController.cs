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
        bar.color = new Color(255, 0, 0, 255);
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

    public void OnPlayerHealed()
    {
        _lastBar += 1;
        if (_lastBar >= _bars.Count)
            return;

        StartCoroutine(ShowBar(_bars[_lastBar]));
    }

    IEnumerator ShowBar(Image bar)
    {
        if (bar.enabled == true)
        {
            yield return null;
        }
        bar.enabled = true;
        bar.color = new Color(0, 255, 0, 0);
        for (float t = 0.5f; t >= 0f; t -= Time.unscaledDeltaTime)
        {            
            bar.color = new Color(bar.color.r, bar.color.g, bar.color.b, (1.0f - (t / 0.5f)));
            yield return null;
        }
        bar.color = new Color(255, 255, 255, 255);

        yield return null;
    }

    public void OnGameStarted()
    {
        StopAllCoroutines();
        ResetBars();
    }
}
