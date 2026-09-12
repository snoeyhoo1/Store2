using UnityEngine;

namespace Store1
{
    public class Store1Bootstrap : MonoBehaviour
    {
        void Awake()
        {
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;
            if (FindFirstObjectByType<AudioManager>() == null)
                new GameObject("AudioManager").AddComponent<AudioManager>();
            if (FindFirstObjectByType<GameState>() == null)
                new GameObject("GameState").AddComponent<GameState>();
        }

        void Start()
        {
            if (FindFirstObjectByType<StoreWorld>() == null)
                new GameObject("StoreWorld").AddComponent<StoreWorld>();
            if (FindFirstObjectByType<CameraController>() == null)
                new GameObject("CameraController").AddComponent<CameraController>();
            if (FindFirstObjectByType<GameUI>() == null)
                new GameObject("GameUI").AddComponent<GameUI>();
            if (FindFirstObjectByType<MissionSystem>() == null)
                new GameObject("MissionSystem").AddComponent<MissionSystem>();
            if (FindFirstObjectByType<MonetizationService>() == null)
                new GameObject("MonetizationService").AddComponent<MonetizationService>();
        }
    }
}
