// Node Class stores pertinent data used for pathfinding 
// and grid generation
using UnityEngine;
using System.Collections.Generic;
using System;

public class Node
{
    public int x { get; private set; }
    public int y { get; private set; }
    public bool walkable { get; set; }
    public bool buildable { get; set; }
    public bool visited { get; set; } 
    // variables gCost, hCost, fCost for Pathfinding
    // prevents agents/spawnPoints from spawning in places 
    // that has no valid path towards its destination
    public int gCost { get; set; } // cost from start node to target node
    public int hCost { get; set; } // heuristic cost from start node to target node
    public int fCost { get { return gCost + hCost; } }
    // Variables for Ant Colony Optimization
    public float pheromoneLevel {get; set;}

    // Parent node for pathfinding purposes
    public Node parent { get; set; }

    public Node(int x, int y, bool walkable)
    {
        this.x = x;
        this.y = y;
        this.walkable = walkable;
        this.buildable = buildable;
        this.visited = false;
        
        this.gCost = 0;
        this.hCost = 0;
        this.pheromoneLevel = 0;
        
    }
    //overloading node constructor so i dont have to modify
    //other existing scripts that dont make use of pheromoneLevel
    public Node(int x, int y, bool walkable, float pheromoneLevel)
    {
        this.x = x;
        this.y = y;
        this.walkable = walkable;
        this.buildable = buildable;
        this.visited = false;

        this.gCost = 0;
        this.hCost = 0;
        this.pheromoneLevel = 0;
    }
    //overload
    public Node(int x, int y, bool walkable, bool buildable)
    {
        this.x = x;
        this.y = y;
        this.walkable = walkable;
        this.buildable = buildable;
        this.visited = false;
        
        this.gCost = 0;
        this.hCost = 0;
        this.pheromoneLevel = 0;
        
    }
}
