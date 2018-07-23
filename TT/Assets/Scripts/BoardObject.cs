using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// object that resides on a tile on the board
/// </summary>
public class BoardObject : MonoBehaviour 
{    
    [Header("Board Settings")]
    public Tile MyTile;
    public int TileLayer;
    public BoardObjectState State = BoardObjectState.SETTLED;
    public bool Swappable = true;
    public bool Matchable = true;
    public float SwapSpeed = 0.2f;

    /// <summary>
    /// used to help decide where the combo originated
    /// </summary>
    [HideInInspector] public int LastMove = 0;

    /// <summary>
    /// keep track of combo count
    /// </summary>
    [HideInInspector] public int ComboCount = 0;

    /// <summary>
    /// was this block influenced by player
    /// important when deciding if match is combo
    /// </summary>
    [HideInInspector] public bool Influenced = false;

    [Header("Sprite Settings")]
    public SpriteRenderer MySpriteRenderer;   
    public Sprite NONE;
    public Sprite RED;
    public Sprite TEAL;
    public Sprite YELLOW;
    public Sprite GREEN;
    public Sprite BLUE;
    public Sprite PURPLE;

    [SerializeField] private BoardObjectColor _color = BoardObjectColor.NONE;
    [SerializeField] private BoardObjectType _type = BoardObjectType.NONE; 

    private bool _active = true;
    private Vector3 _velocity;

	// Use this for initialization
	public void Start() 
    {
        State = BoardObjectState.SETTLED;
        GameEventManager.RegisterForEvent(GameEventType.GAME_LOST, handleLoss);
        GameEventManager.RegisterForEvent(GameEventType.GAME_WON, handleWon);
	}

    public void Init(BoardObjectColor c, int tileLayer)
    {
        TileLayer = tileLayer;
        Color = c;
    }

    public Board MyBoard 
    { 
        get 
        {
            Board b = null;
            if(MyTile != null)
            {
                b = MyTile.Board;
            }
            return b;
        } 
    }

    public int X
    {
        get 
        {
            int x = -1;
            if (MyTile != null)
            {
                x = MyTile.X;
            }
            return x;
        }
    }

    public int Y
    {
        get 
        {
            int y = -1;
            if (MyTile != null)
            {
                y = MyTile.Y;
            }
            return y;
        }
    }

    public BoardObjectColor Color 
    { 
        get { return _color; } 
        set 
        {
            _color = value; 
            setSpriteForColor();
        }
    }

    public BoardObjectType Type 
    {       
        get { return _type; } 
    }

    public bool CanSwap()
    {
        bool settled = State == BoardObjectState.SETTLED;
        return Swappable && settled && _active;
    }

    public bool CanMatch()
    {
        bool settled = State == BoardObjectState.SETTLED;
        return Matchable && settled && _active;
    }

    public void Swap(Tile t)
    {
        State = BoardObjectState.SWAPPING;
        LeanTween.move(gameObject, t.transform.position, 0.1f).setOnComplete(() =>
        {
            State = BoardObjectState.FALLING;
        });
    }

    public void Break(float animDelay)
    {
        if(_active)
        {
            SetActive(false);
            State = BoardObjectState.BREAKING;

            LeanTween.delayedCall(gameObject, animDelay, () =>
            {
                GameEventManager.TriggerEvent(GameEventType.BREAK_BLOCK, this);
                LeanTween.delayedCall(gameObject, 0.15f, () =>
                {
                    MyTile.RemoveBoardObject(TileLayer);
                    if(State == BoardObjectState.BREAKING)
                    {
                        Destroy(gameObject);
                    }
                });
            });

            markCombo();
        }
    }

    private void markCombo()
    {
        for(int y = Y; y >= MyBoard.MinY; y--)
        {
            BoardObject bo = MyBoard.GetBoardObject(X, y);
            if(bo != null)
            {
                bo.ComboCount = ComboCount + 1;
            }
        }
    }

    private void breakParticles()
    {
        GameObject Prefab = Resources.Load<GameObject>("BreakParticles");
        GameObject obj = Instantiate(Prefab);
        if (obj != null)
        {
            obj.transform.position = transform.position;
        }
    }

    private void handleWon(object param)
    {
        SetActive(false);
    }

    private void handleLoss(object param)
    {
        SetActive(false);
    }

    public void SetActive(bool active)
    {
        Color c = MySpriteRenderer.color;
        if(!active)
        {
            c.a = 0.25f;
        }
        else
        {
            c.a = 1f;
        }
        MySpriteRenderer.color = c;
        _active = active;
    }

    protected void setSpriteForColor()
    {
        if(MySpriteRenderer != null)
        {
            if(_color == BoardObjectColor.RED)
            {
                MySpriteRenderer.sprite = RED;
            }
            else if(_color == BoardObjectColor.TEAL)
            {
                MySpriteRenderer.sprite = TEAL;
            }
            else if(_color == BoardObjectColor.YELLOW)
            {
                MySpriteRenderer.sprite = YELLOW;
            }
            else if(_color == BoardObjectColor.GREEN)
            {
                MySpriteRenderer.sprite = GREEN;
            }
            else if(_color == BoardObjectColor.BLUE)
            {
                MySpriteRenderer.sprite = BLUE;
            }
            else if(_color == BoardObjectColor.PURPLE)
            {
                MySpriteRenderer.sprite = PURPLE;
            }
        }
    }

    void Update()
    {
        if(State == BoardObjectState.BREAKING || State == BoardObjectState.COLLECTING)
        {
            return;
        }

        if(MyTile != null)
        {
            if(transform.position.y > MyTile.transform.position.y)
            {
                State = BoardObjectState.FALLING;
            }

            // board object is settled squarely on its tile
            if(State == BoardObjectState.SETTLED)
            {
                // if tile below becomes unoccupied, start falling again!
                Tile tileBelow = getTileAdjacent(0, 1);
                if(tileBelow != null && !tileBelow.IsOccupied(TileLayer))
                {
                    State = BoardObjectState.FALLING;
                }
            }
            // the board object is falling vertically down
            else if(State == BoardObjectState.FALLING)
            {
                // if the board object has fallen further than the targeted tile
                if(transform.position.y <= MyTile.transform.position.y)
                {
                    Tile targetTile = getTileAdjacent(0, 1);
                    if(targetTile == null || (targetTile != null && targetTile.IsOccupied(TileLayer)))
                    {
                        _velocity = Vector3.zero;
                        transform.position = MyTile.transform.position;
                        State = BoardObjectState.SETTLED;

                        if(!isBlockAbove())
                        {
                            MyBoard.BreakMatches();
                        }
                    }
                    // if not blocked then move to it
                    else if(targetTile != null)
                    {
                        MyTile.RemoveBoardObject(TileLayer);
                        targetTile.AddBoardObject(this, false);
                        advanceFallingObject();
                    }
                }
                else
                {
                    advanceFallingObject();
                }
            }
        }
    }

    private void advanceFallingObject()
    {
        _velocity += new Vector3(0, -3f, 0) * Time.deltaTime;
        Vector3 newPos = transform.position + _velocity;
        if (newPos.y < MyTile.transform.position.y)
        {
            newPos = MyTile.transform.position;
        }
        transform.position = newPos;
    }

    private bool isBlockAbove()
    {
        bool flag = false;
        for(int y = Y; y >= 0; y--)
        {
            Tile tile = MyBoard.GetTile(X, y);
            if(tile != null)
            {
                BoardObject bo = tile.GetBoardObect(TileLayer);
                if(bo != null && bo.State == BoardObjectState.FALLING)
                {
                    flag = true;
                    break;
                }
            }
        }
        return flag;
    }

    private Tile getTileAdjacent(int xoffset, int yoffset)
    {
        Tile tileBelow = null;
        int x = X + xoffset;
        int y = Y + yoffset;
        tileBelow = MyBoard.GetTile(x, y);
        return tileBelow;
    }

    void OnDestroy()
    {
        LeanTween.cancel(gameObject);
        GameEventManager.UnRegisterForEvent(GameEventType.GAME_LOST, handleLoss);
        GameEventManager.UnRegisterForEvent(GameEventType.GAME_WON, handleWon);
    }
}
