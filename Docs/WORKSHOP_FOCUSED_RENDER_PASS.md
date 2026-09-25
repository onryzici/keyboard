# WorkshopFocused HDRP rendering pass

Unity 6000.5.6f1 · HDRP 17.5.0 · `LittleSwitch/Assets/Scenes/WorkshopFocused.unity`

The room layout and main furniture placement remain intact. Rendering changes are in the focused scene lighting, camera, probes, `Assets/WorkshopRevision/Focus/FocusedRenderVolume.asset`, and selected materials. Six small PBR mask maps live in `Assets/WorkshopRevision/Focus/RenderMasks`. The follow-up also repairs drawer and keyboard placement interactions.

## Verification

| Area | Result |
| --- | --- |
| Global illumination | HDRP screen space GI enabled in the active player camera Volume Stack. Half resolution, 32 ray steps, denoising and ambient fallback retain bounced light at a lower cost. |
| Ambient occlusion | HDRP SSAO enabled; radius 1.15 scene units and intensity 0.52. Bench, shelving, boxes and wall joints checked in player camera captures. |
| Contact shadows | Enabled in the Volume and on the sun and practical lights. Short 0.38 unit contact length. Indirect fill lights do not cast redundant shadow maps. |
| Main and fill lighting | Existing sun and five fixtures retuned. Three broad neutral wall bounce lights separate shelves, workbench and corners without changing furniture or adding props. |
| Shadow quality | Sun uses 4096 shadow resolution and 2° angular diameter; five existing practical lights use 1024 maps and soft shadows. Four cascades and 55 unit maximum distance. |
| Walls | A clean painted plaster color map and micro-normal map now break up the flat wall response. The lower panel gaps have a continuous dark backing so white wall paint cannot show between timber slats. |
| Wood | Existing grain maps retained; smoothness and normal response retuned for floor, bench, beams and ceiling. |
| Metal | Baked paint remains dielectric; bare steel uses metallic response. Both receive low contrast roughness variation. |
| Other materials | Cardboard, paper and plastic use separate smoothness masks; no broad dirt overlay. |
| Reflections | Three local 64 px reflection probes update on awake. HDRP SSR is enabled on the player camera. |
| Edge response | Existing authored hard-surface bevels were retained. The 15 functional drawer fronts and pulls now use shared small-radius rounded meshes. No imported mesh was destructively edited. |
| Depth | Foreground, bench and rear shelves read as separate layers in player camera views. Under furniture remains darker. |
| Camera | Player camera vertical FOV 58°. The starting view was moved to frame the workbench and pegboard as the first visual focus. Assembly camera mechanics and transitions remain intact. |
| Exposure | Fixed EV 7.3, stable in all inspected views. |
| Color and post | ACES, restrained +3 contrast, -3 saturation, +4 white balance; bloom 0.035 at threshold 2.4. |
| Volumetrics | Global mean free path 1600 and local 430; no smoky room fill. |
| Image stability | TAA active; GI, AO, SSR, contact shadows and post processing enabled in player camera frame settings. Actual Play Mode screen captures were used for close checks because an isolated offscreen camera capture shows temporal smearing immediately after camera teleports. |
| Performance | Probes use 64 px, GI uses half resolution and 32 steps, SSR and AO are one tier below maximum, and broad fill lights do not render shadow maps. A warmed 1600×900 Editor Play Mode sample reported about 22 ms smooth frame time; this is an Editor sample, not a standalone benchmark. |
| Game View and Play Mode | Player camera views, a ten waypoint 19.13 m route, a later six waypoint 12.43 m route, the opening view, close workbench/drawer views, and assembly camera were inspected. The physical order-to-delivery loop and puller/pliers extraction and reinstallation tests passed. |
| Console | Final Editor ground truth: compilation succeeded, 0 current errors and 0 current warnings. Existing Unity 6 obsolete API warnings remain in unrelated editor tooling; a macOS Editor FMOD output-device message appeared on some earlier Play Mode exits. |

## Follow-up to direct visual feedback

- Added dark seam backing behind lower wall slats and a continuous dark ceiling substrate behind the existing ceiling boards. The boards and beams remain in place, and gaps no longer reveal white paint.
- Added a tileable clean plaster paint map and stronger micro-normal response; retuned timber lining and structural wood toward less glossy, less saturated walnut.
- Rotated the three existing task lamps toward their work areas and moved their light emitters to match the bulbs.
- Made the main workbench's 15 existing drawer locations physically openable, with beveled fronts, colliders, three switch-family rows, and switch selection when opened. Drawers can also be clicked from walking view at close range.
- Added staged align/press/click/settle motion to switch and keycap placement, eased held-part orientation, selected the nearest valid socket within a small tolerance, and animated rejected parts back to supply.

Before and after images: [before](visual-pass/render-before-feedback.jpg) and [after](visual-pass/render-final-after-feedback.jpg). The [close drawer capture](visual-pass/drawer-interaction.jpg) shows the physical switch storage.
