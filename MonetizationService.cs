using System;
using UnityEngine;

namespace Store1
{
    public enum AdReward { DoubleIncome10Min, Cash, Gems }
    public enum ProductId { Gems100, Gems650, Gems1400, StarterPack, RemoveAds }

    public class MonetizationService : MonoBehaviour
    {
        public static MonetizationService I {get;private set;}
        public event Action Changed;
        public bool TestMode=true;
        void Awake(){if(I!=null&&I!=this){Destroy(gameObject);return;}I=this;DontDestroyOnLoad(gameObject);}

        // SDK-free gameplay layer. Replace these methods with the production Ad/IAP provider before store release.
        public bool ShowRewarded(AdReward reward)
        {
            if(GameState.I==null)return false;
            GrantAdReward(reward); AudioManager.I?.Play(Sfx.Reward); return true;
        }
        void GrantAdReward(AdReward reward)
        {
            switch(reward){case AdReward.DoubleIncome10Min:GameState.I.ActivateDoubleIncome(10);break;case AdReward.Cash:GameState.I.AddMoney(GameState.I.GetBasePrice()*50);break;case AdReward.Gems:GameState.I.AddGems(10);break;}
            Changed?.Invoke();
        }
        public bool ShowInterstitialIfAllowed(){return GameState.I!=null&&!GameState.I.Data.adsRemoved;}

        public bool Purchase(ProductId product)
        {
            if(GameState.I==null)return false;
            switch(product){
                case ProductId.Gems100:GameState.I.AddGems(100);break;
                case ProductId.Gems650:GameState.I.AddGems(650);break;
                case ProductId.Gems1400:GameState.I.AddGems(1400);break;
                case ProductId.StarterPack:GameState.I.AddMoney(30000);GameState.I.AddGems(150);break;
                case ProductId.RemoveAds:GameState.I.MarkAdsRemoved();break;
            }
            Changed?.Invoke(); AudioManager.I?.Play(Sfx.Gem); return true;
        }
    }
}
