using System.Collections;
using UnityEngine;
namespace LittleSwitch {
public partial class ShopGame {
 WorkbenchTool selectedTool;
 GameObject toolCursor;
 public bool ToolEquipped => selectedTool;
 void QuickToolAction(int index){
  var kind=state.stage==BuildStage.Test?WorkbenchTool.Kind.PinPliers:WorkbenchTool.Kind.Puller;
  if(selectedTool&&selectedTool.kind==kind){UseTool(index);return;}
  ReleaseTool();foreach(var tool in FindObjectsByType<WorkbenchTool>())if(tool.kind==kind){EquipTool(tool);UseTool(index);return;}
 }
 public string ToolHint => selectedTool ? (selectedTool.kind==WorkbenchTool.Kind.Puller ? "Sökücü: tuşa tıkla · Esc: bırak" : "Pense: sorunlu tuşa tıkla · Esc: bırak") : "";
 void EquipTool(WorkbenchTool tool){
  if(state.stage<BuildStage.Switches||state.stage>BuildStage.Test)return;
  ReleaseTool();selectedTool=tool;closeView=true;inspection=false;
  toolCursor=Instantiate(tool.heldVisualPrefab?tool.heldVisualPrefab:tool.gameObject);if(tool.heldVisualPrefab){toolCursor.transform.position=tool.transform.position;toolCursor.transform.rotation=Quaternion.identity;}toolCursor.name="Tool in hand";
  foreach(var c in toolCursor.GetComponentsInChildren<Collider>())c.enabled=false;
  if(tool.heldVisualPrefab)foreach(var r in toolCursor.GetComponentsInChildren<Renderer>())r.sharedMaterial=SoftShapes.Mat(r.name.Contains("Steel")?"A5A5A0":"A46549");
  tool.gameObject.SetActive(false);ui.Refresh();
 }
 void ReleaseTool(){if(selectedTool)selectedTool.gameObject.SetActive(true);selectedTool=null;if(toolCursor)Destroy(toolCursor);toolCursor=null;if(ui)ui.Refresh();}
 void MoveTool(Ray ray){if(!toolCursor)return;var plane=new Plane(Vector3.up,new Vector3(0,4.38f,0));if(plane.Raycast(ray,out float d))toolCursor.transform.position=Vector3.Lerp(toolCursor.transform.position,ray.GetPoint(d),1-Mathf.Exp(-Time.deltaTime*20));}
 void UseTool(int index){
  if(!selectedTool||busy||index<0||index>=61)return;
  if(selectedTool.kind==WorkbenchTool.Kind.PinPliers){if(state.stage!=BuildStage.Test||state.fault!=index)return;}
  else if(state.stage<BuildStage.Switches||state.stage>BuildStage.Test||(state.switches[index]<0&&state.caps[index]<0))return;
  StartCoroutine(UseToolMotion(index));
 }
 IEnumerator UseToolMotion(int index){
  busy=true;bool pull=selectedTool.kind==WorkbenchTool.Kind.Puller;
  Vector3 start=toolCursor.transform.position,end=board.SlotPosition(index)+Vector3.up*(state.caps[index]>=0?.09f:.03f);
  for(float t=0;t<1;t+=Time.deltaTime/.22f){toolCursor.transform.position=Vector3.Lerp(start,end,Mathf.SmoothStep(0,1,t));yield return null;}
  var piece=board.pieces[index];Vector3 rest=piece?piece.transform.position:Vector3.zero;
  Vector3 scale=toolCursor.transform.localScale;
  for(float t=0;t<1;t+=Time.deltaTime/.16f){toolCursor.transform.localScale=Vector3.Scale(scale,new Vector3(1-.12f*Mathf.Sin(t*Mathf.PI),1,1));yield return null;}
  toolCursor.transform.localScale=scale;
  if(pull){
   sound.Click(state.switchChoice,.23f);
   for(float t=0;t<1;t+=Time.deltaTime/.32f){float rise=Mathf.SmoothStep(0,1,t)*.48f;toolCursor.transform.position=end+Vector3.up*rise;if(piece)piece.transform.position=rest+Vector3.up*rise;yield return null;}
   Vector3 lifted=toolCursor.transform.position;Vector3 bin=new Vector3(state.caps[index]>=0?1.60f:-1.60f,4.45f,-.55f);Vector3 pieceOffset=piece?piece.transform.position-lifted:Vector3.zero;
   for(float t=0;t<1;t+=Time.deltaTime/.38f){var point=Vector3.Lerp(lifted,bin,Mathf.SmoothStep(0,1,t))+Vector3.up*Mathf.Sin(t*Mathf.PI)*.10f;toolCursor.transform.position=point;if(piece)piece.transform.position=point+pieceOffset;yield return null;}
   if(state.caps[index]>=0){state.caps[index]=-1;state.stage=BuildStage.Keycaps;}else{state.switches[index]=-1;state.stage=BuildStage.Switches;}
   state.tested[index]=false;board.RenderSlot(index,state,catalog);Changed();
  }else {Repair(index);}
  busy=false;ui.Refresh();
 }
 float roomYaw,roomPitch;
 public void LookAround(Vector2 delta){if(closeView){benchYaw=Mathf.Clamp(benchYaw+delta.x*.055f,-18,18);benchPitch=Mathf.Clamp(benchPitch-delta.y*.04f,-8,8);return;}roomYaw=Mathf.Clamp(roomYaw+delta.x*.055f,-20,20);roomPitch=Mathf.Clamp(roomPitch-delta.y*.04f,-6,7);}
}
}


