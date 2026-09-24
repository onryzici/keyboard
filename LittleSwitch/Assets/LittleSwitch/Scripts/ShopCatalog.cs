using UnityEngine;

namespace LittleSwitch {
// A dedicated script asset is required for Unity to deserialize this catalog in a player build.
[CreateAssetMenu(menuName="Little Switch/Catalog")]
public class ShopCatalog : ScriptableObject {
 public PartDefinition casePart, pcb; public PartDefinition[] switches, keycaps; public CustomerOrder[] orders;
}
}
