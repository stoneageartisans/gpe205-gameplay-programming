using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int Columns = 10;
    public int Rows = 10;

    public GameObject[] TilePrefabList;
    public GameObject CornerTilePrefab;
    public GameObject SideTilePrefab;

    [Tooltip("The tile's rotation for the upper left corner")]
    public float UpperLeftCornerRotationZ = 0;

    [Tooltip("The tile's rotation for the upper side")]
    public float UpperSideRotationZ = 0;

    // Use this for initialization
    void Start()
    {
        int offset = 10;
        int startX = (offset / 2) - ((Columns / 2) * offset);
        int startY = ((Rows / 2) * offset) - (offset / 2);
        float[] tileRotations = { 0, 90, 180, 270 };

        for(int row = 0; row < Rows; row ++)
        {
            for(int column = 0; column < Columns; column ++)
            {
                GameObject tile = null;
                float rotationZ = 0;
                float positionX = 0;
                float positionY = 0;

                // Upper row
                if(row == 0)
                {
                    // Upper left corner
                    if(column == 0)
                    {
                        tile = Instantiate(CornerTilePrefab);
                        rotationZ = UpperLeftCornerRotationZ;
                    }
                    // Upper right corner
                    else if(column == (Columns - 1))
                    {
                        tile = Instantiate(CornerTilePrefab);
                        rotationZ = UpperLeftCornerRotationZ + 270;
                    }
                    // Upper sides
                    else
                    {
                        tile = Instantiate(SideTilePrefab);
                        rotationZ = UpperSideRotationZ;
                    }
                }
                // Lower row
                else if(row == (Rows - 1))
                {
                    // Lower left corner
                    if(column == 0)
                    {
                        tile = Instantiate(CornerTilePrefab);
                        rotationZ = UpperLeftCornerRotationZ + 90;
                    }
                    // Lower right corner
                    else if(column == (Columns - 1))
                    {
                        tile = Instantiate(CornerTilePrefab);
                        rotationZ = UpperLeftCornerRotationZ + 180;
                    }
                    // Lower sides
                    else
                    {
                        tile = Instantiate(SideTilePrefab);
                        rotationZ = UpperSideRotationZ + 180;
                    }
                }
                else
                {
                    // Left sides
                    if(column == 0)
                    {
                        tile = Instantiate(SideTilePrefab);
                        rotationZ = UpperSideRotationZ + 90;
                    }
                    // Right sides
                    else if(column == (Columns - 1))
                    {
                        tile = Instantiate(SideTilePrefab);
                        rotationZ = UpperSideRotationZ - 90;
                    }
                    // Non-corner & non-side tiles
                    else
                    {
                        // Randomly choose a tile from the list
                        int t = Random.Range(0, TilePrefabList.Length);
                        tile = Instantiate(TilePrefabList[t]);

                        // Randomly choose a rotation
                        int r = Random.Range(0, tileRotations.Length);
                        rotationZ = tileRotations[r];
                    }
                }

                // Set the tiles rotation
                tile.transform.Rotate(0, 0, rotationZ);

                // Set the tiles position
                positionX = startX + (column * offset);
                positionY = startY - (row * offset);
                tile.transform.position = new Vector3(positionX, positionY, 0);

                // Make the tile a parent of the map object
                tile.transform.parent = gameObject.transform;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
