using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlaceObstacles : MonoBehaviour
{
    public GameObject box;
    public Sprite box1;
    public Sprite box2;
    public Sprite box3;
    public Sprite box4;

    public void ScrollUp(int index, Transform[] layers)
    {

        Vector3 position0 = layers[index].position;
        Vector3 position1 = new Vector3(position0.x - 1.5f, position0.y - 1.2f, position0.z - 1);
        Vector3 position2 = new Vector3(position0.x - 1.5f, position0.y - 0f, position0.z - 1);
        Vector3 position3 = new Vector3(position0.x - 1.5f, position0.y + 1.2f, position0.z - 1);

        Vector3[] positions = { position1, position2, position3 };
        int numberOfBoxes = Random.Range(1, 3);


        foreach (Vector3 position in positions)
        {
            spawnBoxes(position, numberOfBoxes);
        }


    }

    private void spawnBoxes(Vector3 position, int numberOfBoxes)
    {

        Sprite[] sprites = { box1, box2, box3, box4 };
        for (int i = 0; i < numberOfBoxes; i++)
        {
            position.x = position.x * Random.Range(-1, 1);
            box.GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0, 3)];
            Instantiate(box, position, Quaternion.identity);
        }
    }



}
