using UnityEngine;
using UnityEngine.UI;

public class PathMarkerModel
{
    public string color;
    public Vector3 pathMarkerLoc;

    public PathMarkerModel(string _color, Vector3 _pathMarkerLoc)
    {
        color = _color;
        pathMarkerLoc = _pathMarkerLoc;
    }
}