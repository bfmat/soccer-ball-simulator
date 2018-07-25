using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadProceduralGenerator : MonoBehaviour {

    [SerializeField]
    private GameObject dropDown;
    public static short difficulty = 256;

    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public void LoadProcGen()
    {
        SceneManager.LoadScene("Procedural Generator");
    }

    public void DifficultySetting()
    {
        difficulty = (short) Mathf.Pow(4, dropDown.GetComponent<Dropdown>().value + 1);
    }
}
