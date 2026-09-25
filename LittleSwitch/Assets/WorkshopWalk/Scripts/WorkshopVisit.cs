using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LittleSwitch.Environment
{
    [DefaultExecutionOrder(-100)]
    public sealed class WorkshopVisit : MonoBehaviour
    {
        public ShopGame shop;
        public WorkshopWalker walker;
        public Camera walkingCamera;
        public Transform orderTerminal;
        public float terminalInteractionRange=6;
        bool ready, returning, terminalVisit;
        public bool IsWalking => ready && shop.touring;

        IEnumerator Start()
        {
            while(shop.state==null || !shop.ui)yield return null;
            shop.touring=true;shop.ui.SetVisible(false);
            shop.viewCamera.enabled=false;
            var audio=shop.viewCamera.GetComponent<AudioListener>();if(audio)audio.enabled=false;
            walkingCamera.enabled=true;walker.enabled=true;
            Cursor.lockState=CursorLockMode.Locked;Cursor.visible=false;ready=true;
        }
        void Update()
        {
            if(!ready||returning)return;
            if(terminalVisit&&!shop.touring&&!shop.ui.TerminalOpen&&shop.CanLeaveWorkbench){terminalVisit=false;LeaveWorkbench();return;}
            if(shop.touring && Cursor.lockState==CursorLockMode.Locked && Mouse.current!=null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                var ray=new Ray(walkingCamera.transform.position,walkingCamera.transform.forward);
                if(Physics.Raycast(ray,out var hit,5f))
                {
                    var drawer=hit.collider.GetComponentInParent<WorkshopDrawer>();
                    if(drawer)drawer.Toggle();
                }
            }
            if(Keyboard.current==null)return;
            if(shop.touring && Keyboard.current.eKey.wasPressedThisFrame && (NearTerminal()||NearBench()))EnterWorkbench(NearTerminal());
            else if(!shop.touring && Keyboard.current.escapeKey.wasPressedThisFrame && shop.CanLeaveWorkbench)LeaveWorkbench();
        }
        bool NearBench()
        {
            Vector3 p=walker.transform.position;
            return p.z> -7.3f && p.z< -3.5f && Mathf.Abs(p.x)<3.8f;
        }
        bool NearTerminal(){if(!orderTerminal)return false;var d=orderTerminal.position-walker.transform.position;d.y=0;return d.magnitude<terminalInteractionRange;}
        public void EnterWorkbench(bool terminal)
        {
            if(!ready||!shop.touring)return;
            terminalVisit=terminal;
            walker.enabled=false;walkingCamera.enabled=false;
            shop.viewCamera.transform.SetPositionAndRotation(walkingCamera.transform.position,walkingCamera.transform.rotation);
            shop.viewCamera.enabled=true;shop.closeView=!terminal&&!shop.NeedsPackage;shop.inspection=false;
            shop.viewCamera.fieldOfView=walkingCamera.fieldOfView;
            shop.ResetBenchView();
            shop.touring=false;shop.ui.SetVisible(true);
            Cursor.lockState=CursorLockMode.None;Cursor.visible=true;
            if(terminal)shop.ui.OpenTerminal();
        }
        public void LeaveWorkbench(){if(!returning && shop.CanLeaveWorkbench)StartCoroutine(Return());}
        IEnumerator Return()
        {
            returning=true;shop.touring=true;shop.ui.SetVisible(false);
            Vector3 start=shop.viewCamera.transform.position;Quaternion rotation=shop.viewCamera.transform.rotation;
            float startFov=shop.viewCamera.fieldOfView;
            for(float elapsed=0;elapsed<.9f;elapsed+=Time.deltaTime)
            {
                float t=Mathf.SmoothStep(0,1,elapsed/.9f);
                shop.viewCamera.transform.SetPositionAndRotation(Vector3.Lerp(start,walkingCamera.transform.position,t),Quaternion.Slerp(rotation,walkingCamera.transform.rotation,t));
                shop.viewCamera.fieldOfView=Mathf.Lerp(startFov,walkingCamera.fieldOfView,t);
                yield return null;
            }
            shop.viewCamera.transform.SetPositionAndRotation(walkingCamera.transform.position,walkingCamera.transform.rotation);
            shop.viewCamera.fieldOfView=walkingCamera.fieldOfView;
            shop.viewCamera.enabled=false;walkingCamera.enabled=true;walker.enabled=true;
            Cursor.lockState=CursorLockMode.Locked;Cursor.visible=false;returning=false;
        }
        void OnGUI()
        {
            if(!ready||returning)return;
            string text=shop.touring?(NearTerminal()?"E  ·  Bilgisayarı kullan":NearBench()?"E  ·  Tezgâhta çalış":""):"Esc  ·  Atölyeye dön";
            if(text.Length==0)return;
            var old=GUI.matrix;float scale=Screen.height/1080f;GUI.matrix=Matrix4x4.Scale(new Vector3(scale,scale,1));
            var style=new GUIStyle(GUI.skin.box){fontSize=18,alignment=TextAnchor.MiddleCenter};
            style.normal.textColor=new Color(.95f,.90f,.78f);
            GUI.Box(new Rect(Screen.width/scale/2-135,1020,270,36),text,style);GUI.matrix=old;
        }
    }
}
