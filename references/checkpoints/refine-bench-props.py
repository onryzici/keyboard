import bpy,bmesh,os,json
from mathutils import Vector
out='/Users/trexoinnovation/Desktop/keyboard/LittleSwitch/Assets/LittleSwitch/ThirdParty/Refined'
def load(p):
 bpy.ops.wm.read_factory_settings(use_empty=True);bpy.ops.import_scene.gltf(filepath=p)
def export(name,width):
 for o in list(bpy.context.scene.objects):
  if o.type!='MESH':continue
  bpy.context.view_layer.objects.active=o;o.select_set(True);bpy.ops.object.transform_apply(location=False,rotation=False,scale=True)
  bm=bmesh.new();bm.from_mesh(o.data);bmesh.ops.remove_doubles(bm,verts=list(bm.verts),dist=.00001);bm.to_mesh(o.data);bm.free()
  be=o.modifiers.new('Rounded edges','BEVEL');be.width=width;be.segments=3
  no=o.modifiers.new('Weighted normals','WEIGHTED_NORMAL');no.keep_sharp=True
  for p in o.data.polygons:p.use_smooth=True
 bpy.ops.export_scene.fbx(filepath=out+'/'+name+'.fbx',object_types={'MESH'},bake_anim=False,axis_forward='-Z',axis_up='Y')
load('/tmp/clock2.glb')
for o in list(bpy.context.scene.objects):
 if o.type!='MESH':continue
 bm=bmesh.new();bm.from_mesh(o.data);indices=[i for i,m in enumerate(o.data.materials) if m.name=='mat24'];bmesh.ops.delete(bm,geom=[f for f in bm.faces if f.material_index in indices],context='FACES');bm.to_mesh(o.data);bm.free()
export('BrunoDeskClock',.002)
load('/tmp/kenney-furniture/Models/GLTF format/cardboardBoxOpen.glb')
export('OpenComponentCarton',.006)
load('/tmp/little-switch-import/office/mug.glb')
for o in bpy.context.scene.objects:
 if o.type=='MESH':print('MUG',o.name,tuple(o.dimensions),[(m.name,list(m.diffuse_color)) for m in o.data.materials])
export('GlazedWorkshopMug',.008)
