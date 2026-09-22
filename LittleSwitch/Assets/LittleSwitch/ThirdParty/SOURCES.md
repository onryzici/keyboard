# Third-party workshop models

All four packs are free and released under CC0. Source archives and preview images are retained in ../../../../references/asset-research (relative to the project root's parent).

- KayKit Furniture Bits FREE — Kay Lousberg — https://kaylousberg.itch.io/furniture-bits
  Original License.txt included. Unity FBX files and gradient atlas. Selected furniture/decor only; no paid EXTRA/SOURCE files.
- Office low poly pack — MrEliptik — https://mreliptik.itch.io/office-low-poly-pack
  Original LICENSE.txt included. Selected original GLB files converted to FBX using Blender 5.2.1; original geometry and material colors retained, URP materials rebuilt. Mug color adapted.
- Low poly tools — sjolle — https://sjolle.itch.io/low-poly-tools
  Source page explicitly states Creative Common Zero 1.0. Selected six original FBX tools; embedded matsimple texture extracted through Unity ModelImporter.
- Free Low-Poly Furniture – 22 Props — Quin.GS — https://quin-gs.itch.io/furniture-lowpoly-cc0
  Source page explicitly states CC0. Selected objects exported separately from the original combined LowPolyFurniture1.fbx using Blender.

The scene instances are grouped under Imported artisan props. Replaced prototype objects are inactive, not deleted. Model axes preserved and dimensions fitted to the existing interaction surface. Materials adapted to URP with restrained smoothness.

## Recorded switch audio — MechVibes / Hai Nguyen
Source: https://github.com/hainguyents13/mechvibes/tree/main/src/audio
License: MIT, copyright (c) 2021 Hai Nguyen. Full license in MechVibes/LICENSE.txt.
Cherry MX Red PBT, Brown PBT and Blue PBT recordings. Eight key samples per family, cut using upstream config timestamps, converted to mono 44.1 kHz WAV with 8 ms release fade. Source configs retained alongside license. No synthesized switch audio remains.

## Refined workshop props
ArtisanLamp.fbx: modified MrEliptik CC0 lamp_architect; beveled edges and new matte enamel materials. Original license remains in MrEliptik/LICENSE.txt.
RetroComputer.fbx: original workshop model authored in Blender, with tapered CRT casing, inset screen, chassis vents and individually modeled keys. The tested OpenGameArt computer was not used in the final scene.
CuttingMat.png: original programmatically drawn measuring grid, rulers and angle guides; no third-party logo or image pixels used.

## Furnished scene update — Kenney Furniture Kit
https://kenney.nl/assets/furniture-kit — CC0. Full license in Kenney/License.txt.
Imported cardboardBoxClosed, cardboardBoxOpen, radio, pottedPlant, books and desk. Scene uses cardboard boxes, radio and potted plant; book placeholders were replaced with KayKit book_single.
Additional KayKit models: shelf_A_small, book_single.

## Room surface maps — Poly Haven, CC0
https://polyhaven.com/a/beige_wall_001 — Dimitrios Savva / Rico Cilliers.
https://polyhaven.com/a/fine_grained_wood — Rob Tuytel.
1K diffuse, OpenGL normal, roughness and ambient occlusion maps. Packed Unity masks: R=0 metallic, G=AO, A=1-roughness. Wood diffuse used as a restrained detail layer over the workshop palette. Normal-map imports and mesh tangents enabled.

## Exterior buildings — Kenney City Kit (Commercial), CC0
https://kenney.nl/assets/city-kit-commercial
Building A, B, D, F use the supplied colormap. License retained in KenneyCity/License.txt.

## Detailed workshop props — 2026-09-22
- Poly Haven Metal Toolbox, Mateusz Sadek, CC0: https://polyhaven.com/a/metal_toolbox — original FBX, 1K diffuse/normal/roughness/metal maps. Adapted matte URP material, scaled and rotated to face player.
- Poly Haven Binder Notebook, DaDrood, CC0: https://polyhaven.com/a/binder_notebook — original FBX and 1K maps; replaces block notebook.
- Painterly/WalnutPainted.png: generated original texture with built-in imagegen. Prompt: seamless diffuse albedo of warm honey walnut with horizontal hand-painted grain, restrained knots and scratches, neutral lighting, no objects/text/shadows. Applied to workbench and shelving.
- Painterly/PlasterPainted.png: generated with built-in imagegen, prompt: seamless ivory lime plaster with broad painterly trowel strokes, even lighting and no objects/text. Rejected for wall use after user feedback; not currently assigned.

## Storage revision
- Cardboard Box 01 — Rahul Chaudhary, Poly Haven, CC0: https://polyhaven.com/a/cardboard_box_01 . 1K diffuse/normal/roughness and FBX. Used for worn shelf parcels and floor cartons.
- Drawer Cabinet — Ulan Cabanilla, Poly Haven, CC0: https://polyhaven.com/a/drawer_cabinet . Imported for evaluation; removed from scene after user rejection.
- Painted Wooden Cabinet 02 — Kirill Sannikov, Poly Haven, CC0: https://polyhaven.com/a/painted_wooden_cabinet_02 . Imported for evaluation; removed from scene after user rejection.

## Desktop clock, cup, animated supply packages — 2026-09-22
- Alarm Clock — Bruno Oliveira, CC BY 3.0, https://poly.pizza/m/bgtBPjUm3Rw . Downloaded GLB, converted to FBX, softened edges, adapted matte palette; active desk clock. Attribution: “Alarm Clock” by Bruno Oliveira via Poly Pizza, licensed CC BY 3.0 (https://creativecommons.org/licenses/by/3.0/). Modified.
- GlazedWorkshopMug.fbx: existing MrEliptik CC0 mug, beveled and adapted sage glaze.
- ComponentPackageBody.fbx: Kenney Furniture Kit cardboardBoxOpen (CC0), duplicate faces removed, actual wall thickness, original flaps replaced with independently hinged lids and solid inner liner. Two shelf packages use the model. Kenney licence retained.
- Music/ChillLofi.mp3: “Chill lofi inspired” by omfgdude, CC0, https://opengameart.org/node/74097 . Original MP3 download https://opengameart.org/sites/default/files/ChillLofiR_0.mp3 . Low-volume loop with 3-second fade-in; 123.19 seconds.
- PorcelainSocket.fbx: original modeled rounded porcelain plate with recessed well, pin wells and screw heads.
- Original RoundedTaskLamp and VintageOvalLamp experiments were rejected; all desk lamp scene roots and their light/cable removed at user request. Imported VintageBankerLamp from https://3dassets.dev/assets/police-station-and-detective-office-bankers-lamp-fb4ee0bf (CC0, AI-assisted) was evaluated but NOT used.
