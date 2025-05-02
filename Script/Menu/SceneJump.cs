using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneJump : MonoBehaviour
{
    public void sceneload(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
