using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FigureController : MonoBehaviour
{
    public string Type;
    public string CurrentPosition;
    public GameObject Board;

    private List<string> PossibleMoves;

    // Start is called before the first frame update
    void Start()
    {
        PossibleMoves = new List<string>();
    }

    // Update is called once per frame
    void Update()
    {
        ChangePosition();
    }

    void ChangePosition()
	{
        GameObject tmp = Board.transform.Find("Plane").gameObject;
        GameObject Tile = tmp.transform.Find(CurrentPosition).gameObject;
        transform.Translate(Tile.transform.position-transform.position);
	}
}
