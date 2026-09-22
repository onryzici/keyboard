using System.Collections;
using UnityEngine;
namespace LittleSwitch {
public partial class ShopGame {
 WorkbenchTool selectedTool;
 GameObject toolCursor;
 public bool ToolEquipped => selectedTool;
 public string ToolHint => selectedTool ? (selectedTool.kind==WorkbenchTool.Kind.Puller ? "Sökücü: tuşa tıkla · Esc: bırak" : "Pense: sorunlu tuşa tıkla · Esc: bırak") : "";
 void EquipTool(WorkbenchTool tool){
  if(state.stage<BuildStage.Switches||state.stage>BuildStage.Test)return;
  ReleaseTool();selectedTool=tool;closeView=true;inspection=false;
  toolCursor=Instantiate(tool.gameObject);toolCursor.name="Tool in hand";
  foreach(var c in toolCursor.GetComponentsInChildren<Collider>())c.enabled=false;
  tool.gameObject.SetActive(false);ui.Refresh();
 }
 void ReleaseTool(){if(selectedTool)selectedTool.gameObject.SetActive(true);selectedTool=null;if(toolCursor)Destroy(toolCursor);toolCursor=null;if(ui)ui.Refresh();}
 void MoveTool(Ray ray){if(!toolCursor)return;var plane=new Plane(Vector3.up,new Vector3(0,4.38f,0));if(plane.Raycast(ray,out float d))toolCursor.transform.position=Vector3.Lerp(toolCursor.transform.position,ray.GetPoint(d),1-Mathf.Exp(-Time.deltaTime*20));}
 void UseTool(int index){
  if(!selectedTool||busy||index<0||index>=61)return;
  if(selectedTool.kind==WorkbenchTool.Kind.PinPliers){if(state.stage!=BuildStage.Test||state.fault!=index)return;}
  else if(state.stage==BuildStage.Test||state.stage<BuildStage.Switches||state.stage>BuildStage.Keycaps||(state.switches[index]<0&&state.caps[index]<0))return;
  StartCoroutine(UseToolMotion(index));
 }
 IEnumerator UseToolMotion(int index){
  busy=true;Vector3 start=toolCursor.transform.position;Vector3 end=board.SlotPosition(index)+Vector3.up*.12f;
  for(float t=0;t<1;t+=Time.deltaTime*5){toolCursor.transform.position=Vector3.Lerp(start,end,Mathf.SmoothStep(0,1,t));yield return null;}
  var piece=board.pieces[index];Vector3 rest=piece?piece.transform.localPosition:Vector3.zero;
  for(float t=0;t<1;t+=Time.deltaTime*5){toolCursor.transform.position=end+Vector3.up*(t*.22f);if(piece&&selectedTool.kind==WorkbenchTool.Kind.Puller)piece.transform.localPosition=rest+Vector3.up*t*.4f;yield return null;}
  Repair(index);busy=false;ui.Refresh();
 }
 float roomYaw,roomPitch;
 public void LookAround(Vector2 delta){if(closeView){benchYaw=Mathf.Clamp(benchYaw+delta.x*.055f,-18,18);benchPitch=Mathf.Clamp(benchPitch-delta.y*.04f,-8,8);return;}roomYaw=Mathf.Clamp(roomYaw+delta.x*.055f,-20,20);roomPitch=Mathf.Clamp(roomPitch-delta.y*.04f,-6,7);}
}
}
