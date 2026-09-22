
## 2026-09-22 — camera look, compartment tray and usable tools
- Shop view: hold right mouse and drag for bounded yaw ±20°, pitch -6°/+7°. Tab returns between shop and assembly; assembly right-click repair remains compatible.
- Replaced old flat parts tray with original Blender mesh (raised rounded enamel rim, dark rubber insert, three compartments); retained nine supply parts and existing pickup mechanic.
- Added switch puller and imported Sjolle pliers as clickable WorkbenchTool objects. Selecting enters assembly view; click a key to remove a cap/switch with puller or repair test fault with pliers. Esc returns tool. Only these two bench tools are interactive.
- Tool props placed upper-right on cutting mat so they remain reachable from assembly camera.
- Live verification: camera clamps, both tool raycasts, puller removal animation/state, plier fault repair, returning tools, preserving original order/save. Compilation clean. Repro/model source in references/checkpoints.
- Current scene remains authoritative. Do not run old scene rebuild/migration commands.
