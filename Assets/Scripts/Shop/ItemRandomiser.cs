using TMPro;
using UnityEngine;

public class ItemRandomiser : MonoBehaviour
{
    // all possible shop items
    public GameObject[] items;

    // shopkeeper text
    public TextMeshProUGUI keeperText;
    public bool boughtSomething = false;

    void Start()
    {
        Vector3 pos = items[0].gameObject.transform.position + transform.position;
        Quaternion rot = items[0].gameObject.transform.rotation;

        // pick 3 random shop items, and put them in the shop
        shuffle(items);

        Vector3 item1pos = new(pos.x - 4, pos.y, pos.z);
        Instantiate(items[0], item1pos, rot, this.transform);

        Vector3 item2pos = new(pos.x, pos.y, pos.z);
        Instantiate(items[1], item2pos, rot, this.transform);

        Vector3 item3pos = new(pos.x + 4, pos.y, pos.z);
        Instantiate(items[2], item3pos, rot, this.transform);
    }

    void shuffle(GameObject[] array)
    {
        // Fisher-Yates shuffle - Wikipedia
        for (int i = 0; i < array.Length; i++)
        {
            GameObject temp = array[i];
            int rand = Random.Range(i, array.Length);
            array[i] = array[rand];
            array[rand] = temp;
        }
    }
}
