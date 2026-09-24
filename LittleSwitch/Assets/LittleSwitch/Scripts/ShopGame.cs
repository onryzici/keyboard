using System.Collections; using UnityEngine; using UnityEngine.InputSystem; using UnityEngine.EventSystems;
using static LittleSwitch.SoftShapes;
namespace LittleSwitch {
public partial class ShopGame : MonoBehaviour {
 const float PartScale=.38f;
 public bool touring;
 public bool CanLeaveWorkbench=>state!=null&&!busy&&!held&&!selectedTool&&!movingProp&&!ui.TerminalOpen;
 public ShopCatalog catalog;public Camera viewCamera;public BuildState state;public KeyboardView board;public ShopUI ui;public WorkshopSound sound;
 public bool closeView,inspection; public string hint="";GameObject supply,held,parcel,upgrade;Renderer highlight;int hover=-1;bool busy;float orbit;Vector3 shopPos=new Vector3(1.8f,8.0f,-10.6f),shopTarget=new Vector3(0,5.2f,1.0f);
 void Start(){state=ShopSave.Load();sound=gameObject.AddComponent<WorkshopSound>();var b=new GameObject("Your handmade keyboard");b.transform.position=new Vector3(0,4.135f,-.55f);b.transform.localScale=Vector3.one*PartScale;board=b.AddComponent<KeyboardView>();board.Initialize();board.Refresh(state,catalog);ui=gameObject.AddComponent<ShopUI>();ui.Build(this);closeView=state.stage>=BuildStage.Switches&&state.stage<=BuildStage.Test;DeskLayout.Apply();RestorePackages();if(NeedsPackage)closeView=false;RefreshSupply();RefreshUpgrade();RefreshParcel();ui.Refresh();}
 void Update(){if(state==null||touring)return;var mouse=Mouse.current;var keyboard=Keyboard.current;
 if(ui.TerminalOpen){if(selectedTool)ReleaseTool();if(keyboard!=null&&keyboard.escapeKey.wasPressedThisFrame)ui.CloseTerminal();return;}
 if(mouse!=null&&DeskInput(mouse,keyboard))return;
 if(!busy&&keyboard!=null&&keyboard.escapeKey.wasPressedThisFrame){if(selectedTool){ReleaseTool();}else if(held){Destroy(held);held=null;}else{closeView=!closeView;inspection=false;}}
 if(!busy&&keyboard!=null&&keyboard.tabKey.wasPressedThisFrame){if(!busy)ReleaseTool();closeView=!closeView;inspection=false;}
 CameraInput(mouse,EventSystem.current&&EventSystem.current.IsPointerOverGameObject());
 Vector3 target=closeView?new Vector3(0,4.15f,-.55f):shopTarget;Vector3 pos=closeView?new Vector3(0,7.6f,-2.1f):shopPos;
 if(inspection){pos=new Vector3(Mathf.Sin(orbit)*2.5f,6.1f,-.55f-Mathf.Cos(orbit)*2.5f);target=new Vector3(0,4.25f,-.55f);}
 if(closeView&&!inspection)pos=target+Quaternion.Euler(benchPitch,benchYaw,0)*(pos-target);
 zoom=Mathf.Lerp(zoom,zoomTarget,1-Mathf.Exp(-Time.deltaTime*6));pos=target+(pos-target)*zoom;
 if(!closeView&&!inspection)target=pos+Quaternion.Euler(roomPitch,roomYaw,0)*(target-pos);
 viewCamera.transform.position=Vector3.Lerp(viewCamera.transform.position,pos,1-Mathf.Exp(-Time.deltaTime*4));viewCamera.transform.rotation=Quaternion.Slerp(viewCamera.transform.rotation,Quaternion.LookRotation(target-viewCamera.transform.position),1-Mathf.Exp(-Time.deltaTime*4));
 viewCamera.fieldOfView=Mathf.Lerp(viewCamera.fieldOfView,closeView?40:55,1-Mathf.Exp(-Time.deltaTime*4));
 if(mouse==null||busy)return;Ray ray=viewCamera.ScreenPointToRay(mouse.position.ReadValue());bool overUI=EventSystem.current&&EventSystem.current.IsPointerOverGameObject();
 if(held){UpdateHeldPart(ray,overUI,mouse.leftButton.wasReleasedThisFrame);return;}
 if(selectedTool)MoveTool(ray);
 if(overUI)return;
 if(Physics.Raycast(ray,out var hit,100)){
 var package=hit.collider.GetComponentInParent<PartsPackage>();if(package&&!package.IsOpen){if(mouse.leftButton.wasPressedThisFrame)StartCoroutine(OpenPackage(package));return;}
 var drink=hit.collider.GetComponentInParent<HotDrink>();if(drink){if(mouse.leftButton.wasPressedThisFrame)StartCoroutine(SipDrink(drink));return;}
 var tool=hit.collider.GetComponentInParent<WorkbenchTool>();
 if(tool){if(mouse.leftButton.wasPressedThisFrame)EquipTool(tool);return;}
 if(selectedTool){var toolSlot=hit.collider.GetComponentInParent<KeySlot>();if(mouse.leftButton.wasPressedThisFrame&&toolSlot)UseTool(toolSlot.index);return;}
 var drawer=hit.collider.GetComponentInParent<WorkshopDrawer>();
 if(drawer){if(mouse.leftButton.wasPressedThisFrame)drawer.Toggle();return;}
 if(hit.collider.GetComponentInParent<OrderTerminal>()){if(mouse.leftButton.wasPressedThisFrame)ui.OpenTerminal();return;}
 var slot=hit.collider.GetComponentInParent<KeySlot>();var tray=hit.collider.GetComponentInParent<PartsSupply>();
 if(mouse.leftButton.wasPressedThisFrame&&tray&&(state.stage==BuildStage.Switches||state.stage==BuildStage.Keycaps))PickUp();
 else if(mouse.leftButton.wasPressedThisFrame&&slot&&state.stage==BuildStage.Test)TestKey(slot.index);
 else if(closeView&&mouse.rightButton.wasReleasedThisFrame&&rightTravel<5&&slot&&(state.stage==BuildStage.Test||state.stage==BuildStage.Keycaps||state.stage==BuildStage.Switches))QuickToolAction(slot.index);
 }
 if(state.stage==BuildStage.Test&&keyboard!=null){// Map physical keyboard keys to their visible counterparts.
 foreach(var control in keyboard.allKeys)if(control.wasPressedThisFrame)PhysicalTestKey(control.keyCode,control.displayName);}
 }
 bool ValidSlot(int i)=>state.stage==BuildStage.Switches?state.switches[i]<0:state.stage==BuildStage.Keycaps&&state.switches[i]>=0&&state.caps[i]<0;
 void Highlight(int i){if(hover>=0)board.sockets[hover].sharedMaterial=Mat("8B9A83");hover=i;if(i>=0)board.sockets[i].sharedMaterial=Mat("E7C680");}
 void PickUp(){ReleaseTool();held=new GameObject("Part in your hand");held.transform.localScale=Vector3.one*PartScale;held.transform.position=new Vector3(state.stage==BuildStage.Keycaps?1.60f:-1.60f,4.4f,-.55f);previewSlot=-2;if(state.stage==BuildStage.Switches)KeyboardView.Switch(held.transform,catalog.switches[state.switchChoice].primary);else Box("Held cap",held.transform,Vector3.zero,new Vector3(.32f,.22f,.32f),ColorUtility.ToHtmlStringRGB(catalog.keycaps[state.capChoice].primary),.05f);sound.Click(state.switchChoice,.17f);hint="Yuvanın üzerine taşı, hizala ve bırak.";ui.Refresh();}
 IEnumerator Install(int i){busy=true;var item=held;held=null;Vector3 start=item.transform.position,end=board.SlotPosition(i);Vector3 aligned=end+Vector3.up*.12f;for(float t=0;t<1;t+=Time.deltaTime*6){item.transform.position=Vector3.Lerp(start,aligned,Mathf.SmoothStep(0,1,t));yield return null;}for(float t=0;t<1;t+=Time.deltaTime*9){item.transform.position=Vector3.Lerp(aligned,end,t*t*t);yield return null;}sound.Click(state.switchChoice,.5f);Destroy(item);if(state.stage==BuildStage.Switches){state.switches[i]=state.switchChoice;if(state.InstalledSwitches==61){state.stage=BuildStage.Keycaps;if(!state.capPackageOpened)closeView=false;hint="Switch'ler tamam. Şimdi renkleri yerleştirelim.";}}else{state.caps[i]=state.capChoice;if(state.InstalledCaps==61){state.stage=BuildStage.Test;state.fault=17;hint="Her tuşa dokun. Bir pin iyi oturmamış olabilir.";}}
 board.RenderSlot(i,state,catalog);if(board.pieces[i])yield return Settle(board.pieces[i].transform);busy=false;Changed();}
 IEnumerator Settle(Transform tr){Vector3 rest=tr.localPosition;for(float t=0;t<1;t+=Time.deltaTime*7){if(!tr)yield break;tr.localPosition=rest+Vector3.up*(Mathf.Sin(t*Mathf.PI*2)*.025f*(1-t));yield return null;}if(tr)tr.localPosition=rest;}
 public void TestKey(int i){if(state.stage!=BuildStage.Test||busy)return;sound.Click(state.switchChoice,catalog.switches[state.switchChoice].volume);if(i==state.fault){hint=""+board.keys[i].label+" yanıt vermedi. Sağ tıkla: çıkar, pini düzelt ve yeniden oturt.";board.sockets[i].sharedMaterial=Mat("CA795B");}else{state.tested[i]=true;board.RenderSlot(i,state,catalog);if(board.pieces[i])StartCoroutine(Settle(board.pieces[i].transform));if(state.Tested==61){state.stage=BuildStage.Package;hint="61 / 61. Ellerine sağlık. Paketlemeye hazır.";}}Changed();}
 void Repair(int i){if(state.stage==BuildStage.Test){if(i!=state.fault){hint="Bu tuş sağlıklı; değiştirmeye gerek yok.";ui.Refresh();return;}state.fault=-1;state.tested[i]=false;hint="Pin düzeltildi. Bu tuşa yeniden basıp kontrol et.";sound.Click(1,.3f);if(board.pieces[i])StartCoroutine(Settle(board.pieces[i].transform));Changed();return;}
 if(state.caps[i]>=0){state.caps[i]=-1;state.tested[i]=false;}else if(state.switches[i]>=0){state.switches[i]=-1;state.stage=BuildStage.Switches;}board.RenderSlot(i,state,catalog);Changed();}
 public void Action(){if(busy)return;switch(state.stage){case BuildStage.Order:state.stage=BuildStage.Parts;hint="İsteğe uygun his ve renkleri seç.";break;
 case BuildStage.Parts:if(!state.Compatible(catalog)){hint="Parçaların yerleşimleri uyuşmuyor.";break;}if(state.Cost(catalog)>catalog.orders[state.orderIndex].budget||state.Cost(catalog)>state.money){hint="Bu kombinasyon bütçeyi aşıyor. Daha uygun parçaları seçebilirsin.";break;}state.money-=state.Cost(catalog);state.stage=BuildStage.Switches;closeView=false;hint="Sol tepsiden bir switch alıp boş bir yuvaya sürükle.";break;
 case BuildStage.Package:StartCoroutine(Pack());return;
 case BuildStage.Ready:state.Deliver(catalog);closeView=false;inspection=false;hint=state.review;break;
 case BuildStage.Review:state.orderIndex=(state.orderIndex+1)%catalog.orders.Length;state.ResetBoard();RestorePackages();state.stage=BuildStage.Order;board.Refresh(state,catalog);hint="Yeni bir gün, yeni bir hikâye.";break;}
 Changed();}
 IEnumerator Pack(){if(busy)yield break;busy=true;closeView=true;inspection=false;state.packingStep++;RefreshParcel();if(parcel){var p=parcel.transform;Vector3 end=p.localScale;p.localScale=end*.75f;for(float t=0;t<1;t+=Time.deltaTime*2.5f){p.localScale=Vector3.Lerp(end*.75f,end,Mathf.SmoothStep(0,1,t));yield return null;}p.localScale=end;}sound.Click(0,.2f);if(state.packingStep>=3){state.stage=BuildStage.Ready;hint="Katlandı, bantlandı, etiketlendi. Güle güle kullanılsın.";}else hint=state.packingStep==1?"Kutu katlandı. Şimdi kâğıt bant.":"Bant tamam. Son dokunuş: müşteri etiketi.";busy=false;Changed();}
 public void ChooseSwitch(int i){if(state.stage!=BuildStage.Parts)return;state.switchChoice=i;sound.Click(i,catalog.switches[i].volume);Changed();}
 public void ChooseCaps(int i){if(state.stage!=BuildStage.Parts)return;state.capChoice=i;Changed();}
 public void Upgrade(){if(state.Upgrade()){hint="Yeni raf, yeni bir bitki. Burası biraz daha senin oldu.";RefreshUpgrade();Changed();}else{hint=state.upgraded?"Yeni rafın yerinde.":"Raf için 90 jeton biriktir.";ui.Refresh();}}
 public void View(){if(busy)return;ReleaseTool();closeView=!closeView;inspection=false;ui.Refresh();}
 public void Inspect(){if(busy)return;ReleaseTool();inspection=!inspection;closeView=true;ui.Refresh();}
 void Changed(){ShopSave.Write(state);RefreshSupply();RefreshParcel();ui.Refresh();}
 void RefreshSupply(){
  if(supply)Destroy(supply);supply=new GameObject("Component carton contents");
  for(int kind=0;kind<2;kind++){
   bool caps=kind==1;if(caps?!state.capPackageOpened:!state.switchPackageOpened)continue;var group=new GameObject(caps?"Keycap carton pickup":"Switch carton pickup");group.transform.SetParent(supply.transform,false);group.transform.position=new Vector3(caps?-2.80f:-3.85f,4.09f,-1.55f);foreach(var package in FindObjectsByType<PartsPackage>(FindObjectsSortMode.None))if(package.keycaps==caps){var anchor=group.AddComponent<PartsSupplyAnchor>();anchor.target=package.transform;anchor.movable=package.GetComponent<MovableDeskProp>();group.transform.position=package.transform.position+Vector3.up*.055f;break;}
   bool active=caps?state.stage==BuildStage.Keycaps:state.stage==BuildStage.Switches;
   {var collider=group.AddComponent<BoxCollider>();collider.center=new Vector3(0,.05f,0);collider.size=new Vector3(.70f,.30f,.62f);if(active)group.AddComponent<PartsSupply>();}
   for(int i=0;i<12;i++){
    var part=new GameObject(caps?"Loose keycap":"Loose switch").transform;part.SetParent(group.transform,false);part.localScale=Vector3.one*PartScale;
    float x=(i%4-1.5f)*.126f+Mathf.Sin(i*4.2f)*.014f,z=(i/4-1)*.125f+Mathf.Cos(i*3.1f)*.012f;
    part.localPosition=new Vector3(x,.012f+(i%3)*.012f,z);part.localRotation=Quaternion.Euler((i%3-1)*8,(i*47)%35-17,(i%4-1.5f)*7);
    if(caps){Box("Cap",part,new Vector3(0,.1f,0),new Vector3(.32f,.20f,.32f),ColorUtility.ToHtmlStringRGB(i%4==0?catalog.keycaps[state.capChoice].accent:catalog.keycaps[state.capChoice].primary),.05f);Label(new[]{"Q","W","E","R","A","S","D","F","Z","X","C","V"}[i],part,new Vector3(0,.208f,0),.105f,"39483F",new Vector3(90,0,0));}
    else KeyboardView.Switch(part,catalog.switches[state.switchChoice].primary);
   }
  }
 }

 void RefreshUpgrade(){if(upgrade||!state.upgraded)return;var prefab=Resources.Load<GameObject>("WorkshopUpgrade");if(prefab)upgrade=Instantiate(prefab);}
 void RefreshParcel(){bool show=state.packingStep>0&&(state.stage==BuildStage.Package||state.stage==BuildStage.Ready||state.stage==BuildStage.Review);if(parcel)Destroy(parcel);board.gameObject.SetActive(!show);if(!show)return;parcel=new GameObject("A carefully packed order");parcel.transform.localScale=Vector3.one*PartScale;parcel.transform.position=new Vector3(0,4.2f,-.55f);Box("Folded cardboard",parcel.transform,Vector3.zero,new Vector3(6,.6f,2.35f),"B78E5F",.055f);if(state.packingStep>=2)Box("Kraft tape",parcel.transform,new Vector3(0,.309f,0),new Vector3(.45f,.016f,2.3f),"D6BD87",.008f);if(state.packingStep>=3){Box("Handwritten label",parcel.transform,new Vector3(1.4f,.322f,0),new Vector3(1.7f,.02f,1.1f),"EEE2BC",.02f);Label("made for\n"+catalog.orders[state.orderIndex].name+"\nwith care.",parcel.transform,new Vector3(1.4f,.34f,0),.15f,"5B6955",new Vector3(90,0,0));}}
 void OnApplicationQuit(){if(state!=null)ShopSave.Write(state);}
 }
}





