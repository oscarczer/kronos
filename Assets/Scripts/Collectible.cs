using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    public float timeGained = 20f;

    //  bossfight  things
    public Vector2 thrust = Vector2.zero;
    public bool destroyOffScreen = false;

    public GameObject deathPopup;

    //  Update  is  called  once  per  frame
    void Update()
    {
        //  SPIN  FOR  ME  BOY  SPIN  FOR  ME
        transform.Rotate(new Vector3(0, 0, -90 * Time.deltaTime));

        //  boss  fight  1,  the  collectible  moves
        transform.position = new Vector3(
            transform.position.x + (thrust.x * Time.deltaTime * 5),
            transform.position.y + (thrust.y * Time.deltaTime * 5),
            transform.position.z
        );
        if (destroyOffScreen)
        {
            if (Mathf.Abs(transform.position.x) > 25 || Mathf.Abs(transform.position.y) > 10)
            {
                Destroy(gameObject);
            }
        }
    }

    //  Player  collects  this
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            //  give  player  some  bonus  time
            collision.gameObject.GetComponent<PlayerController>().AlterTime(timeGained);
            //  Add  particle  effects  that  play  when  you  collect  thing  here
            //  Remove
            Destroy(gameObject);
        }
    }
}
