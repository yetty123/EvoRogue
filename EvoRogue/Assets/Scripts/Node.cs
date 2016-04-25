using UnityEngine;
using System.Collections;

public class Node
{
	public int heuristic;
	public int movementCost;
	public int totalCost;
	public Node parent;
	public Point coordinate;

	public static bool operator != (Node node1, Node node2)
	{
		return !(
			node1.coordinate == node2.coordinate &&
			node1.movementCost == node2.movementCost &&
			node1.totalCost == node2.totalCost &&
			node1.heuristic == node2.heuristic);
	}

	public static bool operator == (Node node1, Node node2)
	{
		return (
			node1.coordinate == node2.coordinate &&
			node1.movementCost == node2.movementCost &&
			node1.totalCost == node2.totalCost &&
			node1.heuristic == node2.heuristic);
	}

	public Node ()
	{
		heuristic = 0;
		movementCost = 0;
		totalCost = 0;
		parent = null;
	}

	public Node (Node otherNode)
	{
		coordinate = otherNode.coordinate;
		heuristic = otherNode.heuristic;
		movementCost = otherNode.movementCost;
		totalCost = otherNode.heuristic + otherNode.movementCost;
		parent = otherNode.parent;
	}

	public Node (Point Coordinate, int Heuristic, int MovementCost)
	{
		coordinate = Coordinate;
		heuristic = Heuristic;
		movementCost = MovementCost;
		totalCost = Heuristic + MovementCost;
		parent = this;
	}

	public Node (Point Coordinate, int Heuristic, int MovementCost, Node Parent)
	{
		coordinate = Coordinate;
		heuristic = Heuristic;
		movementCost = MovementCost;
		totalCost = Heuristic + MovementCost;
		parent = Parent;
	}
}
