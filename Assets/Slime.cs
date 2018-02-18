using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : MonoBehaviour
{
    static public int max_visits_first = 2;
    static public int max_visits_middle = 3;
    static public int max_visits_last = 4;

    public int maxDepth;
    public float childScale;
    private int depth;

    public void create_parent(List<TriNode> nodes, Mesh mesh, Transform transform, GameObject slime_template)
    {
        foreach (var node in nodes)
        {
            if (depth < maxDepth)
            {
                bool keep_going = false;
                if (depth < (maxDepth/3) && node.visited < Slime.max_visits_first)
                {
                    keep_going = true;
                }
                if (depth < (2 * maxDepth / 3) && node.visited < Slime.max_visits_middle)
                {
                    keep_going = true;
                }
                if (depth < maxDepth && node.visited < Slime.max_visits_last)
                {
                    keep_going = true;
                }

                if (keep_going)
                {
                    node.visited++;
                    StartCoroutine(create_child(node.vert_index, node, mesh, transform, slime_template));
                }
            }
        }
    }

    public IEnumerator create_child(int vert_index, TriNode node, Mesh mesh, Transform transform, GameObject slime_template)
    {
        yield return new WaitForSeconds(0.5f + Random.Range(0, 1.5f));

        Vector3 spawn_world = transform.localPosition + mesh.vertices[vert_index] * transform.localScale.x;

        GameObject go = GameObject.Instantiate(slime_template);
        go.transform.parent = this.gameObject.transform;

        Slime child_slime = go.AddComponent<Slime>();
        child_slime.maxDepth = this.maxDepth;
        child_slime.childScale = this.childScale;
        child_slime.depth = this.depth + 1;

        go.transform.localScale = Vector3.one * childScale;
        go.transform.position = spawn_world;

        if (child_slime.depth < child_slime.maxDepth)
        {
            child_slime.create_parent(node.neightbours, mesh, transform, slime_template);
        }
    }

}
