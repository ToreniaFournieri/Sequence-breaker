using SequenceBreaker.Translate;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace SequenceBreaker.Play.Battle
{
    public sealed class UnitInfoSet : MonoBehaviour
    {
        //public GameObject iconMask;
        //public Image unitIcon;
        public Image healedShieldBar;
        public Image previousShieldBar;
        public Image shieldBar;
        public Image healedHpBar;
        public Image previousHpBar;
        public Image hPBar;
        [FormerlySerializedAs("UnitInfoText")] public Text unitInfoText;
        public GameObject barrierObject;

        public Text barrierRemains;
        public Text infoText;

        // Start is called before the first frame update
        void Start()
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }

        public void SetValueFromXML(string shieldHPXML)
        {
            float _previousShield = 0f;
            float _followingShield = 0f;
            float _maxShield = 1f;
            float _previousHP = 0f;
            float _followingHP = 0f;
            float _maxHP = 1f;
            int _previousBarrier = 0;
            int _followingBarrier = 0;
            float _effectiveDefense = 0;
            string _otherText = null;

            if (shieldHPXML.Contains("<previousShield>") && shieldHPXML.Contains("</previousShield>"))
            {
                int _startIndex = shieldHPXML.IndexOf("<previousShield>", System.StringComparison.Ordinal) + "<previousShield>".Length;
                int _endIndex = shieldHPXML.IndexOf("</previousShield>", System.StringComparison.Ordinal);

                try
                {
                    _previousShield = float.Parse(shieldHPXML.Substring(_startIndex, _endIndex - _startIndex));
                }
                catch
                {
                    Debug.LogError("Faild to get value: " + shieldHPXML.Substring(_startIndex, _endIndex - _startIndex) + " _previousShield ");
                }

                //Debug.Log(" previousShield:" + _previousShield);
            }
            else
            {
                Debug.LogError("failed to get previousShield");
            }

            if (shieldHPXML.Contains("<followingShield>") && shieldHPXML.Contains("</followingShield>"))
            {
                int _startIndex = shieldHPXML.IndexOf("<followingShield>", System.StringComparison.Ordinal) + "<followingShield>".Length;
                int _endIndex = shieldHPXML.IndexOf("</followingShield>", System.StringComparison.Ordinal);

                try
                {
                    _followingShield = float.Parse(shieldHPXML.Substring(_startIndex, _endIndex - _startIndex));
                }
                catch
                {
                    Debug.LogError("Faild to get value: " + shieldHPXML.Substring(_startIndex, _endIndex - _startIndex) + " _followingShield ");
                }

                //Debug.Log(" followingShield:" + _followingShield);
            }
            else
            {
                Debug.LogError("failed to get followingShield");
            }


            if (shieldHPXML.Contains("<maxShield>") && shieldHPXML.Contains("</maxShield>"))
            {
                int _startIndex = shieldHPXML.IndexOf("<maxShield>", System.StringComparison.Ordinal) + "<maxShield>".Length;
                int _endIndex = shieldHPXML.IndexOf("</maxShield>", System.StringComparison.Ordinal);

                try
                {
                    _maxShield = float.Parse(shieldHPXML.Substring(_startIndex, _endIndex - _startIndex));
                }
                catch
                {
                    Debug.LogError("Faild to get value: " + shieldHPXML.Substring(_startIndex, _endIndex - _startIndex) + " _maxShield ");
                }
                //Debug.Log(" maxShield:" + _maxShield);
            }
            else
            {
                Debug.LogError("failed to get maxShield");
            }


            if (shieldHPXML.Contains("<previousHP>") && shieldHPXML.Contains("</previousHP>"))
            {
                int _startIndex = shieldHPXML.IndexOf("<previousHP>", System.StringComparison.Ordinal) + "<previousHP>".Length;
                int _endIndex = shieldHPXML.IndexOf("</previousHP>", System.StringComparison.Ordinal);

                try
                {
                    _previousHP = float.Parse(shieldHPXML.Substring(_startIndex, _endIndex - _startIndex));
                }
                catch
                {
                    Debug.LogError("Faild to get value: " + shieldHPXML.Substring(_startIndex, _endIndex - _startIndex) + " _previousHP ");
                }

                //Debug.Log(" previousHP:" + _previousHP);
            }
            else
            {
                Debug.LogError("failed to get previousHP");
            }


            if (shieldHPXML.Contains("<followingHP>") && shieldHPXML.Contains("</followingHP>"))
            {
                int _startIndex = shieldHPXML.IndexOf("<followingHP>", System.StringComparison.Ordinal) + "<followingHP>".Length;
                int _endIndex = shieldHPXML.IndexOf("</followingHP>", System.StringComparison.Ordinal);

                try
                {
                    _followingHP = float.Parse(shieldHPXML.Substring(_startIndex, _endIndex - _startIndex));
                }
                catch
                {
                    Debug.LogError("Faild to get value: " + shieldHPXML.Substring(_startIndex, _endIndex - _startIndex) + " _followingHP ");
                }

                //Debug.Log(" followingHP:" + _followingHP);
            }
            else
            {
                Debug.LogError("failed to get followingHP");
            }


            if (shieldHPXML.Contains("<maxHP>") && shieldHPXML.Contains("</maxHP>"))
            {
                int _startIndex = shieldHPXML.IndexOf("<maxHP>", System.StringComparison.Ordinal) + "<maxHP>".Length;
                int _endIndex = shieldHPXML.IndexOf("</maxHP>", System.StringComparison.Ordinal);

                try
                {
                    _maxHP = float.Parse(shieldHPXML.Substring(_startIndex, _endIndex - _startIndex));
                }
                catch
                {
                    Debug.LogError("Faild to get value: " + shieldHPXML.Substring(_startIndex, _endIndex - _startIndex) + " _maxHP ");
                }
                //Debug.Log(" maxHP:" + _maxHP);
            }
            else
            {
                Debug.LogError("failed to get maxHP");
            }


            if (shieldHPXML.Contains("<previousBarrier>") && shieldHPXML.Contains("</previousBarrier>"))
            {
                int _startIndex = shieldHPXML.IndexOf("<previousBarrier>", System.StringComparison.Ordinal) + "<previousBarrier>".Length;
                int _endIndex = shieldHPXML.IndexOf("</previousBarrier>", System.StringComparison.Ordinal);

                try
                {
                    _previousBarrier = int.Parse(shieldHPXML.Substring(_startIndex, _endIndex - _startIndex));
                }
                catch
                {
                    Debug.LogError("Faild to get value: " + shieldHPXML.Substring(_startIndex, _endIndex - _startIndex) + " _previousBarrier ");
                }

                //Debug.Log(" previousBarrier:" + _previousBarrier);
            }
            else
            {
                Debug.LogError("failed to get previousBarrier");
            }


            if (shieldHPXML.Contains("<followingBarrier>") && shieldHPXML.Contains("</followingBarrier>"))
            {
                int _startIndex = shieldHPXML.IndexOf("<followingBarrier>", System.StringComparison.Ordinal) + "<followingBarrier>".Length;
                int _endIndex = shieldHPXML.IndexOf("</followingBarrier>", System.StringComparison.Ordinal);

                try
                {
                    _followingBarrier = int.Parse(shieldHPXML.Substring(_startIndex, _endIndex - _startIndex));
                }
                catch
                {
                    Debug.LogError("Faild to get value: " + shieldHPXML.Substring(_startIndex, _endIndex - _startIndex) + " _followingBarrier ");
                }

                //Debug.Log(" followingBarrier:" + _followingBarrier);
            }
            else
            {
                Debug.LogError("failed to get followingBarrier");
            }


            if (shieldHPXML.Contains("<effectiveDefense>") && shieldHPXML.Contains("</effectiveDefense>"))
            {
                int _startIndex = shieldHPXML.IndexOf("<effectiveDefense>", System.StringComparison.Ordinal) + "<effectiveDefense>".Length;
                int _endIndex = shieldHPXML.IndexOf("</effectiveDefense>", System.StringComparison.Ordinal);

                try
                {
                    _effectiveDefense = float.Parse(shieldHPXML.Substring(_startIndex, _endIndex - _startIndex));
                }
                catch
                {
                    Debug.LogError("Faild to get value: " + shieldHPXML.Substring(_startIndex, _endIndex - _startIndex) + " _effectiveDefense ");
                }

                //Debug.Log(" effectiveDefense:" + _effectiveDefense);
            }
            else
            {
                Debug.LogError("failed to get effectiveDefense");
            }


            if (shieldHPXML.Contains("<otherText>") && shieldHPXML.Contains("</otherText>"))
            {
                int _startIndex = shieldHPXML.IndexOf("<otherText>", System.StringComparison.Ordinal) + "<otherText>".Length;
                int _endIndex = shieldHPXML.IndexOf("</otherText>", System.StringComparison.Ordinal);

                try
                {
                    _otherText = shieldHPXML.Substring(_startIndex, _endIndex - _startIndex);
                }
                catch
                {
                    Debug.LogError("Faild to get value: " + shieldHPXML.Substring(_startIndex, _endIndex - _startIndex) + " _otherText ");
                }

                //Debug.Log(" otherText:" + _otherText);
            }
            else
            {
                Debug.LogError("failed to get <otherText>");
            }


            healedShieldBar.gameObject.SetActive(false);
            healedHpBar.gameObject.SetActive(false);

            if (_previousShield >= _followingShield)
            {
                //get damaged
                shieldBar.gameObject.SetActive(true);
                shieldBar.fillAmount = _followingShield / _maxShield;

            }
            else
            {
                //get healed
                healedShieldBar.gameObject.SetActive(true);
                shieldBar.gameObject.SetActive(false);
                healedShieldBar.fillAmount = _followingShield / _maxShield;

            }


            previousShieldBar.fillAmount = _previousShield / _maxShield;
            previousHpBar.fillAmount = _previousHP / _maxHP;
            hPBar.fillAmount = _followingHP / _maxHP;

            if (_followingBarrier >= 1)
            {
                barrierRemains.text = _followingBarrier.ToString();
                barrierObject.SetActive(true);

            }
            else
            {
                barrierObject.SetActive(false);
            }
            infoText.text = Word.Get("Effective Defense: X", ((int)_effectiveDefense).ToString() + " " + _otherText);


        }
    }
}
