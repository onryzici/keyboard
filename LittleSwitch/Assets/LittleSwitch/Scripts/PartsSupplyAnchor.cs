using UnityEngine;
namespace LittleSwitch { public class PartsSupplyAnchor:MonoBehaviour {public Transform target;public MovableDeskProp movable;void LateUpdate(){if(target)transform.position=target.position+Vector3.up*.055f;}} }
