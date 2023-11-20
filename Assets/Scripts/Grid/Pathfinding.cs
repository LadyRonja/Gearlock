using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public static List<Tile> FindPath(Tile start, Tile end)
    {
        // Keep list of tiles to search and tiles already searched
        List<Tile> toSearch = new List<Tile>() { start };
        List<Tile> processed = new List<Tile>();

        // Keep going as long as we have any tiles to search
        while (toSearch.Any())
        {
            Tile current = toSearch[0];
            foreach (Tile t in toSearch)
            {
                // The current tile is the tile with the
                // lowest sum of
                // tiles away from the start tile,
                // and the lowest possible amount of tiles from the end
                if (t.F < current.F || t.F == current.F && t.H < current.H)
                {
                    current = t;
                }
            }

            processed.Add(current);
            toSearch.Remove(current);

            // End is reached, return the path that lead here
            if (current == end)
            {
                Tile currentPathTile = end;
                List<Tile> path = new List<Tile>();
                while (currentPathTile != start)
                {
                    path.Add(currentPathTile);
                    currentPathTile = currentPathTile.cameFrom;
                }
                path.Add(start);
                path.Reverse();
                return path;
            }

            // Check each non-blocked neighbouring tile that has not already been processed
            foreach (Tile neighbour in current.neighbours.Where(t => !t.blocked && !processed.Contains(t)))
            {
                // Prephare to set cost of neighbour
                bool inSearch = toSearch.Contains(neighbour);
                int costToNeighbour = current.G + GetDistance(current, neighbour);

                // If neighbour hasn't already been searched, or a new path has determined a lower cost than a previous one
                // Update connection and cost
                if (!inSearch || costToNeighbour < neighbour.G)
                {
                    neighbour.G = costToNeighbour;
                    neighbour.cameFrom = current;

                    // If not previously searched, determine heuristic value
                    if (!inSearch)
                    {
                        neighbour.H = GetDistance(neighbour, end);
                        toSearch.Add(neighbour);
                    }
                }
            }
        }

        // No possible path, return null
        return null;
    }

    public static int GetDistance(Tile a, Tile b)
    {
        Vector2 vec = new Vector2(a.X - b.X, a.Y - b.Y);
        return Mathf.CeilToInt(Mathf.Max(math.abs(vec.x), math.abs(vec.y)));
    }
}
