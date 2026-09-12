using UnityEngine;
using UnityEngine.UI;
namespace Store1
{
 public class GameUI:MonoBehaviour
 {
  Canvas canvas; Text moneyText,gemsText,titleText,statsText,nextText,missionText,rewardText,timerText,toastText; Button[] upgrade=new Button[5]; Button daily,adDouble,adCash,shop; float toastTimer;
  Color panel=new Color(.055f,.08f,.11f,.88f), accent=new Color(.95f,.67f,.20f), green=new Color(.20f,.52f,.34f);
  void Start(){Build();GameState.I.Changed+=Refresh;MissionSystem.I.Changed+=Refresh;Refresh();}
  void OnDestroy(){if(GameState.I!=null)GameState.I.Changed-=Refresh;if(MissionSystem.I!=null)MissionSystem.I.Changed-=Refresh;}
  void Update(){if(toastTimer>0){toastTimer-=Time.deltaTime;if(toastTimer<=0)toastText.text="";}RefreshTimers();}
  void Build(){
   var cg=new GameObject("Canvas");cg.transform.SetParent(transform);canvas=cg.AddComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceOverlay;canvas.sortingOrder=50;var scaler=cg.AddComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(1080,1920);cg.AddComponent<GraphicRaycaster>();
   Panel("TopHUD",new Vector2(.5f,1),new Vector2(1040,190),new Vector2(0,-105),panel);
   Panel("MissionCard",new Vector2(.5f,1),new Vector2(930,145),new Vector2(0,-255),new Color(.06f,.12f,.13f,.92f));
   Panel("BottomBar",new Vector2(.5f,0),new Vector2(1040,340),new Vector2(0,155),new Color(.04f,.06f,.08f,.94f));
   moneyText=Label("Money",new Vector2(0,1),new Vector2(470,85),new Vector2(55,-55),42,TextAnchor.MiddleLeft,Color.white);
   gemsText=Label("Gems",new Vector2(1,1),new Vector2(300,70),new Vector2(-45,-48),30,TextAnchor.MiddleRight,new Color(1f,.86f,.35f));
   titleText=Label("Title",new Vector2(.5f,1),new Vector2(500,65),new Vector2(0,-32),29,TextAnchor.MiddleCenter,Color.white);
   statsText=Label("Stats",new Vector2(1,1),new Vector2(440,75),new Vector2(-35,-118),19,TextAnchor.MiddleRight,new Color(.78f,.84f,.87f));
   missionText=Label("Mission",new Vector2(.5f,1),new Vector2(850,65),new Vector2(0,-225),22,TextAnchor.MiddleCenter,new Color(1f,.91f,.64f));
   rewardText=Label("Reward",new Vector2(.5f,1),new Vector2(850,45),new Vector2(0,-285),17,TextAnchor.MiddleCenter,new Color(.78f,.84f,.87f));
   nextText=Label("Next",new Vector2(.5f,0),new Vector2(850,55),new Vector2(0,310),20,TextAnchor.MiddleCenter,new Color(1f,.88f,.55f));
   timerText=Label("Timer",new Vector2(.5f,0),new Vector2(600,45),new Vector2(0,270),17,TextAnchor.MiddleCenter,new Color(.70f,.80f,.84f));
   toastText=Label("Toast",new Vector2(.5f,.5f),new Vector2(900,100),new Vector2(0,0),29,TextAnchor.MiddleCenter,Color.white);
   string[] n={"주방","직원","좌석","확장","다음 가게"};for(int i=0;i<5;i++){int c=i;upgrade[i]=Button(n[i],new Vector2(.5f,0),new Vector2(188,112),new Vector2((i-2)*196,75),20);upgrade[i].onClick.AddListener(()=>ClickUpgrade(c));}
   daily=Button("일일보상",new Vector2(0,0),new Vector2(190,75),new Vector2(110,230),18);daily.onClick.AddListener(ClaimDaily);
   adDouble=Button("수익 2배",new Vector2(0,0),new Vector2(190,75),new Vector2(315,230),18);adDouble.onClick.AddListener(()=>Ad(AdReward.DoubleIncome10));
   adCash=Button("보너스 돈",new Vector2(0,0),new Vector2(190,75),new Vector2(520,230),18);adCash.onClick.AddListener(()=>Ad(AdReward.Cash));
   shop=Button("보석 상점",new Vector2(1,0),new Vector2(190,75),new Vector2(-110,230),18);shop.onClick.AddListener(BuyGems);
  }
  GameObject Panel(string n,Vector2 a,Vector2 s,Vector2 p,Color c){var go=new GameObject(n);go.transform.SetParent(canvas.transform);var r=go.AddComponent<RectTransform>();r.anchorMin=a;r.anchorMax=a;r.sizeDelta=s;r.anchoredPosition=p;var im=go.AddComponent<Image>();im.color=c;return go;}
  Text Label(string n,Vector2 a,Vector2 s,Vector2 p,int f,TextAnchor al,Color c){var go=new GameObject(n);go.transform.SetParent(canvas.transform);var r=go.AddComponent<RectTransform>();r.anchorMin=a;r.anchorMax=a;r.sizeDelta=s;r.anchoredPosition=p;var t=go.AddComponent<Text>();t.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");t.fontSize=f;t.alignment=al;t.color=c;t.horizontalOverflow=HorizontalWrapMode.Overflow;t.verticalOverflow=VerticalWrapMode.Overflow;return t;}
  Button Button(string text,Vector2 a,Vector2 s,Vector2 p,int f){var go=new GameObject(text);go.transform.SetParent(canvas.transform);var r=go.AddComponent<RectTransform>();r.anchorMin=a;r.anchorMax=a;r.sizeDelta=s;r.anchoredPosition=p;var img=go.AddComponent<Image>();img.color=new Color(.10f,.16f,.18f,.97f);var b=go.AddComponent<Button>();var colors=b.colors;colors.normalColor=new Color(.10f,.16f,.18f,.97f);colors.highlightedColor=new Color(.18f,.27f,.29f,.99f);colors.pressedColor=accent;colors.disabledColor=new Color(.10f,.12f,.13f,.55f);b.colors=colors;var l=Label("Text",new Vector2(.5f,.5f),new Vector2(s.x-8,s.y-8),Vector2.zero,f,TextAnchor.MiddleCenter,Color.white);l.transform.SetParent(go.transform,false);l.text=text;return b;}
  void ClickUpgrade(int i){bool ok=i switch{0=>GameState.I.BuyKitchen(),1=>GameState.I.HireStaff(),2=>GameState.I.BuyTable(),3=>GameState.I.Expand(),4=>GameState.I.OpenNextShop(),_=>false};AudioManager.I?.Play(ok?(i==4?Sfx.Shop:Sfx.Upgrade):Sfx.Coin,ok?1f:.25f);Toast(ok?(i==4?"새 가게가 열렸어요!":"업그레이드 완료!"):"돈이 부족해요");}
  void ClaimDaily(){bool ok=GameState.I.ClaimDailyGift();AudioManager.I?.Play(ok?Sfx.Gem:Sfx.Coin);Toast(ok?"보석 25개 획득!":"아직 일일보상 시간이 아니에요");}
  void Ad(AdReward r){bool ok=MonetizationService.I.ShowRewarded(r);AudioManager.I?.Play(ok?Sfx.Reward:Sfx.Coin);Toast(ok?"보상을 받았어요!":"광고를 준비하지 못했어요");}
  void BuyGems(){MonetizationService.I.Purchase(ProductId.Gems100);AudioManager.I?.Play(Sfx.Gem);Toast("보석 100개 획득!");}
  void Toast(string s){toastText.text=s;toastTimer=1.4f;Refresh();}
  void Refresh(){if(GameState.I==null)return;var d=GameState.I.Data;moneyText.text=$"💰 {d.money:N0}";gemsText.text=$"💎 {d.gems:N0}";titleText.text=$"{StoreWorld.GetShopName(d.shopStage)}  ·  Lv.{d.expansion}";statsText.text=$"주방 {d.kitchen}   직원 {d.staff}   좌석 {d.tables}\n손님 {d.served:N0}   평판 {d.reputation:0}";missionText.text=MissionSystem.I!=null?"🎯 "+MissionSystem.I.GetText():"";if(MissionSystem.I!=null&&MissionSystem.I.CurrentIndex<MissionSystem.I.Missions.Length){var m=MissionSystem.I.Missions[MissionSystem.I.CurrentIndex];rewardText.text=$"보상  💰 {m.moneyReward:N0}   💎 {m.gemReward}";}else rewardText.text="";nextText.text=d.shopStage>=5?"최종 가게를 완성했어요!":"다음 가게까지  💰 "+GameState.I.GetNextShopCost().ToString("N0")+"   ·   분당 "+GameState.I.GetIncomePerMinute().ToString("N0");upgrade[0].GetComponentInChildren<Text>().text=$"주방\nLv.{d.kitchen}  ·  {GameState.I.KitchenCost:N0}";upgrade[1].GetComponentInChildren<Text>().text=$"직원\nLv.{d.staff}  ·  {GameState.I.StaffCost:N0}";upgrade[2].GetComponentInChildren<Text>().text=$"좌석\n{d.tables}/10  ·  {GameState.I.TablesCost:N0}";upgrade[3].GetComponentInChildren<Text>().text=$"확장\nLv.{d.expansion}/6  ·  {GameState.I.ExpansionCost:N0}";upgrade[4].GetComponentInChildren<Text>().text=d.shopStage>=5?"완료":$"다음 가게\n{GameState.I.GetNextShopCost():N0}";}
  void RefreshTimers(){if(GameState.I==null)return;long r=GameState.I.DailyGiftRemaining();daily.GetComponentInChildren<Text>().text=r<=0?"일일보상 준비됨":$"일일보상 {r/3600:00}:{(r%3600)/60:00}";timerText.text=GameState.I.IsDoubleIncomeActive?"🔥 수익 2배 활성화 중":"";}
 }
}
