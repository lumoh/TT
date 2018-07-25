using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Utility class to analyze game boards for matches.  Understands matching given a static board state.
/// Exposes the MatchCombo type which can be used for analyzing match data further in systems like
/// board reduction and hinting.
/// </summary>
public class MatchFinder 
{
    /// <summary>
    /// Queue helper for finding connected objects
    /// </summary>
    private Queue<Block> coQueue = new Queue<Block>();

    /// <summary>
    /// Connected objects. Note each call will overwrite the previous list.
    /// </summary>
    private List<Block> coObjects = new List<Block>();

    /// <summary>
    /// The matches.
    /// </summary>
    private List<MatchCombo> matches = new List<MatchCombo>();

    /// <summary>
    /// The match table.
    /// </summary>
    private Hashtable matched = new Hashtable();

    /// <summary>
    /// Helper array. TODO: use consts
    /// </summary>
    private const int MaxSize = 256;
    private bool[,] hasVisited = new bool[MaxSize,MaxSize];

    // Gets all matches on board. WIP
    /// </summary>
    /// <returns>The all matches on board.</returns>
    public List<MatchCombo> GetAllMatchesOnBoard(Board board)
    {
        matches.Clear();
        matched.Clear();
        MatchCombo matchCombo;
        
        for (int y = board.MinY; y <= board.MaxY; y++) 
        {
            for(int x = 0; x < board.Width; x++)
            {
                Block blockModel = board.GetBoardObject(x, y);
                
                if(blockModel != null && matched[blockModel] == null)
                {
                    List<Block> connectedObjects = GetConnectedObjects(board, blockModel);
                    
                    for(int i = 0; i < connectedObjects.Count; i++)
                    {
                        Block connectedObject = connectedObjects[i];
                        matched[connectedObject] = 1;
                    }
                    
                    matchCombo = GetMatch(board, connectedObjects);
                    
                    if(matchCombo != null)
                    {
                        matches.Add(matchCombo);
                    }
                }
            }
        }
        
        return matches;
    }
    
    /// <summary>
    /// Gets the connected objects.
    /// </summary>
    /// <returns>The connected objects.</returns>
    /// <param name="blockObject">Block object.</param>
    public List<Block> GetConnectedObjects(Board board, Block blockObject)
    {
        coObjects.Clear();
        coQueue.Clear();
        Array.Clear(hasVisited, 0, hasVisited.Length);
        
        if (blockObject.CanMatch()) 
        {
            Block left;
            Block right;
            Block up;
            Block down;
            int x;
            int y;
            coQueue.Enqueue(blockObject);
            while(coQueue.Count > 0)
            {
                Block obj = coQueue.Dequeue();
                
                x = obj.X;
                y = obj.Y;
                
                left = board.GetBoardObject((x - 1), y);
                right = board.GetBoardObject((x + 1), y);
                up = board.GetBoardObject(x, (y + 1));
                down = board.GetBoardObject(x, (y - 1));
                
                if(IsBlockNewColorMatch(hasVisited, blockObject.Color, left))
                {
                    coObjects.Add(left);
                    hasVisited[left.X, left.Y] = true;
                    coQueue.Enqueue(left);
                }
                if(IsBlockNewColorMatch(hasVisited, blockObject.Color, right))
                {
                    coObjects.Add(right);
                    hasVisited[right.X, right.Y] = true;
                    coQueue.Enqueue(right);
                }
                if(IsBlockNewColorMatch(hasVisited, blockObject.Color, up))
                {
                    coObjects.Add(up);
                    hasVisited[up.X, up.Y] = true;
                    coQueue.Enqueue(up);
                }
                if(IsBlockNewColorMatch(hasVisited, blockObject.Color, down))
                {
                    coObjects.Add(down);
                    hasVisited[down.X, down.Y] = true;
                    coQueue.Enqueue(down);
                }
            }
        }
        
        return coObjects;
    }

    /// <summary>Small helper for to check if this node is a valid addition to our connected 
    /// color graph.  Reduces copy paste errors if predicate changes.</summary>
    /// <param name="visitedBlocks">blocks that were visited already</param>
    /// <param name="existingColor">existingColor</param>
    /// <param name="newBlock">the block to add</param>
    /// <returns>bool</returns>
    private static bool IsBlockNewColorMatch(bool[,] visitedBlocks, BlockColor existingColor, Block newBlock)
    {
        return newBlock != null 
            && visitedBlocks[newBlock.X, newBlock.Y] == false 
            && newBlock.CanMatch() 
            && newBlock.Color == existingColor
            && newBlock.Color != BlockColor.None;
    }
    
    /// <summary>
    /// Gets the match.
    /// </summary>
    /// <returns>The match.</returns>
    /// <param name="connectedObjets">Connected objets.</param>
    public MatchCombo GetMatch(Board board, List<Block> connectedObjects)
    {
        MatchCombo allMatchCombo = null;
        MatchCombo matchCombo = null;
        
        Block blockObj;
        for(int i = 0; i  < connectedObjects.Count; i++)
        {
            blockObj = connectedObjects[i];
            matchCombo = FindSingleMatch(board, blockObj);
            allMatchCombo = MergeMatch(allMatchCombo, matchCombo);
        }
        
        if(allMatchCombo != null)
        {
            allMatchCombo.matchPiece = FindMatchPiece(allMatchCombo.matches);
        }
        
        return allMatchCombo;
    }

    /// <summary>
    /// Finds the single match.
    /// </summary>
    /// <returns>The single match.</returns>
    /// <param name="board">Board Context. </param> 
    /// <param name="blockObj">Block object.</param>
    public MatchCombo FindSingleMatch(Board board, Block blockObj)
    {
        bool MATCH_4 = false;

        List<Block> hMatches = GetHorizontalMatch (board, blockObj);
        List<Block> vMatches = GetVerticalMatch (board, blockObj);
        
        int maxPieces = 0;
        
        if (hMatches != null) 
        {
            maxPieces = Mathf.Max (maxPieces, hMatches.Count);
        }
        
        if (vMatches != null) 
        {
            maxPieces = Mathf.Max (maxPieces, vMatches.Count);
        }
        
        if (maxPieces == 0) 
        {
            return null;
        } 
        else 
        {
            MatchCombo match = new MatchCombo();
            
            if (maxPieces > 5) 
            {
                match.type = MatchCombo.MatchTypes.MATCH_6_7;
            } 
            else if (maxPieces == 5) 
            {
                match.type = MatchCombo.MatchTypes.MATCH_5;
                
                int midIndex = (maxPieces / 2);
                if (hMatches != null && vMatches != null && hMatches.Count > midIndex && vMatches.Count > midIndex) 
                {
                    if ((blockObj == hMatches [midIndex] && (blockObj == vMatches [0] || blockObj == vMatches [vMatches.Count - 1])) ||
                        (blockObj == vMatches [midIndex] && (blockObj == hMatches [0] || blockObj == hMatches [hMatches.Count - 1]))) 
                    {
                        match.type = MatchCombo.MatchTypes.MATCH_6_7;
                    }
                }
            } 
            else if (hMatches != null && vMatches != null && hMatches.Count > 2 && vMatches.Count > 2) 
            {
                if ((blockObj == hMatches [0] || blockObj == hMatches [hMatches.Count - 1]) &&
                    (blockObj == vMatches [0] || blockObj == vMatches [vMatches.Count - 1])) 
                {
                    match.type = MatchCombo.MatchTypes.MATCH_L;
                } 
                else 
                {
                    match.type = MatchCombo.MatchTypes.MATCH_T;
                }
                
            } 
            else if (hMatches != null && hMatches.Count == 4) 
            {
                vMatches = new List<Block> ();
                match.type = MatchCombo.MatchTypes.MATCH_4_ROW;
            } 
            else if (vMatches != null && vMatches.Count == 4) 
            {
                hMatches = new List<Block> ();
                match.type = MatchCombo.MatchTypes.MATCH_4_COLUMN;
            } 
            else if (hMatches != null && hMatches.Count == 3) 
            {
                bool found2X3Match = false;
                if(MATCH_4)
                {
                    found2X3Match = true;
                }

                int savedY = -1;
                for (int i = 0; i < hMatches.Count; i++)
                {
                    Block bm = hMatches[i];
                    List<Block> moreVMatches = GetVerticalMatch(board, bm);
                    if (moreVMatches == null || moreVMatches.Count < 2)
                    {
                        found2X3Match = false;
                        break;
                    }
                    else
                    {                          
                        for (int j = 0; j < moreVMatches.Count; j++)
                        {
                            if (moreVMatches[j] != bm)
                            {
                                if (savedY == -1)
                                {
                                    savedY = moreVMatches[j].Y;
                                }
                                else if (savedY != moreVMatches[j].Y)
                                {
                                    found2X3Match = false;
                                    break;
                                }
                            }
                        }
                    }
                }

                if (found2X3Match)
                {
                    match.type = MatchCombo.MatchTypes.MATCH_4_SQUARE;
                }
                else
                {
                    vMatches = new List<Block> ();
                    match.type = MatchCombo.MatchTypes.MATCH_3;
                }                   
            } 
            else if (vMatches != null && vMatches.Count == 3) 
            {
                bool found2X3Match = false;
                if(MATCH_4)
                {
                    found2X3Match = true;
                }
                int savedX = -1;
                for (int i = 0; i < vMatches.Count; i++)
                {
                    Block bm = vMatches[i];
                    List<Block> moreHMatches = GetHorizontalMatch (board, bm);
                    if (moreHMatches == null || moreHMatches.Count < 2)
                    {
                        found2X3Match = false;
                        break;
                    }
                    else
                    {                          
                        for (int j = 0; j < moreHMatches.Count; j++)
                        {
                            if (moreHMatches[j] != bm)
                            {
                                if (savedX == -1)
                                {
                                    savedX = moreHMatches[j].X;
                                }
                                else if (savedX != moreHMatches[j].X)
                                {
                                    found2X3Match = false;
                                    break;
                                }
                            }
                        }
                    }
                }

                if (found2X3Match)
                {
                    match.type = MatchCombo.MatchTypes.MATCH_4_SQUARE;
                }
                else
                {
                    hMatches = new List<Block>();
                    match.type = MatchCombo.MatchTypes.MATCH_3;
                }
            }
            // MATCH 4 square logic
            else if (hMatches != null && vMatches != null && MATCH_4) 
            {
                if ((blockObj == hMatches [0] || blockObj == hMatches [hMatches.Count - 1]) &&
                    (blockObj == vMatches [0] || blockObj == vMatches [vMatches.Count - 1])) 
                {
                    int minX;
                    int minY;
                    int maxX;
                    int maxY;

                    // max tile width or tile height?
                    minX = minY = Mathf.Max(board.Width, board.Height);
                    maxX = maxY = 0;

                    for (int i = 0; i < hMatches.Count; i++) 
                    {
                        minX = Mathf.Min (hMatches [i].X, Mathf.Min (vMatches [i].X, minX));
                        minY = Mathf.Min (hMatches [i].Y, Mathf.Min (vMatches [i].Y, minY));

                        maxX = Mathf.Max (hMatches [i].X, Mathf.Max (vMatches [i].X, maxX));
                        maxY = Mathf.Max (hMatches [i].Y, Mathf.Max (vMatches [i].Y, maxY));
                    }

                    Block topLeft = board.GetBoardObject (minX, maxY);
                    Block topRight = board.GetBoardObject (maxX, maxY);
                    Block bottomLeft = board.GetBoardObject (minX, minY);
                    Block bottomRight = board.GetBoardObject (maxX, minY);

                    if ((topLeft == null || topRight == null || bottomLeft == null || bottomRight == null) ||
                        (topLeft.Type == BlockType.Rainbow || topRight.Type == BlockType.Rainbow || bottomLeft.Type == BlockType.Rainbow || bottomRight.Type == BlockType.Rainbow) ||
                        (!topLeft.CanMatch()) || (!topRight.CanMatch()) || (!bottomLeft.CanMatch()) || (!bottomRight.CanMatch())) 
                    {
                        return null;
                    }

                    if (topLeft.Color == topRight.Color && topLeft.Color == bottomLeft.Color && topLeft.Color == bottomRight.Color) 
                    {
                        if (hMatches.IndexOf (topLeft) < 0 && vMatches.IndexOf (topLeft) < 0) 
                        {
                            hMatches.Add (topLeft);
                        } else if (hMatches.IndexOf (topRight) < 0 && vMatches.IndexOf (topRight) < 0) 
                        {
                            hMatches.Add (topRight);
                        } else if (hMatches.IndexOf (bottomLeft) < 0 && vMatches.IndexOf (bottomLeft) < 0) 
                        {
                            hMatches.Add (bottomLeft);
                        } else if (hMatches.IndexOf (bottomRight) < 0 && vMatches.IndexOf (bottomRight) < 0) 
                        {
                            hMatches.Add (bottomRight);
                        }

                        match.type = MatchCombo.MatchTypes.MATCH_4_SQUARE;
                    } 
                    else 
                    {
                        return null;
                    }
                }
            }
            else 
            {
                return null;
            }
            
            if (match.type == MatchCombo.MatchTypes.MATCH_5) 
            {
                if (vMatches != null && vMatches.Count == maxPieces) 
                {
                    match.matches = vMatches;
                } 
                else 
                {
                    match.matches = hMatches;
                }
            } 
            else 
            {
                match.matches = Util.ConcatListWithoutDuplicate (hMatches, vMatches);
            }
            
            return match;
        }
    }
    
    /// <summary>
    /// Merges the match.
    /// </summary>
    /// <returns>The match.</returns>
    /// <param name="match1">Match1.</param>
    /// <param name="match2">Match2.</param>
    private MatchCombo MergeMatch(MatchCombo match1, MatchCombo match2)
    {
        if(match1 == null)
        {
            return match2;
        }
        else if(match2 == null)
        {
            return match1;
        }
        else
        {
            MatchCombo match = new MatchCombo();
            match.type = MergeMatchType(match1, match2);
            match.matches = Util.ConcatListWithoutDuplicate(match1.matches, match2.matches);
            return match;
        }
    }
    
    /// <summary>
    /// Merges the type of the match.
    /// </summary>
    /// <returns>The match type.</returns>
    /// <param name="match1">Match1.</param>
    /// <param name="match2">Match2.</param>
    private MatchCombo.MatchTypes MergeMatchType(MatchCombo match1, MatchCombo match2)
    {
        if(System.Array.IndexOf(MatchCombo.MATCH_PRIORITY, match1.type) < System.Array.IndexOf(MatchCombo.MATCH_PRIORITY, match2.type))
        {
            return match1.type;
        }
        else
        {
            return match2.type;
        }   
    }
    
    /// <summary>
    /// Finds the match piece.
    /// </summary>
    /// <returns>The match piece.</returns>
    /// <param name="matches">Matches.</param>
    private Block FindMatchPiece(List<Block> matches)
    {
        Block blockObj = null;
        int lastMove = 0;
        
        Block match;
        for(int i = 0; i < matches.Count; i++)
        {
            if(matches[i] != null)
            {
                match = matches[i];
                if(match.LastMove > lastMove)
                {
                    lastMove = match.LastMove;
                    blockObj = match;
                }
            }
        }
        
        return blockObj;
    }
    
    /// <summary>
    /// Gets the horizontal match.
    /// </summary>
    /// <returns>The horizontal match.</returns>
    /// <param name="blockObj">Block object.</param>
    private List<Block> GetHorizontalMatch(Board board, Block blockObj)
    {
        int MATCH_COUNT = 3;
        if(true)
        {
            MATCH_COUNT = 2;
        }
        
        int i; int j; int k;
        Block sp;
        
        i = blockObj.X - 1;
        for(;;)
        {
            sp = board.GetBoardObject(i, blockObj.Y);
            if(sp == null || !sp.CanMatch() || sp.Color != blockObj.Color || sp.Color == BlockColor.None) break;
            i--;
        }
        
        j = blockObj.X + 1;
        for(;;)
        {
            sp = board.GetBoardObject(j, blockObj.Y);
            if(sp == null || !sp.CanMatch() || sp.Color != blockObj.Color || sp.Color == BlockColor.None) break;
            j++;
        }
        
        if(j - i > MATCH_COUNT)
        {
            List<Block> res = new List<Block>();
            for(k = i + 1; k < j; k++)
            {
                sp = board.GetBoardObject(k, blockObj.Y);
                res.Add(sp);
            }
            return res;
        }
        else
        {
            return null;
        }
    }
    
    /// <summary>
    /// Gets the vertical match.
    /// </summary>
    /// <returns>The vertical match.</returns>
    /// <param name="blockObj">Block object.</param>
    private List<Block> GetVerticalMatch(Board board, Block blockObj)
    {
        int MATCH_COUNT = 3;
        if (true)
        {
            MATCH_COUNT = 2;
        }
        
        int i; int j; int k;
        Block sp;
        
        i = blockObj.Y - 1;
        for(;;)
        {
            sp = board.GetBoardObject(blockObj.X, i);
            if(sp == null || !sp.CanMatch() || sp.Color != blockObj.Color || sp.Color == BlockColor.None) break;
            i--;
        }
        
        j = blockObj.Y + 1;
        for(;;)
        {
            sp = board.GetBoardObject(blockObj.X, j);
            if(sp == null || !sp.CanMatch() || sp.Color != blockObj.Color || sp.Color == BlockColor.None) break;
            j++;
        }
        
        if(j - i > MATCH_COUNT)
        {
            List<Block> res = new List<Block>();
            for(k = i + 1; k < j; k++)
            {
                sp = board.GetBoardObject(blockObj.X, k);
                res.Add(sp);
            }
            return res;
        }
        else
        {
            return null;
        }
    }
    
    /// <summary>
    /// Blocks from combo.
    /// </summary>
    /// <returns>The from combo.</returns>
    /// <param name="combo">Combo.</param>
    public Block BlockFromCombo(MatchCombo combo)
    {
        Block spawnedBlock = null;

// TODO
//        spawnedBlock = new BoardObject();
//        spawnedBlock.Type = BlockTypeFromCombo(combo);
//        if(spawnedBlock.Type != BoardObjectType.RAINBOW)
//        {
//            spawnedBlock.Color = combo.matchPiece.Color;
//        }
//        else
//        {
//            spawnedBlock.Color = BoardObjectColor.NONE;
//        }
//        if(GameResourceManager.instance != null)
//        {
//            spawnedBlock = GameResourceManager.instance.GetBoardObject(spawnedBlock.BlockName());
//        }
//        spawnedBlock.X = combo.matchPiece.X;
//        spawnedBlock.Y = combo.matchPiece.Y;

        return spawnedBlock;
    }
    
    /// <summary>
    /// Blocks the type from combo.
    /// </summary>
    /// <returns>The type from combo.</returns>
    /// <param name="combo">Combo.</param>
    public BlockType BlockTypeFromCombo(MatchCombo combo)
    {
        BlockType result = BlockType.Block;

        if(combo.type == MatchCombo.MatchTypes.MATCH_4_SQUARE)
        {
            result = BlockType.Projectile;
        }
        else if(combo.type == MatchCombo.MatchTypes.MATCH_4_COLUMN)
        {
            result = BlockType.Row;
        }
        else if(combo.type == MatchCombo.MatchTypes.MATCH_4_ROW)
        {
            result = BlockType.Column;
        }
        else if(combo.type == MatchCombo.MatchTypes.MATCH_L)
        {
            result = BlockType.Bomb;
        }
        else if(combo.type == MatchCombo.MatchTypes.MATCH_T)
        {
            result = BlockType.X;
        }
        else if(combo.type == MatchCombo.MatchTypes.MATCH_5)
        {
            result = BlockType.Rainbow;
        }
        else if(combo.type == MatchCombo.MatchTypes.MATCH_6_7)
        {
            result = BlockType.Rainbow;
        }
        return result;
    }
    
    /// <summary>
    /// Determines whether this instance is special match the specified b1 b2.
    /// A special match is a match that is satisfied by just swapping the 2 pieces
    /// we want to exclude certain pieces from being counted as special match pieces
    /// i.e user cannot match a rainbow with a dropdown
    /// </summary>
    /// <returns><c>true</c> if this instance is special match the specified b1 b2; otherwise, <c>false</c>.</returns>
    /// <param name="b1">B1.</param>
    /// <param name="b2">B2.</param>
    public bool IsSpecialMatch(BlockType b1, BlockType b2)
    {
        bool result = false;
// TODO
//        if(IsValidSpecial(b1,b2) ||
//           BothBlocksEitherType(b1,b2,BoardObjectType.RAINBOW,BoardObjectType.MATCH) ||
//           BothBlocksEitherType(b1,b2,BoardObjectType.RAINBOW,BoardObjectType.SHOE) ||
//           BothBlocksEitherType(b1,b2,BoardObjectType.RAINBOW,BoardObjectType.GOO_SPREADER) ||
//           BothBlocksEitherType(b1,b2,BoardObjectType.RAINBOW,BoardObjectType.CLOCKWISE_ROTATER) ||
//           BothBlocksEitherType(b1,b2,BoardObjectType.RAINBOW,BoardObjectType.CLOCKWISE_FLIPPER) ||
//           BothBlocksEitherType(b1,b2,BoardObjectType.RAINBOW,BoardObjectType.COUNTER_CLOCKWISE_ROTATER) ||
//           BothBlocksEitherType(b1,b2,BoardObjectType.RAINBOW,BoardObjectType.COUNTER_CLOCKWISE_FLIPPER))
//        {
//            result = true;
//        }       
        return result;
    }

    /// <summary>
    /// Determines whether these two blocks are the same type, then if they are an invalid special match.
    /// </summary>
    /// <returns><c>true</c> if invalid special swap; otherwise, <c>false</c>.</returns>
    /// <param name="b1">Block 1 type.</param>
    /// <param name="b2">Block 2 type.</param>
    bool IsValidSpecial(BlockType b1, BlockType b2)
    {
        bool result = false;
// TODO
//        if(!EitherBlockThisType(b1,b2,BoardObjectType.SPREADER_CELL) && 
//           !EitherBlockThisType(b1,b2,BoardObjectType.DUAL_ALTERNATOR) &&
//           !EitherBlockThisType(b1,b2,BoardObjectType.FLAN) &&
//           !EitherBlockThisType(b1,b2,BoardObjectType.MYSTERY_BOX) &&
//           !EitherBlockThisType(b1,b2,BoardObjectType.MATCH) &&
//           !EitherBlockThisType(b1,b2,BoardObjectType.DROPDOWN) &&
//           !EitherBlockThisType(b1,b2,BoardObjectType.GOO_SPREADER) &&
//           !EitherBlockThisType(b1,b2,BoardObjectType.COUNTER_CLOCKWISE_ROTATER) &&
//           !EitherBlockThisType(b1,b2,BoardObjectType.CLOCKWISE_ROTATER) &&
//           !EitherBlockThisType(b1,b2,BoardObjectType.COUNTER_CLOCKWISE_FLIPPER) &&
//           !EitherBlockThisType(b1,b2,BoardObjectType.CLOCKWISE_FLIPPER) &&
//           !EitherBlockThisType(b1,b2,BoardObjectType.SHOE) &&
//           !EitherBlockThisType(b1,b2,BoardObjectType.MOBILE_METAL_ROCK) &&
//           !EitherBlockThisType(b1,b2,BoardObjectType.BLOCK_HIDER))
//        {
//            result = true;
//        }

        return result;
    }

    /// <summary>
    /// Determine if either block is a certain type.
    /// </summary>
    /// <returns><c>true</c>, if either block is certain type, <c>false</c> otherwise.</returns>
    /// <param name="block1">Block1.</param>
    /// <param name="block2">Block2.</param>
    /// <param name="type">Type.</param>
    bool EitherBlockThisType(BlockType block1, BlockType block2, BlockType type)
    {
        bool result = block1.Equals(type) || block2.Equals(type);
        return result;
    }
    
    /// <summary>
    /// Checks the block types for specified types, regardless of order
    /// </summary>
    /// <returns><c>true</c>, if block1 and block2 contain type1 and type2 <c>false</c> otherwise.</returns>
    /// <param name="block1">Block1.</param>
    /// <param name="block2">Block2.</param>
    /// <param name="type1">Block Type1.</param>
    /// <param name="type2">Block Type2.</param>
    bool BothBlocksEitherType(BlockType block1, BlockType block2, BlockType type1, BlockType type2)
    {
        bool firstCheck = block1.Equals(type1) &&  block2.Equals(type2);
        bool secondCheck = block1.Equals(type2) &&  block2.Equals(type1);
        bool result = firstCheck || secondCheck;
        return result;
    }
}
