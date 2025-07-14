using UnityEngine;
using UnityEngine.SceneManagement;

public class EndPanel : MonoBehaviour
{
    public void ToMainMenu()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("MainMenu");
    }
}
