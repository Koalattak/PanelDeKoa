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

    public Position(int verticalInput, int horizontalInput) : this()
    {
        this.vertical = verticalInput;
        this.horizontal = horizontalInput;
    }
};

public class BaseGameplay : MonoBehaviour
{
    [SerializeField] private GameObject[] m_panelArray;
    [SerializeField] private Transform m_leftSpawnPoint;
    [SerializeField] const int m_numberofHorizontalPanels = 6;
    [SerializeField] const int m_numberofVerticalPanels = 12;
    private float m_moveTime;
    [SerializeField] private float m_normalMoveTime;
    [SerializeField] private float m_fastMoveTime;

    [SerializeField] private float m_horizontalLimits;
    [SerializeField] private float m_verticalLimitUp;
    [SerializeField] private float m_verticalLimitDown;


    //Test
    private List<GameObject> m_panelsScreenInArray = new List<GameObject>();
    private PanelColours[,] gameBoard =
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
    private Position m_selector1Pos;
    private Position m_selector2Pos;
 

    const int m_numberOfPixels = 16;

    private List<GameObject> m_panelsOnScreen = new List<GameObject>();
    [SerializeField] private GameObject selector1;
    [SerializeField] private GameObject selector2;

    [SerializeField] private GameObject testObject;

    private int m_pixelsMovedUp;


    void Start()
    {
        m_moveTime = m_normalMoveTime;
        m_selector1Pos.vertical = 12;
        m_selector1Pos.horizontal = 4;
        m_selector2Pos.vertical = 12;
        m_selector2Pos.horizontal = 5;
        SpawnLine();
        StartCoroutine(MovementTimer());
    }

    void Update()
    {
        if(selector1.transform.position.y > m_verticalLimitUp)
        {
            selector1.transform.position = selector1.transform.position + new Vector3(0, -1, 0);
            selector2.transform.position = selector2.transform.position + new Vector3(0, -1, 0);
        }
    }

    private void OnSpeedUp()
    {
        m_moveTime = m_fastMoveTime;
    }
    private void OnSwap()
    {
        GameObject panelToSwap1 = null;
        GameObject panelToSwap2 = null;
        foreach(var panel in m_panelsOnScreen)
        {
            if(panelToSwap1 == null || panelToSwap2 == null)
            {
                if (panel.transform.position.x == selector1.transform.position.x && panel.transform.position.y == selector1.transform.position.y)
                {
                    panelToSwap1 = panel;
                }
                else if (panel.transform.position.x == selector2.transform.position.x && panel.transform.position.y == selector2.transform.position.y)
                {
                    panelToSwap2 = panel;
                }
            }
            else
            {
                break;
            }
        }
        SwapTwoPanels(panelToSwap1, panelToSwap2);
        SwapPanelsArray();
    }

    void OnMove(InputValue value)
    {
        var input = value.Get<Vector2>();
        MoveSelector(input.x, input.y);
    }

    private void MoveSelector(float xMovement, float yMovement)
    {
        if(selector1.transform.position.x + xMovement > m_horizontalLimits && selector2.transform.position.x + xMovement < -m_horizontalLimits
            && selector1.transform.position.y + yMovement > m_verticalLimitDown && selector1.transform.position.y + yMovement < m_verticalLimitUp)
        {
            selector1.transform.position = selector1.transform.position + new Vector3(xMovement, yMovement, 0);
            selector2.transform.position = selector2.transform.position + new Vector3(xMovement, yMovement, 0);
        }

        yMovement = -yMovement;

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
        for (int tileSpawnedCounter = 0; tileSpawnedCounter < m_numberofHorizontalPanels; tileSpawnedCounter++)
        {
            Vector3 spawnPosition = new Vector3(m_leftSpawnPoint.position.x + tileSpawnedCounter, m_leftSpawnPoint.position.y, m_leftSpawnPoint.position.z);
            int randomPanelToSpawn = Random.Range(0, m_panelArray.Length - 1);

            gameBoard[11, tileSpawnedCounter] = IntToColourEnum(randomPanelToSpawn);

            GameObject spawnedPanel = Instantiate(m_panelArray[randomPanelToSpawn], spawnPosition, Quaternion.identity);
            m_panelsOnScreen.Add(spawnedPanel);

            spawnedPanel.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.2f);
        }
        
        for(int i = 0; i < m_numberofVerticalPanels; i++)
        {
            Debug.Log(gameBoard[i, 0] + " " + gameBoard[i, 1] + " " + gameBoard[i, 2] + " " + gameBoard[i, 3] + " " + gameBoard[i, 4] + " " + gameBoard[i, 5]);
        }
        Debug.Log("                                                   ");
        
        Debug.Log(" " + m_selector1Pos.horizontal + " " + m_selector1Pos.vertical + " " + m_selector2Pos.horizontal + " " + m_selector2Pos.vertical);
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
        for (int k = 0; k < m_numberofHorizontalPanels; k++)
        {
            if (gameBoard[0, k] != PanelColours.None)
            {
                Debug.Log("You Lost");
                Time.timeScale = 0;
                return;
            }
        }

        for (int i = 1; i < m_numberofVerticalPanels; i++)
        {
            for (int j = 0; j < m_numberofHorizontalPanels; j++)
            {
                gameBoard[i - 1, j] = gameBoard[i, j];
            }
        }
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
        if (m_pixelsMovedUp >= m_numberOfPixels)
        {
            SpawnLine();
            m_pixelsMovedUp = 0;
        }
        foreach(GameObject panel in m_panelsScreenInArray)
        {
            panel.transform.position = panel.transform.position + new Vector3(0, panel.transform.localScale.y/m_numberOfPixels, 0);
        }
        selector1.transform.position = selector1.transform.position + new Vector3(0, selector1.transform.localScale.y / m_numberOfPixels, 0);
        selector2.transform.position = selector2.transform.position + new Vector3(0, selector2.transform.localScale.y / m_numberOfPixels, 0);

        m_pixelsMovedUp++;

        StartCoroutine(MovementTimer());
    }

    private void SwapTwoPanels(GameObject Panel1, GameObject Panel2)
    {
        if(Panel1 == null && Panel2 == null) return;
        else if(Panel1 == null)
        {
            Panel2.transform.position = new Vector3(selector1.transform.position.x, selector1.transform.position.y, Panel2.transform.position.z);
            StartCoroutine(GravityCheckPause(selector2.transform.position));
            StartCoroutine(GravityCheckPause(selector1.transform.position));
            StartCoroutine(HorizontalCheckMicroPause(Panel2));
            StartCoroutine(VerticalCheckMicroPause(Panel2));
        }
        else if(Panel2 == null)
        {
            Panel1.transform.position = new Vector3(selector2.transform.position.x, selector2.transform.position.y, Panel1.transform.position.z);
            StartCoroutine(GravityCheckPause(selector2.transform.position));
            StartCoroutine(GravityCheckPause(selector1.transform.position));
            StartCoroutine(HorizontalCheckMicroPause(Panel1));
            StartCoroutine(VerticalCheckMicroPause(Panel1));
        }
        else
        {
            Vector3 tempPositionForSwap = Panel1.transform.position;
            Panel1.transform.position = Panel2.transform.position;
            Panel2.transform.position = tempPositionForSwap;
            StartCoroutine(GravityCheckPause(selector1.transform.position));
            StartCoroutine(GravityCheckPause(selector2.transform.position));
            StartCoroutine(VerticalCheckMicroPause(Panel2));
            StartCoroutine(VerticalCheckMicroPause(Panel1));
            StartCoroutine(HorizontalCheckMicroPause(Panel1));
        }
    }

    private void SwapPanelsArray()
    {
        //Swaps Panels Under Both Selectors in the Array
        PanelColours buffer = new PanelColours();
        buffer = gameBoard[m_selector1Pos.vertical, m_selector1Pos.horizontal];
        gameBoard[m_selector1Pos.vertical, m_selector1Pos.horizontal] = gameBoard[m_selector2Pos.vertical, m_selector2Pos.horizontal];
        gameBoard[m_selector2Pos.vertical, m_selector2Pos.horizontal] = buffer;

        //Checks for any cleared Panel
        GravityCheckArray(m_selector2Pos);
        GravityCheckArray(m_selector1Pos);
        ArrayLineCheck(m_selector1Pos);
        ArrayColumnCheck(m_selector1Pos);
        ArrayColumnCheck(m_selector2Pos);

        ShowGame();
    }

    IEnumerator HorizontalCheckMicroPause(GameObject _movedPanel)
    {
        yield return new WaitForSeconds(0.05f);
        FullLineCheck(_movedPanel);
    }
    IEnumerator VerticalCheckMicroPause(GameObject _movedPanel)
    {
        yield return new WaitForSeconds(0.05f);
        FullColumnChack(_movedPanel);
        //GravityCheck();
    }

    private void FullLineCheck(GameObject movedPanel)
    {
        int counter = 0;
        List<GameObject> toDestroy = new List<GameObject>();
        List<GameObject> toDestroyTemp = new List<GameObject>();
        string tagToRemember = "Nothing";

        Collider2D skibidi1 = Physics2D.OverlapPoint(new Vector2(m_leftSpawnPoint.position.x, movedPanel.transform.position.y));
        if (skibidi1 != null)
        {
            tagToRemember = skibidi1.gameObject.tag;
        }

        for (int i = 0;  i < m_numberofHorizontalPanels;  i++)
        {
            Collider2D skibidi = Physics2D.OverlapPoint(new Vector2(m_leftSpawnPoint.position.x + i, movedPanel.transform.position.y));
            
            //Instantiate(testObject, new Vector3(m_leftSpawnPoint.position.x + i, movedPanel.transform.position.y, 0), Quaternion.identity);
            
            if (skibidi != null && skibidi.gameObject.CompareTag(tagToRemember))
            {
                counter++;
                toDestroyTemp.Add(skibidi.gameObject);
            }
            else
            {
                //Checks if at least 3 panels were aligned before reseting the counter 
                if(counter >= 3)
                {
                    //Adds the panels from the temp list to the real list
                    foreach (GameObject panel in toDestroyTemp)
                    {
                        toDestroy.Add(panel);
                    }
                }
                counter = 1;
                toDestroyTemp.Clear();

                if (skibidi != null)
                {
                    toDestroyTemp.Add(skibidi.gameObject);
                    tagToRemember = skibidi.gameObject.tag;
                }
                else tagToRemember = "Nothing";
            }
        }

        if (counter >= 3)
        {
            //Adds the panels from the temp list to the real list
            foreach (GameObject panel in toDestroyTemp)
            {
                toDestroy.Add(panel);
            }
        }
        
        //List<Vector3> panelToDestroyPositions = new List<Vector3>();
        foreach (GameObject panel in toDestroy)
        {
            m_panelsOnScreen.Remove(panel);
            StartCoroutine(GravityCheckPause(panel.transform.position));
            Destroy(panel);
        }
    }
    private void FullColumnChack(GameObject movedPanel)
    {
        int counter = 0;
        List<GameObject> toDestroy = new List<GameObject>();
        List<GameObject> toDestroyTemp = new List<GameObject>();

        string tagToRemember = "Nothing";

        float firstPanelInColumnY = movedPanel.transform.position.y - Mathf.FloorToInt(movedPanel.transform.position.y) +
            (int)m_leftSpawnPoint.position.y;

        Collider2D skibidi1 = Physics2D.OverlapPoint(new Vector2(movedPanel.transform.position.x, firstPanelInColumnY));
        if (skibidi1 != null)
        {
            tagToRemember = skibidi1.gameObject.tag;
        }

        for (int i = 0; i < m_numberofVerticalPanels; i++)
        {
            Collider2D skibidi = Physics2D.OverlapPoint(new Vector2(movedPanel.transform.position.x, firstPanelInColumnY + i));

            //Instantiate(testObject, new Vector3(movedPanel.transform.position.x, firstPanelInColumnY + i, 0), Quaternion.identity);
            

            if (skibidi != null && skibidi.gameObject.CompareTag(tagToRemember))
            {
                counter++;
                toDestroyTemp.Add(skibidi.gameObject);
            }
            else
            {
                //Checks if at least 3 panels were aligned before reseting the counter 
                if (counter >= 3)
                {
                    //Adds the panels from the temp list to the real list
                    foreach (GameObject panel in toDestroyTemp)
                    {
                        toDestroy.Add(panel);
                    }
                }
                counter = 1;
                toDestroyTemp.Clear();

                if (skibidi != null)
                {
                    toDestroyTemp.Add(skibidi.gameObject);
                    tagToRemember = skibidi.gameObject.tag;
                }
                else tagToRemember = "Nothing";
            }
        }

        if (counter >= 3)
        {
            //Adds the panels from the temp list to the real list
            foreach (GameObject panel in toDestroyTemp)
            {
                toDestroy.Add(panel);
            }
        }

        if(toDestroy.Count != 0)
        {
            Vector3 panelPosition = toDestroy[0].transform.position;
            foreach (GameObject panel in toDestroy)
            {
                m_panelsOnScreen.Remove(panel);
                Destroy(panel);
            }
            StartCoroutine(GravityCheckPause(panelPosition));
        }
    }

    private void ArrayColumnCheck(Position checkOrigin)
    {
        int counter = 1;
        List<Position> toDestroy = new List<Position>();
        List<Position> toDestroyTemp = new List<Position>();

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
        List<Position> toDestroy = new List<Position>();
        List<Position> toDestroyTemp = new List<Position>();

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

    IEnumerator GravityCheckPause(Vector3 felixDestroy)
    {
        yield return new WaitForSeconds(0.05f);
        GravityCheck2(felixDestroy);
    }

    private void GravityCheck2(Vector3 felixDestroy)
    {
        //Find Affected Panels
        Vector2 felixVector2 = felixDestroy;
        List<GameObject> panelsToGravify = new List<GameObject>();

        RaycastHit2D panelAboveMovedPanel = Physics2D.Raycast(felixVector2, new Vector2(0, 1));
        
        if (panelAboveMovedPanel)
        {
            GameObject panelToCheckAbove = panelAboveMovedPanel.collider.gameObject;
            panelsToGravify.Add(panelToCheckAbove);
            bool isPanelAbove = true;
            for(int i = 0; i < m_numberofVerticalPanels && isPanelAbove; i++)
            {
                Collider2D panelAbovePanelAboveMovedPanel = Physics2D.OverlapPoint(new Vector2(panelToCheckAbove.transform.position.x, panelToCheckAbove.transform.position.y + 1));
                if (panelAbovePanelAboveMovedPanel)
                {
                    panelToCheckAbove = panelAbovePanelAboveMovedPanel.gameObject;
                    panelsToGravify.Add(panelToCheckAbove);
                }
                else
                {
                    isPanelAbove = false;
                }
            }
        }

        //"Apply" Gravity
        List<GameObject> panelsMoved = new List<GameObject>();

        foreach (GameObject panel in panelsToGravify)
        {
            bool isOnFloor = false;
            while (!isOnFloor)
            {
                //Previously Used a Collider -> Is it better ?
                foreach(GameObject panelInGame in m_panelsOnScreen)
                {
                    if(panel.transform.position + new Vector3(0, -1, 0) == panelInGame.transform.position || panel.transform.position.y - 1 <= m_leftSpawnPoint.position.y)
                    {
                        isOnFloor = true;
                        break;
                    }
                }

                if (!isOnFloor)
                {
                    panel.transform.position = panel.transform.position + new Vector3(0, -1, 0);
                    panelsMoved.Add(panel);
                }
            }
        }
        
        foreach (GameObject panel in panelsMoved)
        {
            StartCoroutine(VerticalCheckMicroPause(panel));
            StartCoroutine(HorizontalCheckMicroPause(panel));
        }
    }

    private void GravityCheckArray(Position felixPos)
    {
        List<int> panelsToGravify = new List<int>();
        for (int verticalIndex = m_numberofVerticalPanels - 2; verticalIndex > 0; verticalIndex--)
        {
            if(gameBoard[verticalIndex, felixPos.horizontal] != PanelColours.None)
            {
                panelsToGravify.Add(verticalIndex);
            }
        }

        List<int> panelsMoved = new List<int>();
        foreach (int panelVerticalPos in panelsToGravify)
        {
            Debug.Log("Vertical : " + panelVerticalPos);
            bool isOnFloor = false;
            bool hasMoved = false;
            int height = panelVerticalPos;

            while(!isOnFloor)
            {
                height++;
                if (height < m_numberofVerticalPanels - 1 && gameBoard[height, felixPos.horizontal] == PanelColours.None)
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
                ArrayColumnCheck(new Position(panelVerticalPos, felixPos.horizontal));
                ArrayLineCheck(new Position(panelVerticalPos, felixPos.horizontal));
            }
        }
    }

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
                GameObject spawnedPanel = Instantiate(m_panelArray[ColourEnumToInt(gameBoard[verticalIndex, horizontalIndex])], 
                    m_leftSpawnPoint.position + new Vector3(horizontalIndex, m_numberofVerticalPanels - verticalIndex - 1, 0), m_leftSpawnPoint.rotation);

                spawnedPanel.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1f);
                m_panelsScreenInArray.Add(spawnedPanel);
            }
        }
    }
}