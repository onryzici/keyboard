using UnityEngine;using TMPro;using UnityEngine.EventSystems;using UnityEngine.InputSystem.UI;
using static LittleSwitch.SoftShapes;
namespace LittleSwitch {
public class ShopUI : MonoBehaviour {
 ShopGame game;Transform root,card,choices,context,menu,shade;TextMeshProUGUI wallet,order,stage,hint,mailHeader;UnityEngine.UI.Button action;TextMeshProUGUI actionText;UnityEngine.UI.Button[] sw=new UnityEngine.UI.Button[3],caps=new UnityEngine.UI.Button[3];Sprite rounded;
 public bool TerminalOpen {get;private set;}
 public void Build(ShopGame g){
 game=g;rounded=Resources.Load<Sprite>("UI/rounded-panel");var canvas=new GameObject("Quiet interface",typeof(Canvas),typeof(UnityEngine.UI.CanvasScaler),typeof(UnityEngine.UI.GraphicRaycaster));root=canvas.transform;canvas.GetComponent<Canvas>().renderMode=RenderMode.ScreenSpaceOverlay;
 var scaler=canvas.GetComponent<UnityEngine.UI.CanvasScaler>();scaler.uiScaleMode=UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(1600,1000);scaler.matchWidthOrHeight=.5f;
 if(!FindAnyObjectByType<EventSystem>())new GameObject("Input",typeof(EventSystem),typeof(InputSystemUIInputModule));
 context=Panel("Work prompt",root,new Vector2(0,28),new Vector2(570,72),new Vector2(.5f,0),"233935",.88f);
 stage=Text("",context,new Vector2(22,-12),new Vector2(526,24),17,"F3E7CC");stage.fontStyle=FontStyles.Bold;
 hint=Text("",context,new Vector2(22,-39),new Vector2(526,23),12,"D3D7C7");
 var view=Button("Tab   Görünüm",root,new Vector2(26,28),new Vector2(138,33),()=>game.View(),new Vector2(0,0));Style(view,"243C35","DBDDCB");
 var more=Button("Menü",root,new Vector2(-26,-24),new Vector2(70,33),()=>{menu.gameObject.SetActive(!menu.gameObject.activeSelf);Refresh();},new Vector2(1,1));Style(more,"243C35","DBDDCB");
 menu=Panel("Workshop options",root,new Vector2(-26,-67),new Vector2(222,207),new Vector2(1,1),"233935",.98f);
 wallet=Text("",menu,new Vector2(16,-14),new Vector2(190,26),14,"E8D8B6");
 Button("Rafı geliştir · 90",menu,new Vector2(12,-52),new Vector2(198,38),()=>{game.Upgrade();menu.gameObject.SetActive(false);});
 Button("Ses açık / kapalı",menu,new Vector2(12,-100),new Vector2(198,38),()=>game.sound.Toggle());
 Button("Klavyeyi incele",menu,new Vector2(12,-148),new Vector2(198,38),()=>{game.Inspect();menu.gameObject.SetActive(false);});menu.gameObject.SetActive(false);
 shade=Panel("Terminal backdrop",root,Vector2.zero,Vector2.zero,new Vector2(.5f,.5f),"0E211C",.62f);var sr=(RectTransform)shade;sr.anchorMin=Vector2.zero;sr.anchorMax=Vector2.one;sr.offsetMin=sr.offsetMax=Vector2.zero;shade.GetComponent<UnityEngine.UI.Image>().raycastTarget=true;
 card=Panel("Computer inbox",shade,Vector2.zero,new Vector2(690,590),new Vector2(.5f,.5f),"EDE7D5");
 var titlebar=Panel("Inbox header",card,Vector2.zero,new Vector2(690,66),new Vector2(0,1),"28463E");
 mailHeader=Text("POSTA KUTUSU",titlebar,new Vector2(26,-22),new Vector2(540,25),17,"E7EBD8");mailHeader.fontStyle=FontStyles.Bold;
 var close=Button("×",titlebar,new Vector2(-16,-14),new Vector2(36,36),CloseTerminal,new Vector2(1,1));Style(close,"3B5A50","EBEAD7");
 order=Text("",card,new Vector2(28,-91),new Vector2(634,170),18,"31483E");
 choices=Panel("Parts selection",card,new Vector2(28,-277),new Vector2(634,203),new Vector2(0,1),"E4DDC8");
 Text("SWITCH   /   Dinlemek için seç",choices,new Vector2(16,-13),new Vector2(600,24),12,"647263");
 for(int i=0;i<3;i++){int n=i;sw[i]=Button(new[]{"Linear · 18","Tactile · 24","Clicky · 20"}[i],choices,new Vector2(16+i*202,-44),new Vector2(190,43),()=>game.ChooseSwitch(n));}
 Text("TUŞ KAPAKLARI",choices,new Vector2(16,-107),new Vector2(600,24),12,"647263");
 for(int i=0;i<3;i++){int n=i;caps[i]=Button(new[]{"Yosun · 26","Kil · 28","Sis · 30"}[i],choices,new Vector2(16+i*202,-137),new Vector2(190,43),()=>game.ChooseCaps(n));}
 action=Button("",root,new Vector2(-26,28),new Vector2(212,44),()=>{game.Action();if(game.state.stage==BuildStage.Switches||game.state.stage==BuildStage.Order)CloseTerminal();Refresh();},new Vector2(1,0));actionText=action.GetComponentInChildren<TextMeshProUGUI>();Style(action,"D3B278","263F36");
 shade.gameObject.SetActive(false);
 }
 Transform Panel(string name,Transform p,Vector2 pos,Vector2 size,Vector2 anchor,string col,float alpha=1){var g=new GameObject(name,typeof(RectTransform),typeof(UnityEngine.UI.Image));g.transform.SetParent(p,false);var r=g.GetComponent<RectTransform>();r.anchorMin=r.anchorMax=anchor;r.pivot=anchor;r.anchoredPosition=pos;r.sizeDelta=size;var image=g.GetComponent<UnityEngine.UI.Image>();image.sprite=rounded;image.type=UnityEngine.UI.Image.Type.Sliced;var c=C(col);c.a=alpha;image.color=c;image.raycastTarget=false;return g.transform;}
 TextMeshProUGUI Text(string s,Transform p,Vector2 pos,Vector2 size,float font,string col){var g=new GameObject("Text",typeof(RectTransform));g.transform.SetParent(p,false);var t=g.AddComponent<TextMeshProUGUI>();var r=t.rectTransform;r.anchorMin=r.anchorMax=new Vector2(0,1);r.pivot=new Vector2(0,1);r.anchoredPosition=pos;r.sizeDelta=size;t.text=s;t.fontSize=font;t.color=C(col);t.textWrappingMode=TextWrappingModes.Normal;t.raycastTarget=false;return t;}
 UnityEngine.UI.Button Button(string label,Transform p,Vector2 pos,Vector2 size,UnityEngine.Events.UnityAction callback,Vector2? anchor=null){var tr=Panel(label,p,pos,size,anchor??new Vector2(0,1),"D3D9C5");tr.GetComponent<UnityEngine.UI.Image>().raycastTarget=true;var b=tr.gameObject.AddComponent<UnityEngine.UI.Button>();b.targetGraphic=tr.GetComponent<UnityEngine.UI.Image>();b.onClick.AddListener(callback);var colors=b.colors;colors.highlightedColor=new Color(1.12f,1.12f,1.12f);colors.pressedColor=new Color(.82f,.88f,.81f);b.colors=colors;var t=Text(label,tr,new Vector2(9,-4),size-new Vector2(18,8),14,"314C40");t.alignment=TextAlignmentOptions.Center;return b;}
 void Style(UnityEngine.UI.Button b,string background,string ink){b.GetComponent<UnityEngine.UI.Image>().color=C(background);b.GetComponentInChildren<TextMeshProUGUI>().color=C(ink);}
 public void OpenTerminal(){TerminalOpen=true;menu.gameObject.SetActive(false);Refresh();}
 public void CloseTerminal(){TerminalOpen=false;Refresh();}
 public void Refresh(){if(!game||game.state==null)return;var s=game.state;var c=game.catalog;var o=c.orders[s.orderIndex];wallet.text=s.money+" jeton  ·  "+s.reputation+" itibar";
 shade.gameObject.SetActive(TerminalOpen);choices.gameObject.SetActive(TerminalOpen&&s.stage==BuildStage.Parts);
 mailHeader.text="POSTA KUTUSU     /     SİPARİŞ "+(s.orderIndex+1).ToString("00");
 order.text="<size=27><b>"+o.name+"</b></size>\n\n"+o.request+"\n\n<size=14>Bütçe: "+o.budget+"     •     Bakiye: "+s.money+(s.stage==BuildStage.Parts?"     •     Parçalar: "+s.Cost(c):"")+"</size>";
 ((RectTransform)card).sizeDelta=new Vector2(690,s.stage==BuildStage.Parts?590:(s.stage==BuildStage.Order||s.stage==BuildStage.Review?370:300));
 string title="",help="";switch(s.stage){
 case BuildStage.Order:help="Yeni sipariş için bilgisayara tıkla.";break;
 case BuildStage.Parts:help="Parçaları bilgisayardan seç.";break;
 case BuildStage.Switches:title="Switch yerleştir    "+s.InstalledSwitches+" / 61";help="Switch kutusundan al, boş yuvaya bırak.  •  Sağ tık: çıkar";break;
 case BuildStage.Keycaps:title="Tuş kapakları    "+s.InstalledCaps+" / 61";help="Tuş kutusundan al, switch üzerine bırak.";break;
 case BuildStage.Test:title="Ses ve tuş testi    "+s.Tested+" / 61";help="Tuşlara tıkla veya yaz.  •  Sorunlu tuşta sağ tık";if(!string.IsNullOrEmpty(game.hint))help=game.hint;break;
 case BuildStage.Package:title="Paketleme";help=new[]{"Kutuyu katla.","Kâğıt bandı yapıştır.","Müşteri etiketini ekle."}[Mathf.Clamp(s.packingStep,0,2)];break;
 case BuildStage.Ready:title="Sipariş hazır";help="Teslim edebilirsin.";break;
 case BuildStage.Review:title="Teslim edildi    "+s.Quality(c)+" / 5";help="Yeni sipariş için bilgisayara tıkla.";break;}
 if(game.NeedsPackage){title=s.stage==BuildStage.Switches?"Switch paketi rafta":"Tuş kapakları rafta";help="Raftaki ilgili pakete tıkla; masaya alıp aç.";}
 context.gameObject.SetActive(!TerminalOpen);stage.text=string.IsNullOrEmpty(title)?"Bilgisayar · Siparişler":title;hint.text=game.ToolEquipped?game.ToolHint:help;
 bool terminalAction=TerminalOpen&&(s.stage==BuildStage.Order||s.stage==BuildStage.Parts||s.stage==BuildStage.Review);
 bool benchAction=!TerminalOpen&&(s.stage==BuildStage.Package||s.stage==BuildStage.Ready);
 action.gameObject.SetActive(terminalAction||benchAction);var ar=(RectTransform)action.transform;ar.SetParent(terminalAction?card:root,false);ar.anchorMin=ar.anchorMax=ar.pivot=new Vector2(1,0);ar.anchoredPosition=terminalAction?new Vector2(-28,24):new Vector2(-26,28);ar.sizeDelta=new Vector2(terminalAction?250:212,44);action.transform.SetAsLastSibling();
 actionText.text=s.stage==BuildStage.Order?"Siparişi kabul et":s.stage==BuildStage.Parts?"Parçaları al · "+s.Cost(c):s.stage==BuildStage.Package?new[]{"Kutuyu katla","Bantla","Etiketi ekle"}[Mathf.Clamp(s.packingStep,0,2)]:s.stage==BuildStage.Ready?"Teslim et · +"+o.budget:"Yeni siparişe geç";
 for(int i=0;i<3;i++){Style(sw[i],i==s.switchChoice?"486B58":"CFD5BE",i==s.switchChoice?"F2EEDB":"3D5548");Style(caps[i],i==s.capChoice?"486B58":"CFD5BE",i==s.capChoice?"F2EEDB":"3D5548");}
 }
}
}
