using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Mathematics;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public static List<Tile> FindPath(Tile start, Tile end, int range, bool ignoreBlocks)
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
                path.Reverse();

                if(range >= 0)
                {
                    while(path.Count > range)
                    {
                        path.RemoveAt(path.Count - 1);
                    }
                }

                return path;
            }

            // Determine which neighbours to investigate
            List<Tile> investigateNeighbours;
            if(ignoreBlocks)
                investigateNeighbours = current.neighbours.Where(t => !t.occupied && !processed.Contains(t)).ToList();
            else
                investigateNeighbours = current.neighbours.Where(t => !t.occupied && !processed.Contains(t) && !t.blocked).ToList();

            // Check each non-blocked neighbouring tile that has not already been processed
            foreach (Tile neighbour in investigateNeighbours)
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

    public static List<Tile> FindPath(Tile start, Tile end)
    {
        return FindPath(start, end, -1, false);
    }

    public static List<Tile> FindPath(Tile start, Tile end, int range)
    {
        return FindPath(start, end, range, false);
    }

    public static int GetDistance(Tile a, Tile b)
    {
        Vector2 vec = new Vector2(a.x - b.x, a.y - b.y);
        return Mathf.CeilToInt(Mathf.Max(math.abs(vec.x), math.abs(vec.y)));
    }
}
