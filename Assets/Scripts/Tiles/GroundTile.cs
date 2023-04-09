using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTile : Tile
{
    [SerializeField] private Color _baseColor, _offsetColor; //6EBC5D - 66B056

    public override void Init(int x, int y)
    {
        base.Init(x, y);
        var isOffset = (x + y) % 2 == 1;

        _renderer.color = isOffset ? _offsetColor : _baseColor;
    }

}
