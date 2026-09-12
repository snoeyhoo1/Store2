using System;
using UnityEngine;

namespace Store1
{
    [Serializable]
    public class SaveData
    {
        public int shopStage = 1;
        public long money = 3000;
        public int gems = 25;
        public int kitchen = 1;
        public int staff = 1;
        public int tables = 2;
        public int expansion = 1;
        public int served = 0;
        public int totalCustomers = 0;
        public float reputation = 50f;
        public long lifetimeRevenue = 0;
        public int missionsCompleted = 0;
        public long lastSavedUnix = 0;
        public long dailyGiftClaimedUnix = 0;
        public long doubleIncomeUntilUnix = 0;
        public bool adsRemoved = false;
        public int rushCount = 0;
    }

    public class GameState : MonoBehaviour
    {
        public static GameState I { get; private set; }
        public SaveData Data { get; private set; }
        public event Action Changed;
        const string SaveKey = "STORE1_SIMPLE_TYCOON_V3";

        void Awake()
        {
            if (I != null && I != this) { Destroy(gameObject); return; }
            I = this; DontDestroyOnLoad(gameObject); Load();
        }

        public void Load()
        {
            long previous = 0;
            if (PlayerPrefs.HasKey(SaveKey))
            {
                try { Data = JsonUtility.FromJson<SaveData>(PlayerPrefs.GetString(SaveKey)); previous = Data.lastSavedUnix; }
                catch { Data = new SaveData(); }
            }
            else Data = new SaveData();
            ApplyOffline(previous);
            Data.lastSavedUnix = Now();
            Save(); Notify();
        }

        static long Now() => DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        void ApplyOffline(long previous)
        {
            if (previous <= 0) return;
            long elapsed = Math.Max(0, Math.Min(Now() - previous, 4 * 60 * 60));
            if (elapsed < 10) return;
            long income = Mathf.Max(1, GetIncomePerMinute());
            long offline = Mathf.RoundToInt(income * (elapsed / 60f) * .65f);
            if (offline > 0) Data.money += offline;
        }

        public long GetBasePrice() => 100L * (long)Mathf.Pow(6f, Data.shopStage - 1);
        public float GetRevenueMultiplier() => (1f + Data.kitchen*.10f + Data.expansion*.08f + Data.shopStage*.12f) * (IsDoubleIncomeActive ? 2f : 1f);
        public float GetServiceSpeed() => 1f + Data.staff*.16f + Data.kitchen*.08f;
        public int GetCapacity() => Mathf.Max(2, Data.tables * (1 + Data.expansion));
        public long GetIncomePerMinute() => Mathf.RoundToInt(Data.staff * GetBasePrice() * 2.1f * GetRevenueMultiplier());
        public bool IsDoubleIncomeActive => Data.doubleIncomeUntilUnix > Now();

        public long KitchenCost => ScaleCost(500, Data.kitchen, 1.55f);
        public long StaffCost => ScaleCost(900, Data.staff, 1.72f);
        public long TablesCost => ScaleCost(700, Data.tables - 1, 1.48f);
        public long ExpansionCost => ScaleCost(2500, Data.expansion, 2.05f);
        long ScaleCost(long b, int l, float g) => Mathf.Max(1, Mathf.RoundToInt(b * Mathf.Pow(g, Mathf.Max(0,l-1)) * Data.shopStage));

        public bool BuyKitchen(){if(!Spend(KitchenCost))return false;Data.kitchen++;AudioManager.I?.Play(Sfx.Upgrade);Notify();return true;}
        public bool HireStaff(){if(!Spend(StaffCost))return false;Data.staff++;AudioManager.I?.Play(Sfx.Staff);Notify();return true;}
        public bool BuyTable(){if(Data.tables>=10||!Spend(TablesCost))return false;Data.tables++;AudioManager.I?.Play(Sfx.Upgrade);Notify();return true;}
        public bool Expand(){if(Data.expansion>=6||!Spend(ExpansionCost))return false;Data.expansion++;AudioManager.I?.Play(Sfx.Expand);Notify();return true;}
        bool Spend(long a){if(Data.money<a)return false;Data.money-=a;return true;}

        public void Earn(long amount)
        {
            amount = Mathf.Max(1, Mathf.RoundToInt(amount * (IsDoubleIncomeActive ? 2f : 1f)));
            Data.money += amount; Data.lifetimeRevenue += amount; Data.served++; AudioManager.I?.Play(Sfx.Coin);
            Data.reputation = Mathf.Clamp(Data.reputation+.08f,0,100); Notify();
        }
        public void Miss(){Data.reputation=Mathf.Clamp(Data.reputation-.35f,0,100);Notify();}
        public void AddGems(int amount){Data.gems=Mathf.Max(0,Data.gems+amount);Notify();}
        public bool SpendGems(int amount){if(Data.gems<amount)return false;Data.gems-=amount;Notify();return true;}

        public bool CanOpenNextShop()=>Data.shopStage<5&&Data.money>=GetNextShopCost();
        public long GetNextShopCost()=>Mathf.RoundToInt(60000f*Mathf.Pow(15f,Data.shopStage-1));
        public bool OpenNextShop()
        {
            long cost=GetNextShopCost(); if(Data.shopStage>=5||Data.money<cost)return false;
            Data.money-=cost; AudioManager.I?.Play(Sfx.Shop); Data.shopStage++; Data.kitchen=1;Data.staff=1;Data.tables=2;Data.expansion=1;Data.reputation=50; Notify(); return true;
        }

        public bool ClaimDailyGift()
        {
            long now=Now(); if(Data.dailyGiftClaimedUnix>0 && now-Data.dailyGiftClaimedUnix<20*60*60)return false;
            Data.dailyGiftClaimedUnix=now; Data.gems+=25; AudioManager.I?.Play(Sfx.Gem); Notify(); return true;
        }
        public long DailyGiftRemaining(){long r=20*60*60-(Now()-Data.dailyGiftClaimedUnix);return Math.Max(0,r);}
        public void ActivateDoubleIncome(int minutes){Data.doubleIncomeUntilUnix=Math.Max(Data.doubleIncomeUntilUnix,Now()+minutes*60);Notify();}
        public void AddMoney(long amount){Data.money+=Math.Max(0,amount);Notify();}
        public void MarkAdsRemoved(){Data.adsRemoved=true;Notify();}
        public void CountRush(){Data.rushCount++;Notify();}

        public void Notify(){Save();Changed?.Invoke();}
        public void Save(){if(Data==null)return;Data.lastSavedUnix=Now();PlayerPrefs.SetString(SaveKey,JsonUtility.ToJson(Data));PlayerPrefs.Save();}
        public void ResetGame(){Data=new SaveData();Save();Notify();}
    }
}
