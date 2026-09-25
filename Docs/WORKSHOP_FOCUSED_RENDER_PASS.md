# WorkshopFocused HDRP rendering pass

Unity 6000.5.6f1 · HDRP 17.5.0 · `LittleSwitch/Assets/Scenes/WorkshopFocused.unity`

The room layout and main furniture placement remain intact. Rendering changes are in the focused scene lighting, camera, probes, `Assets/WorkshopRevision/Focus/FocusedRenderVolume.asset`, and selected materials. Six small PBR mask maps live in `Assets/WorkshopRevision/Focus/RenderMasks`. The follow-up also repairs drawer and keyboard placement interactions.

## Verification

| Area | Result |
| --- | --- |
| Global illumination | Baked indirect lightmaps and Adaptive Probe Volumes replace screen space GI. SSGI was disabled after same-view comparisons identified unstable wall shading. Three mixed lights retain moving-object shadows; six secondary lights are baked. |
| Ambient occlusion | HDRP SSAO enabled; radius 1.15 scene units and intensity 0.52. Bench, shelving, boxes and wall joints checked in player camera captures. |
| Contact shadows | Enabled in the Volume and on the sun and practical lights. Short 0.38 unit contact length. Indirect fill lights do not cast redundant shadow maps. |
| Main and fill lighting | Existing sun and five fixtures retuned. Three broad neutral wall bounce lights separate shelves, workbench and corners without changing furniture or adding props. |
| Shadow quality | Sun uses 4096 shadow resolution and 2° angular diameter. Two mixed practical lights use cached 1024 maps with dynamic casters retained. Two cascades and 45 unit maximum distance. |
| Walls | Upper walls now have shallow beveled brick relief in ten combined mesh panels, with CC0 clay diffuse, normal and roughness maps. Existing openings and wall footprint are preserved. Mortar uses subtle plaster detail. Lower panel gaps have continuous dark backing. |
| Wood | Existing grain maps retained; smoothness and normal response retuned for floor, bench, beams and ceiling. |
| Metal | Baked paint remains dielectric; bare steel uses metallic response. Both receive low contrast roughness variation. |
| Other materials | Cardboard, paper and plastic use separate smoothness masks; no broad dirt overlay. |
| Reflections | Three local 64 px reflection probes update on awake. HDRP SSR is enabled on the player camera. |
| Edge response | Existing authored hard-surface bevels were retained. The 15 functional drawer fronts and pulls now use shared small-radius rounded meshes. No imported mesh was destructively edited. |
| Depth | Foreground, bench and rear shelves read as separate layers in player camera views. Under furniture remains darker. |
| Camera | Player camera vertical FOV 58°. The starting view was moved to frame the workbench and pegboard as the first visual focus. Assembly camera mechanics and transitions remain intact. |
| Exposure | Fixed EV 7.3, stable in all inspected views. |
| Color and post | ACES, restrained +3 contrast and -3 saturation, neutral white balance. Warmth comes from 3850 K sun and 4050 K window bounce. Bloom 0.085 at threshold 2.2, scatter 0.6. |
| Volumetrics | Global mean free path 1600 and local 430; no smoky room fill. |
| Image stability | TAA active; baked GI/APV, AO, SSR, contact shadows and post processing used by the player camera. Actual Play Mode screen captures were used for close checks because an isolated offscreen camera capture shows temporal smearing immediately after camera teleports. |
| Performance | Baked secondary lights, cached static shadows, two directional cascades, shared runtime component meshes and supply caching reduce work. A preliminary fixed-view Editor comparison at 1654×1138 improved median frame time from 25.4 ms to 16.6 ms; its p95 remained 35.1 ms. Final measurements are recorded below. These are Editor measurements, not standalone benchmarks. |
| Game View and Play Mode | Player camera views, a ten waypoint 19.13 m route, a later six waypoint 12.43 m route, the opening view, close workbench/drawer views, and assembly camera were inspected. The physical order-to-delivery loop and puller/pliers extraction and reinstallation tests passed. |
| Console | Final Editor ground truth: compilation succeeded, 0 current errors and 0 current warnings. Existing Unity 6 obsolete API warnings remain in unrelated editor tooling; a macOS Editor FMOD output-device message appeared on some earlier Play Mode exits. |

## Follow-up to direct visual feedback

- Added dark seam backing behind lower wall slats and a continuous dark ceiling substrate behind the existing ceiling boards. The boards and beams remain in place, and gaps no longer reveal white paint.
- Added a tileable clean plaster paint map and stronger micro-normal response; retuned timber lining and structural wood toward less glossy, less saturated walnut.
- Rotated the three existing task lamps toward their work areas and moved their light emitters to match the bulbs.
- Made the main workbench's 15 existing drawer locations physically openable, with beveled fronts, colliders, three switch-family rows, and switch selection when opened. Drawers can also be clicked from walking view at close range.
- Added staged align/press/click/settle motion to switch and keycap placement, eased held-part orientation, selected the nearest valid socket within a small tolerance, and animated rejected parts back to supply.

Before and after images: [before](visual-pass/render-before-feedback.jpg) and [after](visual-pass/render-final-after-feedback.jpg). The [close drawer capture](visual-pass/drawer-interaction.jpg) shows the physical switch storage.

## Baked-lighting and masonry follow-up

- Generated secondary UVs in separate mesh assets for architecture and major static furniture. Imported source meshes remain unchanged. Dynamic drawers, tools, packages and keyboard parts remain outside static batching/lightmaps.
- The room already used static batching. Baking reduces real-time lighting work; it does not by itself reduce geometry draw calls.
- Removed invalid small meshes from GI contribution, retaining their normal rendering and probe lighting.
- Shared repeated runtime rounded meshes and combined switch cross stems. Supply contents rebuild only when their appearance changes. Covered socket surfaces stop rendering while their interaction colliders remain available.
- Keyboard desk placement now eases into its accepted position or returns smoothly after an invalid drop.
- Brick relief source: `Tools/art/generate_workshop_brick_relief.py` and captured existing wall-panel bounds. Texture attribution: `Assets/LittleSwitch/ThirdParty/PolyHaven/red_brick/SOURCE.md`.

### Latest measured rendering cost

Apple M2 Pro, Editor Game View 1654×1138, same fixed player pose. Final sample: 5 seconds warm-up followed by 30 seconds measurement (1579 frames).

| Metric | Before | Baked + masonry + runtime optimization |
| --- | ---: | ---: |
| Median frame time | 25.40 ms | 16.60 ms |
| 95th percentile | 27.57 ms | 35.51 ms |
| Draw calls | 3383 | 1658 |
| Shadow casters reported | 2622 | 1088 |
| Submitted triangles | 3,560,186 | 2,071,661 |

Median cost and draw workload improved; the slower-frame tail did not. This does **not** establish stable 60 FPS or standalone performance. The final seam-culling exclusion was applied after this measurement.

Current screenshots: [wide workshop](visual-pass/warm-baked-workshop.jpg), [workbench](visual-pass/warm-baked-workbench.jpg), [brick surface](visual-pass/brick-material-close.jpg).

Occlusion data is baked for large opaque structure. Thin seam backing is excluded from occlusion culling so narrow gaps cannot reveal the sky. The player CharacterController completed a 21.75 m route through ten viewpoints after the bake.

The final full input-driven assembly run passed all 61 switch and 61 keycap placements, occupied-slot rejection, testing, repair, packaging and delivery. Its test cleanup encountered a destroyed temporary InputSettings reference; the cleanup now guards that reference. The test uses an isolated save and temporarily disables native mice inside Unity to prevent external pointer events from replacing its synthetic input.

Final tool roundtrip passed: cap extraction, switch extraction, physical reinstallation, pliers repair, both supplies in frame and matched return-camera FOV/rotation. Final Console ground truth after this run: 0 errors, 0 warnings, compilation successful. A fresh compilation still reports 20 pre-existing obsolete-API warnings in HDRPWorkshopLighting/HDRPWorkshopReview editor utilities; none originate in the changed runtime code.
