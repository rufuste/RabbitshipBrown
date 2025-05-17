using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefreshGrid : MonoBehaviour
{

    public AstarPath pathfinder;
    // Start is called before the first frame update
    void Start()
    {
        pathfinder = gameObject.GetComponent<AstarPath>();
    }

    

public void Refresh()
    {
        // Set playerfollowing boundaries to centre of map
        gameObject.transform.position = new Vector3(0, gameObject.transform.position.y, gameObject.transform.position.z);

        // Set bounds 
        Bounds bounds = GetComponent<Renderer>().bounds;
        var guo = new GraphUpdateObject(bounds);

        // Set grid graph to placeholder position
        GridGraph gridGraph = AstarPath.active.data.gridGraph;
        gridGraph.center = bounds.center;

        // Update physics
        guo.updatePhysics = true;
        AstarPath.active.UpdateGraphs(guo);
        AstarPath.active.Scan(gridGraph);

    }
}
