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
    public float time_to_life_decision = 15;
    public float time_to_be_new = 10;

    public GameObject SlimePrefab;
    public GameObject PhermonePrefabe;

    public bool alive = true;
    public bool move = true;
    public bool new_slime = false;

    Rigidbody rig_bod;
    
    static int num_slimes = 0;

	// Use this for initialization
	void Awake ()
    {
        MoveSlime.num_slimes++;

        
        alive = true;
        move = true;
        new_slime = true;

        rig_bod = GetComponent<Rigidbody>();

        transform.rotation = Quaternion.Euler(Random.Range(0.0f, 360.0f), Random.Range(0.0f, 360.0f), Random.Range(0.0f, 360.0f));

        StartCoroutine(move_forward());
        StartCoroutine(drop_phermone());
        StartCoroutine(life_decision());
        StartCoroutine(no_longer_new());
    }

    IEnumerator no_longer_new()
    {
        yield return new WaitForSeconds(time_to_be_new);

        if (new_slime)
        {
            new_slime = !new_slime;
        }
    }

    float CHILD_CHANCE = 10.0f;
    float DEATH_CHANCE = 5.0f;
    IEnumerator life_decision()
    {
        yield return new WaitForSeconds(time_to_life_decision);

        while (alive)
        {
            if (!new_slime)
            {
                if (Random.Range(0, 100) < CHILD_CHANCE)
                {
                    // Throw a dice
                    // If bigger then CHILD_CHANCE make a new one
                    get_spawn_point_child();
                }
                else if (Random.Range(0, 100) < DEATH_CHANCE && !new_slime && MoveSlime.num_slimes > 2)
                {
                    // Throw a dice
                    // If bigger then DEATH_CHANCE kill this one
                    alive = false;
                    Destroy(this.gameObject);
                    MoveSlime.num_slimes--;
                }
            }

            yield return new WaitForSeconds(time_to_life_decision);
        }
    }

    IEnumerator move_towards(GameObject target_to_move_to, float move_time, float min_distance)
    {
        float sqrRemainingDist = (transform.position - target_to_move_to.transform.position).sqrMagnitude;
        float inv_move_time = 1f / move_time;

        while (sqrRemainingDist > min_distance && alive)
        {
            Vector3 new_pos = Vector3.MoveTowards(transform.position, target_to_move_to.transform.position, inv_move_time * Time.deltaTime);
            rig_bod.MovePosition(new_pos);
            sqrRemainingDist = (transform.position - target_to_move_to.transform.position).sqrMagnitude;

            yield return null;
        }
    }

    IEnumerator follow_path_phermones(PhermoneBehaviour phermone)
    {
        while (phermone.next_one != null && !found_slime)
        {
            yield return move_towards(phermone.next_one.gameObject, 0.75f, 0.1f);

            phermone = phermone.next_one.GetComponent<PhermoneBehaviour>();
        }
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
                this_phermone.spawner_id = gameObject.GetInstanceID();

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
    bool following_phermone = false;

    void OnTriggerEnter(Collider other)
    {
        var phermone = other.GetComponent<PhermoneBehaviour>();
        var slime = other.GetComponent<MoveSlime>();

        if (other.tag.Equals("Food"))
        {
            move = false;

            StartCoroutine(move_towards(other.gameObject, 2f, 0.8f));
        }

        if (slime != null)
        {
            // Only start following / collide if not a newly spawned slime
            if (!new_slime && !slime.new_slime)
            {
                // Colidded with a another slime
                Debug.Log("Collision between: " + other.gameObject.GetInstanceID() + " " + gameObject.GetInstanceID());

                found_slime = true;

                // Stop slime
                rig_bod.velocity = Vector3.zero;
                rig_bod.angularVelocity = Vector3.zero;

                if (move)
                {
                    move = false;

                    StartCoroutine(move_towards(other.gameObject, 3f, 1.2f));
                }
            }
        }
        
        // Phermone collided
        if (phermone != null && !found_slime && !following_phermone)
        {
            // Ensure the phermone was emitted by someone else and we haven't seen it already
            if (phermone.spawner_id != gameObject.GetInstanceID())
            {
                following_phermone = true;

                // Found a Phermone of another object
                Debug.Log(other.name + " " + phermone.spawner_id + " " + gameObject.GetInstanceID());

                // Stop slime
                rig_bod.velocity = Vector3.zero;
                rig_bod.angularVelocity = Vector3.zero;

                StartCoroutine(follow_path_phermones(phermone));
                
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

    public void get_spawn_point_child()
    {
        Vector3 spawn_point = Random.insideUnitSphere * 5 + transform.position;

        // TODO: Need to make sure this point is inside the box I want.

        var new_slime = Instantiate(SlimePrefab);
        new_slime.transform.position = spawn_point;
    }
}
