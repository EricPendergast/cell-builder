using UnityEngine;

[CreateAssetMenu(fileName = "GelSettings", menuName = "ScriptableObjects/GelSettings")]
public class GelSettings : SingletonScriptableObject<GelSettings> {
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init() {
        SingletonInit();
    }

    [System.Serializable]
    public struct Data {
        public float springConstant;
        public float springLengthMultiplier;
        public float dampingConstant;
        public float solidCoreMultiplier;
    }
    public Data data;
}
