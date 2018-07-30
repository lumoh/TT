using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// object that resides on a tile on the board
/// </summary>
public class Block : MonoBehaviour 
{    
    [Header("Board Settings")]
    public Tile MyTile;
    public int TileLayer = 3;
    public BlockState State = BlockState.SETTLED;
    public bool Swappable = true;
    public bool Matchable = true;
    public bool Fallable = true;
    public float Gravity = -1.5f;

    public int ComboMultiplier = 1;

    /// <summary>
    /// used to help decide where the combo originated
    /// </summary>
    [HideInInspector] public int LastMove = 0;

    [Header("Sprite Settings")]
    public SpriteRenderer MySpriteRenderer;   
    public Sprite NONE;
    public Sprite RED;
    public Sprite TEAL;
    public Sprite YELLOW;
    public Sprite GREEN;
    public Sprite BLUE;
    public Sprite PURPLE;

    [SerializeField] protected BlockColor _color = BlockColor.None;
    [SerializeField] protected BlockType _type = BlockType.None; 

    protected bool _active = true;
    protected Vector3 _velocity;

	// Use this for initialization
	public virtual void Start() 
    {
        State = BlockState.SETTLED;
        GameEventManager.RegisterForEvent(GameEventType.GAME_LOST, handleLoss);
        GameEventManager.RegisterForEvent(GameEventType.GAME_WON, handleWon);
	}

    public void Init(BlockColor c, int tileLayer)
    {
        TileLayer = tileLayer;
        Color = c;
        name = Color.ToString();
    }

    public void Init(BlockColor c)
    {
        Color = c;
        name = Color.ToString();
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

    public BlockColor Color 
    { 
        get { return _color; } 
        set 
        {
            _color = value; 
            setSpriteForColor();
        }
    }

    public BlockType Type 
    {       
        get { return _type; } 
    }

    public bool CanSwap()
    {
        bool settled = State == BlockState.SETTLED;
        return Swappable && settled && _active;
    }

    public bool CanMatch()
    {
        bool settled = State == BlockState.SETTLED;
        return Matchable && settled && _active;
    }

    public void Swap(Tile t)
    {
        LastMove++;
        State = BlockState.SWAPPING;
        LeanTween.move(gameObject, t.transform.position, 0.075f).setOnComplete(() =>
        {
            transform.position = t.transform.position;             
        });
    }

    public virtual void Break(float animDelay, float animOffset = 0f, int multiplier = 1)
    {
        if(_active)
        {
            SetActive(false);
            State = BlockState.BREAKING;

            LeanTween.delayedCall(gameObject, animDelay, () =>
            {
                GameEventManager.TriggerEvent(GameEventType.BREAK_BLOCK, this);
                customBreakEffect();

                MyBoard.ApplyComboMultiplierFromMatch(multiplier, this);
                MyTile.RemoveBoardObject(TileLayer);

                if(State == BlockState.BREAKING)
                {
                    LeanTween.scale(gameObject, Vector3.zero, 0.075f).setOnComplete(() =>
                    {
                        Destroy(gameObject);
                    }).setDelay(animOffset);
                }
            });
        }
    }

    /// <summary>
    /// override this to make other things happen when block breaks
    /// </summary>
    protected virtual void customBreakEffect()
    {
        // EMPTY
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
        if(MySpriteRenderer != null)
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
        }
        _active = active;
    }

    public bool IsActive
    {
        get { return _active; }
    }

    protected void setSpriteForColor()
    {
        if(MySpriteRenderer != null)
        {
            if(_color == BlockColor.Red)
            {
                MySpriteRenderer.sprite = RED;
            }
            else if(_color == BlockColor.Teal)
            {
                MySpriteRenderer.sprite = TEAL;
            }
            else if(_color == BlockColor.Yellow)
            {
                MySpriteRenderer.sprite = YELLOW;
            }
            else if(_color == BlockColor.Green)
            {
                MySpriteRenderer.sprite = GREEN;
            }
            else if(_color == BlockColor.Blue)
            {
                MySpriteRenderer.sprite = BLUE;
            }
            else if(_color == BlockColor.Purple)
            {
                MySpriteRenderer.sprite = PURPLE;
            }
        }
    }

    void Update()
    {
        if(State == BlockState.BREAKING || State == BlockState.COLLECTING || State == BlockState.SWAPPING)
        {
            return;
        }

        if(MyTile != null && Fallable)
        {
            if(transform.position.y > MyTile.transform.position.y)
            {
                State = BlockState.FALLING;
            }

            // board object is settled squarely on its tile
            if(State == BlockState.SETTLED)
            {
                // if tile below becomes unoccupied, start falling again!
                Tile tileBelow = getTileAdjacent(0, 1);
                if(tileBelow != null && !tileBelow.IsOccupied(TileLayer))
                {
                    State = BlockState.FALLING;
                }
            }
            // the board object is falling vertically down
            else if(State == BlockState.FALLING)
            {
                // if the board object has fallen further than the targeted tile
                if(transform.position.y <= MyTile.transform.position.y)
                {
                    Tile targetTile = getTileAdjacent(0, 1);
                    if(targetTile == null || (targetTile != null && targetTile.IsOccupied(TileLayer)))
                    {
                        _velocity = Vector3.zero;
                        transform.position = MyTile.transform.position;
                        State = BlockState.SETTLED;

                        if(!isBlockAbove())
                        {
                            if(!MyBoard.BreakMatchesInColumn(X))
                            {
                                MyBoard.ResetComboMultiplierForColumn(1, this);
                            }
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
        _velocity += new Vector3(0, Gravity, 0) * Time.deltaTime;
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
                Block bo = tile.GetBoardObect(TileLayer);
                if(bo != null && bo.State == BlockState.FALLING)
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

    public virtual void OnDestroy()
    {
        LeanTween.cancel(gameObject);
        GameEventManager.UnRegisterForEvent(GameEventType.GAME_LOST, handleLoss);
        GameEventManager.UnRegisterForEvent(GameEventType.GAME_WON, handleWon);
    }
}
