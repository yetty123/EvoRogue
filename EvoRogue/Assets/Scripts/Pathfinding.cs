using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pathfinding : MonoBehaviour 
{
  public static Pathfinding Instance;

  void Awake () 
  {
    Instance = this;
  }

  List<Node> SearchableNodes(Point currentCoordinate, Point destinationCoordinate, LayerMask obstacleLayer)
  {
    List<Node> ReachableNodes = new List<Node>();

    //assign locations to all directions that can be taken
    Point[] end = new Point[4] {
      currentCoordinate + new Point (1, 0),
      currentCoordinate + new Point (-1, 0),
      currentCoordinate + new Point (0, 1),
      currentCoordinate + new Point (0, -1)
    };

    for (int i = 0; i < 4; i++)
    {
      // Check if we can move to the next tile
      Vector3 pointChecking = new Vector3(end[i].x, end[i].y);
      RaycastHit2D checkValid = Physics2D.Linecast (pointChecking, pointChecking, obstacleLayer);

      // Collider will be null if the linecast didn't hit an obstacle
      if (checkValid.collider == null || checkValid.collider.GetComponent<PerceptionField>() != null)
      {
        ReachableNodes.Add (new Node(end[i], end[i].TravelCost(destinationCoordinate), 1));
      }
      else if (checkValid.collider.gameObject.tag == "Player")
      {
        ReachableNodes.Clear ();
        ReachableNodes.Add (new Node(end[i], end[i].TravelCost(destinationCoordinate), 1));
        return ReachableNodes;
      }
    }
    return ReachableNodes;
  }

  /// <summary>
  /// move A*
  /// </summary>
  public Point MovePickerA(Vector3 startLocation, Vector3 endLocation, LayerMask obstacleLayer)
  {
    //Assign the stat and end coordinates based on their positions in the game
    Point pointA = new Point (startLocation.x, startLocation.y);
    Point pointB = new Point (endLocation.x, endLocation.y);
    Node currentNode = new Node (pointA, pointA.TravelCost (pointB), 0);

    //create an open and closed list as required for A*
    List<Node> OpenList = SearchableNodes (currentNode.coordinate, pointB, obstacleLayer);
    List<Node> ClosedList = new List<Node>(){currentNode};

    List<Node> AdjacentNodes = SearchableNodes (currentNode.coordinate, pointB, obstacleLayer);

    while(OpenList.Count != 0)
    {
      //succesfully found the destination node
      if (AdjacentNodes[0].coordinate == pointB)
      {
        //find the first move to be made in the path found
        //create a temporary node for traversal and make it the destination node
        Node traversalNode = new Node(AdjacentNodes[0]);
        traversalNode.parent = currentNode;

        //if the nodes parent's parent isn't itself, then keep traversing through the parents
        while (traversalNode.parent.parent.coordinate != traversalNode.parent.coordinate)
        {
          traversalNode = traversalNode.parent;
          //Debug.Log ("traversalNode: " + traversalNode.coordinate.x + " , " + traversalNode.coordinate.y);
        }
        //return the point to traverse towards
        return traversalNode.coordinate;
      }

      //Analyze the adjacent nodes discovered
      for(int i = 0; i < AdjacentNodes.Count; i++)
      {
        //we'll assume the node isn't in the open list initially
        bool nodeInOpen = false;
        bool nodeInClosed = false;

        //go through the open list for analysis
        for(int k = 0; k < OpenList.Count; k++)
        {
          //is the node already in the open list?
          if(AdjacentNodes[i].coordinate == OpenList[k].coordinate)
          {
            nodeInOpen = true;

            //is the total cost of getting to the dually present node less from the adjacentNode?
            if(AdjacentNodes[i].totalCost <= OpenList[k].totalCost)
            {
              //if so, make that nodes parent the current node
              OpenList[k].parent = new Node(currentNode);
            }
          }
        }
        //go through the closed list for analysis
        for(int k = 0; k < ClosedList.Count; k++)
        {
          //is the node already in the open list?
          if(AdjacentNodes[i].coordinate == ClosedList[k].coordinate)
          {
            nodeInClosed = true;
          }
        }

        if(!nodeInOpen && !nodeInClosed)
        {
          //if the node isn't in the open or closed list, add it to the open list
          AdjacentNodes[i].parent = new Node(currentNode);
          OpenList.Add(new Node(AdjacentNodes[i]));
        }
      }

      //time to find the next node to focus on
      Node nextNode = new Node(OpenList[0]);

      //go through the updated openList to pick the next lowest cost currentNode
      for(int k = 0; k < OpenList.Count; k++)
      {
        if(OpenList[k].totalCost < nextNode.totalCost)
          nextNode = new Node(OpenList[k]);
      }

      //set the new current node and update the lists
      currentNode = new Node(nextNode);
      ClosedList.Add(new Node(currentNode));
      //OpenList.Remove(currentNode);
      for (int k = 0; k < OpenList.Count; k++)
        if (OpenList[k].coordinate == currentNode.coordinate)
          OpenList.Remove (OpenList[k]);
      //return currentNode.coordinate;


      //get a new list of nodes that are adjacent to the new currentNode
      AdjacentNodes.Clear();
      AdjacentNodes = SearchableNodes(currentNode.coordinate, pointB, obstacleLayer);
    }

    return new Point();
  }

  /// <summary>
  /// Random move
  /// </summary>
  public Point MovePickerRandom(Vector3 currentLocation)
  {
    switch (Random.Range (0, 5))
    {
      case 0:
        return new Point((int)currentLocation.x, (int)currentLocation.y);
      case 1:
        return new Point((int)currentLocation.x, (int)currentLocation.y+1);
      case 2:
        return new Point((int)currentLocation.x, (int)currentLocation.y-1);
      case 3:
        return new Point((int)currentLocation.x+1, (int)currentLocation.y);
      case 4:
        return new Point((int)currentLocation.x-1, (int)currentLocation.y);
    }

    return new Point((int)currentLocation.x, (int)currentLocation.y); 
  }
	
}
