using System.Collections;
using UnityEngine;
namespace LittleSwitch {
 public partial class ShopGame {
  public bool NeedsPackage => state.stage==BuildStage.Switches?!state.switchPackageOpened:state.stage==BuildStage.Keycaps&&!state.capPackageOpened;
  void RestorePackages(){foreach(var p in FindObjectsByType<PartsPackage>(FindObjectsSortMode.None))p.Present(p.keycaps?state.capPackageOpened:state.switchPackageOpened);}
  IEnumerator OpenPackage(PartsPackage p){
   if(busy||p.IsOpen|| (p.keycaps?state.stage!=BuildStage.Keycaps:state.stage!=BuildStage.Switches))yield break;
   ReleaseTool();busy=true;
   try{
    yield return p.BringAndOpen();
    if(p.keycaps)state.capPackageOpened=true;else state.switchPackageOpened=true;
    closeView=true;inspection=false;Changed();
   }finally{busy=false;ui.Refresh();}
  }
 }
}
