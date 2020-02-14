using UnityEngine.Tilemaps;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System;

public class DungeonGenerator : MonoBehaviour
{
    public Tilemap dungeonTilemap;
    public Tile[] tiles = new Tile[4];
    public Rect bounds = new Rect(Vector2.zero, Vector2.one * 101);
    public int numRoomTries = 0;
    public int extraConnectChance = 20;
    public int roomExtraSize = 0;
    public int windingPercent = 0;

    private int m_currentRegion = 1;
    private int[,] m_regions;
    /// <summary>
    /// 0 == wall, 1 == empty, 2 == open door, 3 == closed door
    /// </summary>
    private int[,] m_map;
    private List<Rect> m_rooms;
    private Vector2[] m_cardinalDirections;

    private void Start()
    {
        GenerateDungeon();
        GenerateMesh();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Reset();
            GenerateDungeon();
            GenerateMesh();
        }
    }

    private void Reset()
    {
        m_currentRegion = 1;
        for (int x = 0; x < bounds.width; x++)
        {
            for (int y = 0; y < bounds.height; y++)
            {
                m_regions[x, y] = 0;
                m_map[x, y] = 0;

            }
        }
        m_rooms.Clear();
        dungeonTilemap.ClearAllTiles();
    }

    private void GenerateDungeon()
    {
        bounds.width += (bounds.width % 2 == 0) ? 1 : 0;
        bounds.height += (bounds.height % 2 == 0) ? 1 : 0;
        bounds.position = Vector2.zero;

        m_regions = new int[(int)bounds.width, (int)bounds.height];
        m_map = new int[(int)bounds.width, (int)bounds.height];
        m_rooms = new List<Rect>();

        m_cardinalDirections = new Vector2[4];
        m_cardinalDirections[0] = new Vector2(0, -1);
        m_cardinalDirections[1] = new Vector2(1, 0);
        m_cardinalDirections[2] = new Vector2(0, 1);
        m_cardinalDirections[3] = new Vector2(-1, 0);

        AddRooms();

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

        ConnectRegions();
        RemoveDeadEnds();
    }

    private void GenerateMesh()
    {
        for (int x = 0; x < bounds.width; x++)
        {
            for (int y = 0; y < bounds.height; y++)
            {
                switch (m_map[x, y])
                {
                    case 0:
                        dungeonTilemap.SetTile(new Vector3Int(x, y, 0), tiles[0]);
                        break;
                    case 1:
                        dungeonTilemap.SetTile(new Vector3Int(x, y, 0), tiles[1]);
                        break;
                    case 2:
                        dungeonTilemap.SetTile(new Vector3Int(x, y, 0), tiles[2]);
                        break;
                    case 3:
                        dungeonTilemap.SetTile(new Vector3Int(x, y, 0), tiles[3]);
                        break;
                    default:
                        break;
                }
            }
        }

        TilemapCollider2D collider2D = dungeonTilemap.gameObject.GetComponent<TilemapCollider2D>();
        if (collider2D == null)
        {
            collider2D = dungeonTilemap.gameObject.AddComponent<TilemapCollider2D>();
        }
    }

    private void AddRooms()
    {
        for (int i = 0; i < numRoomTries; i++)
        {
            int size = UnityEngine.Random.Range(1, 3 + roomExtraSize) * 2 + 1;
            int rectangularity = UnityEngine.Random.Range(0, 1 + size / 2) * 2;
            int w = size;
            int h = size;
            if ((rectangularity / 2.0f) % 2 == 0)
            {
                w += rectangularity;
            }
            else
            {
                h += rectangularity;
            }

            int x = UnityEngine.Random.Range(0, ((int)bounds.width - w) / 2) * 2 + 1;
            int y = UnityEngine.Random.Range(0, ((int)bounds.height - h) / 2) * 2 + 1;
            Rect room = new Rect(x, y, w, h);

            bool overlaps = false;
            foreach (var other in m_rooms)
            {
                if (room.Overlaps(other))
                {
                    overlaps = true;
                    break;
                }
            }

            if (overlaps)
            {
                continue;
            }

            m_rooms.Add(room);

            m_currentRegion++;

            foreach (var pos in RectToArray(room))
            {
                Carve(pos);
            }
        }
    }

    private Vector2[] RectToArray(Rect rect)
    {
        Vector2[] rectArray = new Vector2[(int)rect.width * (int)rect.height];
        int index = 0;
        for (int x = 0; x < rect.width; x++)
        {
            for (int y = 0; y < rect.height; y++)
            {
                rectArray[index] = new Vector2(x + (int)rect.x, y + (int)rect.y);
                index++;
            }
        }

        return rectArray;
    }

    private void Carve(Vector2 pos)
    {
        m_map[(int)pos.x, (int)pos.y] = 1;
        m_regions[(int)pos.x, (int)pos.y] = m_currentRegion;
    }

    private void GrowMaze(Vector2 start)
    {
        List<Vector2> cells = new List<Vector2>();
        Vector2 prevDir = new Vector2();

        m_currentRegion++;
        Carve(start);

        cells.Add(start);
        while (cells.Count > 0)
        {
            Vector2 cell = cells[cells.Count - 1];

            List<Vector2> unmadeCells = new List<Vector2>();
            foreach (var dir in m_cardinalDirections)
            {
                if (CanCarve(cell, dir))
                {
                    unmadeCells.Add(dir);
                }
            }

            if (unmadeCells.Count != 0)
            {
                Vector2 dir;
                if (unmadeCells.Contains(prevDir) && UnityEngine.Random.Range(0, 100) > windingPercent)
                {
                    dir = prevDir;
                }
                else
                {
                    dir = unmadeCells[UnityEngine.Random.Range(0, unmadeCells.Count - 1)];
                }

                Carve(cell + dir);
                Carve(cell + dir * 2);

                cells.Add(cell + dir * 2);
                prevDir = dir;
            }
            else
            {
                cells.RemoveAt(cells.Count - 1);
                prevDir = Vector2.zero;
            }
        }
    }

    private bool CanCarve(Vector2 pos, Vector2 dir)
    {
        if (!bounds.Contains(pos + dir * 3))
        {
            return false;
        }

        Vector2 coord = pos + dir * 2;

        return m_map[(int)coord.x, (int)coord.y] == 0;
    }

    private void ConnectRegions()
    {
        Dictionary<Vector2, Set<int>> connectorRegions = new Dictionary<Vector2, Set<int>>();
        Vector2[] borderlessBounds = RectToArray(Inflate(bounds, -1));
        foreach (var pos in borderlessBounds)
        {
            if (m_map[(int)pos.x, (int)pos.y] != 0)
            {
                continue;
            }


            Set<int> regions = new Set<int>();
            foreach (var dir in m_cardinalDirections)
            {
                int region = m_regions[(int)(pos.x + dir.x), (int)(pos.y + dir.y)];
                if (region > 0)
                {
                    regions.Add(region);
                }
            }

            if (regions.Length < 2)
            {
                continue;
            }
            
            connectorRegions[pos] = regions;
        }

        Vector2[] keys = new Vector2[connectorRegions.Count];
        connectorRegions.Keys.CopyTo(keys, 0);

        List<Vector2> connectors = new List<Vector2>();
        foreach (var item in keys)
        {
            connectors.Add(item);
        }

        int[] merged = new int[m_currentRegion + 1];
        Set<int> openRegions = new Set<int>();
        for (int i = 1; i <= m_currentRegion; i++)
        {
            merged[i] = i;
            openRegions.Add(i);
        }

        while (openRegions.Length > 1)
        {
            if (connectors.Count <= 0)
            {
                return;
            }
            Vector2 connector = connectors[UnityEngine.Random.Range(0, connectors.Count - 1)];

            AddJunction(connector);

            Set<int> regions = connectorRegions[connector];
            foreach (var region in regions)
            {
                merged[region] = region;
            }
            int dest = regions.First;
            List<int> sources = new List<int>();
            sources = regions.GetRange(1, regions.Length - 1);

            for (int i = 1; i <= m_currentRegion; i++)
            {
                if (sources.Contains(merged[i]))
                {
                    merged[i] = dest;
                }
            }

            foreach (var item in sources)
            {
                openRegions.Remove(item);
            }
            List<Vector2> removeConnectors = new List<Vector2>();
            foreach (var pos in connectors)
            {
                if (Vector2.Distance(connector, pos) < 2)
                {
                    removeConnectors.Add(pos);
                    continue;
                }

                regions = connectorRegions[pos];
                foreach (var region in regions)
                {
                    merged[region] = region;
                }

                if (regions.Length > 1)
                {
                    continue;
                }

                if (UnityEngine.Random.Range(0, extraConnectChance - 1) == 0)
                {
                    AddJunction(pos);
                }

                removeConnectors.Add(pos);
            }

            foreach (var item in removeConnectors)
            {
                connectors.Remove(item);
            }
        }
    }

    private Rect Inflate(Rect rect, int distance)
    {
        return new Rect(rect.position - (Vector2.one * distance), rect.size + (Vector2.one * distance * 2));
    }

    private void AddJunction(Vector2 pos)
    {
        if (UnityEngine.Random.Range(0, 3) == 0)
        {
            m_map[(int)pos.x, (int)pos.y] = (UnityEngine.Random.Range(0, 2) == 0) ? 2 : 1;
        }
        else
        {
            m_map[(int)pos.x, (int)pos.y] = 3;
        }
    }

    private void RemoveDeadEnds()
    {
        bool done = false;

        while (!done)
        {
            done = true;

            foreach (var pos in RectToArray(Inflate(bounds, -1)))
            {
                if (m_map[(int)pos.x, (int)pos.y] == 0)
                {
                    continue;
                }

                int exits = 0;
                foreach (var dir in m_cardinalDirections)
                {
                    if (m_map[(int)(pos.x + dir.x), (int)(pos.y + dir.y)] != 0)
                    {
                        exits++;
                    }
                }

                if (exits != 1)
                {
                    continue;
                }

                done = false;
                m_map[(int)pos.x, (int)pos.y] = 0;
            }
        }
    }

    public class Set<T> : IEnumerable<T>
    {
        public class SetEnum<T> : IEnumerator<T>
        {
            public List<T> set = new List<T>();
            private int position = -1;

            public SetEnum(List<T> list)
            {
                set = list;
            }

            public bool MoveNext()
            {
                position++;
                return (position < set.Count);
            }

            public void Reset()
            {
                position = -1;
            }

            public void Dispose() { }

            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

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

        public int Length
        {
            get
            {
                return m_set.Count;
            }
        }
        public T First
        {
            get
            {
                return m_set[0];
            }
        }
        public T Last
        {
            get
            {
                return m_set[m_set.Count - 1];
            }
        }

        private List<T> m_set = new List<T>();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)m_set).GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return ((IEnumerable<T>)m_set).GetEnumerator();
        }

        public SetEnum<T> GetEnumerator()
        {
            return new SetEnum<T>(m_set);
        }

        public void Add(T item)
        {
            if (!m_set.Contains(item))
            {
                m_set.Add(item);
            }
        }

        public void Remove(T item)
        {
            m_set.Remove(item);
        }

        public List<T> GetRange(int index, int count)
        {
            return m_set.GetRange(index, count);
        }
    }
}