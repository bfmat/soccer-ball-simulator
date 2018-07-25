using UnityEngine;

public class OptionsMenu : MonoBehaviour {

    [SerializeField]
    private GameObject previousPanel;
    [SerializeField]
    private GameObject[] buttonPanels;

    void OnEnable()
    {
        if (previousPanel != null)
            previousPanel.SetActive(false);
    }

    public void OpenButtonPanel(int index)
    {
        buttonPanels[index].SetActive(true);
        gameObject.SetActive(false);
    }

#if !MOBILE_INPUT
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (previousPanel != null)
            {
                foreach (Transform t in previousPanel.transform)
                {
                    t.gameObject.SetActive(true);
                }
                previousPanel.SetActive(true);
            }
            gameObject.SetActive(false);
        }
    }
#endif
}
