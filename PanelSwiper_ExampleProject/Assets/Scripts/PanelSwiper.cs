//This script is based on the following video:
//https://www.youtube.com/watch?v=rjFgThTjLso
//and was adjusted by me (Marisa Lublewski)

using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PanelSwiper : MonoBehaviour, IDragHandler, IEndDragHandler
{
    //Public variables
    [Tooltip("The GameObjects must be in the correct order (from left to right).")]
    public GameObject[] panelHolders; //An array of the panels. These objects' position will be used to focus the camera. They must be in the correct order.
    
    [Tooltip("This number must be set to the current number of panels that can be swiped.")]
    public int numberOfPanels = 2; //This number must be correct, otherwise the functionality to restrict swiping will not work.
    
    [Tooltip("Defines how long a drag must be in order to swipe between panels.")]
    public float percentThreshold = 0.1f; 
    
    [Tooltip("Defines how long the swiping animation takes.")]
    public float easing = 0.5f; 


    //Private variables
    private Vector3 panelLocation;
    private int currentPanelIndex; //To keep this number the same as the numberOfPanels, count starts at 1 and not 0
    private bool swipePossible = true;

    void Start()
    {
        panelLocation = transform.localPosition;
        currentPanelIndex = 2;
    }

    void Update()
    {

    }

    public void OnDrag(PointerEventData data)
    {
        if (swipePossible)
        {
            float difference = data.pressPosition.x - data.position.x;

            //If it's the rightmost panel, only be able to swipe to the left
            if (currentPanelIndex + 1 > numberOfPanels)
            {
                if (difference < 0)
                {
                    transform.localPosition = panelLocation - new Vector3(difference, 0, 0);
                }
            }
            //If it's the leftmost panel, only be able to swipe to the right
            else if (currentPanelIndex - 1 == 0)
            {
                if (difference > 0f)
                {
                    transform.localPosition = panelLocation - new Vector3(difference, 0, 0);
                }
            }
            else
            {
                transform.localPosition = panelLocation - new Vector3(difference, 0, 0);
            }
        }

    }

    public void OnEndDrag(PointerEventData data)
    {
        if (swipePossible)
        {
            float percentage = (data.pressPosition.x - data.position.x) / Screen.width;
            if (Mathf.Abs(percentage) >= percentThreshold)
            {
                Vector3 newLocation = panelLocation;
                if (percentage > 0)
                {
                    //Move Panel to the right (if it's not the rightmost panel)
                    if (currentPanelIndex + 1 <= numberOfPanels)
                    {
                        currentPanelIndex++;
                        newLocation = new Vector3(-panelHolders[currentPanelIndex - 1].transform.localPosition.x, 0, 0);
                    }
                }
                else if (percentage < 0)
                {
                    //Move Panel to the left (if it's not the leftmost panel)
                    if (currentPanelIndex - 1 > 0)
                    {
                        currentPanelIndex--;
                        newLocation = new Vector3(-panelHolders[currentPanelIndex - 1].transform.localPosition.x, 0, 0);
                    }

                }

                StartCoroutine(SmoothMove(transform.localPosition, newLocation, easing));
                panelLocation = newLocation;

            }
            else
            {
                StartCoroutine(SmoothMove(transform.localPosition, panelLocation, easing));
            }
        }

    }

    //Smooth movement between panels with a Lerp function between current and intended location
    IEnumerator SmoothMove(Vector3 startPos, Vector3 endPos, float seconds)
    {
        float time = 0f;
        while (time <= 1.0f)
        {
            time += Time.deltaTime / seconds;
            transform.localPosition = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0f, 1f, time));
            yield return null;
        }
    }

    //This function can be called by buttons if they should also be used to move between panels
    //They need to include the index of the target panel that should be moved to (the index starts at 1, not 0)
    //If no buttons are used, this function can be deleted
    public void SetNewPanel(int panelIndex)
    {
        if (panelIndex != currentPanelIndex)
        {

            swipePossible = true;

            Vector3 targetPanelLocation = panelHolders[panelIndex - 1].transform.localPosition;

            Vector3 newLocation = new Vector3(-targetPanelLocation.x, -targetPanelLocation.y, targetPanelLocation.z);

            StartCoroutine(SmoothMove(transform.localPosition, newLocation, easing));
            panelLocation = newLocation;
            currentPanelIndex = panelIndex;
        }
    }
}
