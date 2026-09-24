# Workshop composition and finish correction

Current scene: `LittleSwitch/Assets/Scenes/WorkshopFocused.unity`.

## Composition and controls

Board centered at X0, left switch carton X-1.6, right cap carton X1.6, both Z-0.55. Assembly camera targets the board at (0,4.15,-0.55), rests at (0,7.6,-2.1), and smoothly reaches 40-degree FOV. Each visit resets orbit/zoom. Walk return blends position, orientation and FOV for .9 seconds, then matches the destination exactly before swapping cameras. Bench entry is restricted to its front approach.

Puller uses an original Blender wire-loop model when held. It aligns, grips, lifts in world space together with the actual piece, then carries it to the matching supply box. Removing a cap preserves the switch; removing a switch returns to the switch stage. Removal during test works. Pliers repair the actual faulty pin. Right-click shortcuts use the tool animation too.

## Room and materials

East wall and its work areas moved inward1.2m; furniture scale, main bench and window wall preserved. Combined with the preceding .9m depth reduction, the interior is about9.5x7.4m. Original zones remain accessible.

Distinct surface treatments: dark smoked floor with original CC0 fine-grain maps, oiled main bench using existing original WalnutPainted texture, aged beams and ceiling boards. Graphite and putty-painted shelving/panels replace the green surfaces, with restrained powder-coat variation and normal detail.

New authored FBX: fitted horizontal venetian blinds with tilted slats, cord ladders and tilt wands; lower bench cabinet fitted to its existing shelf; wire keycap puller. No purchased or unverified assets. Source generator: Tools/build_blinds_and_bench.py.

Sunlight is shadowed by the real blind slats. HDRP volumetric fog remains the source of light scattering; local mean free path180 game units, sun angular diameter.6 degrees, volumetric dimmer1. Existing practical fixture lighting remains.

## Validation

ToolAndCameraReview verifies cap/switch extraction, reinstallation, pliers repair, both supply cartons inside the camera frame, centered keyboard, and exact FOV/rotation match at walking return. Full assembly review drives actual mouse pickup/drag/release. Personal saves use a separate Editor-only test path and are hash-checked. Player-camera captures and walking-route results are in LittleSwitch/Logs.

