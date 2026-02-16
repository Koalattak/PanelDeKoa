using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

enum PanelColours
{
    Red,
    Green, 
    Blue,
    DarkBlue,
    Purple,
    Yellow,
    None
}

public class BaseGameplay : MonoBehaviour
{
    [SerializeField] private GameObject[] m_panelArray;
    [SerializeField] private Transform m_leftSpawnPoint;
    [SerializeField] const int m_numberofHorizontalPanels = 6;
    [SerializeField] const int m_numberofVerticalPanels = 12;
    [SerializeField] private float m_moveTime;

    [SerializeField] private float m_horizontalLimits;
    [SerializeField] private float m_verticalLimitUp;
    [SerializeField] private float m_verticalLimitDown;

    //Test
    /*
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
    */

    const int m_numberOfPixels = 16;

    private List<GameObject> m_panelsOnScreen = new List<GameObject>();
    [SerializeField] private GameObject selector1;
    [SerializeField] private GameObject selector2;

    [SerializeField] private GameObject testObject;

    private int m_pixelsMovedUp;


    void Start()
    {
        SpawnLine();
        StartCoroutine(MovementTimer());
    }

    // Update is called once per frame
    void Update()
    {
        if(selector1.transform.position.y > m_verticalLimitUp)
        {
            selector1.transform.position = selector1.transform.position + new Vector3(0, -1, 0);
            selector2.transform.position = selector2.transform.position + new Vector3(0, -1, 0);
        }
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
    }

    private void SpawnLine()
    {
        for (int tileSpawnedCounter = 0; tileSpawnedCounter < m_numberofHorizontalPanels; tileSpawnedCounter++)
        {
            Vector3 spawnPosition = new Vector3(m_leftSpawnPoint.position.x + tileSpawnedCounter, m_leftSpawnPoint.position.y, m_leftSpawnPoint.position.z);
            int randomPanelToSpawn = Random.Range(0, m_panelArray.Length);


            GameObject spawnedPanel = Instantiate(m_panelArray[randomPanelToSpawn], spawnPosition, Quaternion.identity);
            m_panelsOnScreen.Add(spawnedPanel);
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
        foreach(GameObject panel in m_panelsOnScreen)
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
            StartCoroutine(HorizontalCheckMicroPause(Panel2));
            StartCoroutine(VerticalCheckMicroPause(Panel2));
            StartCoroutine(GravityCheckPause(selector2.transform.position));
            StartCoroutine(GravityCheckPause(selector1.transform.position));
        }
        else if(Panel2 == null)
        {
            Panel1.transform.position = new Vector3(selector2.transform.position.x, selector2.transform.position.y, Panel1.transform.position.z);
            StartCoroutine(HorizontalCheckMicroPause(Panel1));
            StartCoroutine(VerticalCheckMicroPause(Panel1));
            StartCoroutine(GravityCheckPause(selector2.transform.position));
            StartCoroutine(GravityCheckPause(selector1.transform.position));
        }
        else
        {
            Vector3 tempPositionForSwap = Panel1.transform.position;
            Panel1.transform.position = Panel2.transform.position;
            Panel2.transform.position = tempPositionForSwap;
            StartCoroutine(VerticalCheckMicroPause(Panel2));
            StartCoroutine(VerticalCheckMicroPause(Panel1));
            StartCoroutine(HorizontalCheckMicroPause(Panel1));
            StartCoroutine(GravityCheckPause(selector1.transform.position));
            StartCoroutine(GravityCheckPause(selector2.transform.position));
        }
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
                //Instantiate(testObject, panel.transform.position + new Vector3(0, -1, 0), Quaternion.identity);

                if (!isOnFloor)
                {
                    panel.transform.position = panel.transform.position + new Vector3(0, -1, 0);
                    panelsMoved.Add(panel);
                }
            }
            //panel.GetComponent<SpriteRenderer>().color -= new Color(0, 0, 0, 0.05f);
        }
        
        foreach (GameObject panel in panelsMoved)
        {
            StartCoroutine(VerticalCheckMicroPause(panel));
            StartCoroutine(HorizontalCheckMicroPause(panel));
        }
    }
}