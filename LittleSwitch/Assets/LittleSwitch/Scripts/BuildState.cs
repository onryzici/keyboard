using System; using System.IO; using System.Linq; using UnityEngine;
namespace LittleSwitch {
public enum BuildStage { Order, Parts, Switches, Keycaps, Test, Package, Ready, Review }
[Serializable] public class BuildState {
 public int version=1, orderIndex, money=220, reputation, completed; public bool upgraded; public bool switchPackageOpened,capPackageOpened;
 public BuildStage stage; public int switchChoice, capChoice; public int[] switches=new int[61], caps=new int[61];
 public bool[] tested=new bool[61]; public int fault=-1, packingStep; public string review="";
 public BuildState(){ResetBoard();}
 public void ResetBoard(){switchPackageOpened=false;capPackageOpened=false;for(int i=0;i<61;i++){switches[i]=-1; caps[i]=-1; tested[i]=false;} fault=-1; packingStep=0;}
 public int InstalledSwitches=>switches.Count(x=>x>=0); public int InstalledCaps=>caps.Count(x=>x>=0); public int Tested=>tested.Count(x=>x);
 public int Cost(ShopCatalog c)=>c.casePart.price+c.pcb.price+c.switches[switchChoice].price+c.keycaps[capChoice].price;
 public bool Compatible(ShopCatalog c)=>c.casePart.layout==c.pcb.layout && c.pcb.layout==c.keycaps[capChoice].layout;
 public int Quality(ShopCatalog c){var o=c.orders[orderIndex]; return 3+(c.switches[switchChoice].id==o.switchId?1:0)+(c.keycaps[capChoice].id==o.keycapId?1:0);}
 public void Deliver(ShopCatalog c){if(stage!=BuildStage.Ready)throw new InvalidOperationException("Pack and test first"); var o=c.orders[orderIndex]; int stars=Quality(c); money+=o.budget; reputation+=stars; completed++; review=stars==5?"Tam hayal ettiğim gibi. Ellerine sağlık!":"Özenle yapılmış! "+(c.switches[switchChoice].id!=o.switchId?"Bir dahakine istediğim tuş hissine biraz daha yaklaşabiliriz.":"Bir dahakine renkleri isteğime daha yakın seçebiliriz."); stage=BuildStage.Review;}
 public bool Upgrade(){if(upgraded||money<90)return false;money-=90;upgraded=true;return true;}
}
public static class ShopSave {
 public static string PathName=>Path.Combine(Application.persistentDataPath,"little-switch-v1.json");
 public static void Write(BuildState s){string p=PathName;File.WriteAllText(p+".tmp",JsonUtility.ToJson(s,true));File.Copy(p+".tmp",p,true);File.Delete(p+".tmp");}
 public static BuildState Load(){try{if(File.Exists(PathName)){var s=JsonUtility.FromJson<BuildState>(File.ReadAllText(PathName));if(s.version==1&&s.switches.Length==61&&s.caps.Length==61&&s.tested.Length==61&&s.orderIndex>=0&&s.orderIndex<5&&s.switchChoice>=0&&s.switchChoice<3&&s.capChoice>=0&&s.capChoice<3)return s;}}catch(Exception e){Debug.LogWarning("Save could not be loaded: "+e.Message);}return new BuildState();}
}
}
