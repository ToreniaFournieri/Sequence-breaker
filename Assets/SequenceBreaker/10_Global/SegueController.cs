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
                Debug.Log("home view list count:" + homeViewList.Count);
                for (int i = homeViewList.Count - 1 ; i >= 1; i--)
                {
                    homeViewList[i].GetComponent<PanelAnimator>().StartAnimOut();
                    homeViewList.RemoveAt(homeViewList.Count - 1);
                }
            }
            else
            {
                homeViewList.Clear();
                homeViewList.Add(tabController.defaultHomeView);
            }
        }

        public void InitPlayView()
        {
            if (playViewList.Count >= 1)
            {
                Debug.Log("home view list count:" + playViewList.Count);
                for (int i = playViewList.Count - 1 ; i >= 1; i--)
                {
                    playViewList[i].GetComponent<PanelAnimator>().StartAnimOut();
                    playViewList.RemoveAt(playViewList.Count - 1);
                }
            }
            else
            {
                playViewList.Clear();
                playViewList.Add(tabController.defaultHomeView);
            }
        }
        
        public void InitTimeLineView()
        {
            if (timelineViewList.Count >= 1)
            {
                Debug.Log("home view list count:" + timelineViewList.Count);
                for (int i = timelineViewList.Count - 1 ; i >= 1; i--)
                {
                    timelineViewList[i].GetComponent<PanelAnimator>().StartAnimOut();
                    timelineViewList.RemoveAt(timelineViewList.Count - 1);
                }
            }
            else
            {
                timelineViewList.Clear();
                timelineViewList.Add(tabController.defaultHomeView);
            }
        }

//        public void InitTimeLineView()
//        {
//            timelineViewList.Clear();
//            timelineViewList.Add(tabController.defaultTimelineView);
//
//        }

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
                     break;

                default: 
                    Debug.LogError("unexpected currentTab:" + tabController.currentTab); 
                    break;
            }



        }
        
        
        
        
    }
}
