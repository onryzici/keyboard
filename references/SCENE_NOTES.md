# Workshop review checkpoint — 22 September 2026

Authoritative scene: LittleSwitch/Assets/Scenes/Workshop.unity. Work on the live scene through Unity CLI. WorkshopSetup.Build and the art pass Apply methods are construction/migration scripts; do not rerun them over this scene. Several subsequent composition fixes are saved directly in the scene.

Current worktop top: y=4.0; blue mat top y=4.085. Keyboard root (0,4.135,-0.55), uniform PartScale=0.38. Supply (-2.6,4.245,-1.05), above the tray top y=4.23. Slot positions use TransformPoint. Close camera (-0.35,8.1,-2.1). Shop camera (0,8.25,-11.8), target (0,5.5,1.5). Desk bounds 9.2 wide / 4 high / 4.5 deep; top slab locally thinned with a cloned mesh. Current room x bounds +/-10.2; back wall has a true window opening and shared continuous front geometry.

PC has OrderTerminal + collider. Click it to open ShopUI's inbox. Orders/parts UI is hidden by default. Menu contains wallet, upgrade, audio and inspection. Gameplay save is preserved; user is actively testing and advancing orders, so never reset it.

Switch audio: Resources/SwitchAudio/{Linear,Tactile,Clicky}, 8 recordings each, source MechVibes Cherry MX Red/Brown/Blue PBT. No synthetic switch oscillators. Credits: Assets/LittleSwitch/ThirdParty/SOURCES.md.

Room/wood materials: Poly Haven beige_wall_001 and fine_grained_wood. NormalMap importer, _NORMALMAP, packed AO/smoothness masks and recalculated mesh tangents are enabled. Wood uses a restrained diffuse detail layer to preserve the workshop palette. URP camera has HDR/postprocessing, ACES, restrained bloom and SSAO.

Validation: 61/61 raycast key targets from the assembly camera; all 3 packaging steps reach Ready; original save restored byte-for-byte after isolated validation. Pack Action now returns after starting its coroutine, so the generic Changed() call no longer destroys the animated parcel. Last compile completed successfully. New scene artwork still needs user review against the visual reference; do not claim final reference-quality art.

## Latest visual checkpoint — 2026-09-22
Scene is authoritative; do NOT rerun old room migration scripts. Window moved to left wall, exterior buildings disabled, blank soft exterior backdrop. New continuous rear wall and shelf supporting former windowsill books. Gentle additive window rays use original SoftWindowRay shader (art-directed transparent meshes, not volumetric simulation). Vignette removed, AO reduced, warm localized lighting and cool reflected wall fill. Slimmer desk apron mesh, angled runtime shop camera. New detailed CC0 toolbox faces player; binder notebook replaces old placeholder. Wood grain is now main albedo. Floor has its own PalePlankFloor material and different wood_table diffuse/normal texture, not the desk material. Generated strong plaster texture was rejected and unassigned. Play entry checked with active game controller and zero console errors; no save reset performed.

## User-reviewed storage / dust revision
- Rear wall now z=2.55 (front 2.34), behind desk rear edge z=2.25. Wall props moved forward together, side window shifted along wall to fit.
- Two subtle drifting dust particle systems, ~70 particles each, only in daylight bands. Original circular billboard shader; no simulation gameplay changes.
- Wall organizer with 15 primitive drawers rejected and disabled. Replaced with imported KayKit shelf and three Poly Haven worn cartons. All clean supply parcels replaced with smaller worn carton models; corrected FBX -90-degree axis.
- All cartons and their contents moved outside the desk footprint at user request. Desk underside stays clear.
- Original primitive cabinet, imported modern drawer cabinet, and imported painted cabinet all rejected. All three cabinet scene roots DELETED at user request. Do not reintroduce a cabinet without a new request. WorkshopDrawer.cs remains unused; its 0.5-unit travel and collider raycast were verified before removal.
- Toolbox lid closed and inner tray seated. Two-sided material for thin surfaces.
- Original floating extension strip disabled; compact rounded wall sockets now at right (still authored geometry, not a downloaded model).
- SubtleRearPlaster.mat uses metre-appropriate tiling and PlasterRelief normal derived by Unity TextureImporter from generated plaster. Strong generated plaster albedo remains unassigned. No large painted patches.
- User requested all authored environmental placeholders be replaced with downloaded stylized models. This broader pass is NOT complete: custom CRT, digital clock, wall sockets, upper ledge and lamp lantern geometry still remain. Keep this limitation explicit; do not claim all models have been replaced.

## Latest user-directed desktop / interaction pass
- Imported Bruno Oliveira CC-BY alarm clock replaces old digital clock, on desk. Source/attribution in SOURCES.md.
- World-space 660x440 mail UI sits 0.0015 units above actual CRT front, positioned from mesh vertex projections; old floating POSTA label disabled. Existing PC inbox interaction retained.
- Personal books shelf, its book_set and empty pictureframe DELETED at explicit user request.
- ALL desk lamp variants, Bench lamp light, lamp cable and plug DELETED at explicit user request. Do not add another lamp without asking. Rounded/oval source assets remain only as rejected experiments.
- Porcelain recessed wall outlet replaces old oversized double plate.
- Cup moved onto separate cork coaster, notebook moved back to avoid overlap. Warm tea surface and subtle particle steam. Click cup for ~2-second lift/sip/return; locks concurrent bench interaction, no costs/timers. HotDrink.cs + ShopGame.Drink.cs.
- Low-volume CC0 lo-fi music plays through WorkshopSound, 3-second fade, loop, existing mute controls both sound/music. Streaming Vorbis import.
- TWO CLOSED parts packages now live on component shelf (original three decorative cartons hidden). Click the relevant package after purchase/current build stage: lifted onto desk, seal removed, independent lids fold open. Parts only appear after opening. BuildState stores switchPackageOpened/capPackageOpened; ResetBoard resets both. Existing player save not reset.
- Package desk positions switch(-3.85,4.035,-1.55), caps(-2.8,4.035,-1.55), both outside mat; solid internal floor hides surfaces beneath. Supply matches positions. Close camera(-.85,8.65,-2.5), target(-.85,4.15,-.55).
- Shelf lantern stays orange/warm at user request and slightly brighter (5.2 base, range4.6), Perlin flicker synchronized with glass emission. Source inside lantern; glass and thin uprights do not cast fake hard point-source silhouettes. Soft cap/base shadows retained, strength .32.
- Live validation: both package shelf raycasts and opening animations/state; 61 key targets; sip/return; looped 123-second music playing. Original save restored byte-for-byte after isolated test. references/checkpoints/verify-cozy-interactions.cs.txt.

## 2026-09-22 — movable desk props and cinematic inspection
- Alt + left drag moves computer, clock, drink, notebook, toolbox, loose tools and opened component cartons. Mat and assembly board remain fixed. Footprints keep objects on the desk and reject overlap with other movable props / assembly board. Esc cancels.
- Separate `desk-layout-v1.json` persists layout; package desk destinations persist while closed packages remain on their shelf. Runtime contents follow each carton via `PartsSupplyAnchor`.
- Camera now rotates in both room and assembly views, with smooth scroll dolly (0.65–1.16 scale). Short right-click still repairs; dragging does not. Assembly base position is (-1.15,9.6,-2.8), target (-1.15,4.15,-0.55).
- Vernier and cutter moved away from the notebook to the back-right desk area.
- Removed particle-ball steam. Three tapered translucent curling ribbons follow the cup and hide during sipping.
- Passed: compilation, five-order validator, both package pickup/open animations, all 61 key ray targets, sip return, music playback/loop, layout serialization, actual Alt-drag/release input, carton-content following, blocked overlapping/off-table placements, camera rotation/zoom limits.
- Unity cloud token-exchange errors may appear independently of offline gameplay. No standalone player build was produced during this check.

## Final lighting adjustment
- Window ray opacity 0.027 → 0.044; pale golden color (1,0.91,0.70).
- Window bounce 12 → 14.5 with a pale yellow tint. Room reflected fill shifted from blue/cyan to restrained warm cream; ambient trilight slightly warmed.
- Kept the orange flickering shelf lantern. Reviewed in Play mode; screenshot updated at `references/reviews/workshop-latest.png`.

## Workshop identity pass — 2026-09-22
- User rejected the brick wall experiment. Original `SubtleRearPlaster` restored; all brick instances and generated brick assets removed. Keep this plaster surface in future passes unless explicitly asked otherwise.
- Existing Sjolle tools grouped on a perforated steel wall board. Added two keyboard display builds, two smooth coiled cables, a spare PCB and a physical three-sheet job board. These are environmental props, not extra gameplay/UI controls.
- Empty clock shelf and old isolated two-tool rail disabled. Upper shelf archive regrouped into keyboard showcases and cartons, with plant clearance. Right shelves visually connected by metal mounting uprights and small stock labels.
- Orange reflected fill increased slightly; recessed warm strip under upper shelf gives a focused work-area light. Existing window rays/dust and flickering lantern retained.
- Reopened saved scene: no missing mesh/material references. Verified both package pickup/open animations, all 61 key ray targets, drink return and looping music; player progress restored after checks. Screenshot: references/reviews/workshop-latest.png.
