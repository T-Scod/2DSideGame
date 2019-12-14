using UnityEngine;
using UnityEngine.Tilemaps;

public class TileGenerator : MonoBehaviour
{
    public Tilemap wallsTilemap;
    public Tilemap backgroundTilemap;
    public Tile bottomRight;
    public Tile bottomLeft;
    public Tile bottom;
    public Tile right;
    public Tile left;
    public Tile topRight;
    public Tile topLeft;
    public Tile top;
    public Tile space;
    public Tile wall;

    public SquareGrid squareGrid;

    public void GenerateMesh(int[,] map, float squareSize)
    {
        squareGrid = new SquareGrid(map, squareSize);

        for (int x = 0; x < squareGrid.squares.GetLength(0); x++)
        {
            for (int y = 0; y < squareGrid.squares.GetLength(1); y++)
            {
                SetTile(squareGrid.squares[x, y], x, y, map[x, y]);
            }
        }

        TilemapCollider2D collider2D = wallsTilemap.gameObject.GetComponent<TilemapCollider2D>();
        if (collider2D == null)
        {
            collider2D = wallsTilemap.gameObject.AddComponent<TilemapCollider2D>();
        }
    }

    private void SetTile(Square square, int x, int y, int tileType)
    {
        Vector3Int tilePos = new Vector3Int(x, y, 0);
        if (square.configuration == 5 ||
                        square.configuration == 7 ||
                        square.configuration == 10 ||
                        square.configuration == 11 ||
                        square.configuration == 13 ||
                        square.configuration == 14 ||
                        square.configuration == 15)
        {
            backgroundTilemap.SetTile(tilePos, wall);
        }
        else if (square.configuration == 0)
        {
            backgroundTilemap.SetTile(tilePos, space);
        }
        else if (square.configuration == 1)
        {
            backgroundTilemap.SetTile(tilePos, space);
            wallsTilemap.SetTile(tilePos, bottomLeft);
        }
        else if (square.configuration == 2)
        {
            backgroundTilemap.SetTile(tilePos, space);
            wallsTilemap.SetTile(tilePos, bottomRight);
        }
        else if (square.configuration == 3)
        {
            wallsTilemap.SetTile(tilePos, bottom);
        }
        else if (square.configuration == 4)
        {
            backgroundTilemap.SetTile(tilePos, space);
            wallsTilemap.SetTile(tilePos, topRight);
        }
        else if (square.configuration == 6)
        {
            wallsTilemap.SetTile(tilePos, right);
        }
        else if (square.configuration == 8)
        {
            backgroundTilemap.SetTile(tilePos, space);
            wallsTilemap.SetTile(tilePos, topLeft);
        }
        else if (square.configuration == 9)
        {
            wallsTilemap.SetTile(tilePos, left);
        }
        else if (square.configuration == 12)
        {
            wallsTilemap.SetTile(tilePos, top);
        }
    }

    public class SquareGrid
    {
        public Square[,] squares;

        public SquareGrid(int[,] map, float squareSize)
        {
            int nodeCountX = map.GetLength(0);
            int nodeCountY = map.GetLength(1);
            float mapWidth = nodeCountX * squareSize;
            float mapHeight = nodeCountY * squareSize;

            ControlNode[,] controlNodes = new ControlNode[nodeCountX, nodeCountY];

            for (int x = 0; x < nodeCountX; x++)
            {
                for (int y = 0; y < nodeCountY; y++)
                {
                    Vector3 pos = new Vector3(-mapWidth / 2 + x * squareSize + squareSize / 2, 0, -mapHeight / 2 + y * squareSize + squareSize / 2);
                    controlNodes[x, y] = new ControlNode(pos, map[x, y] == 1, squareSize);
                }
            }

            squares = new Square[nodeCountX - 1, nodeCountY - 1];
            for (int x = 0; x < nodeCountX - 1; x++)
            {
                for (int y = 0; y < nodeCountY - 1; y++)
                {
                    squares[x, y] = new Square(controlNodes[x, y + 1], controlNodes[x + 1, y + 1], controlNodes[x + 1, y], controlNodes[x, y]);
                }
            }

        }
    }

    public class Square
    {
        public ControlNode topLeft, topRight, bottomRight, bottomLeft;
        public Node centreTop, centreRight, centreBottom, centreLeft;
        public int configuration;

        public Square(ControlNode _topLeft, ControlNode _topRight, ControlNode _bottomRight, ControlNode _bottomLeft)
        {
            topLeft = _topLeft;
            topRight = _topRight;
            bottomRight = _bottomRight;
            bottomLeft = _bottomLeft;

            centreTop = topLeft.right;
            centreRight = bottomRight.above;
            centreBottom = bottomLeft.right;
            centreLeft = bottomLeft.above;

            if (topLeft.active)
            {
                configuration += 8;
            }
            if (topRight.active)
            {
                configuration += 4;
            }
            if (bottomRight.active)
            {
                configuration += 2;
            }
            if (bottomLeft.active)
            {
                configuration += 1;
            }
        }
    }

    public class Node
    {
        public Vector3 position;
        public int vertexIndex = -1;

        public Node(Vector3 _pos)
        {
            position = _pos;
        }
    }

    public class ControlNode : Node
    {
        public bool active;
        public Node above, right;

        public ControlNode(Vector3 _pos, bool _active, float squareSize) : base(_pos)
        {
            active = _active;
            above = new Node(position + Vector3.forward * squareSize / 2f);
            right = new Node(position + Vector3.right * squareSize / 2f);
        }
    }
}