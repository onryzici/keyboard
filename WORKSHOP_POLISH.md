# Workshop polish

Current playable scene: `LittleSwitch/Assets/Scenes/WorkshopPolished.unity`.

- Front aisle shortened by 0.9 metres; furniture scale, main bench anchor and recognizable work areas retained.
- 37 mesh surfaces receive physical-scale wood UVs instead of a whole-object packed texture atlas. Existing original painted walnut color texture and CC0 Poly Haven normal map; 16x anisotropy, trilinear filtering and higher import limits. Floor, ceiling, wall timber, shelves and bench show finer grain.
- Authored Blender FBX additions: routed parts tray with modeled compartments/screws, coiled USB cable with connector, dished rounded keycap profile. Existing CC0 Poly Haven toolbox reuses its workshop material. No purchases or unverified third-party assets.
- Existing HDRP lighting preserved; thin grain intensity .075. Small lit mesh particles drift beside the window, receiving actual light/shadow. Reduced to 4 particles/sec, maximum90, physical size 2–3.5mm. The light shaft remains real HDRP volumetrics.
- Assembly camera closer. Held caps preview the correct width, accent color and legend; broad keys have matching placement targets. Smooth alignment and press/settle retained. Occupied slots reject replacement. Entering the bench shows the shelf view when a package must be opened.
- Space, Shift, Ctrl/Alt, Backspace, Caps Lock and other common physical-key aliases map to visible test keys. Fn remains clickable because normal keyboards do not send a standalone Fn event.

Validation: all61 switches and61 keycaps placed through virtual mouse input into the actual gameplay Update path, followed by test/repair, packaging and delivery. Tests use Editor-only isolated save files; current personal save hash compared unchanged. Detailed logs and player-camera screenshots are in LittleSwitch/Logs.

Build: `Builds/WalkableWorkshop/Little Switch Workshop.exe`, 3840x2160 fullscreen.
