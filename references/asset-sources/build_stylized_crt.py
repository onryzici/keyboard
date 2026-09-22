import bpy,math
from mathutils import Vector
out='/Users/trexoinnovation/Desktop/keyboard/LittleSwitch/Assets/LittleSwitch/ThirdParty/Refined/RetroComputer.fbx'
bpy.ops.wm.read_factory_settings(use_empty=True)
def mat(n,c):
 m=bpy.data.materials.new(n);m.diffuse_color=(*c,1);return m
ivory=mat('Warm ivory casing',(.61,.54,.40));edge=mat('Bezel edge',(.37,.36,.28));screen=mat('CRT glass',(.035,.075,.07));slot=mat('Vent recess',(.12,.15,.13));sage=mat('Power LED',(.33,.7,.36));key=mat('Keycap cream',(.71,.66,.52));orange=mat('Warm function key',(.54,.3,.18))
def box(n,p,s,m,r=.035):
 bpy.ops.mesh.primitive_cube_add(size=1,location=p);o=bpy.context.object;o.name=n;o.dimensions=s;bpy.ops.object.transform_apply(location=False,rotation=False,scale=True);o.data.materials.append(m)
 be=o.modifiers.new('Crafted round edges','BEVEL');be.width=r;be.segments=4;o.modifiers.new('Weighted surface normals','WEIGHTED_NORMAL');return o
# Low desktop system, layered face plate and readable mechanical details.
box('Desktop base',(0,.10,.22),(1.92,1.42,.42),ivory,.08)
box('Front faceplate',(0,-.628,.22),(1.82,.06,.30),ivory,.045)
for z in [.22,.32]:box('Drive opening',(.36,-.667,z),(.68,.025,.045),slot,.01)
box('Floppy eject',(.67,-.691,.22),(.075,.022,.035),edge,.009)
box('Round power button',(-.75,-.672,.24),(.12,.03,.11),edge,.04)
box('Power indicator',(-.57,-.679,.25),(.037,.013,.028),sage,.01)
for x in [-.38,-.31,-.24,-.17,-.10]:box('Chassis vent',(x,-.674,.21),(.025,.018,.14),slot,.007)
box('Tilt stand',(0,.11,.52),(.64,.65,.19),edge,.065)
# Original tapered CRT shape, with thick continuous bevels.
verts=[]
for y,w,h in [(-.38,.86,.64),(.67,.61,.5)]:
 for x,z in [(-w,-h),(w,-h),(w,h),(-w,h)]:verts.append((x,y,z+1.24))
me=bpy.data.meshes.new('Tapered CRT shell');me.from_pydata(verts,[],[(0,3,2,1),(4,5,6,7),(0,1,5,4),(1,2,6,5),(2,3,7,6),(3,0,4,7)]);me.update();o=bpy.data.objects.new('Rounded CRT housing',me);bpy.context.collection.objects.link(o);o.data.materials.append(ivory);be=o.modifiers.new('Large soft bevels','BEVEL');be.width=.075;be.segments=4;o.modifiers.new('Weighted normals','WEIGHTED_NORMAL')
box('Inset monitor gasket',(0,-.429,1.29),(1.48,.045,1.02),edge,.12)
box('Curved CRT glass',(0,-.46,1.30),(1.34,.075,.87),screen,.13)
box('Lower monitor lip',(0,-.435,.675),(1.58,.105,.14),ivory,.035)
for i in range(3):box('Monitor adjustment key',(.45+i*.1,-.5,.68),(.054,.022,.032),edge,.01)
box('Monitor status lamp',(.71,-.494,.72),(.024,.015,.024),sage,.009)
for i in range(9):box('Side cooling fin',(.748-i*.002,.13+i*.045,1.19),(.023,.022,.35),slot,.005)
# Compact vintage keyboard on the desk, with geometry for each row.
box('Vintage keyboard',(0,-1.02,.10),(1.8,.66,.15),ivory,.055)
for row in range(5):
 for col in range(16):
  if row==0 and 4<=col<=10:continue
  m=orange if (row==4 and col%4==0) else key
  box('Vintage key',( -.79+col*.105,-1.26+row*.113,.204),(.09,.096,.072),m,.014)
box('Vintage spacebar',(-.05,-1.26,.204),(.73,.096,.072),key,.015)
bpy.ops.export_scene.fbx(filepath=out,use_selection=False,object_types={'MESH'},add_leaf_bones=False,bake_anim=False,axis_forward='-Z',axis_up='Y')
