using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhermoneBehaviour : MonoBehaviour
{
    public float how_long_to_stay = 20;

    public GameObject next_one;
    public GameObject spawner;

    public int spawner_id;

    // Use this for initialization
    void Start ()
    {
        StartCoroutine(self_destroy());
	}

    IEnumerator self_destroy()
    {
        yield return new WaitForSeconds(how_long_to_stay);

        Destroy(gameObject);
    }
}
