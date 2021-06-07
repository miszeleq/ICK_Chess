using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FigureController : MonoBehaviour
{
    public string Type;
    public string CurrentPosition;
    public string color;
    public bool firstMove;
    public GameController Controller;
    public GameObject Board;
    public GameObject Pointer;
    public Rigidbody m_Rigidbody;

    private List<string> PossibleMoves;
    private Vector3 screenPoint;
    private Vector3 offset;
    public bool Clicked;
    public bool isMoving;
    public bool Active;
    private float ZPosition;
    private Camera MainCamera;
    private string startTile;
    private string lastTile;
    private GameObject Cursor;

    // Start is called before the first frame update
    void Start()
    {
        MainCamera = Controller.MainCamera;
        firstMove = true;
        isMoving = false;
        Clicked = false;
        Active = false;
        PossibleMoves = new List<string>();
        ZPosition = MainCamera.WorldToScreenPoint(transform.position).z;
        //controllerScript cur = Cursor.GetComponent<PointerController>();
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (isMoving)
        {
            Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, ZPosition);
            transform.position = MainCamera.ScreenToWorldPoint(position + new Vector3(offset.x, 0));
            Vector3 check_position = MainCamera.ScreenToWorldPoint(position + new Vector3(offset.x, 0));
            check_position.y = 0;

            for(int i=0; i<8; i++)
            {
                for(int j=0; j<8; j++)
                {
                    MeshCollider TileCollider = Controller.Tiles[i, j].transform.GetChild(0).gameObject.GetComponent<MeshCollider>();
                    if (TileCollider.bounds.Contains(check_position)){
                        lastTile = Controller.Tiles[i, j].name;
                        break;
                    }
                }
            }
        }
        */

        if (Active)
        {
            if (Input.GetKeyDown("return") || Input.GetMouseButtonDown(0))
            {
                FindPossibleMoves();
                PossibleMoves.ForEach(Debug.Log);
                startTile = CurrentPosition;
                isMoving = true;
                print("aa");
            }
            
        }

        if (isMoving)
        {
            if (Pointer.GetComponent<PointerController>().CurrentPosition != CurrentPosition)
            {
                if (Input.GetKeyDown("return") || Input.GetMouseButtonDown(0))
                {
                    if (PossibleMoves.Contains(Pointer.GetComponent<PointerController>().CurrentPosition))
                    {
                        ChangePosition(Pointer.GetComponent<PointerController>().CurrentPosition);
                    }
                    else
                    {
                        ChangePosition(startTile);
                    }
                    isMoving = false;
                    Clicked = false;
                    print("bb");
                }
            }
            
        }
    }

    void OnTriggerEnter()
    {
        Active = true;
    }

    void OnTriggerExit()
    {
        Active = false;
    }

    TileCheck GetTileCheck(int ind_1, int ind_2)
    {
        return Controller.Tiles[ind_1, ind_2].gameObject.GetComponent<TileCheck>();
    }

    FigureController GetFigureController(TileCheck tile)
    {
        return tile.Figure.gameObject.GetComponent<FigureController>();
    }

    void checkMove(int ind_1, int ind_2)
    {
        TileCheck Tile = GetTileCheck(ind_1, ind_2);
        if (Tile.Figure != null)
        {
            if (GetFigureController(Tile).color != color)
            {
                PossibleMoves.Add(Controller.index_to_tile(ind_1, ind_2));
            }
        }
    }

    void checkMoveWithoutCapture(int ind_1, int ind_2)
    {
        TileCheck Tile = GetTileCheck(ind_1, ind_2);
        if (Tile.Figure == null)
        {
            PossibleMoves.Add(Controller.index_to_tile(ind_1, ind_2));
        }
        else
        {
            if (GetFigureController(Tile).color != color)
            {
                PossibleMoves.Add(Controller.index_to_tile(ind_1, ind_2));
            }
        }
    }

    void CaptureFigure(GameObject Figure)
    {
        Destroy(Figure.gameObject);
    }

    void FindPossibleMoves()
    {
        PossibleMoves = new List<string>();
        List<int> indices = Controller.tile_to_index(CurrentPosition);
        switch (Type)
        {
            // pawn
            case "P":
            case "p":
                if (color == "white")
                {
                    if (indices[1] + 1 <= 7)
                    {
                        TileCheck Tile = GetTileCheck(indices[0], indices[1] + 1);
                        if (Tile.Figure == null)
                        {
                            PossibleMoves.Add(Controller.index_to_tile(indices[0], indices[1] + 1));
                        }

                        if (indices[0] + 1 <= 7)
                        {
                            checkMove(indices[0] + 1, indices[1] + 1);
                        }
                        if (indices[0] - 1 >= 0)
                        {
                            checkMove(indices[0] - 1, indices[1] + 1);
                        }
                    }

                    if(firstMove == true)
                    {
                        if (indices[1] + 2 <= 7)
                        {
                            TileCheck Tile = GetTileCheck(indices[0], indices[1] + 2);
                            if (Tile.Figure == null)
                            {
                                PossibleMoves.Add(Controller.index_to_tile(indices[0], indices[1] + 2));
                            }
                        }
                    }
                }
                else if (color == "black")
                {
                    if (indices[1] - 1 >= 0)
                    {
                        TileCheck Tile = GetTileCheck(indices[0], indices[1] - 1);
                        if (Tile.Figure == null)
                        {
                            PossibleMoves.Add(Controller.index_to_tile(indices[0], indices[1] - 1));
                        }

                        if (indices[0] + 1 <= 7)
                        {
                            checkMove(indices[0] + 1, indices[1] - 1);
                        }
                        if (indices[0] - 1 >= 0)
                        {
                            checkMove(indices[0] - 1, indices[1] - 1);
                        }
                    }

                    if (firstMove == true)
                    {
                        if (indices[1] - 2 >= 0)
                        {
                            TileCheck Tile = GetTileCheck(indices[0], indices[1] - 2);
                            if (Tile.Figure == null)
                            {
                                PossibleMoves.Add(Controller.index_to_tile(indices[0], indices[1] - 2));
                            }
                        }
                    }
                }


                break;
            // rook
            case "R":
            case "r":
                // rows
                // right
                for (int row = indices[0] + 1; row < 8; row++)
                {
                    TileCheck Tile = GetTileCheck(row, indices[1]);
                    if (Tile.Figure == null)
                    {
                        PossibleMoves.Add(Controller.index_to_tile(row, indices[1]));
                    }
                    else
                    {
                        if (GetFigureController(Tile).color != color)
                        {
                            PossibleMoves.Add(Controller.index_to_tile(row, indices[1]));
                        }
                        break;
                    }
                }
                // left
                for (int row = indices[0] - 1; row >= 0; row--)
                {
                    TileCheck Tile = GetTileCheck(row, indices[1]);
                    if (Tile.Figure == null)
                    {
                        PossibleMoves.Add(Controller.index_to_tile(row, indices[1]));
                    }
                    else
                    {
                        if (GetFigureController(Tile).color != color)
                        {
                            PossibleMoves.Add(Controller.index_to_tile(row, indices[1]));
                        }
                        break;
                    }
                }
                // columns
                // up
                for (int col = indices[1] + 1; col < 8; col++)
                {
                    TileCheck Tile = GetTileCheck(indices[0], col);
                    if (Tile.Figure == null)
                    {
                        PossibleMoves.Add(Controller.index_to_tile(indices[0], col));
                    }
                    else
                    {
                        if (GetFigureController(Tile).color != color)
                        {
                            PossibleMoves.Add(Controller.index_to_tile(indices[0], col));
                        }
                        break;
                    }
                }
                // down
                for (int col = indices[1] - 1; col >= 0; col--)
                {
                    TileCheck Tile = GetTileCheck(indices[0], col);
                    if (Tile.Figure == null)
                    {
                        PossibleMoves.Add(Controller.index_to_tile(indices[0], col));
                    }
                    else
                    {
                        if (GetFigureController(Tile).color != color)
                        {
                            PossibleMoves.Add(Controller.index_to_tile(indices[0], col));
                        }
                        break;
                    }
                }
                break;
            // knight
            case "N":
            case "n":
                // 2 up 1 left
                if (indices[0] - 1 >= 0 && indices[1] + 2 <= 7)
                {
                    
                    checkMoveWithoutCapture(indices[0] - 1, indices[1] + 2);
                }
                // 2 up 1 right
                if (indices[0] + 1 <= 7 && indices[1] + 2 <= 7)
                {
                    checkMoveWithoutCapture(indices[0] + 1, indices[1] + 2);
                }
                // 2 down 1 left
                if (indices[0] - 1 >= 0 && indices[1] - 2 >= 0)
                {
                    checkMoveWithoutCapture(indices[0] - 1, indices[1] - 2);
                }
                // 2 down 1 right
                if (indices[0] + 1 <= 7 && indices[1] - 2 >= 0)
                {
                    checkMoveWithoutCapture(indices[0] + 1, indices[1] - 2);
                }
                // 2 right 1 up
                if (indices[0] + 2 <= 7 && indices[1] + 1 <= 7)
                {
                    checkMoveWithoutCapture(indices[0] + 2, indices[1] + 1);
                }
                // 2 right 1 down
                if (indices[0] + 2 <= 7 && indices[1] - 1 >= 0)
                {
                    checkMoveWithoutCapture(indices[0] + 2, indices[1] - 1);
                }
                // 2 left 1 up
                if (indices[0] - 2 >= 0 && indices[1] + 1 <= 7)
                {
                    checkMoveWithoutCapture(indices[0] - 2, indices[1] + 1);
                }
                // 2 left 1 down
                if (indices[0] - 2 >= 0 && indices[1] - 1 >= 0)
                {
                    checkMoveWithoutCapture(indices[0] - 2, indices[1] - 1);
                }
                break;
            // bishop
            case "B":
            case "b":
                // up right
                int cols = indices[1] + 1;
                for (int row = indices[0] + 1; row < 8; row++)
                {
                    if (cols <= 7)
                    {
                        TileCheck Tile = GetTileCheck(row, cols);
                        if (Tile.Figure == null)
                        {
                            PossibleMoves.Add(Controller.index_to_tile(row, cols));
                        }
                        else
                        {
                            if (GetFigureController(Tile).color != color)
                            {
                                PossibleMoves.Add(Controller.index_to_tile(row, cols));
                            }
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                    cols++;
                }
                // down left
                cols = indices[1] - 1;
                for (int row = indices[0] - 1; row >= 0; row--)
                {
                    if (cols >= 0)
                    {
                        TileCheck Tile = GetTileCheck(row, cols);
                        if (Tile.Figure == null)
                        {
                            PossibleMoves.Add(Controller.index_to_tile(row, cols));
                        }
                        else
                        {
                            if (GetFigureController(Tile).color != color)
                            {
                                PossibleMoves.Add(Controller.index_to_tile(row, cols));
                            }
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                    cols--;
                }
                // up left
                cols = indices[1] + 1;
                for (int row = indices[0] - 1; row >= 0; row--)
                {
                    if (cols <= 7)
                    {
                        TileCheck Tile = GetTileCheck(row, cols);
                        if (Tile.Figure == null)
                        {
                            PossibleMoves.Add(Controller.index_to_tile(row, cols));
                        }
                        else
                        {
                            if (GetFigureController(Tile).color != color)
                            {
                                PossibleMoves.Add(Controller.index_to_tile(row, cols));
                            }
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                    cols++;
                }
                // down right
                cols = indices[1] - 1;
                for (int row = indices[0] + 1; row <= 7; row++)
                {
                    if (cols >= 0)
                    {
                        TileCheck Tile = GetTileCheck(row, cols);
                        if (Tile.Figure == null)
                        {
                            PossibleMoves.Add(Controller.index_to_tile(row, cols));
                        }
                        else
                        {
                            if (GetFigureController(Tile).color != color)
                            {
                                PossibleMoves.Add(Controller.index_to_tile(row, cols));
                            }
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                    cols--;
                }
                break;
            // king
            case "K":
            case "k":
                // right
                if (indices[0] + 1 <= 7)
                {
                    checkMoveWithoutCapture(indices[0] + 1, indices[1]);
                }
                // up right
                if (indices[0] + 1 <= 7 && indices[1] + 1 <= 7)
                {
                    checkMoveWithoutCapture(indices[0] + 1, indices[1] + 1);
                }
                // down right
                if (indices[0] + 1 <= 7 && indices[1] - 1 >= 0)
                {
                    checkMoveWithoutCapture(indices[0] + 1, indices[1] - 1);
                }
                // left
                if (indices[0] - 1 >= 0)
                {
                    checkMoveWithoutCapture(indices[0] - 1, indices[1]);
                }
                // up left
                if (indices[0] - 1 >= 0 && indices[1] - 1 >= 0)
                {
                    checkMoveWithoutCapture(indices[0] - 1, indices[1] - 1);
                }
                // down left
                if (indices[0] + 1 <= 7 && indices[1] - 1 >= 0)
                {
                    checkMoveWithoutCapture(indices[0] + 1, indices[1] - 1);
                }
                // down
                if (indices[1] - 1 >= 0)
                {
                    checkMoveWithoutCapture(indices[0], indices[1] - 1);
                }
                // up
                if (indices[1] + 1 <= 7)
                {
                    checkMoveWithoutCapture(indices[0], indices[1] + 1);
                }
                break;
            // queen
            case "Q":
            case "q":
                // up right
                cols = indices[1] + 1;
                for (int row = indices[0] + 1; row < 8; row++)
                {
                    if (cols <= 7)
                    {
                        TileCheck Tile = GetTileCheck(row, cols);
                        if (Tile.Figure == null)
                        {
                            PossibleMoves.Add(Controller.index_to_tile(row, cols));
                        }
                        else
                        {
                            if (GetFigureController(Tile).color != color)
                            {
                                PossibleMoves.Add(Controller.index_to_tile(row, cols));
                            }
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                    cols++;
                }
                // down left
                cols = indices[1] - 1;
                for (int row = indices[0] - 1; row >= 0; row--)
                {
                    if (cols >= 0)
                    {
                        TileCheck Tile = GetTileCheck(row, cols);
                        if (Tile.Figure == null)
                        {
                            PossibleMoves.Add(Controller.index_to_tile(row, cols));
                        }
                        else
                        {
                            if (GetFigureController(Tile).color != color)
                            {
                                PossibleMoves.Add(Controller.index_to_tile(row, cols));
                            }
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                    cols--;
                }
                // up left
                cols = indices[1] + 1;
                for (int row = indices[0] - 1; row >= 0; row--)
                {
                    if (cols <= 7)
                    {
                        TileCheck Tile = GetTileCheck(row, cols);
                        if (Tile.Figure == null)
                        {
                            PossibleMoves.Add(Controller.index_to_tile(row, cols));
                        }
                        else
                        {
                            if (GetFigureController(Tile).color != color)
                            {
                                PossibleMoves.Add(Controller.index_to_tile(row, cols));
                            }
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                    cols++;
                }
                // down right
                cols = indices[1] - 1;
                for (int row = indices[0] + 1; row <= 7; row++)
                {
                    if (cols >= 0)
                    {
                        TileCheck Tile = GetTileCheck(row, cols);
                        if (Tile.Figure == null)
                        {
                            PossibleMoves.Add(Controller.index_to_tile(row, cols));
                        }
                        else
                        {
                            if (GetFigureController(Tile).color != color)
                            {
                                PossibleMoves.Add(Controller.index_to_tile(row, cols));
                            }
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                    cols--;
                }
                // rows
                // right
                for (int row = indices[0] + 1; row < 8; row++)
                {
                    TileCheck Tile = GetTileCheck(row, indices[1]);
                    if (Tile.Figure == null)
                    {
                        PossibleMoves.Add(Controller.index_to_tile(row, indices[1]));
                    }
                    else
                    {
                        if (GetFigureController(Tile).color != color)
                        {
                            PossibleMoves.Add(Controller.index_to_tile(row, indices[1]));
                        }
                        break;
                    }
                }
                // left
                for (int row = indices[0] - 1; row >= 0; row--)
                {
                    TileCheck Tile = GetTileCheck(row, indices[1]);
                    if (Tile.Figure == null)
                    {
                        PossibleMoves.Add(Controller.index_to_tile(row, indices[1]));
                    }
                    else
                    {
                        if (GetFigureController(Tile).color != color)
                        {
                            PossibleMoves.Add(Controller.index_to_tile(row, indices[1]));
                        }
                        break;
                    }
                }
                // columns
                // up
                for (int col = indices[1] + 1; col < 8; col++)
                {
                    TileCheck Tile = GetTileCheck(indices[0], col);
                    if (Tile.Figure == null)
                    {
                        PossibleMoves.Add(Controller.index_to_tile(indices[0], col));
                    }
                    else
                    {
                        if (GetFigureController(Tile).color != color)
                        {
                            PossibleMoves.Add(Controller.index_to_tile(indices[0], col));
                        }
                        break;
                    }
                }
                // down
                for (int col = indices[1] - 1; col >= 0; col--)
                {
                    TileCheck Tile = GetTileCheck(indices[0], col);
                    if (Tile.Figure == null)
                    {
                        PossibleMoves.Add(Controller.index_to_tile(indices[0], col));
                    }
                    else
                    {
                        if (GetFigureController(Tile).color != color)
                        {
                            PossibleMoves.Add(Controller.index_to_tile(indices[0], col));
                        }
                        break;
                    }
                }
                break;
            default:
                Debug.Log("Wrong figure type!");
                break;
        }
    }
    /*
    void OnMouseDown()
    {
        startTile = CurrentPosition;
        BeginDragging();
    }

    void OnMouseUp()
    {
        EndDragging();
        isMoving = false;
    }

    void BeginDragging()
    {
        isMoving = true;
        offset = MainCamera.WorldToScreenPoint(transform.position) - Input.mousePosition;
    }

    void EndDragging()
    {
        isMoving = false;
        Vector3 position = MainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, ZPosition) + new Vector3(offset.x, 0));
        position.y = 0;
        MeshCollider BoardCollider = Board.transform.GetChild(0).gameObject.GetComponent<MeshCollider>();

        if (BoardCollider.bounds.Contains(position)){
            ChangePosition(lastTile);
        } else
        {
            ChangePosition(startTile);
        }
    }
    */
    void ChangePosition(string TileName)
    {
        List<int> indices = Controller.tile_to_index(TileName);
        List<int> current_indices = Controller.tile_to_index(CurrentPosition);

        if (indices[0] > 8 || indices[0] < 0 || indices[1] > 8 || indices[1] < 0)
        {
            Debug.Log("Wrong tile name");
        }

        GameObject tmp = Board.transform.Find("Plane").gameObject;
        GameObject Tile = Controller.Tiles[indices[0], indices[1]];
        transform.Translate(Tile.transform.position - transform.position);

        if (TileName != CurrentPosition)
        {
            TileCheck nextTile = GetTileCheck(indices[0], indices[1]);
            if (nextTile.Figure != null)
            {
                CaptureFigure(nextTile.Figure);
            }
            GetTileCheck(current_indices[0], current_indices[1]).Figure = null;
            nextTile.Figure = gameObject;
        }
        CurrentPosition = TileName;

        if(firstMove == true)
        {
            firstMove = false;
        }

        FindPossibleMoves();
    }
}
