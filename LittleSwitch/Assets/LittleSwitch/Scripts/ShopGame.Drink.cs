using System.Collections;
namespace LittleSwitch {
 public partial class ShopGame {
  IEnumerator SipDrink(HotDrink drink){
   if(busy||drink.IsSipping)yield break;ReleaseTool();busy=true;
   try{yield return drink.Sip(viewCamera);}finally{busy=false;ui.Refresh();}
  }
 }
}
