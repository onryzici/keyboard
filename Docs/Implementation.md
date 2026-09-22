# Cozy Custom Keyboard Shop — first playable

## Art direction
A small original workshop named **Little Switch**: walnut workbench, sage cutting mat, cream keyboard chassis, terracotta accents, organized tools, parts drawers, packing nook and a dusk window. Authored clusters frame an uncluttered assembly area. Rounded bevel geometry and matte materials carry the visual style.

## Scope
One compact 60% keyboard, one case and PCB, three switch families, three keycap colorways, five customer orders. Complete order → select compatible parts → physically install switches and keycaps → test and repair → pack → deliver → receive payment → purchase one visible shop upgrade.

## Architecture
- ScriptableObject catalog: stable IDs, prices, colors, switch sound and feel.
- Serializable build/save state: installed slots, tested slots, selected catalog IDs, order, balance, reputation and upgrade.
- Pure validation/economy rules separate from scene presentation.
- Physical drag-and-release installation with alignment feedback and settle animation.
- Controlled shop/assembly/inspection camera transitions.
- Small contextual uGUI overlay; customer request displayed as a paper slip.

## Verification
Compile in Unity, exercise valid and invalid build transitions, reload a saved build, run the complete five-order progression, inspect shop and assembly screenshots, then build a local playable target.
