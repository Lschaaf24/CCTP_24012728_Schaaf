using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapVisualizer : MonoBehaviour
{
    [SerializeField]
    private Tilemap floorTilemap, wallTilemap;
    [SerializeField]
    private TileBase floorTile,
    floorTopLeft, floorTop, floorTopRight, floorLeft, floorRight,
    floorBottomLeft, floorBottomRight, floorBottom,
    floorBothSides, floorTopShade, floorLeftShade, floorRightShade, floorBottomShade,
    wallTop, wallRight, wallLeft, wallBottom, wallFull,
     wallInnerCornerDownLeft, wallInnerCornerDownRight,
     wallDiagonalCornerDownRight, wallDiagonalCornerDownLeft, wallDiagonalCornerUpRight, wallDiagonalCornerUpLeft;

    public void PaintFloorTile(IEnumerable<Vector2Int> floorPositions)
    {
        PaintTiles(floorPositions, floorTilemap, floorTile);
    }
    private void PaintTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, TileBase tile)
    {
        foreach (var position in positions)
        {
            PaintSingleTile(tilemap, tile, position);
        }
    }

    private void PaintSingleTile(Tilemap tilemap, TileBase tile, Vector2Int position)
    {
        var tilePosition = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(tilePosition, tile);
    }
    public void PaintFloorWithType(HashSet<Vector2Int> floorPositions)
    {
        foreach (var position in floorPositions)
        {
            string binaryType = "";
            foreach (var direction in Direction2D.cardinalDirectionsList)
            {
                Vector2Int neighbour = position + direction;
                binaryType += floorPositions.Contains(neighbour) ? "1" : "0";
            }

            PaintFloorTileWithType(position, binaryType);
        }
    }

    private void PaintFloorTileWithType(Vector2Int position, string binaryType)
    {
        TileBase selectedTile = floorTile;

        switch (binaryType)
        {
            case "0110":
                selectedTile = floorTopLeft;
                break;
            case "0111":
                selectedTile = floorTop;
                break;
            case "0101":
                selectedTile = floorTop;
                break;
            case "0011":
                selectedTile = floorTopRight;
                break;
            case "1110":
                selectedTile = floorLeft;
                break;
            case "1011":
                selectedTile = floorRight;
                break;
            case "1101":
                selectedTile = floorBottom;
                break;
            case "1100":
                selectedTile = floorBottomLeft;
                break;
            case "1001":
                selectedTile = floorBottomRight;
                break;
            case "1010":
                selectedTile = floorBothSides;
                break;
            case "0100":
                selectedTile = floorLeftShade;
                break;
            case "0001":
                selectedTile = floorRightShade;
                break;
            case "1000":
                selectedTile = floorBottomShade;
                break;
            case "0010":
                selectedTile = floorTopShade;
                break;
            default:
                selectedTile = floorTile;
                break;
        }
        PaintSingleTile(floorTilemap, selectedTile, position);
        
    }



    public void Clear()
    {
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
    }

    internal void PaintSingleBasicWall(Vector2Int position, string binaryType)
    {
        int typeAsInt = Convert.ToInt32(binaryType, 2);
        TileBase tile = null;
        if (WallTypesHelper.wallTop.Contains(typeAsInt))
        {
            tile = wallTop;
        }
        else if (WallTypesHelper.wallSideRight.Contains(typeAsInt))
        {
            tile = wallRight;
        }
        else if (WallTypesHelper.wallSideLeft.Contains(typeAsInt))
        {
            tile = wallLeft;
        }
        else if (WallTypesHelper.wallBottm.Contains(typeAsInt))
        {
            tile = wallBottom;
        }
        else if (WallTypesHelper.wallFull.Contains(typeAsInt))
        {
            tile = wallFull;
        }
        if (tile != null)
        {
            PaintSingleTile(wallTilemap, tile, position);
        }
    }

    internal void PaintSingleCornerWall(Vector2Int position, string binaryType)
    {
        int typeAsInt = Convert.ToInt32(binaryType, 2);
        TileBase tile = null;

        if (WallTypesHelper.wallInnerCornerDownLeft.Contains(typeAsInt))
        {
            tile = wallInnerCornerDownLeft;
        }
        else if (WallTypesHelper.wallInnerCornerDownRight.Contains(typeAsInt))
        {
            tile = wallInnerCornerDownRight;
        }
        else if (WallTypesHelper.wallDiagonalCornerDownLeft.Contains(typeAsInt))
        {
            tile = wallDiagonalCornerDownLeft;
        }
        else if (WallTypesHelper.wallDiagonalCornerDownRight.Contains(typeAsInt))
        {
            tile = wallDiagonalCornerDownRight;
        }
        else if (WallTypesHelper.wallDiagonalCornerUpRight.Contains(typeAsInt))
        {
            tile = wallDiagonalCornerUpRight;
        }
        else if (WallTypesHelper.wallDiagonalCornerUpLeft.Contains(typeAsInt))
        {
            tile = wallDiagonalCornerUpLeft;
        }
        else if (WallTypesHelper.wallFullEightDirections.Contains(typeAsInt))
        {
            tile = wallFull;
        }
        else if (WallTypesHelper.wallBottmEightDirections.Contains(typeAsInt))
        {
            tile = wallBottom;
        }

        if (tile != null)
        {
            PaintSingleTile(wallTilemap, tile, position);
        }
    }
}
