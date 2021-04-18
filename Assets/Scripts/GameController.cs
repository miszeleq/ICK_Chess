using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject Board;
    public Camera MainCamera;
    public GameObject[,] Tiles;

    public string index_to_tile(int i, int j)
    {
        return (char)(97 + i) + (j + 1).ToString();
    }

    public List<int> tile_to_index(string tile)
    {
        List<int> tile_indices = new List<int>();
        tile_indices.Add((int)(tile[0] - 97));
        tile_indices.Add((int)(tile[1] - 49));
        return tile_indices;
    }

    // Start is called before the first frame update
    void Start()
    {
        Tiles = new GameObject[8, 8];

        for (int i = 0; i <8; i++)
        {
            for (int j = 0; j <8; j++)
            {
                string tile_str = index_to_tile(i, j);
                GameObject tmp = Board.transform.Find("Plane").gameObject;
                GameObject Tile = tmp.transform.Find(tile_str).gameObject;
                Bounds TileBounds = Tile.transform.GetChild(0).gameObject.GetComponent<MeshFilter>().mesh.bounds;
                //TileBounds.center = Tile.transform.position;
                //TileBounds.extents = new Vector3(0.1f, 2f, 0.1f);
                Tiles[i, j] = Tile;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
     
    }

}