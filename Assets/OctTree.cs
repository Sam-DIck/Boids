using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;
#nullable enable
public class OctTree<T>
{
    OctTree<T> parent;
    public Bounds bounds { get; private set; }
    public OctTree(Bounds bounds, OctTree<T> parent=null)
    {
        this.bounds = bounds;
        this.Values= new List<T>();
        children = new OctTree<T>[8];
        this.parent = parent;
    }
    protected OctTree(Bounds bounds,List<T> Values,Vector3 position, OctTree<T> parent = null)
    {
        this.bounds = bounds;
        this.Values = Values;
        this.ValPos = position;
        children = new OctTree<T>[8];
        this.parent = parent;
    }
    protected OctTree(Bounds bounds,T Values, Vector3 position, OctTree<T> parent = null)
    {
        this.bounds = bounds;
        this.Values = new List<T> { Values};
        this.ValPos = position;
        children = new OctTree<T>[8];
        this.parent = parent;
    }

    public int GetRegion(Vector3 pos)
    {
        int i =  (pos.x > bounds.center.x ? 4 : 0) +
            (pos.y > bounds.center.y ? 2 : 0) +
            (pos.z > bounds.center.z ? 1 : 0);
        return i;
    }
    public Bounds GetRegionBounds(int region)
    {
        Vector3 center = bounds.center
            + Vector3.right * bounds.size.x / 4 * ((region & 4) > 0 ? 1 : -1)
            + Vector3.up * bounds.size.y / 4 * ((region & 2) > 0 ? 1 : -1)
            + Vector3.forward * bounds.size.z / 4 * ((region & 1) > 0 ? 1 : -1);
        return new Bounds(center, bounds.size / 2);
    }
    public int depth { get { return parent != null ? parent.depth + 1 : 0; } }
    public int nodeIndex { get { return parent != null ? parent.nodeIndex * 8 + Array.IndexOf(parent.children, this)+1 : 0; } }
    public List<T> Values { get; private set; }
    public Vector3? ValPos { get; private set; } = null;
    
    public void Add(T item, Vector3 pos)
    {
        if(isLeaf)
        {
            if (!Values.Any()||ValPos == null)
            {
                Values.Add(item);
                ValPos = pos;
                return;
            }
            else if ((pos - (Vector3)ValPos).magnitude < float.Epsilon)
            {
                Values.Add(item);
                return;
            }
            else
            {
                children[GetRegion((Vector3)ValPos)] = new OctTree<T>(GetRegionBounds(GetRegion((Vector3)ValPos)), Values, (Vector3)ValPos, this);
                Values = new();
                ValPos = null;
            }
        }
        int index = GetRegion(pos);
        if (children[index] == null)
        {
            children[index] = new OctTree<T>(GetRegionBounds(index), item, pos, this);
        }
        else
        {
            children[index].Add(item, pos);
        }
    }

    public List<T> GetValues()
    {
        if (isLeaf) { return Values; }
        else
        {
            List<T> values = new List<T>();
            foreach (OctTree<T> child in children)
            {
                if (child!= null)
                    values.AddRange(child.GetValues());
            }
            return values;
        }
    }
    public List<OctTree<T>> GetLeaves()
    {
        if (isLeaf) { return new() { this }; }
        else
        {
            List<OctTree<T>> values = new List<OctTree<T>>();
            foreach (OctTree<T> child in children)
            {
                if (child != null)
                    values.AddRange(child.GetLeaves());
            }
            return values;
        }
    }
    public List<OctTree<T>> GetSiblings()
    {
        if (parent == null) return new();
        if (parent.children.All((c) => c == null)) return new();
        return new(from c in parent.children where c != null&&c!=this select c);
    }



    public OctTree<T>[] children { get; private set; }


    public bool isLeaf => children==null ||  children.All((c)=>c==null);

    public List<T> GetInRange(Vector3 pos, float range, int maxDepth=6)
    {
        if (ValPos != null)
        { 
            if (((Vector3)ValPos - pos).sqrMagnitude < range*range)
            {
                return Values;
            }
        }
        var inRange = new List<T>();
        foreach (var region in children)
        {
            if (region != null)
            {
                if ( (pos - region.bounds.ClosestPoint(pos)).sqrMagnitude < range * range)
                {
                    inRange.AddRange(region.GetInRange(pos, range));
                }
            }
        }
        return inRange;
    }

    public IEnumerable<T> Where(Func<T,bool> filter)
    {
        return from val in GetValues() where filter(val) select val;
    }


    public (int nodeId, T item)[] FlattenToArray()
    {
        Queue<OctTree<T>> queue = new();
        List<int> arr = new ();
        List<T> vals = new List<T>();
        queue.Enqueue(this);
        while (queue.Count > 0)
        {
            var node = queue.Dequeue();
            if (node == null)
            {
                arr.Add(-arr.Count);
            }
            else if (node.isLeaf)
            {
                arr.Add(vals.Count);
                vals.Add(node.Values[0]);
            }
            else
            {
                arr.Add(arr.Count+queue.Count);
                foreach(var c in node.children)
                {
                    queue.Enqueue(c);
                }
            }
        }
        return arr.Zip(vals, (a,b)=>(a,b)).ToArray();
    }
}
