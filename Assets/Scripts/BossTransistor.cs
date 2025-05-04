using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossTransistor : MonoBehaviour
{
    // Update player stats
    private GameObject playerObj;
    private PlayerController player;
    public string newScene;
    public string oldScene;

    void Start()
    {
        playerObj = GameObject.FindGameObjectWithTag("Player");
        player = playerObj.GetComponent<PlayerController>();
    }

    void Update()
    {
        // Purchase items
        if (Input.GetKeyDown(KeyCode.Return))
        {
            // Confirm that the text is visible
            if (transform.GetChild(0).gameObject.activeSelf)
            {
                player.SetCurrentDataFields();
                StartCoroutine(CreateScene());
            }
        }
    }

    public IEnumerator CreateScene()
    {
        if (oldScene.StartsWith("Level"))
        {
            var previousScene = SceneManager.GetSceneByName(oldScene);
            if (previousScene.IsValid())
            {
                yield return SceneManager.UnloadSceneAsync(previousScene);
            }

            SceneManager.LoadScene(newScene, LoadSceneMode.Additive);
            var scene = SceneManager.GetSceneByName(newScene);
            while (!scene.isLoaded)
            {
                yield return new WaitForSeconds(0.1f);
            }

            SceneManager.MoveGameObjectToScene(playerObj.gameObject, scene);

            SceneManager.SetActiveScene(SceneManager.GetSceneByName(newScene));
            playerObj.transform.position = new Vector2(0, -7.5f);
            SceneManager.UnloadSceneAsync(oldScene);
        }
        else
        {
            // Just finished a bossfight, therefore going to a new level
            SceneManager.LoadScene(newScene, LoadSceneMode.Single);
        }
    }

    // Handle boss things
    private void OnTriggerStay2D(Collider2D collision)
    {
        string objname = collision.gameObject.name;
        if (objname == "Player")
        {
            // make text visible
            transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        string objname = collision.gameObject.name;
        if (objname == "Player")
        {
            // Turn text off
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}
