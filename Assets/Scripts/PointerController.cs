using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerController : MonoBehaviour

{
    public GameController Controller;
    public GameObject Board;
	public SocketCommunication SocketCommunication;
    public string CurrentPosition;

    private List<string> PossibleMoves;
    private Vector3 screenPoint;
    private Vector3 offset;
    private bool isMoving;
    private float ZPosition;
    private Camera MainCamera;
    private string startTile;
    private string lastTile;

    // Start is called before the first frame update
    void Start()
    {
        MainCamera = Controller.MainCamera;
        ZPosition = MainCamera.WorldToScreenPoint(transform.position).z;
    }

    public void MoveUp()
    {
        int tmpPos;
        tmpPos = CurrentPosition.ToCharArray()[1] - '0' + 1;
        if (tmpPos == 9)
        {
            tmpPos = 1;
        }
        lastTile = CurrentPosition.Remove(1) + tmpPos.ToString();
        print(lastTile);
    }

    public void MoveDown()
    {
        int tmpPos;
        tmpPos = CurrentPosition.ToCharArray()[1] - '0' - 1;
        if (tmpPos == 0)
        {
            tmpPos = 8;
        }
        lastTile = CurrentPosition.Remove(1) + tmpPos.ToString();
        print(lastTile);
    }

    public void MoveRight()
    {
        char tmpPos;
        tmpPos = CurrentPosition.ToCharArray()[0];
        tmpPos++;
        if (tmpPos == 'i')
        {
            tmpPos = 'a';
        }
        lastTile = tmpPos.ToString() + CurrentPosition.Remove(0, 1);
        print(lastTile);
    }

    public void MoveLeft()
    {
        char tmpPos;
        tmpPos = CurrentPosition.ToCharArray()[0];
        tmpPos--;
        if (tmpPos < 'a')
        {
            tmpPos = 'h';
        }
        lastTile = tmpPos.ToString() + CurrentPosition.Remove(0, 1);
        print(lastTile);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, ZPosition);
        transform.position = MainCamera.ScreenToWorldPoint(position + new Vector3(offset.x, 0));
        Vector3 check_position = MainCamera.ScreenToWorldPoint(position + new Vector3(offset.x, 0));
        check_position.y = 0;

        if (Input.GetKeyDown("up") || SocketCommunication.signal == "UP")
        {
            MoveUp();
            SocketCommunication.signal = "None";
        }

        if (Input.GetKeyDown("down") || SocketCommunication.signal == "DOWN")
        {
            MoveDown();
            SocketCommunication.signal = "None";
        }

        if (Input.GetKeyDown("right") || SocketCommunication.signal == "RIGHT")
        {
            MoveRight();
            SocketCommunication.signal = "None";
        }

        if (Input.GetKeyDown("left") || SocketCommunication.signal == "LEFT")
        {
            MoveLeft();
            SocketCommunication.signal = "None";
        }

        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    MeshCollider TileCollider = Controller.Tiles[i, j].transform.GetChild(0).gameObject.GetComponent<MeshCollider>();
                    if (TileCollider.bounds.Contains(check_position))
                    {
                        lastTile = Controller.Tiles[i, j].name;
                        break;
                    }
                }
            }
        }

        if(lastTile == null)
        {
            lastTile = CurrentPosition;
        }

        ChangePosition(lastTile);
    }

    void ChangePosition(string TileName)
    {
        List<int> indices = Controller.tile_to_index(TileName);
        if (indices[0] > 8 || indices[0] < 0 || indices[1] > 8 || indices[1] < 0)
        {
            Debug.Log("Wrong tile name");
        }
        GameObject tmp = Board.transform.Find("Plane").gameObject;
        CurrentPosition = TileName;
        GameObject Tile = Controller.Tiles[indices[0], indices[1]];
        transform.Translate(Tile.transform.position - transform.position);
    }
}
