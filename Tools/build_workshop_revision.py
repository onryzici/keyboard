"""Original compact timber workshop shell. Coordinates in metres; legacy gameplay uses 4 units/m."""
import os
source=os.path.join(os.path.dirname(__file__),'build_walkable_workshop.py')
helpers=open(source,encoding='utf-8').read().split('# Architectural shell:')[0]
helpers=helpers.replace(" bpy.ops.export_scene.fbx(",""" # Architectural grain uses metric planar UVs, independent of packed mesh islands.
 for obj in bpy.context.scene.objects:
  layer=obj.data.uv_layers.active
  for face in obj.data.polygons:
   faceNormal=face.normal
   for li in face.loop_indices:
    co=obj.data.vertices[obj.data.loops[li].vertex_index].co
    layer.data[li].uv=(co.x*.6,co.y*.6) if abs(faceNormal.z)>.6 else ((co.y*.6,co.z*.6) if abs(faceNormal.x)>.6 else (co.x*.6,co.z*.6))
 bpy.ops.export_scene.fbx(""")
exec(helpers)
OUT=os.path.join(ROOT,'LittleSwitch','Assets','WorkshopRevision','Models');os.makedirs(OUT,exist_ok=True)

# Continuous construction under individually beveled, staggered floor boards.
box('Solid subfloor',(0,-.06,-2.48),(5.2,.10,6.25),'DarkWood')
for i in range(23):
 x=-2.52+i*.225
 z=-5.53
 first=.55 if i%2 else 1.08
 j=0
 while z<.575:
  length=min(first if j==0 else 1.10,.59-z)
  box('Hand finished oak floorboard',(x,-.011,z+length/2),(.220,.04,length-.006),'HoneyWood',.005)
  z+=length;j+=1
export('CraftedFloor')

# Cream plaster infill and solid walls, a west-facing recessed window.
box('Rear lime plaster',(0,1.64,.64),(5.22,3.28,.17),'Plaster',.025)
box('Right lime plaster',(2.62,1.64,-2.48),(.18,3.28,6.4),'Plaster',.025)
box('West below window',(-2.62,.6075,-2.48),(.18,1.215,6.4),'Plaster')
box('West above window',(-2.62,2.945,-2.48),(.18,.79,6.4),'Plaster')
box('West front pier',(-2.62,1.885,-3.45),(.18,1.34,4.38),'Plaster')
box('West back pier',(-2.62,1.885,.62),(.18,1.34,.18),'Plaster')
for x,w in [(-1.76,1.72),(1.45,2.34)]:box('Entrance plaster',(x,1.64,-5.64),(w,3.28,.18),'Plaster')
box('Door lintel',(-.31,2.74,-5.64),(1.23,1.08,.18),'Plaster')
box('Warm plaster ceiling',(0,3.32,-2.48),(5.4,.12,6.6),'Plaster')
export('CraftedPlaster')

# Thick rounded timber, sparse supports frame the workbench rather than slicing it.
for x in [-2.49,2.49]:
 for z in [-5.46,-2.70,.45]:
  box('Hand shaped timber upright',(x,1.61,z),(.16,3.22,.17),'DarkWood',.024)
  box('Timber post foot',(x,.15,z),(.21,.30,.22),'DarkWood',.025)
for z in [-5.45,-3.85,-2.25,-.65,.45]:
 box('Rounded ceiling crossbeam',(0,3.17,z),(5.13,.20,.17),'DarkWood',.025)
 for x in [-2.45,2.45]:rod('Timber corner brace',(x,2.80,z),(x-math.copysign(.40,x),3.16,z),.07,'DarkWood')
for x in [-1.5,0,1.5]:box('Warm ceiling joist',(x,3.24,-2.48),(.075,.09,6.20),'HoneyWood',.012)
for x in [-2.50,2.50]:
 box('Solid timber baseboard',(x,.14,-2.48),(.075,.28,6.12),'DarkWood',.016)
 box('Dado cap',(x,.98,-2.48),(.095,.07,6.12),'HoneyWood')
 for z in [-5.3+i*.14 for i in range(42)]:box('Vertical oak wainscot',(x,.56,z),(.05,.70,.134),'HoneyWood',.007)
box('Rear timber picture rail',(0,2.58,.515),(5.0,.09,.09),'DarkWood')
for z in [-1.265,.535]:box('Recessed window jamb',(-2.53,1.88,z),(.23,1.42,.085),'DarkWood',.018)
for y in [1.215,2.55]:box('Recessed window rail',(-2.53,y,-.365),(.23,.10,1.91),'DarkWood',.018)
box('Rounded deep sill',(-2.45,1.20,-.365),(.39,.075,2.03),'HoneyWood',.021)
box('Fine timber mullion',(-2.62,1.88,-.365),(.065,1.25,.035),'DarkWood',.008)
box('Window transom',(-2.62,2.18,-.365),(.065,.035,1.72),'DarkWood',.008)
for x in [-.94,.32]:box('Door timber casing',(x,1.13,-5.48),(.10,2.26,.17),'DarkWood')
box('Door carved header',(-.31,2.26,-5.48),(1.37,.13,.17),'DarkWood')
box('Warm timber entry door',(-.31,1.10,-5.55),(1.13,2.18,.075),'Sage',.024)
for y in [.55,1.52]:box('Recessed door panel',(-.31,y,-5.50),(.91,.79,.032),'Sage',.020)
rod('Door brass lever',(.10,1.05,-5.45),(.21,1.05,-5.45),.017,'Screw')
export('CraftedTimber')

# Proper original lamp, emitting shade faces DOWN, articulated arm leans over the mat.
cyl('Weighted round lamp foot',(0,.025,0),.12,.05,'Metal',40)
rod('Lamp lower arm',(0,.05,0),(.045,.27,0),.018,'Steel')
rod('Lamp upper arm',(.045,.27,0),(.20,.40,0),.018,'Steel')
for p in [(0,.07,0),(.045,.27,0),(.20,.40,0)]:
 o=cyl('Lamp hinge',p,.03,.036,'Screw');o.rotation_euler.x=math.pi/2
bpy.ops.mesh.primitive_cone_add(vertices=40,radius1=.11,radius2=.045,depth=.09,location=v((.20,.35,0)))
o=bpy.context.object;o.data.materials.append(mats['Sage']);be=o.modifiers.new('Rolled lamp edge','BEVEL');be.width=.008;be.segments=3
cyl('Downward opal lamp diffuser',(.20,.303,0),.101,.006,'Glow',40)
tube('Lamp cable',[(0,.03,.04),(-.09,.01,.09),(-.12,.01,.23)],.004,'Rubber')
export('CorrectTaskLamp')
print('CRAFTED_WORKSHOP_COMPLETE')
