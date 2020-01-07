using SequenceBreaker.Play.Battle;
using UnityEngine;
using UnityEngine.UI;

namespace SequenceBreaker.Timeline
{
    public  sealed class LogButton : MonoBehaviour
    {
        public Button button;
        public Text orderCondition;
        public Text log;
        public Image iconImage;

        private BattleLogClass _battleLog;
        
        // Start is called before the first frame update
        public void Setup(BattleLogClass currentLog)
        {
            _battleLog = currentLog;
            orderCondition.text = _battleLog.OrderCondition.ToString();
            log.text = _battleLog.LogList[0];

            transform.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);

        }

    }
}
