using System.Collections.Generic;
using UnityEngine;

namespace Store1
{
    public class StoreWorld : MonoBehaviour
    {
        readonly List<CustomerAgent> customers = new();
        readonly List<StaffAgent> staff = new();
        Transform customerRoot, staffRoot, storeRoot;
        float spawnTimer;
        int lastStage=-1,lastKitchen=-1,lastTables=-1,lastExpansion=-1,lastStaff=-1;
        static readonly string[] ShopNames={"작은 카페","동네 분식집","패스트푸드점","레스토랑","대형 레스토랑"};
        public static string GetShopName(int stage)=>ShopNames[Mathf.Clamp(stage-1,0,ShopNames.Length-1)];
        static readonly Color Wood=new(.42f,.22f,.11f), Cream=new(.96f,.88f,.72f), Green=new(.20f,.48f,.30f), Gold=new(.95f,.67f,.20f);

        void Start(){
            storeRoot=new GameObject("3D_Store").transform; customerRoot=new GameObject("Customers").transform; staffRoot=new GameObject("Staff").transform;
            BuildWorld(); GameState.I.Changed+=Refresh; Refresh();
        }
        void Update(){
            if(GameState.I==null)return;
            spawnTimer-=Time.deltaTime;
            int desired=Mathf.Clamp(GameState.I.Data.tables+GameState.I.Data.expansion-1,2,9);
            if(spawnTimer<=0&&customers.Count<desired){SpawnCustomer();spawnTimer=Mathf.Max(.9f,3.2f/GameState.I.GetServiceSpeed());}
            CleanupCustomers();
        }
        void BuildWorld(){
            Make("Ground",new Vector3(0,-.18f,0),new Vector3(18,.35f,15),new Color(.70f,.72f,.70f));
            Make("Road",new Vector3(0,-.02f,-5.7f),new Vector3(18,.08f,2.4f),new Color(.18f,.19f,.20f));
            for(int i=-8;i<=8;i+=2) Make("RoadMark",new Vector3(i,.035f,-5.7f),new Vector3(1.0f,.02f,.12f),new Color(.88f,.76f,.40f));
            BuildStreetProps();
            BuildStoreShell(); BuildTables(); BuildDecor();
        }
        void BuildStoreShell(){
            Make("BackWall",new Vector3(0,2.35f,5.35f),new Vector3(15,4.7f,.25f),Cream,storeRoot);
            Make("LeftWall",new Vector3(-7.35f,2.35f,.1f),new Vector3(.25f,4.7f,10.5f),Cream,storeRoot);
            Make("RightWall",new Vector3(7.35f,2.35f,.1f),new Vector3(.25f,4.7f,10.5f),Cream,storeRoot);
            Make("FloorBase",new Vector3(0,.04f,.1f),new Vector3(14.4f,.1f,10.2f),new Color(.88f,.76f,.57f),storeRoot);
            for(int x=-6;x<=6;x+=2) for(int z=-4;z<=4;z+=2) Make("Tile",new Vector3(x,.105f,z),new Vector3(1.85f,.025f,1.85f),new Color(.91f,.82f,.66f));
            Make("Counter",new Vector3(0,.72f,3.35f),new Vector3(6.2f,1.35f,1.05f),Wood,storeRoot);
            Make("CounterTop",new Vector3(0,1.42f,3.35f),new Vector3(6.45f,.16f,1.12f),new Color(.70f,.42f,.20f),storeRoot);
            Make("KitchenBack",new Vector3(0,1.25f,4.25f),new Vector3(6.0f,2.2f,.65f),new Color(.75f,.77f,.76f),storeRoot);
            for(int i=-2;i<=2;i++) Make("KitchenUnit",new Vector3(i*1.05f,1.65f,3.86f),new Vector3(.72f,.65f,.55f),new Color(.38f,.42f,.43f),storeRoot);
            BuildWindows(); BuildAwning(); BuildSign(); BuildDoor();
        }
        void BuildWindows(){
            for(int i=-2;i<=2;i++){
                float x=i*2.45f;
                Make("WindowFrame",new Vector3(x,2.35f,5.18f),new Vector3(1.9f,1.55f,.12f),Wood,storeRoot);
                Make("Window",new Vector3(x,2.35f,5.105f),new Vector3(1.62f,1.27f,.04f),new Color(.43f,.72f,.83f),storeRoot);
                Make("WindowCross",new Vector3(x,2.35f,5.03f),new Vector3(.08f,1.25f,.05f),Cream,storeRoot);
                Make("WindowCross",new Vector3(x,2.35f,5.03f),new Vector3(1.58f,.08f,.05f),Cream,storeRoot);
            }
        }
        void BuildAwning(){
            for(int i=-6;i<=6;i+=2){
                var c=(i/2)%2==0?new Color(.85f,.25f,.22f):new Color(.98f,.89f,.70f);
                Make("Awning",new Vector3(i*.95f,4.45f,5.0f),new Vector3(1.8f,.32f,.75f),c,storeRoot);
            }
        }
        void BuildSign(){
            Make("SignBoard",new Vector3(0,4.55f,4.78f),new Vector3(6.3f,.95f,.16f),new Color(.16f,.34f,.23f),storeRoot);
            Make("SignGlow",new Vector3(0,4.56f,4.68f),new Vector3(5.5f,.56f,.03f),new Color(1f,.82f,.36f),storeRoot);
        }
        void BuildDoor(){
            Make("DoorFrame",new Vector3(5.75f,1.65f,5.16f),new Vector3(1.55f,3.2f,.16f),Wood,storeRoot);
            Make("DoorGlass",new Vector3(5.75f,1.7f,5.05f),new Vector3(1.15f,2.65f,.04f),new Color(.24f,.50f,.58f),storeRoot);
            Make("DoorHandle",new Vector3(5.35f,1.65f,4.98f),new Vector3(.08f,.42f,.08f),Gold,storeRoot);
        }
        void BuildTables(Transform parent=null){
            parent??=storeRoot; int count=GameState.I==null?2:GameState.I.Data.tables; int cols=Mathf.Min(4,Mathf.CeilToInt(Mathf.Sqrt(count)));
            for(int i=0;i<count;i++){
                int row=i/cols,col=i%cols; float x=(col-(cols-1)*.5f)*2.65f; float z=.65f-row*2.05f;
                Make("Table",new Vector3(x,.78f,z),new Vector3(1.45f,.16f,1.05f),new Color(.56f,.31f,.16f),parent);
                Make("TableLeg",new Vector3(x,.40f,z),new Vector3(.16f,.65f,.16f),Wood,parent);
                foreach(float dx in new[]{-.95f,.95f}) Make("Chair",new Vector3(x+dx,.43f,z),new Vector3(.42f,.72f,.42f),new Color(.20f,.42f,.30f),parent);
            }
        }
        void BuildDecor(){
            int exp=GameState.I.Data.expansion;
            for(int i=0;i<exp+2;i++){float x=-6.2f+i*2.0f; Make("PlantPot",new Vector3(x,.45f,2.75f),new Vector3(.48f,.55f,.48f),new Color(.70f,.38f,.20f)); Make("Plant",new Vector3(x,.95f,2.75f),new Vector3(.55f,1.1f,.55f),Green);}
            for(int i=0;i<3;i++){ Make("Pendant",new Vector3(-3+i*3,3.5f,2.8f),new Vector3(.12f,.7f,.12f),new Color(.20f,.20f,.18f)); var lamp=Make("Lamp",new Vector3(-3+i*3,3.08f,2.8f),new Vector3(.38f,.18f,.38f),new Color(1f,.76f,.28f)); if(i<2){var l=lamp.AddComponent<Light>();l.type=LightType.Point;l.range=4.2f;l.intensity=.65f;l.color=new Color(1f,.72f,.42f);l.shadows=LightShadows.None;}}
        }
        void BuildStreetProps(){
            for(int x=-7;x<=7;x+=4){ Make("TreeTrunk",new Vector3(x,.8f,-3.9f),new Vector3(.35f,1.6f,.35f),Wood); Make("TreeTop",new Vector3(x,1.9f,-3.9f),new Vector3(1.15f,1.15f,1.15f),Green); }
            Make("Bench",new Vector3(-5,.45f,-3.0f),new Vector3(2.0f,.35f,.55f),Wood);
            Make("BenchBack",new Vector3(-5,1.0f,-3.25f),new Vector3(2.0f,.7f,.18f),Wood);
        }
        void Refresh(){
            var d=GameState.I.Data;
            if(d.shopStage!=lastStage||d.kitchen!=lastKitchen||d.tables!=lastTables||d.expansion!=lastExpansion){lastStage=d.shopStage;lastKitchen=d.kitchen;lastTables=d.tables;lastExpansion=d.expansion;RebuildStoreVisuals();}
            if(d.staff!=lastStaff){lastStaff=d.staff;RebuildStaff();}
        }
        void RebuildStoreVisuals(){
            // Upgrade layers: brighter floor accents, extra decor and equipment.
            foreach(Transform t in storeRoot) Destroy(t.gameObject);
            BuildStoreShell(); BuildTables(); BuildDecor();
            int k=GameState.I.Data.kitchen;
            for(int i=0;i<k;i++) Make("KitchenUpgrade",new Vector3(-2.5f+i*.8f,2.35f,4.0f),new Vector3(.55f,.5f,.5f),new Color(.88f,.90f,.92f),storeRoot);
            int exp=GameState.I.Data.expansion;
            if(exp>=3){Make("SideWing",new Vector3(-7.9f,1.3f,.8f),new Vector3(1.0f,2.6f,7.0f),Cream,storeRoot);}
            if(exp>=5){Make("GoldTrim",new Vector3(0,4.9f,4.65f),new Vector3(7.2f,.16f,.16f),Gold,storeRoot);}
        }
        void RebuildStaff(){foreach(var s in staff)if(s)Destroy(s.gameObject);staff.Clear();for(int i=0;i<GameState.I.Data.staff;i++){var go=CharacterFactory.CreateStaff("Staff_"+i,new Vector3(-1.8f+i*.7f,.5f,2.1f),staffRoot);var a=go.AddComponent<StaffAgent>();a.Init(i);staff.Add(a);}}
        void SpawnCustomer(){float x=Random.Range(-4.8f,4.8f);var go=CharacterFactory.CreateCustomer("Customer",new Vector3(x,.5f,6.0f),customerRoot);var c=go.AddComponent<CustomerAgent>();c.Init(customers.Count);customers.Add(c);GameState.I.Data.totalCustomers++;}
        void CleanupCustomers(){for(int i=customers.Count-1;i>=0;i--)if(!customers[i])customers.RemoveAt(i);}
        GameObject Make(string n,Vector3 pos,Vector3 scale,Color color,Transform parent=null){var go=GameObject.CreatePrimitive(PrimitiveType.Cube);go.name=n;go.transform.SetParent(parent);go.transform.position=pos;go.transform.localScale=scale;go.GetComponent<Renderer>().material.color=color;return go;}
    }
}
