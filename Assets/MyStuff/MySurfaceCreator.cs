using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MySurfaceCreator : MonoBehaviour {

    public Mesh mesh;
    public GameObject slime_template;

    public int[] origin_indexes = new int[3];
    public int[] target_indexes = new int[3];

    public bool showOrigin = true;
    public bool showTarget = true;
    public bool showNormals = true;

    public int max_depth = 2;
    public float scale = 0.9f;
    
    //List<TriNode> list = new List<TriNode>();

    Dictionary<int, TriNode> map = new Dictionary<int, TriNode>(); // mesh.verticies index to TriNode map
    private void OnEnable()
    {
        GetComponent<MeshFilter>().mesh = mesh;

        for (int tri_index = 0; tri_index < mesh.triangles.Length; tri_index = tri_index + 3)
        {
            int v0_i = mesh.triangles[tri_index];
            int v1_i = mesh.triangles[tri_index + 1];
            int v2_i = mesh.triangles[tri_index + 2];

            // Get current vertice
            TriNode node0;
            TriNode node1;
            TriNode node2;

            // Node 0
            if (map.ContainsKey(v0_i))
            {
                node0 = map[v0_i];
            }
            else
            {
                node0 = new TriNode(v0_i);
                map.Add(v0_i, node0);
            }

            // Node 1
            if (map.ContainsKey(v1_i))
            {
                node1 = map[v1_i];
            }
            else
            {
                node1 = new TriNode(v1_i);
                map.Add(v1_i, node1);
            }

            // Node 2
            if (map.ContainsKey(v2_i))
            {
                node2 = map[v2_i];
            }
            else
            {
                node2 = new TriNode(v2_i);
                map.Add(v2_i, node2);
            }

            // Add neighbours
            if (!node0.neightbours.Contains(node1))
                node0.add_neighbour(node1);
            if (!node0.neightbours.Contains(node2))
                node0.add_neighbour(node2);

            if (!node1.neightbours.Contains(node0))
                node1.add_neighbour(node0);
            if (!node1.neightbours.Contains(node2))
                node1.add_neighbour(node2);

            if (!node2.neightbours.Contains(node0))
                node2.add_neighbour(node0);
            if (!node2.neightbours.Contains(node1))
                node2.add_neighbour(node1);
        }

        //Vector3 spawn_world = transform.localPosition + mesh.vertices[origin_index] * transform.localScale.x;
        //Debug.DrawRay(spawn_world, mesh.normals[origin_index] * scale, Color.red, 60, false);

        //map[origin_index].visited = true;
        //foreach (TriNode node in map[origin_index].neightbours)
        //{
        //    if (!node.visited)
        //    {
        //        Debug.DrawRay(transform.localPosition + mesh.vertices[node.vert_index] * transform.localScale.x, mesh.normals[node.vert_index] * scale, Color.magenta, 60, true);
        //        node.visited = true;
        //    }
        //}

        //foreach (var node in map.Values)
        //{
        //    node.visited = false;
        //}

        foreach (int i in origin_indexes)
        {
            StartCoroutine(spawn_point(i, 0.1f));
            StartCoroutine(spawn_point(i, 1.0f + Random.Range(0, 2f)));
            StartCoroutine(spawn_point(i, 2.0f + Random.Range(0, 3.5f)));
            StartCoroutine(spawn_point(i, 3.0f + Random.Range(0, 5.2f)));
            StartCoroutine(spawn_point(i, 4.0f + Random.Range(0, 7f)));
        }
    }

    public IEnumerator spawn_point(int index, float delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject first_parent = Instantiate(slime_template);

        Vector3 spawn_world = transform.localPosition + mesh.vertices[index] * transform.localScale.x;
        first_parent.transform.position = spawn_world;

        var slime = first_parent.AddComponent<Slime>();
        slime.maxDepth = max_depth;
        slime.childScale = scale;

        slime.create_parent(map[index].neightbours, mesh, transform, slime_template);
    }
    
}
