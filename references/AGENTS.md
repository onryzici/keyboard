# AGENTS.md --- Cozy Custom Keyboard Shop

## Project Goal

Create a cozy stylized 3D game about owning and growing a custom
mechanical-keyboard shop.

The player receives customer orders, understands what each customer
actually wants, chooses compatible parts, builds keyboards by hand,
tests them, fixes mistakes, packages finished builds, earns money and
reputation, unlocks new components and gradually transforms a tiny
workshop into a respected boutique keyboard shop.

The game must feel relaxing, tactile, warm and handcrafted. It is
**not** a stressful time-management game, generic tycoon, factory
simulator or sterile PC-building simulator.

Core fantasy:

> I own a tiny beautiful keyboard workshop. I spend my day building
> satisfying custom keyboards for interesting customers.

------------------------------------------------------------------------

# PRIMARY VISUAL REFERENCES

These images are the project's main art-direction references. Study them
before creating environments, keyboard models, lighting, materials, UI
or interaction presentation.

Do **not** literally reproduce copyrighted props, layouts, characters,
text, architecture or keyboard designs from them. Use them as references
for mood, presentation, shape language, lighting, readability and
interaction philosophy.

## Reference A --- Workshop / Shop Mood

![Workshop reference](references/shop_visual_reference.jpeg)

Use this image as the primary reference for the shop.

What matters: - cozy stylized 3D - warm evening atmosphere - handcrafted
/ slightly hand-painted look - soft rounded geometry - strong bevels
rather than razor-sharp CAD edges - dense environmental storytelling -
controlled cinematic camera - workbench as the visual center of
gameplay - shelves, boxes, tools, old electronics, cables and personal
objects around the player - warm interior lighting - colorful but
restrained palette - matte materials - slightly imperfect surfaces -
inviting clutter - clear silhouettes despite a detailed environment - a
miniature-diorama feeling - a shop that feels genuinely lived in

The environment should look authored rather than procedurally filled
with random props.

### Important interpretation

The reference is not attractive because it contains many objects. It is
attractive because objects are grouped deliberately.

Use visual clusters: - tool cluster - packaging cluster - electronics
cluster - personal-object cluster - keyboard-parts cluster - storage
cluster

Keep the central interaction area relatively clean.

------------------------------------------------------------------------

## Reference B --- Keyboard Assembly Presentation

![Keyboard assembly
reference](references/keyboard_assembly_reference.jpeg)

Use this image as the primary reference for keyboard assembly.

Important qualities: - near-top-down presentation - keyboard occupies
most of the interaction area - physical 3D object rather than flat UI -
large readable keycaps - soft stylized forms - rounded keyboard case -
warm cream / muted pastel presentation - empty switch locations are
immediately understandable - installed and missing parts are visually
distinct - soft contact shadows - components are large enough to select
comfortably - minimal UI during physical assembly - satisfying physical
snap-in behavior

Do not reproduce this exact keyboard, layout, colors, legends or
interface. Create original keyboard designs.

------------------------------------------------------------------------

# ART DIRECTION

Target:

**Stylized 3D + cozy workshop + soft hand-painted materials + chunky
readable props + warm cinematic lighting.**

Avoid: - photorealism - ultra-low-poly faceted geometry - obvious
asset-pack / asset-flip appearance - sterile showroom environments -
excessive saturation - horror lighting - cyberpunk overload - excessive
bloom - razor-sharp hard-surface models - perfectly clean materials

## Shape Language

Most objects should use: - rounded corners - bevels - chunky
proportions - simple readable silhouettes - mild asymmetry - subtle
imperfections

Interactive objects may be slightly oversized compared with real-world
scale if that improves readability.

Keyboard switches, keycaps, switch pullers and screws must be visually
easy to identify.

## Materials

Favor: - matte plastic - painted wood - powder-coated metal -
cardboard - ceramic - rubber - cloth - slightly aged electronics

Add restrained imperfections such as: - roughness variation - faded
edges - small scratches - labels - tape - sticker residue - handwritten
notes

Do not add noisy procedural dirt everywhere.

------------------------------------------------------------------------

# COLOR DIRECTION

Environment base: - warm cream - dusty beige - walnut brown - muted
sage - faded orange - soft yellow - dusty blue - desaturated teal -
muted terracotta

Custom keyboards may provide stronger accent colors.

The environment should stay calm enough that the current keyboard
remains the hero object.

------------------------------------------------------------------------

# LIGHTING

Lighting is critical to the style.

Target: - late-afternoon / evening feeling - warm desk lamps - soft
indirect illumination - localized pools of light - gentle ambient
shadows - subtle exterior light - soft contact shadows around small
components

For Unity URP, prefer: - soft shadows - ambient occlusion - subtle
bloom - restrained color grading - ACES tonemapping when appropriate -
reflection probes where useful - mild vignette only when composition
benefits from it

Never use bloom or post-processing to hide weak materials.

The assembly surface must remain readable at all times.

------------------------------------------------------------------------

# CAMERA

Do not begin with unrestricted first-person movement.

Use a controlled cinematic camera system.

## Shop View

A composed workshop view inspired by Reference A. The workbench
dominates the foreground while the shop remains visible around it.

## Assembly View

Smoothly move into a near-top-down keyboard view inspired by Reference
B.

## Detail View

Move closer for: - switch installation - stabilizer work - lubing -
soldering - keycap placement - repairs

## Inspection View

Allow limited orbit and zoom around a completed keyboard.

Prefer smooth camera transitions instead of abrupt menu-like cuts.

------------------------------------------------------------------------

# CORE LOOP

1.  Receive customer request.
2.  Read needs, budget and preferences.
3.  Choose an appropriate keyboard configuration.
4.  Retrieve or purchase components.
5.  Prepare the workbench.
6.  Assemble the board.
7.  Tune or modify parts when needed.
8.  Install switches.
9.  Install keycaps.
10. Configure optional features.
11. Test every key.
12. Listen to the completed keyboard.
13. Fix mistakes.
14. Package the order.
15. Deliver it.
16. Receive payment, review and reputation.
17. Buy tools, parts or shop upgrades.
18. Unlock more complex orders.

Keep this loop understandable even when later systems become deeper.

------------------------------------------------------------------------

# CUSTOMER DESIGN

Customers should usually describe the experience they want rather than
giving the player an exact technical parts list.

Example:

> "I work late and share an apartment. I need something compact and
> really quiet. I like green and cream. Budget: \$160."

The player determines the appropriate technical solution.

Customer dimensions can include: - budget - layout size - typing feel -
sound preference - gaming vs office use - aesthetics - RGB -
wired/wireless - portability - durability - theme - deadline - special
requests

Hidden preferences may exist, but requirements must remain logically
inferable. Do not create unfair guessing.

------------------------------------------------------------------------

# KEYBOARD COMPONENT MODEL

Support modular construction.

Core categories: - case - PCB - plate - stabilizers - switches -
keycaps - foam - cable / wireless module - optional knob - optional
display - decorative insert

Layouts can gradually unlock: - 60% - 65% - 75% - TKL - full-size -
ergonomic/specialty layouts later

Each part can influence: - price - compatibility - sound - typing feel -
quality - aesthetics - rarity

Introduce technical complexity gradually.

------------------------------------------------------------------------

# SWITCH SYSTEM

Switches are one of the game's signature systems.

Initial families: - Linear - Tactile - Clicky

Later: - silent linear - silent tactile - speed - heavy - boutique -
limited switches

Possible properties: - actuation force - travel - sound level - sound
character - smoothness - durability - lubrication - price - rarity

## Physical Installation

Do not use "click button -\> installed."

Desired sequence: 1. Pick up switch. 2. Valid slot receives subtle
feedback. 3. Align switch. 4. Press downward. 5. Tiny resistance
animation. 6. Distinct click. 7. Small settle/bounce. 8. Switch remains
physically installed.

Later, allow: - switch opening - stem removal - lubrication -
reassembly - testing

These tasks should be relaxing, not exhausting. Unlock batch tools
later.

------------------------------------------------------------------------

# KEYCAP SYSTEM

Keycaps are collectible and visually important.

Support: - profiles - materials - colorways - themed sets - novelty
keys - artisan keys - limited editions

Keycap placement should be physical and satisfying.

The final 3D keyboard must accurately show the player's actual choices.

------------------------------------------------------------------------

# SOUND DESIGN

Keyboard sound is a major gameplay pillar.

Different combinations of: - case - plate - foam - switch - keycap -
modifications

should affect sound.

Useful descriptors: - deep - creamy - muted - clacky - poppy - crisp -
hollow - loud - quiet - sharp

Players should be able to physically type on completed keyboards at a
sound-test station.

A customer might say:

> "I want a deep and quiet board."

The game should teach the player which combinations achieve that result.

Audio must feel rewarding enough that players enjoy pressing keys simply
for pleasure.

------------------------------------------------------------------------

# TESTING & QUALITY CONTROL

Every completed board goes through testing.

Possible faults: - missing switch - bent pin - defective switch - wrong
keycap - stabilizer rattle - incorrect layout - firmware/configuration
issue

Testing should be a satisfying final ritual rather than a spreadsheet.

Make failures easy to identify and pleasant to fix.

------------------------------------------------------------------------

# THE WORKBENCH

The workbench is the heart of the game.

Over time it may contain: - cutting mat - keycap puller - switch
puller - screwdriver set - lubricant - brushes - switch opener -
soldering station - parts trays - switch containers - spare keycaps -
microfiber cloth - labels - tape - packaging supplies - coffee mug -
personal decorations

Clutter should frame gameplay, never obstruct it.

------------------------------------------------------------------------

# SHOP PROGRESSION

## Early Game

-   tiny workshop
-   cheap workbench
-   limited inventory
-   basic tools
-   three switch families
-   local customers

## Mid Game

-   improved workbench
-   display shelves
-   larger inventory
-   premium switches
-   lubing tools
-   soldering
-   custom keycap options
-   online orders
-   better packaging

## Late Game

-   boutique shop
-   custom cases
-   fabrication access
-   own switch line
-   brand collaborations
-   streamer/professional orders
-   limited editions
-   collector customers
-   premium showroom

Progression must physically transform the environment. Do not hide all
progression inside menus.

------------------------------------------------------------------------

# POSSIBLE UPGRADES

Examples: - larger workbench - better lighting - component drawers -
switch organizer - switch tester - lubrication station - soldering
station - keycap display - inventory shelving - packaging station -
photography corner - sound-testing area - engraving machine - keycap
printer - CNC access - larger storage

Each upgrade should improve efficiency, customization, quality, capacity
or available job types.

------------------------------------------------------------------------

# ECONOMY & REPUTATION

Money matters, but the game should not feel punishing.

Income: - custom builds - repairs - modifications - premium
commissions - limited editions - later retail products

Expenses: - parts - shipping - tools - upgrades - replacement components

Track reputation separately.

Good work unlocks: - better customers - premium suppliers - special
commissions - collaborations - followers

Customer reviews must reference actual build choices and teach the
player why the result succeeded or failed.

------------------------------------------------------------------------

# SOCIAL / PHOTOGRAPHY

Allow players to photograph completed builds.

Optional choices: - angle - background - lighting - small props

Posting builds can produce: - followers - reputation - special orders -
collaborations

Keep this lightweight. It exists to celebrate finished keyboards.

------------------------------------------------------------------------

# REPAIRS & SPECIAL JOBS

Not every job should be a new build.

Possible jobs: - replace broken switch - clean old keyboard - repair
PCB - replace stabilizers - liquid-damage cleanup - restore retro
keyboard - replace keycaps - reduce noise - diagnose intermittent key -
modernize an old board

This creates variety while reusing the same interaction language.

------------------------------------------------------------------------

# ENVIRONMENTAL STORYTELLING

The shop should become more personal as the game progresses.

Examples: - customer thank-you cards - old keyboards - first premium
build displayed on a shelf - supplier stickers - shipping labels -
photos - notes - limited keycaps - failed prototypes - retro computer -
radio - plants - coffee mugs - packaging piles

Avoid meaningless prop spam.

Every decorative cluster should communicate keyboard culture, workshop
life, progression or personality.

------------------------------------------------------------------------

# UI DIRECTION

UI should not dominate the screen.

Prefer: - physical objects - paper/order cards - computer or tablet
interfaces - small contextual prompts - subtle highlights - diegetic
information

Avoid giant floating RPG panels.

Customer orders may arrive through an in-world computer, tablet, phone
or printed order slip.

------------------------------------------------------------------------

# INTERACTION FEEL

Every important action needs: - anticipation - response - sound -
micro-animation - clear completion state

Examples:

Switch: pick up -\> align -\> press -\> click -\> settle.

Keycap: align -\> press -\> pop -\> tiny bounce.

Drawer: grab -\> slide -\> stop -\> subtle object rattle.

Package: fold -\> tape -\> label -\> finished parcel.

Micro-feedback is more important than complicated animation.

------------------------------------------------------------------------

# TECHNICAL ARCHITECTURE

Keep systems modular and data-driven.

Prefer: - ScriptableObjects for part definitions - unique part IDs -
inventory separated from visual prefabs - customer request data model -
persistent KeyboardBuild data - compatibility validator - save/load
layer - generic interaction interfaces - sound-profile calculation -
progression/unlock system

Conceptual part definition:

KeyboardPartDefinition - id - displayName - category - price - rarity -
compatibilityTags - prefab - gameplayStats - soundProperties -
unlockRequirement

Do not hard-code individual keyboard products into gameplay scripts.

A keyboard should exist as structured data:

KeyboardBuild - case - pcb - plate - foam - stabilizers - switchMap -
keycapMap - optionalModules - tuningMods - calculatedSoundProfile -
calculatedTypingProfile - totalCost - saleValue - quality

The physical keyboard should render from this data.

------------------------------------------------------------------------

# DEVELOPMENT STRATEGY

Do **not** attempt the entire game immediately.

Build a vertical slice first.

## Vertical Slice

Create: - one small workshop - one workbench - one keyboard layout - one
case - one PCB - three switch types - three keycap sets - five customer
orders - one testing interaction - one packaging interaction - one shop
upgrade

Required complete loop:

**receive order -\> choose parts -\> install switches -\> install
keycaps -\> test -\> fix -\> package -\> deliver -\> get paid -\>
upgrade**

Only expand after this loop feels genuinely satisfying.

------------------------------------------------------------------------

# FIRST PLAYABLE QUALITY BAR

The first playable must prove:

1.  Installing switches feels satisfying.
2.  Installing keycaps feels satisfying.
3.  A completed keyboard feels meaningfully different based on chosen
    parts.
4.  The shop feels cozy even with a small number of assets.
5.  Customer requests create interesting decisions.
6.  The player wants to build "just one more keyboard."

If these six points do not work, do not add more systems yet.

------------------------------------------------------------------------

# IMPLEMENTATION RULES FOR THE CODING AGENT

When working on this project:

1.  Read this file before major implementation decisions.
2.  Preserve the visual and gameplay direction above.
3.  Prefer small modular systems over giant manager scripts.
4.  Keep data separate from presentation.
5.  Do not silently replace physical gameplay with menu interactions.
6.  Do not add unrelated mechanics merely to increase scope.
7.  Do not import an asset pack and treat its default look as final art
    direction.
8.  Maintain a coherent stylized visual language.
9.  Prototype the tactile interaction first.
10. Test after each meaningful system.
11. Avoid rewriting stable systems without a concrete reason.
12. Document important architectural decisions.
13. Use placeholder art when necessary, but build it to the same
    approximate proportions and interaction scale as final assets.
14. Keep the project playable throughout development.
15. Favor polish of the core loop over feature count.

------------------------------------------------------------------------

# FINAL CREATIVE TEST

Before adding any feature, ask:

**Does this make owning and operating a cozy custom keyboard workshop
more satisfying?**

If the answer is no, it is probably outside the core vision.

The finished game should make players want to sit at the workbench,
listen to the room ambience, open a box of switches, build a beautiful
keyboard and enjoy the sound of the finished board.
