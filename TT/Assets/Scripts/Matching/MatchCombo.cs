using System.Collections.Generic;
using System.Text;

public class MatchCombo : System.Object 
{
	/// match types
	public enum MatchTypes
	{
		MATCH_3,
		MATCH_4_COLUMN,
		MATCH_4_SQUARE,
		MATCH_4_ROW,
		MATCH_5,
		MATCH_L,
		MATCH_T,
		MATCH_6_7
	};

	// special combos
	public enum SpecialCombos
	{
		LINE,
		LINE_LINE,
		LINE_BOMB,
		LINE_RAINBOW,
		LINE_BUTTERFLY,
		BOMB,
		BOMB_BOMB,
		BOMB_RAINBOW,
		BOMB_BUTTERFLY,
		BUTTERFLY,
		BUTTERFLY_BUTTERFLY,
		RAINBOW,
		RAINBOW_RAINBOW
	};

	/// <summary>
	/// the match priority so if both occur it knows which to make
	/// </summary>
	public static MatchTypes[] MATCH_PRIORITY = {MatchTypes.MATCH_6_7, MatchTypes.MATCH_5, MatchTypes.MATCH_T, MatchTypes.MATCH_L, MatchTypes.MATCH_4_ROW, MatchTypes.MATCH_4_COLUMN, MatchTypes.MATCH_4_SQUARE, MatchTypes.MATCH_3};

	/// <summary>
	/// The type.
	/// </summary>
	public MatchTypes type;

	/// <summary>
	/// The matches.
	/// </summary>
	public List<Block> matches;

	/// <summary>
	/// The match piece.
	/// </summary>
	public Block matchPiece;

	/// <summary>
	/// Initializes a new instance of the <see cref="MatchCombo"/> class.
	/// </summary>
	public MatchCombo()
	{

	}

	/// <summary>
	/// True if this match type spawns a new piece (breaker)
	/// </summary>
	/// <returns><c>true</c> if this instance is create type; otherwise, <c>false</c>.</returns>
	public bool IsCreateType()
	{
		return (matchPiece != null && type != MatchTypes.MATCH_3);
	}

    /// <summary>
    /// Gets the point value for this combo
    /// </summary>
    /// <returns>The points.</returns>
    public int GetPoints()
    {
        int points = 0;
//        for (int i = 0; i < matches.Count; i++)
//        {
//            points += matches[i].Points;
//        }
        return points;
    }
}
