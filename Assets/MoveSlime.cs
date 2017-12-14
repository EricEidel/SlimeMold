using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSlime : MonoBehaviour
{

    public float min_x_speed = 0.5f;
    public float min_y_speed = 0.5f;
    public float min_z_speed = 0.5f;
    public float forward_force = 20;

    public float time_to_move = 5;
    public float time_to_spawn_phermone = 2;

    public GameObject SlimePrefab;
    public GameObject PhermonePrefabe;

    public bool alive = true;
    public bool hover = false;
    public bool move = true;

    public bool follow = false;
    public GameObject target;

    Rigidbody rig_bod;

	// Use this for initialization
	void Start ()
    {
        rig_bod = GetComponent<Rigidbody>();

        transform.rotation = Quaternion.Euler(Random.Range(0.0f, 360.0f), Random.Range(0.0f, 360.0f), Random.Range(0.0f, 360.0f));

        StartCoroutine(move_forward());
        StartCoroutine(drop_phermone());
    }
	
    IEnumerator move_forward()
    {
        while (alive)
        {
            if (rig_bod.velocity.x < min_x_speed && rig_bod.velocity.y < min_y_speed && rig_bod.velocity.z < min_z_speed && move)
            {
                transform.rotation = Quaternion.Euler(Random.Range(0.0f, 360.0f), Random.Range(0.0f, 360.0f), Random.Range(0.0f, 360.0f));
                rig_bod.AddForce(-transform.forward * forward_force);
            }

            if (hover)
            {
                transform.rotation = Quaternion.Euler(Random.Range(0.0f, 360.0f), Random.Range(0.0f, 360.0f), Random.Range(0.0f, 360.0f));
                rig_bod.AddForce(transform.forward * forward_force/20);
            }

            if (follow && target != null)
            {
                // Push slightly towards other
                var heading = transform.position - target.transform.position;

                var distance = heading.magnitude;
                var direction = heading / distance; // This is now the normalized direction.

                rig_bod.AddForce(-direction * forward_force );
            }

            yield return new WaitForSeconds(time_to_move);
        }
    }

    GameObject prev;
    IEnumerator drop_phermone()
    {
        while (alive)
        {
            if (move)
            {
                var go = Instantiate(PhermonePrefabe, this.transform.position, this.transform.rotation);

                var this_phermone = go.GetComponent<PhermoneBehaviour>();
                this_phermone.spawner = gameObject;
                this_phermone.spawner_id = GetInstanceID();

                if (prev != null)
                {
                    var prev_phermone = prev.GetComponent<PhermoneBehaviour>();
                    prev_phermone.next_one = go;
                }
                prev = go;
            }

            yield return new WaitForSeconds(time_to_spawn_phermone);
        }
    }

    bool found_slime = false;
    HashSet<int> already_seen = new HashSet<int>();

    void OnTriggerEnter(Collider other)
    {
        var phermone = other.GetComponent<PhermoneBehaviour>();
        var slime = other.GetComponent<MoveSlime>();
 
        if (slime != null)
        {
            // Colidded with a another slime
            Debug.Log("Collision between: " + other.gameObject.GetInstanceID() + " " + GetInstanceID());

            found_slime = true;

            // Stop slime
            rig_bod.velocity = Vector3.zero;
            rig_bod.angularVelocity = Vector3.zero;

            if (move)
            {
                move = false;
                follow = true;

                target = other.gameObject;

                StartCoroutine(delay_to_hover());
            }
        }
        
        // Phermone collided
        if (phermone != null && !found_slime)
        {
            // Ensure the phermone was emitted by someone else and we haven't seen it already
            if (phermone.spawner_id != GetInstanceID() && !already_seen.Contains(phermone.GetInstanceID()))
            {
                // Found a Phermone of another object
                Debug.Log(other.name + " " + phermone.spawner_id + " " + GetInstanceID());

                // Stop slime

                //rig_bod.velocity = Vector3.zero;
                //rig_bod.angularVelocity = Vector3.zero;

                //if (move)
                //{
                //    move = false;
                //    rig_bod.MovePosition(phermone.next_one.transform.position);

                //    // Mark as already seen
                //    already_seen.Add(phermone.gameObject.GetInstanceID());
                //}
            }
        }
    }

    public IEnumerator delay_to_hover()
    {
        yield return new WaitForSeconds(5);

        hover = true;
        follow = false;
        target = null;
    }
}
