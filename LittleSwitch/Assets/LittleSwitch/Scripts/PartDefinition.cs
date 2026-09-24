using UnityEngine;
namespace LittleSwitch {
public enum PartKind { Case, PCB, Switch, Keycaps }
[CreateAssetMenu(menuName="Little Switch/Part")]
public class PartDefinition : ScriptableObject {
 public string id, displayName, description; public PartKind kind; public int price;
 public string layout="60"; public Color primary=Color.white, accent=Color.white;
 public float pitch=1, volume=.35f; public string feel;
}
[System.Serializable] public class CustomerOrder {
 public string name, request; public int budget; public string switchId, keycapId;
}
}
