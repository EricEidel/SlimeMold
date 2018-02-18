using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriNode
{
    public int vert_index;
    public List<TriNode> neightbours = new List<TriNode>();
    public int visited = 0;

    public TriNode(int vert_index)
    {
        this.vert_index = vert_index;
    }

    public void add_neighbour(TriNode neighbour)
    {
        this.neightbours.Add(neighbour);
    }

    public override bool Equals(object obj)
    {
        // STEP 1: Check for null
        if (obj == null)
        {
            return false;
        }

        // STEP 3: equivalent data types
        if (this.GetType() != obj.GetType())
        {
            return false;
        }
        return Equals((TriNode)obj);
    }

    public bool Equals(TriNode obj)
    {
        // STEP 1: Check for null if nullable (e.g., a reference type)
        if (obj == null)
        {
            return false;
        }
        // STEP 2: Check for ReferenceEquals if this is a reference type
        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        // STEP 3: Compare identifying fields for equality.
        return ((this.vert_index.Equals(obj.vert_index)));
    }

    public override int GetHashCode()
    {
        return this.vert_index.GetHashCode();
    }
}
