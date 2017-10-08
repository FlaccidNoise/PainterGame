using UnityEngine;
using System.Collections;

public enum ColourList
{
    RED = 0,
    ORANGE,
    YELLOW,
    GREEN,
    BLUE,
    PURPLE
}

public class Colours : MonoBehaviour {
    public static Color PG_RED = new Color(1.0f, 0.3098f, 0);
    public static Color PG_ORANGE = new Color(1.0f, 0.6313f, 0.25098f);
    public static Color PG_YELLOW = new Color(1.0f, 0.9176f, 0.4709f);
    public static Color PG_GREEN = new Color(0.275f, 1.0f, 0.48627f);
    public static Color PG_BLUE = new Color(0.00784f, 0.75294f, 1.0f);
    public static Color PG_PURPLE = new Color(0.4784f, 0.27451f, 0.89804f);
}
