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
            homeViewList.Add(tabController.defaultHomeView);
            playViewList.Add(tabController.defaultPlayView);
            timelineViewList.Add(tabController.defaultTimelineView);
            

        }

        public void StackTimeLineView(GameObject timelineView)
        {
            timelineViewList.Add(timelineView);
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
