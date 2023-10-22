using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Chunk
{
    public int Height { get; private set; }
    public int Width { get; private set; }
    public int Left { get; private set; }
    public int Top { get; private set; }
    public int Right { get; private set; }
    public int Bottom { get; private set; }

    public int InnerLeft { get; private set; }
    public int InnerTop { get; private set; }
    public int InnerRight { get; private set; }
    public int InnerBottom { get; private set; }
    public int InnerHeight { get; private set; }
    public int InnerWidth { get; private set; }

    public TileBase[] allTiles { get; private set; }

    public Chunk(Tilemap map)
    {
        BoundsInt bounds = map.cellBounds;
        allTiles = map.GetTilesBlock(bounds);

        Left = int.MaxValue;
        Right = int.MaxValue;

        Height = bounds.size.y;
        Width = bounds.size.x;

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                TileBase tile = allTiles[x + y * Width];
                if (tile != null && Left == int.MaxValue)
                {
                    Left = x;
                    Top = y;
                }
            }
        }

        for (int x = Width - 1; x >= 0; x--)
        {
            for (int y = Height - 1; y >= 0; y--)
            {
                TileBase tile = allTiles[x + y * Width];
                if (tile != null && Right == int.MaxValue)
                {
                    Right = x;
                    Bottom = y;
                }
            }
        }

        InnerLeft = int.MaxValue;
        InnerRight = int.MaxValue;
        // inner bounds
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                TileBase tile = allTiles[x + y * Width];
                if (tile == null 
                    && x > Left && x < Right 
                    && y > Top && y < Bottom
                    && InnerLeft == int.MaxValue)
                {
                    InnerLeft = x;
                    InnerTop = y;
                }
            }
        }

        for (int x = Width - 1; x >= 0; x--)
        {
            for (int y = Height - 1; y >= 0; y--)
            {
                TileBase tile = allTiles[x + y * Width];
                if (tile == null 
                    && x > Left && x < Right
                    && y > Top && y < Bottom
                    && InnerRight == int.MaxValue)
                {
                    InnerRight = x;
                    InnerBottom = y;
                }
            }
        }

        InnerWidth = Mathf.Abs(InnerLeft - InnerRight);
        InnerHeight = Mathf.Abs(InnerTop - InnerBottom) + 1;
    }
}
