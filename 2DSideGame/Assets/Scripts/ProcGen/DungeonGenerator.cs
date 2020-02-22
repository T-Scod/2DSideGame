using UnityEngine.Tilemaps;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System;

public class DungeonGenerator : MonoBehaviour
{
    /// <summary>
    /// Tilemap that all the walls will be drawn to.
    /// </summary>
    [Tooltip("Tilemap that all the walls will be drawn to.")]
    public Tilemap dungeonWallsTilemap;
    /// <summary>
    /// Tilemap that the background will be drawn to.
    /// </summary>
    [Tooltip("Tilemap that the background will be drawn to.")]
    public Tilemap dungeonSpacesTilemap;
    /// <summary>
    /// Tilemap that items will be drawn to.
    /// </summary>
    [Tooltip("Tilemap that the background will be drawn to.")]
    public Tilemap dungeonItemsTilemap;
    /// <summary>
    /// The different tiles that can be drawn.
    /// </summary>
    [Tooltip("The different tiles that can be drawn.")]
    public Tile[] tiles = new Tile[4];
    /// <summary>
    /// Position and dimensions of the dungeon.
    /// </summary>
    [Tooltip("Position and dimensions of the dungeon.")]
    public Rect bounds = new Rect(Vector2.zero, Vector2.one * 101);
    /// <summary>
    /// Used to specify a random generation.
    /// </summary>
    [Tooltip("Used to specify a random generation.")]
    public string seed = string.Empty;
    /// <summary>
    /// The amount of times a room is attempted to be genereated.
    /// </summary>
    [Range(0, 1000), Tooltip("The amount of times a room is attempted to be genereated.")]
    public int numRoomTries = 0;
    /// <summary>
    /// Chance of adding a connection, the lower the value the higher the chance.
    /// </summary>
    [Range(0, 100), Tooltip("Chance of adding a connection, the lower the value the higher the chance.")]
    public int extraConnectChance = 20;
    /// <summary>
    /// The maximum potential room dimension.
    /// </summary>
    [Range(0, 10), Tooltip("The maximum potential room dimension.")]
    public int roomExtraSize = 0;
    /// <summary>
    /// The amount of winding in the paths.
    /// </summary>
    [Range(0, 100), Tooltip("The amount of winding in the paths.")]
    public int windingPercent = 0;
    /// <summary>
    /// The minimum amonut of vertical pathways to warrant a ladder.
    /// </summary>
    [Range(1, 100), Tooltip("The minimum amonut of vertical pathways to warrant a ladder.")]
    public int ladderThreshold = 1;

    /// <summary>
    /// The current amount of regions.
    /// </summary>
    private int m_regionCount = 1;
    /// <summary>
    /// Maps which tile belongs to which region.
    /// </summary>
    private int[,] m_regions;
    /// <summary>
    /// Determines the type of tile at each coordinate.
    /// 0 == wall, 1 == empty
    /// </summary>
    private int[,] m_map;
    /// <summary>
    /// Contains all the rooms.
    /// </summary>
    private List<Rect> m_rooms = new List<Rect>();
    /// <summary>
    /// Contains all the pathways.
    /// </summary>
    private Set<Vector2> m_pathways = new Set<Vector2>();
    /// <summary>
    /// Contains the 4 cardinal directions.
    /// N == (0, -1), E == (1, 0), S == (0, 1), W == (-1, 0)
    /// </summary>
    private Vector2[] m_cardinalDirections;

    /// <summary>
    /// Initialises the random generation, generates the dungeon and the tilemap.
    /// </summary>
    private void Start()
    {
        // uses the seed to initialise the random generation
        UnityEngine.Random.InitState(seed.GetHashCode());
        GenerateDungeon();
        GenerateWallTiles();
        AddLadders();
    }

    /// <summary>
    /// Creates a new dungeon every time the left mouse button is pressed.
    /// </summary>
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Reset();
            GenerateDungeon();
            GenerateWallTiles();
            AddLadders();
        }
    }

    /// <summary>
    /// Resets the variables and tilemaps.
    /// </summary>
    private void Reset()
    {
        m_regionCount = 1;
        for (int x = 0; x < bounds.width; x++)
        {
            for (int y = 0; y < bounds.height; y++)
            {
                m_regions[x, y] = 0;
                m_map[x, y] = 0;

            }
        }
        m_rooms.Clear();
        dungeonWallsTilemap.ClearAllTiles();
        dungeonSpacesTilemap.ClearAllTiles();
        dungeonItemsTilemap.ClearAllTiles();
    }

    /// <summary>
    /// Generates a randomised dungeon and stores it in a 2d array.
    /// </summary>
    private void GenerateDungeon()
    {
        // makes the dimensions of the map odd so that there is a border.
        bounds.width += (bounds.width % 2 == 0) ? 1 : 0;
        bounds.height += (bounds.height % 2 == 0) ? 1 : 0;
        bounds.position = Vector2.zero;

        // initialises the maps based on the dimensions of the dungeon
        m_regions = new int[(int)bounds.width, (int)bounds.height];
        m_map = new int[(int)bounds.width, (int)bounds.height];

        // initialises the 4 cardinal directions; North, East, South, West
        m_cardinalDirections = new Vector2[4];
        m_cardinalDirections[0] = new Vector2(0, -1);
        m_cardinalDirections[1] = new Vector2(1, 0);
        m_cardinalDirections[2] = new Vector2(0, 1);
        m_cardinalDirections[3] = new Vector2(-1, 0);

        // generates and places rooms in the dungeon
        AddRooms();

        // creates a maze from the coordinate if it is a wall
        // does not include the border and only checks odd coordinates
        for (int y = 1; y < bounds.height; y += 2)
        {
            for (int x = 1; x < bounds.width; x += 2)
            {
                if (m_map[x, y] != 0)
                {
                    continue;
                }
                GrowMaze(new Vector2(x, y));
            }
        }

        // connects the rooms and the mazes
        ConnectRegions();
        RemoveDeadEnds();
    }

    /// <summary>
    /// 
    /// </summary>
    private void GenerateWallTiles()
    {
        for (int x = 0; x < bounds.width; x++)
        {
            for (int y = 0; y < bounds.height; y++)
            {
                switch (m_map[x, y])
                {
                    case 0:
                        dungeonWallsTilemap.SetTile(new Vector3Int(x, y, 0), tiles[0]);
                        break;
                    case 1:
                        dungeonSpacesTilemap.SetTile(new Vector3Int(x, y, 0), tiles[1]);
                        break;
                    default:
                        break;
                }
            }
        }

        TilemapCollider2D collider2D = dungeonWallsTilemap.gameObject.GetComponent<TilemapCollider2D>();
        if (collider2D == null)
        {
            collider2D = dungeonWallsTilemap.gameObject.AddComponent<TilemapCollider2D>();
        }
    }

    /// <summary>
    /// Generates and places rooms in the dungeon.
    /// </summary>
    private void AddRooms()
    {
        // attempts to add rooms numRoomTries times
        for (int i = 0; i < numRoomTries; i++)
        {
            // random room size, the "* 2 + 1" is to ensure that the result is an odd number
            int size = UnityEngine.Random.Range(1, 3 + roomExtraSize) * 2 + 1;
            // used to add to one dimesion to make the room rectangular
            int rectangularity = UnityEngine.Random.Range(0, 1 + size / 2) * 2;

            // room dimensions
            int w = size;
            int h = size;

            // 50/50 chance of width or height becoming larger than the other
            if ((rectangularity / 2.0f) % 2 == 0)
            {
                w += rectangularity;
            }
            else
            {
                h += rectangularity;
            }

            // chooses a random coordinate within the bounds
            int x = UnityEngine.Random.Range(0, ((int)bounds.width - w) / 2) * 2 + 1;
            int y = UnityEngine.Random.Range(0, ((int)bounds.height - h) / 2) * 2 + 1;
            Rect room = new Rect(x, y, w, h);

            // checks if the newly created room overlaps with any other rooms
            bool overlaps = false;
            foreach (var other in m_rooms)
            {
                if (room.Overlaps(other))
                {
                    overlaps = true;
                    break;
                }
            }
            // does not add room if it overlaps
            if (overlaps)
            {
                continue;
            }

            // adds the room to the collection of rooms and increments the amount of regions
            m_rooms.Add(room);
            m_regionCount++;

            // carves out the room in the dungeon
            foreach (var pos in RectToArray(room))
            {
                Carve(pos);
            }
        }
    }

    /// <summary>
    /// Creates an array of coordinates from the rectangle.
    /// </summary>
    /// <param name="rect">The rectangle to be converted to an array.</param>
    /// <returns>Returns a collection of coordinates.</returns>
    private Vector2[] RectToArray(Rect rect)
    {
        // creates an array that will be able to fit all the coordinates
        Vector2[] rectArray = new Vector2[(int)rect.width * (int)rect.height];

        int index = 0;
        for (int x = 0; x < rect.width; x++)
        {
            for (int y = 0; y < rect.height; y++)
            {
                // adds each coordinate to the array
                rectArray[index] = new Vector2(x + (int)rect.x, y + (int)rect.y);
                index++;
            }
        }

        return rectArray;
    }

    /// <summary>
    /// Converts the specified position from a wall into a space.
    /// </summary>
    /// <param name="pos">The position to be carved out in the map.</param>
    private void Carve(Vector2 pos)
    {
        // turns the position into a space
        m_map[(int)pos.x, (int)pos.y] = 1;
        // sets the position's region to be the current region
        m_regions[(int)pos.x, (int)pos.y] = m_regionCount;
    }

    /// <summary>
    /// Creates a maze from the specified position.
    /// </summary>
    /// <param name="start">The starting position of the maze.</param>
    private void GrowMaze(Vector2 start)
    {
        // contains the cells in the maze
        List<Vector2> cells = new List<Vector2>();
        Vector2 prevDir = new Vector2();

        // the mmaze is its own region
        m_regionCount++;
        Carve(start);
        m_pathways.Add(start);

        cells.Add(start);
        while (cells.Count > 0)
        {
            // gets the last cell
            Vector2 cell = cells[cells.Count - 1];

            // collection of potential cells around the current cell that can be carved
            List<Vector2> unmadeCells = new List<Vector2>();
            // adds the cells around the current cell if they can be carved
            foreach (var dir in m_cardinalDirections)
            {
                if (CanCarve(cell, dir))
                {
                    unmadeCells.Add(dir);
                }
            }

            // checks if there are any unmade cells to carve
            if (unmadeCells.Count != 0)
            {
                Vector2 dir;
                // if the previously used direction is available and the cell is not winding then carve in the previous direction
                if (unmadeCells.Contains(prevDir) && UnityEngine.Random.Range(0, 100) > windingPercent)
                {
                    dir = prevDir;
                }
                else
                {
                    // chooses a random available direction to carve along
                    dir = unmadeCells[UnityEngine.Random.Range(0, unmadeCells.Count - 1)];
                }

                // carves along the determined direction
                Carve(cell + dir);
                Carve(cell + dir * 2);
                m_pathways.Add(cell + dir);
                m_pathways.Add(cell + dir * 2);

                // adds the carved cell to the maze
                cells.Add(cell + dir * 2);
                prevDir = dir;
            }
            else
            {
                // removes the previous cell if it does not lead anywhere
                cells.RemoveAt(cells.Count - 1);
                prevDir = Vector2.zero;
            }
        }
    }

    /// <summary>
    /// Determines if the coordinate in the direction dir from pos can be carved.
    /// </summary>
    /// <param name="pos">Position being carved from.</param>
    /// <param name="dir">Direction from pos that is being checked against.</param>
    /// <returns></returns>
    private bool CanCarve(Vector2 pos, Vector2 dir)
    {
        // checks if the coordinate in the given direction is outside of the bounds
        if (!bounds.Contains(pos + dir * 3))
        {
            return false;
        }

        // gets the coord in the direction
        Vector2 coord = pos + dir * 2;

        // checks if the coord has not been carved yet
        return m_map[(int)coord.x, (int)coord.y] == 0;
    }

    /// <summary>
    /// Connects the rooms and the mazes to each other.
    /// </summary>
    private void ConnectRegions()
    {
        // maps all the regions surrounding a coordinate to the coordinate
        Dictionary<Vector2, Set<int>> connectorRegions = new Dictionary<Vector2, Set<int>>();
        // gets a borderless map as an array of coordinates
        Vector2[] borderlessBounds = RectToArray(Inflate(bounds, -1));

        // maps all the regions surrounding a coordinate to the coordinate for every position in the bounds
        foreach (var pos in borderlessBounds)
        {
            // skips carved positions
            if (m_map[(int)pos.x, (int)pos.y] != 0)
            {
                continue;
            }

            // stores all the regions surrounding pos
            Set<int> regions = new Set<int>();
            // adds any relevant regions around pos to the regions set
            foreach (var dir in m_cardinalDirections)
            {
                int region = m_regions[(int)(pos.x + dir.x), (int)(pos.y + dir.y)];
                if (region > 0)
                {
                    regions.Add(region);
                }
            }

            // must have at least 2 surrounding regions for it to be considered a connection
            if (regions.Length < 2)
            {
                continue;
            }
            
            connectorRegions[pos] = regions;
        }

        // gets all the connectors
        List<Vector2> connectors = new List<Vector2>();
        foreach (var item in connectorRegions.Keys)
        {
            connectors.Add(item);
        }

        // keep track of which regions have been merged
        int[] merged = new int[m_regionCount + 1];
        Set<int> openRegions = new Set<int>();
        for (int i = 1; i <= m_regionCount; i++)
        {
            merged[i] = i;
            openRegions.Add(i);
        }

        // keep connectng regions until there is only 1 region
        while (openRegions.Length > 1)
        {
            if (connectors.Count <= 0)
            {
                return;
            }

            Vector2 connector = connectors[UnityEngine.Random.Range(0, connectors.Count - 1)];
            // carve the connection
            Carve(connector);
            m_pathways.Add(connector);

            // merge the connected region
            Set<int> regions = connectorRegions[connector];
            foreach (var region in regions)
            {
                merged[region] = region;
            }

            // map all the surrounding regions onto 1 of the regions, in this case the first region
            int dest = regions.First;
            List<int> sources = new List<int>();
            sources = regions.GetRange(1, regions.Length - 1);
            for (int i = 1; i <= m_regionCount; i++)
            {
                if (sources.Contains(merged[i]))
                {
                    merged[i] = dest;
                }
            }

            // removes the merged regions
            foreach (var item in sources)
            {
                openRegions.Remove(item);
            }

            // remove any of the connectors that aren't needed anymore
            List<Vector2> removeConnectors = new List<Vector2>();
            foreach (var pos in connectors)
            {
                // don't allow connectors right next to each other
                if (Vector2.Distance(connector, pos) < 2)
                {
                    removeConnectors.Add(pos);
                    continue;
                }

                // if the region no longer connects to other regions then it is not a connector
                regions = connectorRegions[pos];
                foreach (var region in regions)
                {
                    merged[region] = region;
                }

                if (regions.Length > 1)
                {
                    continue;
                }

                // occasionally make a connection so that the dungeon is not singly-connected
                if (UnityEngine.Random.Range(0, extraConnectChance - 1) == 0)
                {
                    Carve(pos);
                    m_pathways.Add(pos);
                }

                removeConnectors.Add(pos);
            }
            foreach (var item in removeConnectors)
            {
                connectors.Remove(item);
            }
        }
    }

    /// <summary>
    /// Inflates the rect from the middle in all directions.
    /// </summary>
    /// <param name="rect">The rect being inflated.</param>
    /// <param name="distance">Amount the rect is being inflated by.</param>
    /// <returns>Returns a rect that has been inflated.</returns>
    private Rect Inflate(Rect rect, int distance)
    {
        // increases the size and moves back the position by the distance
        return new Rect(rect.position - (Vector2.one * distance), rect.size + (Vector2.one * distance * 2));
    }

    /// <summary>
    /// Removes parts of the maze that don't lead anywhere.
    /// </summary>
    private void RemoveDeadEnds()
    {
        // continues to remove dead ends until none are left
        bool done = false;
        while (!done)
        {
            done = true;

            // checks each coordinate within borderless bounds
            foreach (var pos in RectToArray(Inflate(bounds, -1)))
            {
                // ignores walls as they are not part of the maze
                if (m_map[(int)pos.x, (int)pos.y] == 0)
                {
                    continue;
                }

                // counts the amount of openings surrounding the position
                int exits = 0;
                foreach (var dir in m_cardinalDirections)
                {
                    if (m_map[(int)(pos.x + dir.x), (int)(pos.y + dir.y)] != 0)
                    {
                        exits++;
                    }
                }

                // if the position has multiple exits it is not a dead end
                if (exits != 1)
                {
                    continue;
                }

                // sets the position as a wall
                done = false;
                m_map[(int)pos.x, (int)pos.y] = 0;
                m_pathways.Remove(pos);
            }
        }
    }

    /// <summary>
    /// Adds ladders in vertical pathways.
    /// </summary>
    private void AddLadders()
    {
        // attempts to create a ladder from every section of the pathway
        foreach (var path in m_pathways)
        {
            Set<Vector2> ladder = new Set<Vector2>();
            // upward and downward pos are used to iterate in both directions from the source path
            Vector2 upwardPos = path;
            Vector2 downwardPos = path;
            bool done = false;
            // continues to create a ladder until both sides of the pathway can't go any further
            while (!done)
            {
                done = true;

                // checks if the coord above is open and adds the source path if so
                Vector2 n = m_cardinalDirections[0];
                if (m_map[(int)(upwardPos.x + n.x), (int)(upwardPos.y + n.y)] != 0)
                {
                    ladder.Add(upwardPos);
                    // ensures that the open path is not part of a room
                    if (m_pathways.Contains(upwardPos + n))
                    {
                        // moves the iterator upward
                        upwardPos += n;
                        ladder.Add(upwardPos);
                        done = false;
                    }
                }

                // checks if the coord below is open and adds the source path if so
                Vector2 s = m_cardinalDirections[2];
                if (m_map[(int)(downwardPos.x + s.x), (int)(downwardPos.y + s.y)] != 0)
                {
                    ladder.Add(downwardPos);
                    // ensures that the open path is not part of a room
                    if (m_pathways.Contains(downwardPos + s))
                    {
                        // moves the iterator downward
                        downwardPos += s;
                        ladder.Add(downwardPos);
                        done = false;
                    }
                }
            }

            // checks if the consecutive pathway is large enough to be a ladder
            if (ladder.Length >= ladderThreshold)
            {
                // draws the ladder and removes it from the pathway to prevent from being reiterated
                foreach (var item in ladder)
                {
                    dungeonItemsTilemap.SetTile(new Vector3Int((int)item.x, (int)item.y, 0), tiles[2]);
                    m_pathways.Remove(item);
                }
            }
            else
            {
                // removes the coordinates in the ladder from the pathway to prevent them from being reiterated
                foreach (var item in ladder)
                {
                    m_pathways.Remove(item);
                }
            }
        }
    }

    /// <summary>
    /// A container that does not have multiple copies of the same value.
    /// </summary>
    /// <typeparam name="T">Type of value stored.</typeparam>
    public class Set<T> : IEnumerable<T>
    {
        /// <summary>
        /// Used to iterate over a Set.
        /// </summary>
        /// <typeparam name="T">Type of value stored in set.</typeparam>
        public class SetEnum<T> : IEnumerator<T>
        {
            /// <summary>
            /// Values in the set.
            /// </summary>
            public List<T> set = new List<T>();

            /// <summary>
            /// Current position in the set.
            /// </summary>
            private int position = -1;

            /// <summary>
            /// Initialises the set.
            /// </summary>
            /// <param name="list">Sets the values to that of the list.</param>
            public SetEnum(List<T> list)
            {
                set = list;
            }

            /// <summary>
            /// Moves to the next position if it is capable of doing so.
            /// </summary>
            /// <returns>Returns if the next position is available.</returns>
            public bool MoveNext()
            {
                position++;
                return (position < set.Count);
            }

            /// <summary>
            /// Resets the position of the iterator.
            /// </summary>
            public void Reset()
            {
                position = -1;
            }

            /// <summary>
            /// Cleans up variables.
            /// </summary>
            public void Dispose() { }

            /// <summary>
            /// Returns the current enum.
            /// </summary>
            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

            /// <summary>
            /// Returns the current value.
            /// </summary>
            public T Current
            {
                get
                {
                    try
                    {
                        return set[position];
                    }
                    catch (IndexOutOfRangeException)
                    {
                        throw new InvalidOperationException();
                    }
                }
            }
        }

        /// <summary>
        /// Amount of items in the set.
        /// </summary>
        public int Length
        {
            get
            {
                return m_set.Count;
            }
        }
        /// <summary>
        /// First item in the set.
        /// </summary>
        public T First
        {
            get
            {
                return m_set[0];
            }
        }
        /// <summary>
        /// Last item in the set.
        /// </summary>
        public T Last
        {
            get
            {
                return m_set[m_set.Count - 1];
            }
        }

        /// <summary>
        /// Stores all the values in a list.
        /// </summary>
        private List<T> m_set = new List<T>();

        /// <summary>
        /// Gets the current enum.
        /// </summary>
        /// <returns>Returns an enum.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)m_set).GetEnumerator();
        }

        /// <summary>
        /// Gets the current enum.
        /// </summary>
        /// <returns>Returns an enum.</returns>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return ((IEnumerable<T>)m_set).GetEnumerator();
        }

        /// <summary>
        /// Gets the current enum.
        /// </summary>
        /// <returns>Returns an enum.</returns>
        public SetEnum<T> GetEnumerator()
        {
            return new SetEnum<T>(m_set);
        }

        /// <summary>
        /// Adds an item to the set.
        /// </summary>
        /// <param name="item">Item being added.</param>
        public void Add(T item)
        {
            // checks if the set already contains the item before removing it.
            if (!m_set.Contains(item))
            {
                m_set.Add(item);
            }
        }

        /// <summary>
        /// Removes the item.
        /// </summary>
        /// <param name="item">Item being removed.</param>
        public void Remove(T item)
        {
            m_set.Remove(item);
        }

        /// <summary>
        /// Gets a subsection of the values from the set.
        /// </summary>
        /// <param name="index">Starting index of the subsection.</param>
        /// <param name="count">Amount of items taken from the set.</param>
        /// <returns>Returns a list of values from index with count amount of items.</returns>
        public List<T> GetRange(int index, int count)
        {
            return m_set.GetRange(index, count);
        }

        /// <summary>
        /// Determines if the set contains the item.
        /// </summary>
        /// <param name="item">Item being checked for.</param>
        /// <returns>Returns whether or not the set contains the item.</returns>
        public bool Contains(T item)
        {
            return m_set.Contains(item);
        }
    }
}