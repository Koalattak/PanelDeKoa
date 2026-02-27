using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

enum PanelColours
{
    Red,
    Green, 
    Blue,
    Yellow,
    Purple,
    DarkBlue,
    None
};

public struct Position
{
    public int vertical;
    public int horizontal;

    //Constructor
    public Position(int verticalInput, int horizontalInput) : this()
    {
        this.vertical = verticalInput;
        this.horizontal = horizontalInput;
    }
};

public class BaseGameplay : MonoBehaviour
{
    //Base Stats For Panels
    [SerializeField] private GameObject[] m_panelArray;
    [SerializeField] private Transform m_leftSpawnPoint;
    const int m_numberofHorizontalPanels = 6;
    const int m_numberofVerticalPanels = 12;

    //Vertical Movement Stats 
    private float m_moveTime;
    [SerializeField] private float m_normalMoveTime;
    [SerializeField] private float m_fastMoveTime;
    const int m_numberOfPixels = 16;
    private int m_pixelsMovedUp;

    //Board Limits
    [SerializeField] private float m_horizontalLimits;
    [SerializeField] private float m_verticalLimitUp;
    [SerializeField] private float m_verticalLimitDown;


    //Array Details
    private readonly List<GameObject> m_panelsScreenInArray = new();
    private readonly PanelColours[,] gameBoard =
    {
        {PanelColours.None, PanelColours.None, PanelColours.None, PanelColours.None, PanelColours.None, PanelColours.None},
        {PanelColours.None, PanelColours.None, PanelColours.None, PanelColours.None, PanelColours.None, PanelColours.None},
        {PanelColours.None, PanelColours.None, PanelColours.None, PanelColours.None, PanelColours.None, PanelColours.None},
        {PanelColours.None, PanelColours.None, PanelColours.None, PanelColours.None, PanelColours.None, PanelColours.None},
        {PanelColours.None, PanelColours.None, PanelColours.None, PanelColours.None, PanelColours.None, PanelColours.None},
        {PanelColours.None, PanelColours.None, PanelColours.None, PanelColours.None, PanelColours.None, PanelColours.None},
        {PanelColours.None, PanelColours.None, PanelColours.None, PanelColours.None, PanelColours.None, PanelColours.None},
        {PanelColours.None, PanelColours.None, PanelColours.None, PanelColours.None, PanelColours.None, PanelColours.None},
        {PanelColours.None, PanelColours.None, PanelColours.None, PanelColours.None, PanelColours.None, PanelColours.None},
        {PanelColours.None, PanelColours.None, PanelColours.None, PanelColours.None, PanelColours.None, PanelColours.None},
        {PanelColours.None, PanelColours.None, PanelColours.None, PanelColours.None, PanelColours.None, PanelColours.None},
        {PanelColours.None, PanelColours.None, PanelColours.None, PanelColours.None, PanelColours.None, PanelColours.None}
    };
 

    //SelectorDetails
    private Position m_selector1Pos = new(12, 4);
    private Position m_selector2Pos = new(12, 5);
    [SerializeField] private GameObject selector1;
    [SerializeField] private GameObject selector2;

    [SerializeField] private GameObject testObject;



    void Start()
    {
        m_moveTime = m_normalMoveTime;
        SpawnLine();
        StartCoroutine(MovementTimer());
    }

    void Update()
    {
        //Checks if the selector is going to be too high and Lowers it's position
        if(selector1.transform.position.y > m_verticalLimitUp)
        {
            selector1.transform.position = selector1.transform.position + new Vector3(0, -1, 0);
            selector2.transform.position = selector2.transform.position + new Vector3(0, -1, 0);
        }
    }

    private void OnSpeedUp(InputValue value)
    {
        //Change to Accomodate for on and Off
        float input = value.Get<float>();
        if(input > 0)
        {
            m_moveTime = m_fastMoveTime;
        }
        else
        {
            m_moveTime = m_normalMoveTime;
        }
    }
    private void OnSwap()
    {
        SwapPanelsArray();
    }

    void OnMove(InputValue value)
    {
        var input = value.Get<Vector2>();
        MoveSelector(input.x, input.y);
    }

    private void MoveSelector(float xMovement, float yMovement)
    {
        //Moves the Visual Selector
        if(selector1.transform.position.x + xMovement > m_horizontalLimits && selector2.transform.position.x + xMovement < -m_horizontalLimits
            && selector1.transform.position.y + yMovement > m_verticalLimitDown && selector1.transform.position.y + yMovement < m_verticalLimitUp)
        {
            selector1.transform.position = selector1.transform.position + new Vector3(xMovement, yMovement, 0);
            selector2.transform.position = selector2.transform.position + new Vector3(xMovement, yMovement, 0);
        }

        yMovement = -yMovement;

        //Move the Array Selector Positions
        if (m_selector1Pos.vertical + (int)yMovement >= 0 && m_selector1Pos.vertical + (int)yMovement < m_numberofVerticalPanels
            && m_selector1Pos.horizontal + (int)xMovement >= 0 && m_selector1Pos.horizontal + (int)xMovement < m_numberofHorizontalPanels)
        {
            m_selector1Pos.horizontal += (int)xMovement;
            m_selector2Pos.horizontal += (int)xMovement;
            m_selector1Pos.vertical += (int)yMovement;
            m_selector2Pos.vertical += (int)yMovement;
            
        }
    }

    private void SpawnLine()
    {
        MoveArrayUp();

        //Spawns Panels in the Array
        for (int tileSpawnedCounter = 0; tileSpawnedCounter < m_numberofHorizontalPanels; tileSpawnedCounter++)
        {
            //Vector3 spawnPosition = new(m_leftSpawnPoint.position.x + tileSpawnedCounter, m_leftSpawnPoint.position.y, m_leftSpawnPoint.position.z);
            int randomPanelToSpawn = Random.Range(0, m_panelArray.Length - 1);

            gameBoard[11, tileSpawnedCounter] = IntToColourEnum(randomPanelToSpawn);
        }

        //Prints the Field in the Console For Testing
        /*
        for (int i = 0; i < m_numberofVerticalPanels; i++)
        {
            Debug.Log(gameBoard[i, 0] + " " + gameBoard[i, 1] + " " + gameBoard[i, 2] + " " + gameBoard[i, 3] + " " + gameBoard[i, 4] + " " + gameBoard[i, 5]);
        }
        Debug.Log("                                                   ");
        Debug.Log(" " + m_selector1Pos.horizontal + " " + m_selector1Pos.vertical + " " + m_selector2Pos.horizontal + " " + m_selector2Pos.vertical);
        */
        ShowGame();
    }

    private PanelColours IntToColourEnum(int index)
    {
        switch (index)
        {
            case 0: return PanelColours.Red;
            case 1: return PanelColours.Green;
            case 2: return PanelColours.Blue;
            case 3: return PanelColours.Purple;
            case 4: return PanelColours.Yellow;
            case 5: return PanelColours.DarkBlue;
            case 6: return PanelColours.None;
            default:
                break;
        }
        return PanelColours.None;
    }

    private int ColourEnumToInt(PanelColours colour)
    {
        switch (colour)
        {
            case PanelColours.Red: return 0;
            case PanelColours.Green: return 1;
            case PanelColours.Blue: return 2;
            case PanelColours.Purple: return 3;
            case PanelColours.Yellow: return 4;
            case PanelColours.DarkBlue: return 5;
            case PanelColours.None: return 6;
            default:
                break;
        }
        return 6;
    }

    private void MoveArrayUp()
    {
        //Checks if the Game Is Over
        for (int k = 0; k < m_numberofHorizontalPanels; k++)
        {
            if (gameBoard[0, k] != PanelColours.None)
            {
                Debug.Log("You Lost");
                Time.timeScale = 0;
                return;
            }
        }

        //Moves Every Element of the Array Up (Because the Array is Vertically Reversed -> Moving up = -1)
        for (int i = 1; i < m_numberofVerticalPanels; i++)
        {
            for (int j = 0; j < m_numberofHorizontalPanels; j++)
            {
                gameBoard[i - 1, j] = gameBoard[i, j];
            }
        }

        //Moves the Selector to Follow the Array's Movement
        if(m_selector1Pos.vertical > 0)
        {
            m_selector1Pos.vertical--;
            m_selector2Pos.vertical--;
        }
    }

    IEnumerator MovementTimer()
    {
        yield return new WaitForSeconds(m_moveTime);
        VerticalMovement();
    }

    private void VerticalMovement()
    {
        //Checks if it is Time to Spawn a new Line
        if (m_pixelsMovedUp >= m_numberOfPixels)
        {
            m_pixelsMovedUp = 0;
            SpawnLine();
        }

        //Move Up by One Pixel
        foreach(GameObject panel in m_panelsScreenInArray)
        {
            panel.transform.position = panel.transform.position + new Vector3(0, panel.transform.localScale.y/m_numberOfPixels, 0);
        }
        selector1.transform.position = selector1.transform.position + new Vector3(0, selector1.transform.localScale.y / m_numberOfPixels, 0);
        selector2.transform.position = selector2.transform.position + new Vector3(0, selector2.transform.localScale.y / m_numberOfPixels, 0);

        m_pixelsMovedUp++;

        //Calls the Coroutine to Loop the Check
        StartCoroutine(MovementTimer());
    }

    private void SwapPanelsArray()
    {
        //Swaps Panels Under Both Selectors in the Array
        PanelColours buffer;
        buffer = gameBoard[m_selector1Pos.vertical, m_selector1Pos.horizontal];
        gameBoard[m_selector1Pos.vertical, m_selector1Pos.horizontal] = gameBoard[m_selector2Pos.vertical, m_selector2Pos.horizontal];
        gameBoard[m_selector2Pos.vertical, m_selector2Pos.horizontal] = buffer;

        //Checks if the Panel has to Move Vertically
        GravityCheckArray(m_selector2Pos);
        GravityCheckArray(m_selector1Pos);
        //Checks for any cleared Panel
        ArrayLineCheck(m_selector1Pos);
        ArrayColumnCheck(m_selector1Pos);
        ArrayColumnCheck(m_selector2Pos);

        ShowGame();
    }
    private void ArrayColumnCheck(Position checkOrigin)
    {
        int counter = 1;
        List<Position> toDestroy = new();
        List<Position> toDestroyTemp = new();

        //Loops the Whole Column to Check for Cleared Panels
        for (int verticalIndex = 0; verticalIndex < m_numberofVerticalPanels - 1; verticalIndex++)
        {
            if (gameBoard[verticalIndex, checkOrigin.horizontal] == gameBoard[verticalIndex + 1, checkOrigin.horizontal])
            {
                counter++;
                toDestroyTemp.Add(new Position(verticalIndex, checkOrigin.horizontal));
            }
            else
            {
                //When at Least 3 Panels are Aligned -> Add them to the Destroy Array
                if (counter >= 3)
                {
                    toDestroyTemp.Add(new Position(verticalIndex, checkOrigin.horizontal));

                    foreach (Position panelPosition in toDestroyTemp)
                    {
                        toDestroy.Add(panelPosition);
                    }
                }
                counter = 1;
                toDestroyTemp.Clear();
            }
        }
        if (counter >= 3)
        {
            toDestroyTemp.Add(new Position(m_numberofVerticalPanels - 1, checkOrigin.horizontal));

            foreach (Position panelPosition in toDestroyTemp)
            {
                toDestroy.Add(panelPosition);
            }
        }

        if (toDestroy.Count > 0)
        {
            //"Destroy" Everything in the Destroy Array
            foreach (Position panelPosition in toDestroy)
            {
                gameBoard[panelPosition.vertical, panelPosition.horizontal] = PanelColours.None;
            }
            GravityCheckArray(checkOrigin);
        }
    }

    private void ArrayLineCheck(Position checkOrigin)
    {
        int counter = 1;
        List<Position> toDestroy = new();
        List<Position> toDestroyTemp = new();

        //Loops the Whole Row to Check for Cleared Panels
        for (int horizontalIndex = 0; horizontalIndex < m_numberofHorizontalPanels - 1; horizontalIndex++)
        {
            if (gameBoard[checkOrigin.vertical, horizontalIndex] == gameBoard[checkOrigin.vertical, horizontalIndex + 1])
            {
                counter ++;
                toDestroyTemp.Add(new Position(checkOrigin.vertical, horizontalIndex));
            }
            else
            {
                //When at Least 3 Panels are Aligned -> Add them to the Destroy Array
                if (counter >= 3)
                {
                    toDestroyTemp.Add(new Position(checkOrigin.vertical, horizontalIndex));

                    foreach (Position panelPosition in toDestroyTemp)
                    {
                        toDestroy.Add(panelPosition);
                    }
                }
                counter = 1;
                toDestroyTemp.Clear();
            }
        }
        if (counter >= 3)
        {
            toDestroyTemp.Add(new Position(checkOrigin.vertical, m_numberofHorizontalPanels - 1));

            foreach (Position panelPosition in toDestroyTemp)
            {
                toDestroy.Add(panelPosition);
            }
        }

        if (toDestroy.Count > 0)
        {
            //"Destroy" Everything in the Destroy Array
            foreach (Position panelPosition in toDestroy)
            {
                gameBoard[panelPosition.vertical, panelPosition.horizontal] = PanelColours.None;
                GravityCheckArray(panelPosition);
            }
        }
    }

    private void GravityCheckArray(Position felixPos)
    {
        List<int> panelsToGravify = new();

        //Finds the Panels Present in the Column and Adds their Vertical Position in the GravifyArray
        for (int verticalIndex = m_numberofVerticalPanels - 2; verticalIndex > 0; verticalIndex--)
        {
            if(gameBoard[verticalIndex, felixPos.horizontal] != PanelColours.None)
            {
                panelsToGravify.Add(verticalIndex);
            }
        }


        List<int> panelsMoved = new();
        //Checks every Panel in the Column to see if they can be Lowered, if so, Lower them Until they are Above Another Panel in the Array
        foreach (int panelVerticalPos in panelsToGravify)
        {
            //Debug.Log("Vertical : " + panelVerticalPos);
            bool isOnFloor = false;
            bool hasMoved = false;
            int height = panelVerticalPos;

            while(!isOnFloor)
            {
                height++;
                if (height < m_numberofVerticalPanels && gameBoard[height, felixPos.horizontal] == PanelColours.None)
                {
                    gameBoard[height, felixPos.horizontal] = gameBoard[height - 1, felixPos.horizontal];
                    gameBoard[height - 1, felixPos.horizontal] = PanelColours.None;
                    hasMoved = true;
                }
                else
                {
                    if(hasMoved)
                    {
                        panelsMoved.Add(height);
                    }
                    isOnFloor = true;
                }
            }
        }


        if(panelsMoved.Count > 0)
        {
            foreach(int panelVerticalPos in panelsMoved)
            {
                //Used to Avoid Exceeding Array Size
                if (panelVerticalPos < m_numberofVerticalPanels)
                {
                    ArrayColumnCheck(new Position(panelVerticalPos, felixPos.horizontal));
                    ArrayLineCheck(new Position(panelVerticalPos, felixPos.horizontal));
                }
            }
        }
    }

    /// <summary>
    /// TODO:
    /// Modify (not optimal and not F0unctionning Correctly)
    /// </summary>
    private void ShowGame()
    {
        foreach(GameObject panel in m_panelsScreenInArray)
        {
            Destroy(panel);
        }
        m_panelsScreenInArray.Clear();

        for(int verticalIndex = 0; verticalIndex < m_numberofVerticalPanels; verticalIndex++)
        {
            for(int horizontalIndex = 0; horizontalIndex < m_numberofHorizontalPanels; horizontalIndex++)
            {
                float spawnHeight = m_numberofVerticalPanels - verticalIndex - 1 + ((float)m_pixelsMovedUp / m_numberOfPixels);

                GameObject spawnedPanel = Instantiate(m_panelArray[ColourEnumToInt(gameBoard[verticalIndex, horizontalIndex])], 
                    m_leftSpawnPoint.position + new Vector3(horizontalIndex, spawnHeight, 0), m_leftSpawnPoint.rotation);

                spawnedPanel.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1f);
                m_panelsScreenInArray.Add(spawnedPanel);
            }
        }
    }
}