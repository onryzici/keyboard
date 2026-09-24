# Little Switch — walkable workshop

Historical environment-only pass. The current default scene is `CraftedWorkshop.unity`, with restored gameplay and revised art direction. See [WORKSHOP_REVISION.md](WORKSHOP_REVISION.md).

Correct project: `keyboard/LittleSwitch`, Unity 6000.5.6f1, HDRP 17.5.

Windows opens at **3840 × 2160 (4K), borderless fullscreen**. The workshop reapplies this default at launch so an old cached window size cannot keep it in windowed mode.

Open `Assets/Scenes/WalkableWorkshop.unity` and press Play. Windows build: `Builds/WalkableWorkshop/Little Switch Workshop.exe`. WASD walks, mouse looks, Escape releases the cursor, click resumes. Eye height is 1.65 m; speed 2.15 m/s; capsule radius 0.24 m. No interaction or business systems run in this scene.

## Environment

Approximately 10.7 × 8.3 m of floor area, 3.6 m ceiling. Thick plaster walls, recessed divided windows, timber beams and knee braces, metal shoes/bolts, trims, conduit, fuse box, ventilation and modeled entrance. The window courtyard contains trees, paving and distant neighborhood buildings.

- Entrance/receiving: paneled door, hooks/apron, receiving table, parcels and noticeboard.
- Main bench: thick wood top, dark steel frame, drawers, clear sage work mat, keyboard display, component drawers, task lamp, stool, trays/jar and tool wall.
- Tool wall: six imported hand tools, brush rack, three cable loops, display shelf and keyboard.
- Parts storage: two tall racks, labeled bins, component drawers, clear switch jars and cartons.
- Electronics: secondary bench, soldering station/iron/coil, multimeter/leads, drawers, lamp and mat.
- Customization: compact extractor booth, paint bottles, brushes, drying rack and four shell color samples.
- Computer desk: original CRT computer/keyboard, stool, lamp, speaker, mug, books, paperwork and radio shelf.
- Shipping: packing table, paper roll, tape/dispenser, cartons and dispatch shelves next to receiving.
- Rolling service cart supplies a middle-distance layer while preserving circulation.

## Assets and licenses

28 original Blender FBX modules with modeled bevels, joinery and hardware; generator retained at `Tools/build_walkable_workshop.py`. Existing project models and selected CC0 KayKit, MrEliptik, Sjolle, Quin.GS, Kenney Furniture/City and Poly Haven wood assets are reused. Full links and provenance: [asset sources](LittleSwitch/Assets/WorkshopWalk/SOURCES.md). No purchased assets.

## HDRP rig

| Setting | Value |
| --- | --- |
| Active lights | 4: one Directional, three Rectangle; zero Point/Spot |
| Sun | 30,000 lux; 5100 K; Euler 18.5°, 72°, 0° |
| Sun shadows | Soft, angular diameter 1.25°, 2048 map, 35 m distance |
| Contact shadows | Enabled, 0.12 m length |
| Sky | Realtime Gradient Sky, muted cool top/mid/bottom; exposure 9.8, multiplier 0.5 |
| Global fog | True HDRP volumetrics; mean free path 90 m, max height 4 m, depth extent 24 m |
| Local window air | 6.6 × 2.5 × 5 m, mean free path 28 m; soft fades |
| Anisotropy | 0.48 |
| Sun volumetric dimmer | 1.6 |
| Fog quality | 96 slices, 20% screen resolution, probe dimmer 0.2 |
| AO | HDRP SSAO 0.45 intensity, 0.20 m radius |
| Indirect light | Full-resolution SSGI, 64 steps, denoised; sky/environment fallback |
| Exposure | Fixed EV100 10; no adaptation/pumping |
| Tonemapping | ACES |
| Bloom | 0.035 |
| Main pendant practical | 650 lm, 0.40 × 0.40 m Rectangle |
| Electronics practical | 180 lm, 0.18 × 0.12 m Rectangle |
| Order desk practical | 150 lm, 0.18 × 0.12 m Rectangle |
| Practical temperature | 3800 K; volumetric dimmer 0.03 |

Sunlight passes through actual architecture. Fog scattering is HDRP; there is no beam mesh. The camera explicitly enables volumetrics, SSAO, contact shadows and SSGI. Light/shadow contrast is intentionally preserved while shaded stations remain visible.

## Validation and limits

Play Mode route covered all stations through the real CharacterController with collisions enabled: **23.28 m, 10 waypoints**. The batch editor has no focused Game View, so an editor-only review input drives the same movement code; the route does not teleport. Player-camera screenshots were taken at each stop. Runtime verification confirmed HDRP, zero ShopGame instances and four active lights. Evidence: `LittleSwitch/Logs/walk-route-complete.txt`, `walk-validation.txt`, and `walk-route-*.png`.

The earlier keyboard gameplay scene remains `Assets/Scenes/Workshop.unity`; this environment scene is the enabled build scene. Existing saves are not read or changed.

Windows build succeeded with zero errors and was launched successfully. The final running-player image is `LittleSwitch/Logs/walk-player.png`; runtime log is `walk-player.log`. A final player-camera capture checked the adjusted sky fill; a continuous dark subfloor closes light leaks between the modeled planks.

Remaining simplifications: monitor/instrument screens and paper graphics are static; display keyboards have no key legends or assembly behavior; some inherited small CC0 props retain simpler silhouettes. Exterior is a visual backdrop, not accessible. SSGI depends on visible geometry and can lose off-screen bounce; this pass has no baked lightmaps/APV. Volumetric shaft strength varies with viewing direction. No paid pack was required or purchased; there is no researched paid recommendation for this pass.
