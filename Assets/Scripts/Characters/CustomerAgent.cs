using UnityEngine;

namespace Store1
{
    public class CustomerAgent : MonoBehaviour
    {
        enum State { Enter, Sit, Wait, Eat, Leave }
        State state=State.Enter; Vector3 target; float timer; int index; CharacterMotion motion;
        public void Init(int i)
        {
            index=i; motion=gameObject.AddComponent<CharacterMotion>(); motion.walking=true;
            target=new Vector3(Random.Range(-4f,4f),.5f,Random.Range(-1.2f,1.8f));
            timer=Random.Range(.3f,.8f);
        }
        void Update()
        {
            if(GameState.I==null) return;
            if(state==State.Enter || state==State.Leave)
            {
                MoveTo(target);
                if(Vector3.Distance(transform.position,target)<.12f)
                {
                    if(state==State.Enter){state=State.Sit; timer=Random.Range(.3f,.7f); motion.walking=false; AudioManager.I?.Play(Sfx.Customer); AudioManager.I?.Play(Sfx.Sit);}
                    else Destroy(gameObject);
                }
            }
            else if(state==State.Sit)
            {
                timer-=Time.deltaTime; if(timer<=0){state=State.Wait; timer=Random.Range(1.2f,2.8f);}
            }
            else if(state==State.Wait)
            {
                timer-=Time.deltaTime;
                if(timer<=0)
                {
                    long reward=Mathf.RoundToInt(GameState.I.GetBasePrice()*GameState.I.GetRevenueMultiplier());
                    GameState.I.Earn(reward); AudioManager.I?.Play(Sfx.Serve); state=State.Eat; timer=Random.Range(1.2f,2.2f);
                }
            }
            else if(state==State.Eat)
            {
                timer-=Time.deltaTime;
                if(timer<=0){state=State.Leave; motion.walking=true; target=new Vector3(transform.position.x, .5f, 6.2f);}
            }
        }
        void MoveTo(Vector3 p)
        {
            Vector3 dir=p-transform.position; dir.y=0; if(dir.sqrMagnitude<.001f)return;
            transform.position += dir.normalized*Time.deltaTime*(1.1f+GameState.I.GetServiceSpeed()*.12f);
            transform.rotation=Quaternion.Slerp(transform.rotation,Quaternion.LookRotation(dir),Time.deltaTime*7f);
            motion.SetBase(transform.position); motion.walking=true;
        }
    }
}
