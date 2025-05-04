using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    private GameObject player;

    void Start()
    {
        player = GameObject.Find("Player");
    }

    void Update()
    {
        if (player.transform.position.y < -10f)
        {
            SceneManager.LoadScene("Level1", LoadSceneMode.Single);
        }
    }
}
