using UnityEngine;
using UnityEngine.InputSystem;

namespace LittleSwitch.Environment
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class WorkshopWalker : MonoBehaviour
    {
        public Camera playerCamera;
        public float walkingSpeed = 1.4f;
        public float sensitivity = .045f;
        public float worldUnitsPerMetre = 1;
        public float lookSmoothTime = .09f;
        CharacterController body;
        Vector2 velocity;
        float pitch, gravity, helpUntil;
        float targetYaw,targetPitch,yawVelocity,pitchVelocity;
        void OnEnable(){targetYaw=transform.eulerAngles.y;targetPitch=playerCamera?Mathf.DeltaAngle(0,playerCamera.transform.localEulerAngles.x):0;pitch=targetPitch;yawVelocity=pitchVelocity=0;velocity=Vector2.zero;}
#if UNITY_EDITOR
        // Batch editor has no focused Game View. Review supplies movement through the same controller.
        public bool reviewDriving;
        public Vector2 reviewMovement;
#endif
        void Awake() { body = GetComponent<CharacterController>(); helpUntil = Time.time + 9; }
        void Start() { Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false; }
        void OnApplicationFocus(bool focused) { if (!focused) ReleaseCursor(); }
        void OnDisable() => ReleaseCursor();
        static void ReleaseCursor() { Cursor.lockState = CursorLockMode.None; Cursor.visible = true; }
        void Update()
        {
            var keyboard = Keyboard.current; var mouse = Mouse.current;
            if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame) ReleaseCursor();
            if (mouse != null && mouse.leftButton.wasPressedThisFrame && Cursor.lockState != CursorLockMode.Locked)
            { Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false; }
            Vector2 input = Vector2.zero;
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                if (mouse != null)
                {
                    Vector2 delta = mouse.delta.ReadValue() * sensitivity;
                    targetYaw+=delta.x;
                    targetPitch=Mathf.Clamp(targetPitch-delta.y,-75,75);
                    float yaw=Mathf.SmoothDampAngle(transform.eulerAngles.y,targetYaw,ref yawVelocity,lookSmoothTime);
                    pitch=Mathf.SmoothDampAngle(pitch,targetPitch,ref pitchVelocity,lookSmoothTime);
                    transform.rotation=Quaternion.Euler(0,yaw,0);
                    playerCamera.transform.localRotation = Quaternion.Euler(pitch,0,0);
                }
                if (keyboard != null)
                    input = new Vector2((keyboard.dKey.isPressed ? 1 : 0) - (keyboard.aKey.isPressed ? 1 : 0),
                        (keyboard.wKey.isPressed ? 1 : 0) - (keyboard.sKey.isPressed ? 1 : 0));
            }
#if UNITY_EDITOR
            if (reviewDriving) input = reviewMovement;
#endif
            input = Vector2.ClampMagnitude(input, 1);
            velocity = Vector2.MoveTowards(velocity, input * walkingSpeed * worldUnitsPerMetre, Time.deltaTime * 6 * worldUnitsPerMetre);
            gravity = body.isGrounded && gravity < 0 ? -2 * worldUnitsPerMetre : Mathf.Max(-20 * worldUnitsPerMetre, gravity - 20 * worldUnitsPerMetre * Time.deltaTime);
            Vector3 movement = transform.right * velocity.x + transform.forward * velocity.y + Vector3.up * gravity;
            body.Move(movement * Time.deltaTime);
        }
        void OnGUI()
        {
            if (Time.time > helpUntil && Cursor.lockState == CursorLockMode.Locked) return;
            var style = new GUIStyle(GUI.skin.label) { fontSize = 16, alignment = TextAnchor.MiddleCenter };
            style.normal.textColor = new Color(.91f,.88f,.78f);
            GUI.Box(new Rect(Screen.width / 2 - 235, Screen.height - 65, 470, 38), GUIContent.none);
            GUI.Label(new Rect(Screen.width / 2 - 235, Screen.height - 65, 470, 38),
                Cursor.lockState == CursorLockMode.Locked ? "WASD  Yürü     •     Fare  Bak     •     Esc  İmleç" : "Atölyede dolaşmak için tıkla", style);
        }
    }
}
