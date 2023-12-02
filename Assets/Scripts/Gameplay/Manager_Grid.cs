using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager_Grid : MonoBehaviour
{
    [Tooltip("Prefab object for the cells that make up the grid")]
    [SerializeField] private GameObject CellPrefab;

    [Tooltip("Number of cells wide and high to make the grid")]
    [SerializeField] private int width, hight;

    [Tooltip("Vector 3 representing the base center of grid before any x or y offsetting")]
    [SerializeField] private Vector3 center;

    [Tooltip("How much % of the top of the sceen is blank. " +
        "0 would make the grid perfectly center on screen," +
        " 20 would make the top 20% of the screen blank space. " +
        "This would reduce the over all size of the grid to fit the extra space.")]
    [SerializeField] private float aboveScreenBlankSpacePercent = 20;

    private Vector3[] aboveGridBlockPositions;

    private float XYScaleFactor = 0;

    //The grid itself. 2D array of the grid cell objects.
    private GameObject[,] Grid;

    public GameObject[,] grid { get => Grid;}
    public Vector3[] AboveGridBlockPositions { get => aboveGridBlockPositions; }
    public float xYScaleFactor { get => XYScaleFactor; }
    public int Width { get => width;}
    public int Hight { get => hight;}

    // Start is called before the first frame update
    void Start()
    {
        //init global
        

        //GridSetUp();//probably remove this call here and move it to manager_game or have something from manager_game call this class.
    }

    public void GridSetUp()
    {
        if(Grid == null)
        {
            Grid = new GameObject[width, hight];
            aboveGridBlockPositions = new Vector3[width];
        }
        Vector2 topRightCorner = new Vector2(1, 1);

        Vector2 edgeVector = Camera.main.ViewportToWorldPoint(topRightCorner);

        float screenHeightScaleOffset = (edgeVector.y * 2) * (aboveScreenBlankSpacePercent / 100f);
        center = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, (Screen.height / 2) - screenHeightScaleOffset, Camera.main.nearClipPlane));
        //Debug.Log("screen Height: " + Screen.height + "  Offest Height: " + screenHeightScaleOffset);

        float Sheight = (edgeVector.y * 2) - ((edgeVector.y * 2) * aboveScreenBlankSpacePercent / 100f);
        float Swidth = edgeVector.x * 2;

        float cellSizeY = CellPrefab.GetComponent<SpriteRenderer>().bounds.size.y;
        float cellSizeX = CellPrefab.GetComponent<SpriteRenderer>().bounds.size.x;

        float oldCellSizeY = cellSizeY;
        cellSizeY = cellSizeY / ((cellSizeY * hight) / Sheight);
        cellSizeX = cellSizeY;

        XYScaleFactor = 1 / (oldCellSizeY / cellSizeY);

        float baseY = ((center.y + ((hight * cellSizeY) / 2)) - (cellSizeY / 2)) - 0.9f;
        float baseX = (center.x - ((width * cellSizeX) / 2)) + (cellSizeX / 2);
        //Debug.Log("Base y =  " + baseY + " centerY = " + center.y + " height = " + hight + " cellSizeY = " + cellSizeY);

        for (int i = 0; i < hight; i++)
        {
            for (int j = 0; j < width; j++)
            {
                Vector3 instPos = new Vector3(baseX + (j * cellSizeX), baseY - (i * cellSizeY), 0);

                GameObject cell = Instantiate(CellPrefab);
                cell.transform.position = instPos;
                cell.transform.localScale = new Vector3(XYScaleFactor, XYScaleFactor, 1);

                Grid[j, i] = cell;
                cell.GetComponent<Grid_Cell>().grid_Location = new Vector2Int(j, i);
                if (i == 0)
                {
                    aboveGridBlockPositions[j] = new Vector3(cell.transform.position.x, cell.transform.position.y + cellSizeY, 0);
                    //Debug.Log(aboveGridBlockPositions[j]);
                }
            }
        }
    }
    public void updateGrid()
    {
        
        for(int i = Width - 1; i >= 0; i--)
        {
            Grid_Cell lowestCell = null;
            int difference = 0;
            for (int j = Hight - 1; j >= 0; j--)
            {
                if (lowestCell != null && grid[i, j].GetComponent<Grid_Cell>().Contained_Letter_Block != null)
                {
                    if(difference == 0)
                    {
                        difference = lowestCell.grid_Location.y - grid[i, j].GetComponent<Grid_Cell>().grid_Location.y;
                    }
                    Debug.Log("i = " + i + ": j = " + j + "diff = " + difference);
                    grid[i, j + difference].GetComponent<Grid_Cell>().SetLetterBlock(
                            grid[i, j].GetComponent<Grid_Cell>().Contained_Letter_Block);
                    grid[i, j].GetComponent<Grid_Cell>().Contained_Letter_Block = null;
                }
                else if(lowestCell == null && grid[i,j].GetComponent<Grid_Cell>().Contained_Letter_Block == null)
                {
                    lowestCell = grid[i, j].GetComponent<Grid_Cell>();
                    Debug.Log("Low Cell");
                    //break;
                }
                
            }
        }
        
    }
}
