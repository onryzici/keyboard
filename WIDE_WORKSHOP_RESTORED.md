# Approved wide workshop restored

Current scene: `LittleSwitch/Assets/Scenes/WideWorkshop.unity`.
Executable: `Builds/WalkableWorkshop/Little Switch Workshop.exe` (3840x2160 borderless).

The saved WalkableWorkshop is the source, preserving its seven areas, furniture meshes and placement. An audit verifies 388 original renderer transforms under a single uniform coordinate mapping. The whole room uses 4.3 game units per metre to align the existing physical assembly system without rewriting its stable coordinate-dependent interactions. Walking speed, camera clipping, light ranges and fog distances follow that scale.

Changes: timber ceiling lining, warm plaster and lower timber wall cladding, three worn floor cartons. Previous small corrections retained: blue cutting mat, removed stools/area labels, smooth slower walking and gentler HDRP volumetrics. Shelf keyboards and existing equipment remain; only the main tabletop display keyboard is replaced by the playable board.

WASD/mouse to walk. E near the main bench opens physical keyboard design/assembly. E near the separate computer opens customer orders and component selection. Esc closes the terminal/held item first, then returns to walking. Existing testing, repair, packing and delivery remain.

Assets: original WorkshopWalk Blender models; existing refined CC0 tools/lamp and component packages; Poly Haven CC0 worn cardboard box. Existing licenses remain in ThirdParty/SOURCES.md and WorkshopWalk documentation. No purchased assets.

Validation: actual player camera screenshots in Logs/wide-view-*.png; collision-enabled 10-waypoint walking route; isolated mechanics test covers order/purchase, physical package opening, switch/keycap placement, all 61 keys, repair, packaging, delivery and return to walking. Tests use an Editor-only save path; the current personal save is retained. This does not recover the earlier lost save documented in WORKSHOP_REVISION.md.

## Lighting revision

Lighting-only follow-up: fixed EV7.8, cooler gradient sky EV7.6, 4500 lux/4600 K sun, 4-degree solar angular diameter. Five rectangle sources align with the two existing pendants and three desk lamps; total six lights including sun. Practical lamps 3400-3800 K. ACES, bloom .06, SSAO .35, SSGI with .25 ambient fallback. Volumetric MFP1000 (local500), anisotropy .35, sun volumetric dimmer .5. Values use the scene's 4.3-unit/metre scale. Brighter localized work surfaces and softer sunlight retain shadow contrast. Reviewed six actual player-camera positions. SSGI remains view-dependent; there is no baked indirect-light solution yet. Reference-quality ceiling bounce and material richness are not fully matched by this lighting-only pass.
