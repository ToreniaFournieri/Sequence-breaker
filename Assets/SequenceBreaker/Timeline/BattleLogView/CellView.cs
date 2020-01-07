using System.Collections.Generic;
using _00_Asset._05_EnhancedScroller_v2.Plugins;
using _00_Asset.I2.Localization.Scripts.Manager;
using SequenceBreaker.Environment;
using SequenceBreaker.Play.Battle;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace SequenceBreaker.Timeline.BattleLogView
{
    public sealed class CellView : EnhancedScrollerCellView
    {
        public Text reactText;
        public GameObject separateLine;
        public Text unitInfo;
        public Text firstLine;
        //public Text mainText        ;

        /// <summary>
        /// A reference to the rect transform (this is Transform) which will be
        /// updated by the content size fitter
        /// </summary>
        public Transform multiMainTextTransform;
        //public List<GameObject> mainGameObjectList;

        public int turn;
        public Text bigText;
        public Image backgroundImage;
        public Image nestedLinePrevious;
        public Image nestedLine;

        // Battle Unit icon
        public GameObject mainUnit;
        public GameObject iconMask;
        public Image unitIcon;

        public Image previousShieldBar;
        public Image healedShieldBar;
        public Image previousHPBar;
        public Image healedHpBar;
        public GameObject barrierObject;
        public Text barrierRemains;

        public GameObject headerContent;
        [FormerlySerializedAs("HeaderInfo")] public Text headerInfo;
        public ShowTargetUnit showTargetUnit;
        public SimpleObjectPool unitIconObjectPool;

        public SimpleObjectPool longTextObjectPool;
        public SimpleObjectPool textOnShieldBarObjectPool;
        public SimpleObjectPool statusDisplayObjectPool;
        public BattleLogEnhancedScrollController battleLogEnhancedScrollController;

        ///// <summary>
        ///// A reference to the rect transform which will be
        ///// updated by the content size fitter
        ///// </summary>
        //public RectTransform textRectTransform;

        /// <summary>
        /// The space around the text label so that we
        /// aren't up against the edges of the cell
        /// </summary>
        public RectOffset textBuffer;

        public void SetData(Data data, Data nextData, bool calculateLayout, Sprite unitSprite)
        //public void SetData(BattleLogClass battleLogLists, bool calculateLayout)
        {
            reactText.text = data.ReactText;

            headerContent.SetActive(false);


            if (data.IsHeaderInfo)
            {
                headerInfo.text = data.HeaderText;
                showTargetUnit.unitIconObjectPool = unitIconObjectPool;
                showTargetUnit.SetUnitInfo(data.Characters);
                headerContent.SetActive(true);
            }
            else
            {
                //Need to false in case recycle object.
            }


            //Font set
            reactText.font = LocalizationManager.GetTranslatedObjectByTermName<Font>("FONT");
            unitInfo.font = LocalizationManager.GetTranslatedObjectByTermName<Font>("FONT");
            firstLine.font = LocalizationManager.GetTranslatedObjectByTermName<Font>("FONT");

            //foreach (GameObject mainObject in mainGameObjectList)
            //{ }


            int childrenCount = 0;

            List<GameObject> children = new List<GameObject>();

            // Somehow, this works.
            foreach (Transform child in multiMainTextTransform)
            {

                childrenCount++;
                children.Add(child.gameObject);

            }

            foreach (GameObject child in children)
            {

                if (child.name.Contains("Long Text"))
                {
                    longTextObjectPool.ReturnObject(child);
                }

                if (child.name.Contains("SB_UnitText"))
                {
                    textOnShieldBarObjectPool.ReturnObject(child);

                }

                if (child.name.Contains("SB_HPandShield"))
                {
                    statusDisplayObjectPool.ReturnObject(child);

                }
            }

            //hPShieldBarObjectPool.ReturnObject(children.gameObject);


            //if (children.GetComponent<UnitInfoSet>() != null)
            //{
            //   
            //}
            //else
            //{
            //    //children.GetComponent<Text>().text = null;
            //    mainTextObjectPool.ReturnObject(children.gameObject);
            //}


            //int childrenRemindCount = 0;

            //foreach (Transform child in multiMainTextTransform)
            //{
            //    childrenRemindCount++;
            //    //Destroy(children.gameObject);
            //}


            //Debug.Log("main children: " + childrenCount + " / " + childrenRemindCount + " this should be 0.");
            //GameObject[] _gameObjects = MultiMainTextTransform.gameObject.GetComponentsInChildren<GameObject>();
            //foreach (GameObject _gameObject in _gameObjects)
            //{
            //    if (_gameObject.GetComponent<UnitInfoSet>() != null)
            //    {
            //        hPShieldBarObjectPool.ReturnObject(_gameObject);
            //    } else
            //    {
            //        mainTextObjectPool.ReturnObject(_gameObject);
            //    }
            //}

            if (data.MainTextList != null)
            {

                foreach (string mainString in data.MainTextList)
                {

                    if (mainString.Contains("<SB_HPandShield>"))
                    {
                        // show hp and shield bar
                        GameObject _gameObject = statusDisplayObjectPool.GetObject();
                        UnitInfoSet _unitInfoSet = _gameObject.GetComponent<UnitInfoSet>();

                        _unitInfoSet.SetValueFromXML(mainString);
                        _gameObject.transform.SetParent(multiMainTextTransform);
                        _gameObject.transform.localScale = new Vector3(1, 1, 1);


                    }
                    else if (mainString.Contains("<SB_UnitText>") && mainString.Contains("</SB_UnitText>"))
                    {
                        GameObject _gameObject = textOnShieldBarObjectPool.GetObject();

                        int _startIndex = 0;
                        int _endIndex = 0;
                        try
                        {
                            _startIndex = mainString.IndexOf("<SB_UnitText>", System.StringComparison.Ordinal) + "<SB_UnitText>".Length;
                            _endIndex = mainString.IndexOf("</SB_UnitText>", System.StringComparison.Ordinal);
                            _gameObject.GetComponentInChildren<Text>().text = (mainString.Substring(_startIndex, _endIndex - _startIndex));
                        }
                        catch
                        {
                            Debug.LogError("Faild to get value: " + mainString.Substring(_startIndex, _endIndex - _startIndex) + " SB_UnitText ");
                        }

                        //_gameObject.GetComponent<Text>().text = mainString;

                        _gameObject.GetComponentInChildren<Text>().font = LocalizationManager.GetTranslatedObjectByTermName<Font>("FONT");
                        // linespace not works well becareful!
                        //_gameObject.GetComponent<Text>().lineSpacing = float.Parse(LocalizationManager.GetTranslation("FONT-LineSpace"));

                        _gameObject.transform.SetParent(multiMainTextTransform);
                        _gameObject.transform.localScale = new Vector3(1, 1, 1);

                    }
                    else
                    {
                        //others basic long text
                        //Debug.Log(" string: " + mainString);
                        GameObject _gameObject = longTextObjectPool.GetObject();

                        _gameObject.GetComponent<Text>().text = mainString;
                        _gameObject.GetComponent<Text>().font = LocalizationManager.GetTranslatedObjectByTermName<Font>("FONT");

                        _gameObject.transform.SetParent(multiMainTextTransform);
                        _gameObject.transform.localScale = new Vector3(1, 1, 1);

                    }


                }
            }
            //multiMainTextTransform.GetComponent<RectTransform>().transform.localScale = new Vector3(1, 1, 0);
            //multiMainTextTransform.GetComponent<RectTransform>().ForceUpdateRectTransforms();

            turn = data.Turn;
            battleLogEnhancedScrollController.currentActiveTurn = data.Turn;
            bigText.font = LocalizationManager.GetTranslatedObjectByTermName<Font>("FONT");
            barrierRemains.font = LocalizationManager.GetTranslatedObjectByTermName<Font>("FONT");

            bigText.lineSpacing = float.Parse(LocalizationManager.GetTranslation("FONT-LineSpace"));

            unitInfo.text = data.UnitInfo;
            firstLine.text = data.FirstLine;
            bigText.text = data.BigText;
            //someTextText.text = battleLogLists.Log;
            Color darkRed = new Color32(36, 20, 20, 255); // dark red
            Color darkGreen = new Color32(20, 36, 20, 255); // dark green
            Color color = new Color32(24, 24, 24, 255);
            Color unitColor = new Color32(120, 120, 120, 255);

            unitIcon.color = unitColor;

            if (data.IsDead)
            {
                mainUnit.SetActive(false);
            }
            else
            {
                mainUnit.SetActive(true);
            }

            if (unitSprite != null)
            {
                unitIcon.sprite = unitSprite;
                iconMask.SetActive(true);
            }
            else
            {
                iconMask.SetActive(false);
            }

            // Actor Hp display

            previousShieldBar.fillAmount = data.PreviousShieldRatio;
            previousHPBar.fillAmount = data.PreviousHpRatio;
            healedHpBar.fillAmount = data.HpRatio;
            healedShieldBar.fillAmount = data.ShieldRatio;

            //Set barrier remains 
            if (data.BarrierRemains > 0)
            { barrierObject.SetActive(true); }
            else
            { barrierObject.SetActive(false); }
            barrierRemains.text = data.BarrierRemains.ToString();

            switch (data.Affiliation)
            {
                case Affiliation.Ally:
                    color = darkGreen;
                    break;
                case Affiliation.Enemy:
                    color = darkRed;
                    break;
            }

            backgroundImage.color = color;


            if (data.NestLevel > 0)
            {
                separateLine.SetActive(false);
            }
            else
            {
                separateLine.SetActive(true);
            }
            nestedLinePrevious.rectTransform.sizeDelta = new Vector2(4 * data.NestLevel, 136);

            if (nextData != null)
            {
                nestedLine.rectTransform.sizeDelta = new Vector2(4 * nextData.NestLevel, 0);
            }

            // Only calculate the layout on the first pass.
            // This will save processing on subsequent passes.
            if (calculateLayout)
            {
                //// force update the canvas so that it can calculate the size needed for the text immediately
                //Canvas.ForceUpdateCanvases();



                // set the data's cell size and add in some padding so the the text isn't up against the border of the cell
                //data.CellSize = textRectTransform.rect.height + textBuffer.top + textBuffer.bottom;
                data.CellSize = multiMainTextTransform.GetComponent<RectTransform>().rect.height + textBuffer.top + textBuffer.bottom;
                //battleLogLists.cellSize = textRectTransform.rect.height + textBuffer.top + textBuffer.bottom;
            }

            //this.textRectTransform.ForceUpdateRectTransforms();
            this.gameObject.GetComponent<RectTransform>().ForceUpdateRectTransforms();
            //this.multiMainTextTransform.GetComponent<RectTransform>().ForceUpdateRectTransforms();
        }
    }
}