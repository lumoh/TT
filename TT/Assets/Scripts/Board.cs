using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// board is compromised of tiles which hold board objects
/// </summary>
public class Board : MonoBehaviour 
{
    /// <summary>
    /// helper to find matches on board
    /// </summary>
    [HideInInspector] public MatchFinder MatchFinder;

    /// <summary>
    /// The board camera.
    /// </summary>
    public Camera BoardCamera;

    [Header("Board Dimensions")]
    /// <summary>
    /// num blocks vertical
    /// </summary>
    public int Height;

    /// <summary>
    /// num block horizontal
    /// </summary>
    public int Width;

    /// <summary>
    /// The minimum playable y range
    /// </summary>
    public int MinY;

    /// <summary>
    /// The max playable y range
    /// </summary>
    public int MaxY;

    /// <summary>
    /// num rows to queue up
    /// </summary>
    public int QueuedRows;

    /// <summary>
    /// The tiles.
    /// </summary>
    private List<List<Tile>> _tiles;

    /// <summary>
    /// The swap on cooldown.
    /// </summary>
    private bool _swapOnCooldown = false;

    /// <summary>
    /// The velocity.
    /// </summary>
    private Vector3 _velocity;

	/// <summary>
    /// initialize a board and setup camera
    /// </summary>
	void Start () 
    {
        Application.targetFrameRate = 60;

        MatchFinder = new MatchFinder();
        BoardCamera.transform.localPosition = new Vector3(2.5f, -5.75f, -10);
        transform.localPosition = new Vector3(0, 0, 0);
        Init(6, 13);
	}

    /// <summary>
    /// Init the specified width and height
    /// </summary>
    /// <param name="width">Width.</param>
    /// <param name="height">Height.</param>
    public void Init(int width, int height)
    {
        Height = height;
        Width = width;

        MinY = 0;
        MaxY = Height - 1;

        QueuedRows = 2;

        _tiles = new List<List<Tile>>();
        createTiles();

        addRandomBlocks();
        //addSpawners();

        LeanTween.delayedCall(0.5f, () =>
        {
            FindMatches();
        });
    }

    /// <summary>
    /// Creates the tiles.
    /// </summary>
    private void createTiles()
    {
        for (int y = 0; y < Height + QueuedRows; y++)
        {
            List<Tile> row = new List<Tile>();
            for (int x = 0; x < Width; x++)
            {
                GameObject TilePrefab = Resources.Load<GameObject>("Tile");
                GameObject tileObj = Instantiate(TilePrefab);
                if (tileObj != null)
                {
                    Tile tile = tileObj.GetComponent<Tile>();
                    if (tile != null)
                    {
                        tile.Init(this, x, y);
                        row.Add(tile);
                    }
                }
            }
            _tiles.Add(row);
        }
    }

    /// <summary>
    /// Adds the random blocks.
    /// </summary>
    private void addRandomBlocks()
    {
        for (int y = 0; y < Height + QueuedRows; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                Tile tile = GetTile(x, y);
                if (tile != null) 
                {
                    AddRandomBlock("Block", tile);
                }
            }
        }
    }

    private void addSpawners()
    {
        for (int x = 0; x < Width; x++)
        {
            Tile tile = GetTile(x, 0);
            if (tile != null) 
            {
                addSpawner(tile, "Block", 3);
            }
        }
    }

    /// <summary>
    /// Gets the tile.
    /// </summary>
    /// <returns>The tile.</returns>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <param name="inPlay">If set to <c>true</c> will only return tiles in playable bounds</param>
    public Tile GetTile(int x, int y, bool inPlay = false)
    {
        Tile tile = null;
        if (_tiles != null)
        {
            if(inPlay && InPlay(x, y))
            {
                tile = _tiles[y][x];
            }
            else if(InBounds(x, y))
            {
                tile = _tiles[y][x];
            }
        }
        return tile;
    }

    /// <summary>
    /// Adds the board object.
    /// </summary>
    /// <param name="boardObject">Board object.</param>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    public void AddBoardObject(BoardObject boardObject, int x, int y, bool centerPos = true)
    {
        Tile t = GetTile(x, y);
        if (t != null)
        {
            t.AddBoardObject(boardObject, centerPos);
        }
    }

    /// <summary>
    /// Gets the board object.
    /// </summary>
    /// <returns>The board object.</returns>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <param name="layer">Layer.</param>
    public BoardObject GetBoardObject(int x, int y, int layer = 3)
    {
        BoardObject boardObject = null;
        Tile t = GetTile(x, y);
        if (t != null)
        {
            boardObject = t.GetBoardObect(layer);
        }
        return boardObject;
    }
	
    /// <summary>
    /// checks if x,y is within playable bounds of board
    /// </summary>
    /// <returns><c>true</c>, if bounds was ined, <c>false</c> otherwise.</returns>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    public bool InPlay(int x, int y)
    {
        bool flag = false;
        if (x >= 0 && x < Width && y >= MinY && y <= MaxY)
        {
            flag = true;
        }
        return flag;
    }

    /// <summary>
    /// check if x,y is within tile array
    /// </summary>
    /// <returns><c>true</c>, if bounds was ined, <c>false</c> otherwise.</returns>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    public bool InBounds(int x, int y)
    {
        bool flag = false;
        if (x >= 0 && x < Width && y >= 0 && y <= MaxY + QueuedRows)
        {
            flag = true;
        }
        return flag;
    }

    /// <summary>
    /// Finds the matches.
    /// </summary>
    /// <returns><c>true</c>, if matches was found, <c>false</c> otherwise.</returns>
    public bool FindMatches()
    {
        List<MatchCombo> matchCombos = MatchFinder.GetAllMatchesOnBoard(this);
        if (matchCombos.Count == 0)
        {
            return false;
        }
        for (int i = 0; i < matchCombos.Count; ++i) 
        {
            for(int j = 0; j < matchCombos[i].matches.Count; ++j)
            {
                BoardObject boardObject = matchCombos[i].matches[j];
                if(boardObject != null)
                {
                    boardObject.Break();
                }
            }
        }
        return true;
    }

    /// <summary>
    /// Swap the specified t1 and t2.
    /// </summary>
    /// <param name="t1">T1.</param>
    /// <param name="t2">T2.</param>
    public void Swap(Tile t1, Tile t2)
    {
        if(t1 != null && t2 != null && !_swapOnCooldown)
        {
            BoardObject b1 = t1.GetBoardObect(3);
            BoardObject b2 = t2.GetBoardObect(3);

            _swapOnCooldown = true;
            bool swapB1 = false;
            bool swapB2 = false;

            if(b1 != null)
            {
                if((b2 == null || b2.CanSwap()) && b1.CanSwap())
                {
                    swapB1 = true;
                }
            }

            if(b2 != null)
            {
                if(b1 == null || b1.CanSwap() && b2.CanSwap())
                {
                    swapB2 = true;
                }
            }

            if(swapB1 && swapB2)
            {
                t1.AddBoardObject(b2, false);
                t2.AddBoardObject(b1, false);
                b1.Swap(t2);
                b2.Swap(t1);
            }
            else if(swapB1 && !swapB2)
            {
                t1.RemoveBoardObject(b1.TileLayer);
                t2.AddBoardObject(b1, false);
                b1.Swap(t2);
            }
            else if(swapB2 && !swapB1)
            {
                t2.RemoveBoardObject(b2.TileLayer);
                t1.AddBoardObject(b2, false);
                b2.Swap(t1);
            }

            LeanTween.delayedCall(0.12f, () =>
            {
                _swapOnCooldown = false;
            });
        }
    }

    public void AddRandomBlock(string type, Tile tile)
    {
        GameObject Prefab = Resources.Load<GameObject>(type);
        GameObject obj = Instantiate(Prefab);
        if (obj != null)
        {
            BoardObject boardObject = obj.GetComponent<BoardObject>();
            boardObject.Init(ColorUtil.PickRandom(), boardObject.TileLayer);
            AddBoardObject(boardObject, tile.X, tile.Y);

            if(!InPlay(tile.X, tile.Y)) 
            {
                boardObject.SetActive(false);
            }
        }
    }

    private void addSpawner(Tile tile, string spawnType, int layer)
    {
        GameObject Prefab = Resources.Load<GameObject>("Spawner");
        GameObject obj = Instantiate(Prefab);
        if (obj != null)
        {
            Spawner spawner = obj.GetComponent<Spawner>();
            if (spawner != null)
            {
                spawner.Init(tile, spawnType, layer);
            }
            tile.AddSpawner(spawner);
        }
    }

    /// <summary>
    /// move board up
    /// </summary>
    void Update()
    {
        // slowly move board up
        _velocity = new Vector3(0, 0.4f, 0) * Time.deltaTime;
        Vector3 newPos = transform.position + _velocity;
        transform.position = newPos;

        int currentPos = Mathf.FloorToInt(newPos.y);
        if(currentPos > MinY)
        {
            MinY++;
            MaxY++;
            for(int x = 0; x < Width; x++)
            {
                Tile tile = GetTile(x, MaxY);
                if(tile != null)
                {
                    BoardObject bo = tile.GetBoardObect(3);
                    if(bo != null)
                    {
                        bo.SetActive(true);
                    }
                }
            }

            List<Tile> row = new List<Tile>();
            for (int x = 0; x < Width; x++)
            {
                GameObject TilePrefab = Resources.Load<GameObject>("Tile");
                GameObject tileObj = Instantiate(TilePrefab);
                if (tileObj != null)
                {
                    Tile tile = tileObj.GetComponent<Tile>();
                    if (tile != null)
                    {
                        tile.Init(this, x, MaxY + 2);
                        row.Add(tile);
                    }
                }
            }
            _tiles.Add(row);

            for(int x = 0; x < Width; x++)
            {
                Tile tile = GetTile(x, MaxY + 2);
                if(tile != null)
                {
                    AddRandomBlock("Block", tile);
                }
            }
        }
    }
}
