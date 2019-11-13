using System;
using System.Collections.Generic;
using System.Linq;
using _00_Asset._08_Easy_Panel_Transitions.Scripts;
using SequenceBreaker._03_Controller._00_Global;
using UnityEngine;

namespace SequenceBreaker._10_Global
{
    public sealed class SegueController : MonoBehaviour
    {

        // to get current tab
        public TabController tabController;

        // Home Tab
        public List<GameObject> homeViewList;

        // Play tab
        public List<GameObject> playViewList;
        
        // Timeline tab
        public List<GameObject> timelineViewList;

        
        // it is bad one but .. 
        public DragAndClose dragAndClose;

        public void Start()
        {
            InitHomeView();
            InitPlayView();
            InitTimeLineView();

        }

        public void StackHomeView(GameObject homeView)
        {
            homeViewList.Add(homeView);
        }

        public void StackTimeLineView(GameObject timelineView)
        {
            timelineViewList.Add(timelineView);
        }

        public void InitHomeView()
        {
            if (homeViewList.Count >= 1)
            {
//                Debug.Log("home view list count:" + homeViewList.Count);
                for (int i = homeViewList.Count - 1 ; i >= 1; i--)
                {
                    homeViewList[i].GetComponent<PanelAnimator>().StartAnimOut();
                    homeViewList.RemoveAt(homeViewList.Count - 1);
                }
            }
            
            dragAndClose.Init();

        }

        public void InitPlayView()
        {
            if (playViewList.Count >= 1)
            {
                for (int i = playViewList.Count - 1 ; i >= 1; i--)
                {
                    playViewList[i].GetComponent<PanelAnimator>().StartAnimOut();
                    playViewList.RemoveAt(playViewList.Count - 1);
                }
            }
            dragAndClose.Init();

        }
        
        public void InitTimeLineView()
        {
            if (timelineViewList.Count >= 1)
            {
                for (int i = timelineViewList.Count - 1 ; i >= 1; i--)
                {
                    timelineViewList[i].GetComponent<PanelAnimator>().StartAnimOut();
                    timelineViewList.RemoveAt(timelineViewList.Count - 1);
                }
            }
            dragAndClose.Init();
            
        }


        public void CancelView()
        {
            GameObject currentView;
             switch (tabController.currentTab)
            {
                case "HomeTab":
                    currentView = homeViewList.Last(); 
                    currentView.GetComponent<PanelAnimator>().StartAnimIn();

                    break;
                case "PlayTab":
                    currentView = playViewList.Last();
                    currentView.GetComponent<PanelAnimator>().StartAnimIn();
                    break; 
                
                case "TimelineTab":
                     currentView = timelineViewList.Last();
                     currentView.GetComponent<PanelAnimator>().StartAnimIn();


                     break;

                default: 
                    Debug.LogError("unexpected currentTab:" + tabController.currentTab); 
                    break;
            }

            
        }

        public GameObject GetCurrentView()
        {
            switch (tabController.currentTab)
            {
                case "HomeTab":
                    return  homeViewList.Last();
                    break;
                case "PlayTab":
                    return  playViewList.Last();
                    break; 
                
                case "TimelineTab":
                     return timelineViewList.Last();
                     break;

                default: 
                    Debug.LogError("unexpected currentTab:" + tabController.currentTab);

                    return null;
                    break;
            }
        }

        public void BackPreviousView()
        {

            GameObject currentView;
            GameObject previousView;
            
            switch (tabController.currentTab)
            {
                case "HomeTab":
                    currentView = homeViewList.Last();
                    if (homeViewList.Count >= 2)
                    {
                        homeViewList.RemoveAt(homeViewList.Count - 1);
                        previousView = homeViewList.Last();
                        previousView.SetActive(true);
                        currentView.GetComponent<PanelAnimator>().StartAnimOut();
                        
                        previousView.SetActive(true);
                        currentView.GetComponent<PanelAnimator>().StartAnimOut(); 

                    }
                    else
                    {
                        CancelView();
                    }
                    
                    break;
                case "PlayTab":
                    currentView = playViewList.Last();
                    if (playViewList.Count >= 2)
                    {
                        playViewList.RemoveAt(playViewList.Count - 1);
                        previousView = playViewList.Last();
                        previousView.SetActive(true);
                        currentView.GetComponent<PanelAnimator>().StartAnimOut();
                        
                        previousView.SetActive(true);
                        currentView.GetComponent<PanelAnimator>().StartAnimOut(); 

                    }
                    else
                    {
                        CancelView();
                    }
                    break; 
                
                case "TimelineTab":
                     currentView = timelineViewList.Last();

                     if (timelineViewList.Count >= 2)
                     {
                         timelineViewList.RemoveAt(timelineViewList.Count - 1);
                         previousView = timelineViewList.Last();
                         previousView.SetActive(true);
                         currentView.GetComponent<PanelAnimator>().StartAnimOut();
                         
                         previousView.SetActive(true);
                         currentView.GetComponent<PanelAnimator>().StartAnimOut(); 

                         
                     }
                     else
                     {
                         CancelView();
                     }

                    
                     break;

                default: 
                    Debug.LogError("unexpected currentTab:" + tabController.currentTab); 
                    break;
            }

            dragAndClose.Init();


        }
        
        
        
        
    }
}
