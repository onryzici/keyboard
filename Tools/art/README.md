# Workshop masonry relief

The existing `WorkshopShell_Plaster` panel bounds are recorded in `workshop-wall-panels.json`. Room dimensions and openings are preserved.

1. Run `generate_workshop_brick_relief.py` with Python and NumPy. It writes mesh buffers to `/tmp/workshop-brick-meshes`.
2. In the existing WorkshopFocused scene, execute `ImportWorkshopBrickRelief.cs.txt` using Unity CLI `eval` in Edit Mode. It updates the existing mesh assets, preserving GUIDs, and replaces only the `Masonry surface relief` child.
3. Rebake lighting and occlusion after geometry changes.

The C# snippet is an authoring command, not a runtime component. Existing lightmap UV clones are separate assets so imported source models remain unchanged.
