using UnityEngine.SceneManagement;
using UnityEngine;

public class LoadScene : MonoBehaviour
{
    public void LoadLevel(string name)
    {
        SceneManager.LoadScene(name);
    }
}
