using System.Collections.Generic;
using UnityEngine;

namespace _00_Asset._01_Asset_SuperScrollView.Demo.Scripts.Common
{
    public enum MsgTypeEnum
    {
        Str = 0,
        Picture,
        Count,
    }


    public class PersonInfo
    {
        public int MId;
        public string MName;
        public string MHeadIcon;
    }

    public class ChatMsg
    {
        public int MPersonId;
        public MsgTypeEnum MMsgType;
        public string MSrtMsg;
        public string MPicMsgSpriteName;
    }

    public class ChatMsgDataSourceMgr : MonoBehaviour
    {
        Dictionary<int, PersonInfo> _mPersonInfoDict = new Dictionary<int, PersonInfo>();
        List<ChatMsg> _mChatMsgList = new List<ChatMsg>();
        static ChatMsgDataSourceMgr _instance = null;
        static string[] _mChatDemoStrList = {
            "Support ListView and GridView.",
            "Support Infinity Vertical and Horizontal ScrollView.",
            "Support items in different sizes such as widths or heights. Support items with unknown size at init time.",
            "Support changing item count and item size at runtime. Support looping items such as spinners. Support item padding.",
            "Use only one C# script to help the UGUI ScrollRect to support any count items with high performance.",
            };

        static string[] _mChatDemoPicList = {
            "grid_pencil_128_g2",
            "grid_flower_200_3",
            "grid_pencil_128_g3",
            "grid_flower_200_7",
            };

        public static ChatMsgDataSourceMgr Get
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Object.FindObjectOfType<ChatMsgDataSourceMgr>();
                }
                return _instance;
            }

        }

        void Awake()
        {
            Init();
        }


        public PersonInfo GetPersonInfo(int personId)
        {
            PersonInfo ret = null;
            if(_mPersonInfoDict.TryGetValue(personId, out ret))
            {
                return ret;
            }
            return null;
        }

        public void Init()
        {
            _mPersonInfoDict.Clear();
            PersonInfo tInfo = new PersonInfo();
            tInfo.MHeadIcon = "grid_pencil_128_g8";
            tInfo.MId = 0;
            tInfo.MName = "Jaci";
            _mPersonInfoDict.Add(tInfo.MId, tInfo);

            tInfo = new PersonInfo();
            tInfo.MHeadIcon = "grid_pencil_128_g5";
            tInfo.MId = 1;
            tInfo.MName = "Toc";
            _mPersonInfoDict.Add(tInfo.MId, tInfo);

            InitChatDataSource();

        }

        public ChatMsg GetChatMsgByIndex(int index)
        {
            if (index < 0 || index >= _mChatMsgList.Count)
            {
                return null;
            }
            return _mChatMsgList[index];
        }

        public int TotalItemCount
        {
            get
            {
                return _mChatMsgList.Count;
            }
        }

        void InitChatDataSource()
        {
            _mChatMsgList.Clear();
            int count = _mChatDemoStrList.Length;
            int count1 = _mChatDemoPicList.Length;
            for (int i = 0; i < 100; ++i)
            {
                ChatMsg tMsg = new ChatMsg();
                tMsg.MMsgType = (MsgTypeEnum)(Random.Range(0, 99) % 2); ;
                tMsg.MPersonId = Random.Range(0, 99) % 2;
                tMsg.MSrtMsg = _mChatDemoStrList[Random.Range(0, 99) % count];
                tMsg.MPicMsgSpriteName = _mChatDemoPicList[Random.Range(0, 99) % count1];
                _mChatMsgList.Add(tMsg);
            }
        }

        public void AppendOneMsg()
        {
            int count = _mChatDemoStrList.Length;
            int count1 = _mChatDemoPicList.Length;
            ChatMsg tMsg = new ChatMsg();
            tMsg.MMsgType = (MsgTypeEnum)(Random.Range(0, 99) % 2); ;
            tMsg.MPersonId = Random.Range(0, 99) % 2;
            tMsg.MSrtMsg = _mChatDemoStrList[Random.Range(0, 99) % count];
            tMsg.MPicMsgSpriteName = _mChatDemoPicList[Random.Range(0, 99) % count1];
            _mChatMsgList.Add(tMsg);
        }

    }

}