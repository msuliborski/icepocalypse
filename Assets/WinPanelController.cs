using UnityEngine;
using UnityEngine.UI;
using Scripts.Variables;

public class WinPanelController : MonoBehaviour {
    public IntVariable PlayTime;
    public Text WinTimeText;

    public void UpdatePanel()
    {
        UpdateTime();
    }

    void UpdateTime()
    {
        int minutes = PlayTime.Value / 60;
        int seconds = PlayTime.Value % 60;
        string time = minutes.ToString("D2") + ":" + seconds.ToString("D2");
        WinTimeText.text = time;
    }
}
