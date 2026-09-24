Superseded: the current scene is WideWorkshop.unity. See WIDE_WORKSHOP_RESTORED.md. This file records the earlier rejected small-room revision.

# Little Switch — workshop revision

Current scene: `LittleSwitch/Assets/Scenes/CraftedWorkshop.unity`. Windows build: `Builds/WalkableWorkshop/Little Switch Workshop.exe`, 3840 × 2160 borderless fullscreen.

## Requested corrections

- Restored the original working workshop as the furnishing/gameplay base: worn Poly Haven cartons, existing open component boxes, original computer, real hand tools, toolbox, clock, wall storage and the original printed blue cutting mat.
- Restored orders, component selection, physical package opening, switch/keycap placement, testing, repair, packing and delivery. The save format is unchanged; see the save incident below.
- Added walking/workbench handoff: E near the workbench enters the controlled assembly camera; Esc returns smoothly to the saved walking position. Esc first handles an open terminal or held tool/part. The controller and game do not compete for the same camera/input.
- Reduced walking speed to 1.35 m/s, mouse sensitivity to .045 degrees/pixel, with .09-second look smoothing. Movement acceleration is 6 m/s². Legacy gameplay uses four world units per metre; controller dimensions are scaled consistently.
- Removed stools and the floating electronics/color-studio/area headings. Small labels physically attached to supply packages and job sheets remain functional.
- Replaced the architecture with original beveled Blender meshes: cream plaster, warmer rounded timber beams/braces, tongue-and-groove style wainscot, recessed window, paneled entrance and staggered floorboards.
- New softer painted walnut albedo has metre-based UV mapping. HDRP normal and roughness/AO maps are applied with restrained strength, rather than leaving flat color-only surfaces.
- Two new modeled desk lamps have downward-facing diffusers. Warm overhead fixtures support the main bench and receiving table.
- Installation now aligns the part just above the socket, then presses through a short resistance phase before the existing click and settle feedback.

## Lighting

One 6,500-lux sun at 5100 K, Euler 24/80/0, angular diameter 2.2°. Sun volumetric dimmer .30, global fog mean free path 400 world units, local window fog 180, anisotropy .25. This intentionally reduces the previously dominant shaft effect.

Fixed exposure EV100 9.7; Gradient Sky exposure 9.4, multiplier .5; ACES; bloom .025; HDRP SSAO/contact shadows and SSGI retained. Warm Rectangle practicals: main bench 9000 lm, receiving 2800 lm, desk lamp 650 lm. Large lumen values reflect the legacy four-units-per-metre geometry. No new point/spot lights or fake beam geometry.

## Provenance

New architectural and lamp FBX source: `Tools/build_workshop_revision.py`. The supplied image is used for warm timber/plaster proportions and mood; its blacksmith props and layout were not copied. A local reference copy is `references/workshop_interior_revision.png`.

`Assets/WorkshopRevision/Textures/PaintedWalnut.png` is an original imagegen-generated stylized albedo. Normal and packed roughness/AO maps reuse the project's CC0 [Poly Haven fine-grained wood](https://polyhaven.com/a/fine_grained_wood), at low strength. Existing plaster relief is reused. Restored third-party models retain their original licenses and provenance in [SOURCES.md](LittleSwitch/Assets/LittleSwitch/ThirdParty/SOURCES.md), including the clock's CC BY attribution. No paid assets purchased.

## Scope and limits

The original scenes remain available; the new combined scene is the enabled build scene. SSGI remains screen dependent. Some inherited small assets retain their simpler silhouettes. The generated albedo is paired with subtle existing grain normal detail rather than a scanned, physically matched texture set.

## Validation

The gameplay review exercised order acceptance, purchase, actual package opening, the switch/keycap installation coroutines, all 61 key tests, fault repair, three packing steps, delivery and return to walking. The walk review covered six waypoints and 9.66 m using the real controller with collisions. Five player-camera views include the entrance, both bench approaches, receiving and assembly.

Windows build completed successfully with zero errors (`Logs/revision-windows-build.txt`). The isolated test was rerun successfully; the personal save's SHA-256 was unchanged by that run. The game has not been relaunched after this build while the user's save recovery preference is pending.

## Save incident

An initial test restored the personal save during `ExitingPlayMode`, but `ShopGame.OnApplicationQuit` subsequently wrote the completed test order over it. The original byte-for-byte record was not recoverable; the first visual review showed seven installed switches. This was disclosed to the user and a choice between reconstructing that stage or starting a new order was requested. The overwritten test-state file is retained at `LittleSwitch/Logs/save-incident-test-state.json`.

Tests now use an editor-only save-path override pointing to `Logs/mechanics-isolated-save.json`, including the shutdown write. The override is cleared only after `EnteredEditMode`. This test path does not exist in standalone builds.

