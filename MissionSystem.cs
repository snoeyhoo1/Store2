using System;
using UnityEngine;

namespace Store1
{
    public enum MissionType { Serve, Kitchen, Staff, Revenue, Expansion, ShopStage, Gems }

    [Serializable]
    public class MissionData
    {
        public string id; public string title; public MissionType type; public int target; public int moneyReward; public int gemReward;
        public MissionData(string i,string t,MissionType ty,int tar,int money,int gems){id=i;title=t;type=ty;target=tar;moneyReward=money;gemReward=gems;}
    }

    public class MissionSystem : MonoBehaviour
    {
        public static MissionSystem I {get;private set;}
        public event Action Changed;
        public MissionData[] Missions {get;private set;}
        public int CurrentIndex {get;private set;}
        const string Key="STORE1_MISSION_INDEX_V2";
        void Awake(){if(I!=null&&I!=this){Destroy(gameObject);return;}I=this;DontDestroyOnLoad(gameObject);Build();CurrentIndex=Mathf.Clamp(PlayerPrefs.GetInt(Key,0),0,10);}
        void Build(){Missions=new[]{
            new MissionData("m1","손님 20명 받기",MissionType.Serve,20,2000,5),
            new MissionData("m2","주방 Lv.3 달성",MissionType.Kitchen,3,5000,5),
            new MissionData("m3","직원 3명 고용",MissionType.Staff,3,8000,8),
            new MissionData("m4","누적 매출 30,000",MissionType.Revenue,30000,12000,10),
            new MissionData("m5","가게 확장 Lv.4",MissionType.Expansion,4,20000,10),
            new MissionData("m6","두 번째 가게 열기",MissionType.ShopStage,2,30000,15),
            new MissionData("m7","손님 500명 받기",MissionType.Serve,500,80000,25),
            new MissionData("m8","보석 100개 모으기",MissionType.Gems,100,0,30)
        };}
        void Update(){if(GameState.I==null||CurrentIndex>=Missions.Length)return;if(GetProgress()>=Missions[CurrentIndex].target)Complete();}
        public int GetProgress(){var d=GameState.I.Data;var m=Missions[CurrentIndex];return m.type switch{MissionType.Serve=>d.served,MissionType.Kitchen=>d.kitchen,MissionType.Staff=>d.staff,MissionType.Revenue=>(int)Mathf.Min(int.MaxValue,d.lifetimeRevenue),MissionType.Expansion=>d.expansion,MissionType.ShopStage=>d.shopStage,MissionType.Gems=>d.gems,_=>0};}
        public string GetText(){if(CurrentIndex>=Missions.Length)return "모든 미션 완료!";var m=Missions[CurrentIndex];return $"{m.title}  {Mathf.Min(GetProgress(),m.target):N0}/{m.target:N0}";}
        public bool Complete(){if(CurrentIndex>=Missions.Length)return false;var m=Missions[CurrentIndex];GameState.I.AddMoney(m.moneyReward);GameState.I.AddGems(m.gemReward);GameState.I.Data.missionsCompleted++;CurrentIndex++; AudioManager.I?.Play(Sfx.Mission); PlayerPrefs.SetInt(Key,CurrentIndex);PlayerPrefs.Save();Changed?.Invoke();return true;}
        public void ResetMissions(){CurrentIndex=0;PlayerPrefs.SetInt(Key,0);PlayerPrefs.Save();Changed?.Invoke();}
    }
}
