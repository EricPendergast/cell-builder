using UnityEngine;

[CreateAssetMenu(fileName = "ViscositySettings", menuName = "ScriptableObjects/ViscositySettings")]
public class ViscositySettings : SingletonScriptableObject<ViscositySettings> {
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init() {
        SingletonInit();
    }

    [System.Serializable]
    public struct Data {
        [Tooltip("For highly viscous behavior, make this nonzero")]
        public float sigma;
        [Tooltip("Prevents particle interpenetration")]
        public float beta;
    }
    public Data data;
}
