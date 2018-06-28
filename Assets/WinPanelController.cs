using UnityEngine;
using UnityEngine.UI;
using Scripts.Variables;

public class WinPanelController : MonoBehaviour {
    public IntVariable PlayTime;
    public Text WinTime;
    public Text BestTime;

    public void UpdatePanel()
    {
        UpdateTime();
    }

    void UpdateTime()
    {
        WinTime.text = GetFormattedTime(PlayTime.Value);
        BestTime.text = GetFormattedTime(PlayerPrefs.GetInt("bestTime", 500));
    }

    string GetFormattedTime(int whole)
    {
        int minutes = whole / 60;
        int seconds = whole % 60;
        string time = minutes.ToString("D2") + ":" + seconds.ToString("D2");
        return time;
    }
}
