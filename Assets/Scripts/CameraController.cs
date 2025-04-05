using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float speed;
    private float currentPosX;
    private float currentPosY;
    private Vector3 velocity = Vector3.zero;
    private Transform playerTransform;
    public GameObject blackBars;

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        currentPosX = transform.position.x;
        currentPosY = transform.position.y;
        blackBars.SetActive(true);
    }

    private void Update()
    {
        transform.position = Vector3.SmoothDamp(
            transform.position,
            new Vector3(currentPosX, currentPosY, transform.position.z),
            ref velocity,
            speed
        );

        // When player goes out of camera view, start a screen transition
        if (playerTransform.transform.position.x > currentPosX + 13f)
        {
            // move screen right
            MoveToNewRoom(new Vector2(currentPosX + 26, currentPosY));
        }
        if (playerTransform.transform.position.x < currentPosX - 13f)
        {
            // move screen left
            MoveToNewRoom(new Vector2(currentPosX - 26, currentPosY));
        }
        if (playerTransform.transform.position.y > currentPosY + 5.5f)
        {
            // move screen up
            MoveToNewRoom(new Vector2(currentPosX, currentPosY + 11));
        }
        if (playerTransform.transform.position.y < currentPosY - 5.5f)
        {
            // move screen down
            MoveToNewRoom(new Vector2(currentPosX, currentPosY - 11));
        }
    }

    private void MoveToNewRoom(Vector2 newRoom)
    {
        currentPosX = newRoom.x;
        currentPosY = newRoom.y;
    }
}
