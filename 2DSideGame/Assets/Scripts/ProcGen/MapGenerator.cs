using UnityEngine;
using System.Collections.Generic;
using System;

public class MapGenerator : MonoBehaviour
{
    /// <summary>
    /// Width of the map.
    /// </summary>
    [Tooltip("Width of the map.")]
    public int width;
    /// <summary>
    /// Height of the map.
    /// </summary>
    [Tooltip("Height of the map.")]
    public int height;
    /// <summary>
    /// Sets the random generation based on the seed.
    /// </summary>
    [Tooltip("Sets the random generation based on the seed.")]
    public string seed;
    /// <summary>
    /// Gets a seed based on time.
    /// </summary>
    [Tooltip("Gets a seed based on time.")]
    public bool useRandomSeed;
    /// <summary>
    /// Chance of a tile to be filled.
    /// </summary>
    [Range(0, 100), Tooltip("Chance of a tile to be filled.")]
    public int randomFillPercent;

    /// <summary>
    /// 2D grid that holds the tiles.
    /// </summary>
    private int[,] m_map;

    /// <summary>
    /// Generates the map at the start of the game.
    /// </summary>
    private void Start()
    {
        GenerateMap();
    }

    /// <summary>
    /// Generates the map.
    /// </summary>
    private void GenerateMap()
    {
        // initialises the map based on the width and height
        m_map = new int[width, height];
        // fills the map
        RandomFillMap();

        // smooths the map multiple times
        for (int i = 0; i < 5; i++)
        {
            SmoothMap();
        }

        ProcessMap();

        int borderSize = 1;
        int[,] borderedMap = new int[width + borderSize * 2, height + borderSize * 2];

        for (int x = 0; x < borderedMap.GetLength(0); x++)
        {
            for (int y = 0; y < borderedMap.GetLength(1); y++)
            {
                if (x >= borderSize && x < width + borderSize && y >= borderSize && y < height + borderSize)
                {
                    borderedMap[x, y] = m_map[x - borderSize, y - borderSize];
                }
                else
                {
                    borderedMap[x, y] = 1;
                }
            }
        }

        TileGenerator tileGen = GetComponent<TileGenerator>();
        if (tileGen != null)
        {
            tileGen.GenerateMesh(borderedMap, 1);
        }

        MeshGenerator meshGen = GetComponent<MeshGenerator>();
        if (meshGen != null)
        {
            meshGen.GenerateMesh(borderedMap, 1);
        }
    }

    private void ProcessMap()
    {
        List<List<Coord>> wallRegions = GetRegions(1);
        int wallThresholdSize = 50;

        foreach (List<Coord> wallRegion in wallRegions)
        {
            if (wallRegion.Count < wallThresholdSize)
            {
                foreach (Coord tile in wallRegion)
                {
                    m_map[tile.tileX, tile.tileY] = 0;
                }
            }
        }

        List<List<Coord>> roomRegions = GetRegions(0);
        int roomThresholdSize = 50;
        List<Room> survivingRooms = new List<Room>();

        foreach (List<Coord> roomRegion in roomRegions)
        {
            if (roomRegion.Count < roomThresholdSize)
            {
                foreach (Coord tile in roomRegion)
                {
                    m_map[tile.tileX, tile.tileY] = 1;
                }
            }
            else
            {
                survivingRooms.Add(new Room(roomRegion, m_map));
            }
        }

        survivingRooms.Sort();
        survivingRooms[0].isMainRoom = true;
        survivingRooms[0].isAccessibleFromMainRoom = true;

        ConnectClosestRooms(survivingRooms);
    }

    private void ConnectClosestRooms(List<Room> allRooms, bool forceAccessibilityFromMainRoom = false)
    {
        List<Room> roomListA = new List<Room>();
        List<Room> roomListB = new List<Room>();

        if (forceAccessibilityFromMainRoom)
        {
            foreach (Room room in allRooms)
            {
                if (room.isAccessibleFromMainRoom)
                {
                    roomListB.Add(room);
                }
                else
                {
                    roomListA.Add(room);
                }
            }
        }
        else
        {
            roomListA = allRooms;
            roomListB = allRooms;
        }

        int bestDistance = 0;
        Coord bestTileA = new Coord();
        Coord bestTileB = new Coord();
        Room bestRoomA = new Room();
        Room bestRoomB = new Room();
        bool possibleConnectionFound = false;

        foreach (Room roomA in roomListA)
        {
            if (!forceAccessibilityFromMainRoom)
            {
                possibleConnectionFound = false;
                if (roomA.connectedRooms.Count > 0)
                {
                    continue;
                }
            }

            foreach (Room roomB in roomListB)
            {
                if (roomA == roomB || roomA.IsConnected(roomB))
                {
                    continue;
                }

                for (int tileIndexA = 0; tileIndexA < roomA.edgeTiles.Count; tileIndexA++)
                {
                    for (int tileIndexB = 0; tileIndexB < roomB.edgeTiles.Count; tileIndexB++)
                    {
                        Coord tileA = roomA.edgeTiles[tileIndexA];
                        Coord tileB = roomB.edgeTiles[tileIndexB];
                        int distanceBetweenRooms = (int)(Mathf.Pow(tileA.tileX - tileB.tileX, 2) + Mathf.Pow(tileA.tileY - tileB.tileY, 2));

                        if (distanceBetweenRooms < bestDistance || !possibleConnectionFound)
                        {
                            bestDistance = distanceBetweenRooms;
                            possibleConnectionFound = true;
                            bestTileA = tileA;
                            bestTileB = tileB;
                            bestRoomA = roomA;
                            bestRoomB = roomB;
                        }
                    }
                }
            }

            if (possibleConnectionFound && !forceAccessibilityFromMainRoom)
            {
                CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
            }
        }

        if (possibleConnectionFound && forceAccessibilityFromMainRoom)
        {
            CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
            ConnectClosestRooms(allRooms, true);
        }

        if (!forceAccessibilityFromMainRoom)
        {
            ConnectClosestRooms(allRooms, true);
        }
    }

    private void CreatePassage(Room roomA, Room roomB, Coord tileA, Coord tileB)
    {
        Room.ConnectRooms(roomA, roomB);

        List<Coord> line = GetLine(tileA, tileB);
        foreach (Coord c in line)
        {
            DrawCircle(c, 5);
        }
    }

    private void DrawCircle(Coord c, int r)
    {
        for (int x = -r; x <= r; x++)
        {
            for (int y = -r; y <= r; y++)
            {
                if (x * x + y * y <= r * r)
                {
                    int drawX = c.tileX + x;
                    int drawY = c.tileY + y;
                    if (IsInMapRange(drawX, drawY))
                    {
                        m_map[drawX, drawY] = 0;
                    }
                }
            }
        }
    }

    private List<Coord> GetLine(Coord from, Coord to)
    {
        List<Coord> line = new List<Coord>();

        int x = from.tileX;
        int y = from.tileY;

        int dx = to.tileX - from.tileX;
        int dy = to.tileY - from.tileY;

        bool inverted = false;
        int step = Math.Sign(dx);
        int gradientStep = Math.Sign(dy);

        int longest = Mathf.Abs(dx);
        int shortest = Mathf.Abs(dy);

        if (longest < shortest)
        {
            inverted = true;
            longest = Mathf.Abs(dy);
            shortest = Mathf.Abs(dx);

            step = Math.Sign(dy);
            gradientStep = Math.Sign(dx);
        }

        int gradientAccumulation = longest / 2;
        for (int i = 0; i < longest; i++)
        {
            line.Add(new Coord(x, y));

            if (inverted)
            {
                y += step;
            }
            else
            {
                x += step;
            }

            gradientAccumulation += shortest;
            if (gradientAccumulation >= longest)
            {
                if (inverted)
                {
                    x += gradientStep;
                }
                else
                {
                    y += gradientStep;
                }
                gradientAccumulation -= longest;
            }
        }

        return line;
    }

    private Vector3 CoordToWorldPoint(Coord tile)
    {
        return new Vector3(-width / 2 + .5f + tile.tileX, 2, -height / 2 + .5f + tile.tileY);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tileType"></param>
    /// <returns></returns>
    private List<List<Coord>> GetRegions(int tileType)
    {
        List<List<Coord>> regions = new List<List<Coord>>();
        int[,] mapFlags = new int[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (mapFlags[x, y] == 0 && m_map[x, y] == tileType)
                {
                    List<Coord> newRegion = GetRegionTiles(x, y);
                    regions.Add(newRegion);

                    foreach (Coord tile in newRegion)
                    {
                        mapFlags[tile.tileX, tile.tileY] = 1;
                    }
                }
            }
        }

        return regions;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="startX"></param>
    /// <param name="startY"></param>
    /// <returns></returns>
    private List<Coord> GetRegionTiles(int startX, int startY)
    {
        List<Coord> tiles = new List<Coord>();
        int[,] mapFlags = new int[width, height];
        int tileType = m_map[startX, startY];

        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(new Coord(startX, startY));
        mapFlags[startX, startY] = 1;

        while (queue.Count > 0)
        {
            Coord tile = queue.Dequeue();
            tiles.Add(tile);

            for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++)
            {
                for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++)
                {
                    if (IsInMapRange(x, y) && (y == tile.tileY || x == tile.tileX))
                    {
                        if (mapFlags[x, y] == 0 && m_map[x, y] == tileType)
                        {
                            mapFlags[x, y] = 1;
                            queue.Enqueue(new Coord(x, y));
                        }
                    }
                }
            }
        }

        return tiles;
    }

    /// <summary>
    /// Checks that the coord is in the map.
    /// </summary>
    /// <param name="x">X coord.</param>
    /// <param name="y">Y coord.</param>
    /// <returns>Returns true if the coord is in the graph.</returns>
    private bool IsInMapRange(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    /// <summary>
    /// Fills the grid with either walls (1) or spaces (0).
    /// </summary>
    private void RandomFillMap()
    {
        if (useRandomSeed)
        {
            seed = Time.time.ToString();
        }

        System.Random pseudoRandom = new System.Random(seed.GetHashCode());

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // checks if it is a boader coordinate
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    // sets the tile to a wall
                    m_map[x, y] = 1;
                }
                else
                {
                    // gets a random fill type
                    m_map[x, y] = (pseudoRandom.Next(0, 100) < randomFillPercent) ? 1 : 0;
                }
            }
        }
    }

    /// <summary>
    /// Gets rid of specs of tiles.
    /// </summary>
    private void SmoothMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // gets the amount of walls perpendicular to the current tile
                int neighbourWallTiles = GetSurroundingWallCount(x, y);

                // if the tile is surrounded by walls then make the tile a wall
                if (neighbourWallTiles > 4)
                {
                    m_map[x, y] = 1;
                }
                // if the tile is surrounded by spaces then make the tile a space
                else if (neighbourWallTiles < 4)
                {
                    m_map[x, y] = 0;
                }

            }
        }
    }

    /// <summary>
    /// Gets the amount of walls perpendicular to the given grid coordinate.
    /// </summary>
    /// <param name="gridX">X coord.</param>
    /// <param name="gridY">Y coord.</param>
    /// <returns>The amount of walls perpendicular to the tile.</returns>
    private int GetSurroundingWallCount(int gridX, int gridY)
    {
        // the amount of walls perpendicular to the tile
        int wallCount = 0;

        // checks the tiles left and right of the coord
        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
        {
            // checks the tiles above and beneath the coord
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
            {
                // ensures that the coord is in the map
                if (IsInMapRange(neighbourX, neighbourY))
                {
                    // 
                    if (neighbourX != gridX || neighbourY != gridY)
                    {
                        wallCount += m_map[neighbourX, neighbourY];
                    }
                }
                else
                {
                    wallCount++;
                }
            }
        }

        return wallCount;
    }

    /// <summary>
    /// Contains an X and Y coord.
    /// </summary>
    private struct Coord
    {
        /// <summary>
        /// X coord.
        /// </summary>
        public int tileX;
        /// <summary>
        /// Y coord.
        /// </summary>
        public int tileY;

        /// <summary>
        /// Constructor takes in an X and Y value.
        /// </summary>
        /// <param name="x">X coord.</param>
        /// <param name="y">Y coord.</param>
        public Coord(int x, int y)
        {
            tileX = x;
            tileY = y;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private class Room : IComparable<Room>
    {
        /// <summary>
        /// Collection of coords within the room.
        /// </summary>
        public List<Coord> tiles;
        /// <summary>
        /// Collection of coords along the edge of the room.
        /// </summary>
        public List<Coord> edgeTiles;
        /// <summary>
        /// Collection of rooms that are connected.
        /// </summary>
        public List<Room> connectedRooms;
        /// <summary>
        /// Size of the room.
        /// </summary>
        public int roomSize;
        /// <summary>
        /// Determines if the main room can access this room.
        /// </summary>
        public bool isAccessibleFromMainRoom;
        /// <summary>
        /// Determines if this is the main room.
        /// </summary>
        public bool isMainRoom;

        /// <summary>
        /// Empty constructor that does not initialise anything.
        /// </summary>
        public Room()
        {
        }

        /// <summary>
        /// Sets the tiles and edge tiles.
        /// </summary>
        /// <param name="roomTiles">The tiles within the room.</param>
        /// <param name="map">The entire map.</param>
        public Room(List<Coord> roomTiles, int[,] map)
        {
            tiles = roomTiles;
            roomSize = tiles.Count;
            connectedRooms = new List<Room>();

            edgeTiles = new List<Coord>();
            foreach (Coord tile in tiles)
            {
                // checks the coords left and right of the tile
                for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++)
                {
                    // checks the coords above and below the tile
                    for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++)
                    {
                        // checks if it is a coord perpendicular or equal to the tile
                        if (x == tile.tileX || y == tile.tileY)
                        {
                            // adds the tile to the list if it is a wall
                            if (map[x, y] == 1)
                            {
                                edgeTiles.Add(tile);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sets the room as accessible from the main room.
        /// </summary>
        public void SetAccessibleFromMainRoom()
        {
            // checks if it is not already accessible
            if (!isAccessibleFromMainRoom)
            {
                isAccessibleFromMainRoom = true;
                // makes all connected rooms accessible
                foreach (Room connectedRoom in connectedRooms)
                {
                    connectedRoom.SetAccessibleFromMainRoom();
                }
            }
        }

        /// <summary>
        /// Connects room A to room B.
        /// </summary>
        /// <param name="roomA">First room.</param>
        /// <param name="roomB">Second room.</param>
        public static void ConnectRooms(Room roomA, Room roomB)
        {
            // makes room B assessible from the main room if room A already is
            if (roomA.isAccessibleFromMainRoom)
            {
                roomB.SetAccessibleFromMainRoom();
            }
            // makes room A assessible from the main room if room B already is
            else if (roomB.isAccessibleFromMainRoom)
            {
                roomA.SetAccessibleFromMainRoom();
            }
            // adds the rooms to eachothers list of connected rooms
            roomA.connectedRooms.Add(roomB);
            roomB.connectedRooms.Add(roomA);
        }

        /// <summary>
        /// Checks if the room is connected to this room.
        /// </summary>
        /// <param name="otherRoom">The room being checked.</param>
        /// <returns>Returns true if the room is connected.</returns>
        public bool IsConnected(Room otherRoom)
        {
            return connectedRooms.Contains(otherRoom);
        }

        /// <summary>
        /// Checks if this room has the same size as the other room.
        /// </summary>
        /// <param name="otherRoom">The room being checked.</param>
        /// <returns>Returns 0 if the rooms have the same size.</returns>
        public int CompareTo(Room otherRoom)
        {
            return otherRoom.roomSize.CompareTo(roomSize);
        }
    }
}