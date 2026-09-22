using UnityEngine;
using static LittleSwitch.SoftShapes;
namespace LittleSwitch {
public static class WorkshopArt {
 public static Transform Build(){
 var root=new GameObject("Little Switch · Workshop").transform;
 Box("Floor",root,new Vector3(0,-.18f,1),new Vector3(15,.3f,12),"665345");
 for(int i=0;i<20;i++) Box("Oak floorboard",root,new Vector3(-7+i*.72f,0,1),new Vector3(.69f,.08f,12),i%3==0?"806650":"8E7257",.02f);
 Box("Back plaster",root,new Vector3(0,3.4f,4.7f),new Vector3(14,6.8f,.25f),"B9AD91");
 Box("Left plaster",root,new Vector3(-6.5f,3.4f,.5f),new Vector3(.25f,6.8f,8.5f),"899685");
 for(int i=0;i<15;i++)Box("Wainscot",root,new Vector3(-6.3f+i*.9f,1.2f,4.5f),new Vector3(.86f,2.4f,.12f),"657D71",.025f);
 Box("Dado rail",root,new Vector3(0,2.42f,4.37f),new Vector3(13,.1f,.18f),"D2C4A5");
 // A dusk window, with an original miniature hillside beyond the glass.
 Box("Window frame",root,new Vector3(-1.65f,4.25f,4.45f),new Vector3(5.7f,3.4f,.22f),"775D49");
 var sky=Box("Evening sky",root,new Vector3(-1.65f,4.25f,4.29f),new Vector3(5.4f,3.12f,.08f),"C7AC9D");
 var sm=new Material(Mat("C7AC9D"));sm.EnableKeyword("_EMISSION");sm.SetColor("_EmissionColor",C("BA9485")*.6f);sky.GetComponent<Renderer>().sharedMaterial=sm;
 for(int i=0;i<7;i++){float x=-4+i*.73f;Box("Distant townhouse",root,new Vector3(x,3.35f+(i%3)*.14f,4.19f),new Vector3(.65f,.9f+(i%3)*.28f,.08f),i%2==0?"96968E":"9D8A80");for(int w=0;w<2;w++)Box("Evening window",root,new Vector3(x-.15f+w*.3f,3.55f,4.12f),new Vector3(.12f,.19f,.02f),"ECD4A0",.01f);}
 for(int i=0;i<3;i++) Box("Window mullion",root,new Vector3(-4.35f+i*2.7f,4.25f,4.07f),new Vector3(.09f,3.25f,.2f),"E0D1AF",.015f);
 Box("Window crossbar",root,new Vector3(-1.65f,4.65f,4.04f),new Vector3(5.45f,.08f,.2f),"E0D1AF",.015f);
 Box("Deep windowsill",root,new Vector3(-1.65f,2.65f,3.98f),new Vector3(5.9f,.16f,.7f),"CFBA91");
 // Main workbench; large clear central mat.
 Box("Walnut worktop",root,new Vector3(0,1.65f,0),new Vector3(11.5f,.3f,5.5f),"916745",.12f);
 for(int i=0;i<6;i++)Box("Wood inlay",root,new Vector3(0,1.803f,-2.3f+i*.86f),new Vector3(11.2f,.008f,.014f),"79573D",.002f);
 for(int x=-1;x<=1;x+=2)Box("Workbench leg",root,new Vector3(x*4.8f,.78f,0),new Vector3(.45f,1.6f,4.3f),"455F59");
 Box("Cutting mat edge",root,new Vector3(0,1.83f,-.55f),new Vector3(6.75f,.05f,3.55f),"6C705B",.07f);
 Box("Sage cutting mat",root,new Vector3(0,1.87f,-.55f),new Vector3(6.62f,.025f,3.42f),"8B9277",.04f);
 // Shelf clusters: component archive and finished boards.
 for(int shelf=0;shelf<3;shelf++){
 float y=2.5f+shelf*1.25f;Box("Display shelf",root,new Vector3(4.25f,y,4),new Vector3(3.35f,.13f,1.15f),"8C6547");
 for(int j=0;j<3;j++){if(shelf==2&&j==2)continue;Box("Parts archive",root,new Vector3(3.2f+j*1.0f,y+.39f,4),new Vector3(.82f,.65f,.76f),shelf==1?"BD9A70":"70897B");Box("Paper label",root,new Vector3(3.2f+j*1.0f,y+.41f,3.607f),new Vector3(.5f,.2f,.013f),"E6D9BA",.018f);Label(shelf==0?new[]{"LINEAR","TACTILE","CLICKY"}[j]:new[]{"CAPS","CABLES","EXTRAS"}[j],root,new Vector3(3.2f+j, y+.41f,3.586f),.08f,"5C6558",Vector3.zero);}
 }
 // Personal cluster: plant, postcards, shop sign.
 Box("Studio sign",root,new Vector3(2.85f,6.05f,4.45f),new Vector3(5.3f,.85f,.15f),"47665E");
 Label("little switch",root,new Vector3(2.85f,6.09f,4.34f),.48f,"F3E2BC",Vector3.zero);
 Label("CUSTOM KEYBOARDS  ·  SLOWLY, WITH CARE",root,new Vector3(2.85f,5.79f,4.32f),.1f,"C9D0B3",Vector3.zero);
 Plant(root,new Vector3(-3.85f,2.75f,3.8f),.9f);Plant(root,new Vector3(5.15f,5.06f,4),.7f);
 Box("Pinboard",root,new Vector3(-5.28f,4.07f,4.28f),new Vector3(1.35f,2.5f,.13f),"A88761");
 for(int i=0;i<4;i++){var p=Box("Thank you note",root,new Vector3(-5.28f+(i%2)*.22f-.12f,3.3f+i*.47f,4.19f),new Vector3(.8f,.42f,.02f),i%2==0?"E5CAA0":"E0DFBE",.01f);p.transform.localEulerAngles=new Vector3(0,0,i%2==0?-7:5);}
 // Retro terminal on the left.
 Box("Terminal body",root,new Vector3(-4.2f,2.58f,1.35f),new Vector3(1.8f,1.4f,1.3f),"D2C8AE",.16f);
 Box("Screen bezel",root,new Vector3(-4.2f,2.65f,.66f),new Vector3(1.49f,1.06f,.12f),"535F51",.12f);
 Box("Phosphor glass",root,new Vector3(-4.2f,2.65f,.585f),new Vector3(1.28f,.85f,.04f),"203B37",.08f);
 Label("little switch\n\n5 lovely commissions\nno rush. just craft.",root,new Vector3(-4.2f,2.65f,.55f),.105f,"BFD4A2",Vector3.zero);
 Box("Terminal foot",root,new Vector3(-4.2f,1.97f,1.35f),new Vector3(1.5f,.3f,1.25f),"BDB296");
 // Switch tray near front left, ready for dragging.
 Box("Parts tray",root,new Vector3(-4.05f,1.91f,-1.2f),new Vector3(1.6f,.16f,1.65f),"D0B896");
 Box("Tray inner",root,new Vector3(-4.05f,2,-1.2f),new Vector3(1.39f,.06f,1.43f),"6E7E6A");
 // Tools grouped at the back edge.
 Box("Tool rail",root,new Vector3(.35f,2.09f,1.97f),new Vector3(3.9f,.35f,.3f),"46625C");
 for(int i=0;i<5;i++){float x=-1.05f+i*.64f;Rod(root,new Vector3(x,2.14f,1.95f),new Vector3(x+.07f,2.81f,1.99f),.055f,"B9B9A5");Box("Screwdriver grip",root,new Vector3(x+.06f,2.69f,1.99f),new Vector3(.16f,.37f,.16f),i%2==0?"CB8B59":"D2BE7F");}
 // Small mug and tea surface.
 Cyl("Ceramic mug",root,new Vector3(3.52f,2.12f,1.15f),new Vector3(.49f,.28f,.49f),"D8B679");
 Cyl("Tea",root,new Vector3(3.52f,2.405f,1.15f),new Vector3(.4f,.008f,.4f),"584735");
 var handle=Sphere("Mug handle",root,new Vector3(3.81f,2.15f,1.15f),new Vector3(.24f,.33f,.15f),"D8B679");
 Box("Notebook",root,new Vector3(4.35f,1.9f,-.2f),new Vector3(1.22f,.11f,1.55f),"C9AC80");
 Box("Notebook paper",root,new Vector3(4.35f,1.97f,-.2f),new Vector3(1.1f,.03f,1.44f),"EEE0BC");
 Label("one key\nat a time.",root,new Vector3(4.35f,1.994f,-.2f),.18f,"796548",new Vector3(90,0,0));
 Rod(root,new Vector3(4.85f,2.01f,-.55f),new Vector3(4.65f,2.01f,.43f),.045f,"476B64");
 // Desk lamp / soft warm pool.
 Cyl("Lamp base",root,new Vector3(2.9f,1.88f,1.65f),new Vector3(.65f,.07f,.65f),"3E5B56");
 Rod(root,new Vector3(2.9f,1.9f,1.65f),new Vector3(3.05f,3.27f,1.8f),.08f,"455D54");
 Rod(root,new Vector3(3.05f,3.27f,1.8f),new Vector3(2.3f,3.7f,1.15f),.07f,"455D54");
 Sphere("Lamp shade",root,new Vector3(2.3f,3.62f,1.15f),new Vector3(.85f,.42f,.8f),"5E7966");
 Cyl("Warm diffuser",root,new Vector3(2.3f,3.43f,1.15f),new Vector3(.65f,.025f,.65f),"FFE4A2");
 Light(root,"Bench lamp",new Vector3(2.3f,3.3f,1.15f),C("FFE0A3"),35,6);
 // Packaging nook below the shelves.
 for(int i=0;i<3;i++){Box("Shipping carton",root,new Vector3(4.5f,1.15f+i*.37f,2.75f),new Vector3(1.65f,.35f,1.1f),"B48F61");Box("Paper tape",root,new Vector3(4.5f,1.332f+i*.37f,2.75f),new Vector3(.23f,.015f,1.05f),"D5B784",.005f);}
 // Ambient and large directional dusk light.
 RenderSettings.ambientMode=UnityEngine.Rendering.AmbientMode.Trilight;RenderSettings.ambientSkyColor=C("D8C4A6")*.7f;RenderSettings.ambientEquatorColor=C("A1AEA0")*.55f;RenderSettings.ambientGroundColor=C("796653")*.5f;
 var sun=new GameObject("Late afternoon").AddComponent<Light>();sun.transform.SetParent(root);sun.type=LightType.Directional;sun.transform.rotation=Quaternion.Euler(42,-32,0);sun.color=C("FFE0B6");sun.intensity=1.0f;sun.shadows=LightShadows.Soft;
 Light(root,"Window bounce",new Vector3(-2,4,2.7f),C("FFD5AB"),26,8);
 Light(root,"Soft room fill",new Vector3(-3,4,-3),C("D5E1CF"),18,10);
 return root;
 }
 static void Light(Transform p,string name,Vector3 pos,Color c,float intensity,float range){var l=new GameObject(name).AddComponent<Light>();l.transform.SetParent(p);l.transform.localPosition=pos;l.type=LightType.Point;l.color=c;l.intensity=intensity*.12f;l.range=range;l.shadows=LightShadows.None;}
 public static void Plant(Transform p,Vector3 pos,float scale){var root=new GameObject("A little green").transform;root.SetParent(p,false);root.localPosition=pos;root.localScale=Vector3.one*scale;Cyl("Terracotta pot",root,new Vector3(0,.22f,0),new Vector3(.6f,.23f,.6f),"B67F61");Cyl("Soil",root,new Vector3(0,.455f,0),new Vector3(.5f,.01f,.5f),"675641");for(int i=0;i<9;i++){float a=i*2.4f;Vector3 end=new Vector3(Mathf.Cos(a)*.42f,.85f+(i%3)*.18f,Mathf.Sin(a)*.35f);Rod(root,new Vector3(0,.44f,0),end,.035f,"677C4E");var leaf=Sphere("Leaf",root,end,new Vector3(.28f,.45f,.12f),i%2==0?"82945E":"5A7F58");leaf.transform.localEulerAngles=new Vector3(20,i*41,35);}}
}
}
