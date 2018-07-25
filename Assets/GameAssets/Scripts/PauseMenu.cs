using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
	private float m_TimeScaleRef = 1f;
    private float m_VolumeRef = 1f;
    private bool m_Paused = true;
    [SerializeField]
    private GameObject mainPanel;

    private void Start()
    {
        OnMenuStatusChange();
    }

    private void MenuOn ()
    {
        m_TimeScaleRef = Time.timeScale;
        Time.timeScale = 0f;

        m_VolumeRef = AudioListener.volume;
        AudioListener.volume = 0f;

        m_Paused = true;
        Cursor.visible = true;
    }


    public void MenuOff ()
    {
        Time.timeScale = m_TimeScaleRef;
        AudioListener.volume = m_VolumeRef;

        m_Paused = false;
        Cursor.visible = false;
    }


    public void OnMenuStatusChange ()
    {
        if (!m_Paused)
        {
            MenuOn();
        }
        else if (m_Paused)
        {
            MenuOff();
        }
        foreach (Transform t in transform)
        {
            if (t != transform)
            t.gameObject.SetActive(m_Paused);
        }
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

#if !MOBILE_INPUT
    void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
            OnMenuStatusChange();
        }
	}
#endif
}
