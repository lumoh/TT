﻿using System.Collections;
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
    public BoardCamera BoardCamera;

    /// <summary>
    /// num blocks vertical
    /// </summary>
    public int Height;

    /// <summary>
    /// num block horizontal
    /// </summary>
    public int Width;

    /// <summary>
    /// The number colors.
    /// </summary>
    public int NumColors;

    /// <summary>
    /// The break delay.
    /// </summary>
    public float BreakDelay;

    /// <summary>
    /// does the board freeze movement when break occurs
    /// </summary>
    public bool IsBreakDelay;

    /// <summary>
    /// current speed
    /// </summary>
    public float Speed;

    /// <summary>
    /// amount to speed up each line
    /// </summary>
    public float SpeedUpAmount;

    /// <summary>
    /// speed when forcing speed up
    /// </summary>
    public float ForceSpeed;

    /// <summary>
    /// The has spawners.
    /// </summary>
    public bool HasSpawners;

    /// <summary>
    /// The minimum playable y range
    /// </summary>
    [HideInInspector] public int MinY;

    /// <summary>
    /// The max playable y range
    /// </summary>
    [HideInInspector] public int MaxY;

    /// <summary>
    /// num rows to queue up
    /// </summary>
    private int _queuedRows;

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
    /// The speeding up.
    /// </summary>
    private bool _speedingUp;

    /// <summary>
    /// The scrolling delay.
    /// </summary>
    private float _scrollingDelay = 0f;

    /// <summary>
    /// The game over.
    /// </summary>
    private bool _gameOver = false;

	/// <summary>
    /// initialize a board and setup camera
    /// </summary>
	void Start()
    {

	}

    /// <summary>
    /// Init the specified width and height
    /// </summary>
    /// <param name="width">Width.</param>
    /// <param name="height">Height.</param>
    public void Init(BoardCamera cam)
    {
        GameEventManager.RegisterForEvent(GameEventType.GAME_WON, handleGameWon);
        GameEventManager.RegisterForEvent(GameEventType.SPEEDUP_UP, handleSpeedUpUp);
        GameEventManager.RegisterForEvent(GameEventType.SPEEDUP_DOWN, handleSpeedUpDown);
        GameEventManager.RegisterForEvent(GameEventType.NATURAL_SPEEDUP, handleNaturalSpeedUp);

        MatchFinder = new MatchFinder();
        BoardCamera = cam;
        BoardCamera.Init(this);
        transform.localPosition = new Vector3(0, 0, 0);

        MinY = 0;
        MaxY = Height - 1;
        _queuedRows = 2;

        _tiles = new List<List<Tile>>();
        createTiles();

        _velocity = new Vector3(0, Speed, 0);
        _speedingUp = false;

        addRandomBlocks();
    }

    /// <summary>
    /// Creates the tiles.
    /// </summary>
    private void createTiles()
    {
        for (int y = 0; y < Height + _queuedRows; y++)
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
        for (int y = 0; y < Height + _queuedRows; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if(y > 6)
                {
                    Tile tile = GetTile(x, y);
                    if(tile != null)
                    {
                        AddRandomBlock("Block", tile);

                        while(MatchExists())
                        {
                            Block bo = tile.GetBoardObect(3);
                            tile.RemoveBoardObject(3);
                            Destroy(bo.gameObject);
                            AddRandomBlock("Block", tile);
                        }
                    }

                    if(y == 7 && HasSpawners)
                    {
                        addSpawner(tile, "Block", 3);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Adds the spawners.
    /// </summary>
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
    public void AddBoardObject(Block boardObject, int x, int y, bool centerPos = true)
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
    public Block GetBoardObject(int x, int y, int layer = 3)
    {
        Block boardObject = null;
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
        if (x >= 0 && x < Width && y >= 0 && y <= MaxY + _queuedRows)
        {
            flag = true;
        }
        return flag;
    }

    public bool BreakMatches(bool fromSwap = false)
    {
        List<MatchCombo> matchCombos = MatchFinder.GetAllMatchesOnBoard(this);
        return breakCombos(matchCombos, fromSwap);
    }

    public bool BreakMatchesInColumn(int x, bool fromSwap = false)
    {
        List<MatchCombo> matchCombos = MatchFinder.GetMatchesInColumn(x, this);
        return breakCombos(matchCombos, fromSwap);
    }

    private bool breakCombos(List<MatchCombo> matchCombos, bool fromSwap)
    {
        if (matchCombos.Count == 0)
        {
            return false;
        }
        for (int i = 0; i < matchCombos.Count; ++i) 
        {
            MatchCombo combo = matchCombos[i];
            int maxMultiplier = 1;
            if(!fromSwap)
            {
                for(int j = 0; j < combo.matches.Count; ++j)
                {                
                    Block boardObject = combo.matches[j];
                    if(boardObject != null)
                    {
                        maxMultiplier = Mathf.Max(maxMultiplier, boardObject.ComboMultiplier);
                    }
                }
            }

            for(int j = 0; j < combo.matches.Count; ++j)
            {                
                Block boardObject = combo.matches[j];
                if(boardObject != null)
                {
                    boardObject.Break(BreakDelay, (float)j * 0.1f, maxMultiplier + 1);
                    if(IsBreakDelay)
                    {
                        _scrollingDelay = BreakDelay;
                    }
                }
            }

            Block bestBlock = combo.matches[0];
            if(maxMultiplier > 1)
            {
                GameObject Prefab = Resources.Load<GameObject>("Multiplier");
                if(Prefab != null)
                {
                    GameObject obj = Instantiate(Prefab);
                    if(obj != null)
                    {
                        Multiplier multiplier = obj.GetComponent<Multiplier>();
                        multiplier.Init(maxMultiplier, BoardCamera.cam);
                        obj.transform.position = bestBlock.transform.position + new Vector3(-1, 1, 0);
                    }
                }
            }

            if(combo.IsCreateType())
            {
                GameObject Prefab = Resources.Load<GameObject>("Multiplier");
                if(Prefab != null)
                {
                    GameObject obj = Instantiate(Prefab);
                    if(obj != null)
                    {
                        Multiplier multiplier = obj.GetComponent<Multiplier>();
                        multiplier.Init(combo.matches.Count, BoardCamera.cam);
                        obj.transform.position = bestBlock.transform.position + new Vector3(1, 1, 0);
                    }
                }
                //                BlockType spawnType = MatchFinder.BlockTypeFromCombo(combo);
                //                Block spawnBlock = SpawnBlock(spawnType.ToString(), combo.matchPiece.Color);
                //                combo.matchPiece.MyTile.AddBoardObject(spawnBlock);

            }
        }
        return true;
    }

    public void ApplyComboMultiplierFromMatch(int multiplier, Block b)
    {
        for(int y = b.Y - 1; y >= MinY; y--)
        {
            Tile tile = GetTile(b.X, y);
            if(tile != null)
            {
                Block block = tile.GetBoardObect();
                if(block != null)
                {
                    if(block.IsActive)
                    {
                        block.ComboMultiplier = multiplier;
                    }
                }
            }
        }
    }

    public void ResetComboMultiplierForColumn(int multiplier, Block b)
    {
        for(int y = b.Y; y <= MaxY; y++)
        {
            Tile tile = GetTile(b.X, y);
            if(tile != null)
            {
                Block block = tile.GetBoardObect();
                if(block != null)
                {
                    if(block.IsActive)
                    {
                        block.ComboMultiplier = multiplier;
                    }
                }
            }
        }
    }

    public bool MatchExists()
    {
        List<MatchCombo> matchCombos = MatchFinder.GetAllMatchesOnBoard(this);
        if (matchCombos.Count == 0)
        {
            return false;
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
            Block b1 = t1.GetBoardObect(3);
            Block b2 = t2.GetBoardObect(3);

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

            if(swapB1 || swapB2)
            {
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
                GameEventManager.TriggerEvent(GameEventType.MOVE);
            }

            LeanTween.delayedCall(0.1f, () =>
            {
                if(swapB1 || swapB2)
                {
                    if(swapB1 && swapB2)
                    {
                        b1.State = BlockState.SETTLED;
                        b2.State = BlockState.SETTLED;

                        BreakMatchesInColumn(b1.X, true);
                        BreakMatchesInColumn(b2.X, true);
                    }
                    else if(swapB1 && !swapB2)
                    {
                        b1.State = BlockState.FALLING;
                        b1.ComboMultiplier = 1;
                    }
                    else if(swapB2 && !swapB1)
                    {
                        b2.State = BlockState.FALLING;
                        b2.ComboMultiplier = 1;
                    }
                }

                _swapOnCooldown = false;
            });
        }
    }        

    /// <summary>
    /// Adds the random block.
    /// </summary>
    /// <param name="type">Type.</param>
    /// <param name="tile">Tile.</param>
    public void AddRandomBlock(string type, Tile tile)
    {
        GameObject Prefab = Resources.Load<GameObject>(type);
        GameObject obj = Instantiate(Prefab);
        if (obj != null)
        {
            Block boardObject = obj.GetComponent<Block>();
            boardObject.Init(ColorUtil.PickRandom(NumColors), boardObject.TileLayer);
            AddBoardObject(boardObject, tile.X, tile.Y);

            if(!InPlay(tile.X, tile.Y)) 
            {
                boardObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Spawns the block.
    /// </summary>
    /// <returns>The block.</returns>
    /// <param name="type">Type.</param>
    /// <param name="c">C.</param>
    public Block SpawnBlock(string type, BlockColor c)
    {
        Block block = null;
        GameObject Prefab = Resources.Load<GameObject>(type);
        if(Prefab != null)
        {
            GameObject obj = Instantiate(Prefab);
            if(obj != null)
            {
                Block boardObject = obj.GetComponent<Block>();
                boardObject.Init(c, boardObject.TileLayer);
                block = boardObject;
            }
        }
        return block;
    }

    /// <summary>
    /// Adds the spawner.
    /// </summary>
    /// <param name="tile">Tile.</param>
    /// <param name="spawnType">Spawn type.</param>
    /// <param name="layer">Layer.</param>
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
        if(_gameOver)
        {
            return;
        }

        _scrollingDelay -= Time.deltaTime;
        if(_scrollingDelay <= 0)
        {
            Vector3 velocity = _velocity;
            if(_speedingUp)
            {
                velocity = new Vector3(0, ForceSpeed, 0);
            }
            Vector3 newPos = transform.position + (velocity * Time.deltaTime);
            transform.position = newPos;

            int currentPos = Mathf.FloorToInt(newPos.y);
            if(currentPos > MinY)
            {
                activateNewRow();
            }
        }
    }

    public void SpeedUp(bool active)
    {
        _speedingUp = active;
    }

    /// <summary>
    /// Check for game over
    /// </summary>
    /// <returns><c>true</c>, if game over was checked, <c>false</c> otherwise.</returns>
    private bool checkGameOver()
    {
        bool flag = false;
        for(int x = 0; x < Width; x++)
        {
            Block bo = GetBoardObject(x, MinY);
            if(bo != null)
            {
                flag = true;
                _velocity = Vector3.zero;
                _gameOver = true;
                GameEventManager.TriggerEvent(GameEventType.GAME_LOST);
            }
        }
        return flag;
    }

    /// <summary>
    /// Activate the new row.
    /// </summary>
    private void activateNewRow()
    {
        if(checkGameOver())
        {
            return;
        }

        MinY++;
        MaxY++;

        for(int x = 0; x < Width; x++)
        {
            Tile tile = GetTile(x, MaxY);
            if(tile != null)
            {
                Block bo = tile.GetBoardObect(3);
                if(bo != null)
                {
                    bo.SetActive(true);
                }
            }
        }

        // add new row to bottom
        List<Tile> row = new List<Tile>();
        for(int x = 0; x < Width; x++)
        {
            GameObject TilePrefab = Resources.Load<GameObject>("Tile");
            GameObject tileObj = Instantiate(TilePrefab);
            if(tileObj != null)
            {
                Tile tile = tileObj.GetComponent<Tile>();
                if(tile != null)
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
            BreakMatchesInColumn(x, true);
        }
    }

    private void handleGameWon(object param)
    {
        _gameOver = true;
    }

    private void handleSpeedUpDown(object param)
    {
        _speedingUp = true;
    }

    private void handleSpeedUpUp(object param)
    {
        _speedingUp = false;
    }

    private void handleNaturalSpeedUp(object param)
    {
        _velocity += new Vector3(0, SpeedUpAmount, 0);
    }

    void OnDestroy()
    {
        LeanTween.cancel(gameObject);
        GameEventManager.UnRegisterForEvent(GameEventType.GAME_WON, handleGameWon);
        GameEventManager.UnRegisterForEvent(GameEventType.SPEEDUP_UP, handleSpeedUpUp);
        GameEventManager.UnRegisterForEvent(GameEventType.SPEEDUP_DOWN, handleSpeedUpDown);
        GameEventManager.UnRegisterForEvent(GameEventType.NATURAL_SPEEDUP, handleNaturalSpeedUp);
    }
}
