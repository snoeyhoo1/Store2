using UnityEngine;

namespace Store1
{
    public class StaffAgent : MonoBehaviour
    {
        Vector3 home; float phase; CharacterMotion motion;
        public void Init(int index){home=transform.position; phase=index*.8f; motion=gameObject.AddComponent<CharacterMotion>(); motion.walking=true;}
        void Update()
        {
            if(GameState.I==null)return;
            phase+=Time.deltaTime*(1f+GameState.I.Data.kitchen*.04f);
            Vector3 target=new Vector3(Mathf.Sin(phase*.65f)*2.1f, .5f, 2.1f+Mathf.Cos(phase*.55f)*.65f);
            Vector3 dir=target-transform.position; dir.y=0;
            if(dir.sqrMagnitude>.05f){transform.position+=dir.normalized*Time.deltaTime*.85f; transform.rotation=Quaternion.Slerp(transform.rotation,Quaternion.LookRotation(dir),Time.deltaTime*6f);}
            motion.SetBase(transform.position);
        }
    }
}
