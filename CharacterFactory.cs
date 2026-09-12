using UnityEngine;

namespace Store1
{
    public static class CharacterFactory
    {
        public static GameObject CreateCustomer(string name, Vector3 pos, Transform parent)
        {
            var root = New(name, pos, parent);
            Make(root.transform, "Body", PrimitiveType.Capsule, new Vector3(0,.72f,0), new Vector3(.52f,.68f,.52f), new Color(.34f,.56f,.84f));
            Make(root.transform, "Shirt", PrimitiveType.Cube, new Vector3(0,.72f,.20f), new Vector3(.39f,.45f,.08f), new Color(.25f,.43f,.72f));
            Make(root.transform, "Head", PrimitiveType.Sphere, new Vector3(0,1.62f,0), new Vector3(.50f,.50f,.50f), new Color(.95f,.72f,.55f));
            Make(root.transform, "Hair", PrimitiveType.Sphere, new Vector3(0,1.88f,-.05f), new Vector3(.53f,.28f,.53f), new Color(.16f,.10f,.06f));
            Make(root.transform, "EyeL", PrimitiveType.Sphere, new Vector3(-.14f,1.65f,.43f), new Vector3(.055f,.075f,.035f), Color.black);
            Make(root.transform, "EyeR", PrimitiveType.Sphere, new Vector3(.14f,1.65f,.43f), new Vector3(.055f,.075f,.035f), Color.black);
            Make(root.transform, "ArmL", PrimitiveType.Capsule, new Vector3(-.40f,.78f,0), new Vector3(.13f,.30f,.13f), new Color(.95f,.72f,.55f));
            Make(root.transform, "ArmR", PrimitiveType.Capsule, new Vector3(.40f,.78f,0), new Vector3(.13f,.30f,.13f), new Color(.95f,.72f,.55f));
            Make(root.transform, "LegL", PrimitiveType.Capsule, new Vector3(-.16f,.28f,0), new Vector3(.15f,.32f,.15f), new Color(.12f,.16f,.22f));
            Make(root.transform, "LegR", PrimitiveType.Capsule, new Vector3(.16f,.28f,0), new Vector3(.15f,.32f,.15f), new Color(.12f,.16f,.22f));
            Make(root.transform, "Bag", PrimitiveType.Cube, new Vector3(.48f,.72f,0), new Vector3(.18f,.35f,.30f), new Color(.45f,.20f,.10f));
            root.AddComponent<CharacterMotion>();
            return root;
        }

        public static GameObject CreateStaff(string name, Vector3 pos, Transform parent)
        {
            var root = New(name, pos, parent);
            Make(root.transform, "Body", PrimitiveType.Capsule, new Vector3(0,.72f,0), new Vector3(.52f,.68f,.52f), new Color(.92f,.94f,.95f));
            Make(root.transform, "Apron", PrimitiveType.Cube, new Vector3(0,.76f,.28f), new Vector3(.44f,.56f,.12f), new Color(.22f,.38f,.27f));
            Make(root.transform, "Head", PrimitiveType.Sphere, new Vector3(0,1.62f,0), new Vector3(.50f,.50f,.50f), new Color(.95f,.72f,.55f));
            Make(root.transform, "Hat", PrimitiveType.Cylinder, new Vector3(0,2.00f,0), new Vector3(.36f,.18f,.36f), Color.white);
            Make(root.transform, "EyeL", PrimitiveType.Sphere, new Vector3(-.14f,1.65f,.43f), new Vector3(.055f,.075f,.035f), Color.black);
            Make(root.transform, "EyeR", PrimitiveType.Sphere, new Vector3(.14f,1.65f,.43f), new Vector3(.055f,.075f,.035f), Color.black);
            Make(root.transform, "ArmL", PrimitiveType.Capsule, new Vector3(-.40f,.80f,0), new Vector3(.13f,.30f,.13f), new Color(.95f,.72f,.55f));
            Make(root.transform, "ArmR", PrimitiveType.Capsule, new Vector3(.40f,.80f,0), new Vector3(.13f,.30f,.13f), new Color(.95f,.72f,.55f));
            Make(root.transform, "LegL", PrimitiveType.Capsule, new Vector3(-.16f,.28f,0), new Vector3(.15f,.32f,.15f), new Color(.16f,.20f,.16f));
            Make(root.transform, "LegR", PrimitiveType.Capsule, new Vector3(.16f,.28f,0), new Vector3(.15f,.32f,.15f), new Color(.16f,.20f,.16f));
            root.AddComponent<CharacterMotion>();
            return root;
        }

        static GameObject New(string n, Vector3 p, Transform parent)
        {
            var g = new GameObject(n);
            g.transform.SetParent(parent);
            g.transform.position = p;
            return g;
        }

        static void Make(Transform p, string n, PrimitiveType type, Vector3 pos, Vector3 scale, Color c)
        {
            var x = GameObject.CreatePrimitive(type);
            x.name = n;
            x.transform.SetParent(p);
            x.transform.localPosition = pos;
            x.transform.localScale = scale;
            x.GetComponent<Renderer>().material.color = c;
        }
    }

    public class CharacterMotion : MonoBehaviour
    {
        Vector3 basePos;
        float phase;
        public bool walking;
        public float walkSpeed = .9f;

        void Start() { basePos = transform.position; phase = Random.value * 10f; }

        void Update()
        {
            phase += Time.deltaTime * (walking ? walkSpeed * 7f : 2.2f);
            var p = basePos;
            p.y += Mathf.Abs(Mathf.Sin(phase)) * (walking ? .055f : .018f);
            transform.position = Vector3.Lerp(transform.position, p, Time.deltaTime * 8f);
            if (walking) transform.Rotate(0, Mathf.Sin(phase) * .3f, 0);
        }

        public void SetBase(Vector3 p) { basePos = p; }
    }
}
