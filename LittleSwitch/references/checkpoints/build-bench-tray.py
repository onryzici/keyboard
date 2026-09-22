import bpy,math,os
from mathutils import Vector
bpy.ops.object.select_all(action='SELECT');bpy.ops.object.delete(use_global=False)
out='/Users/trexoinnovation/Desktop/keyboard/LittleSwitch/Assets/LittleSwitch/Art/BenchProps';os.makedirs(out,exist_ok=True)
def mat(n,c):
 m=bpy.data.materials.new(n);m.diffuse_color=(*c,1);return m
cream=mat('Tray cream enamel',(.68,.66,.53));blue=mat('Tray blue rubber',(.19,.31,.31));brass=mat('Tray brass',(.57,.43,.22));steel=mat('Puller steel',(.46,.54,.53));teal=mat('Puller grip',(.22,.38,.36))
def cube(n,loc,scale,m,bevel):
 bpy.ops.mesh.primitive_cube_add(size=1,location=loc);o=bpy.context.object;o.name=n;o.dimensions=scale;bpy.ops.object.transform_apply(location=False,rotation=False,scale=True);o.data.materials.append(m);mod=o.modifiers.new('Soft manufactured edge','BEVEL');mod.width=bevel;mod.segments=4;bpy.context.view_layer.objects.active=o;bpy.ops.object.modifier_apply(modifier=mod.name);o.modifiers.new('Weighted corner normals','WEIGHTED_NORMAL');return o
def curve(n,points,width,m):
 c=bpy.data.curves.new(n,'CURVE');c.dimensions='3D';c.bevel_depth=width;c.bevel_resolution=3;p=c.splines.new('POLY');p.points.add(len(points)-1)
 for a,b in zip(p.points,points):a.co=(*b,1)
 o=bpy.data.objects.new(n,c);bpy.context.collection.objects.link(o);o.data.materials.append(m);return o
# Z is up in Blender. Centered origin at underside, game parts keep same footprint.
cube('Cast enamel tray base',(0,0,.033),(.92,.98,.066),cream,.05)
cube('Inset nonslip liner',(0,0,.071),(.78,.83,.022),blue,.055)
for side in [-1,1]:
 cube('Raised side lip',(side*.425,0,.104),(.07,.90,.15),cream,.028)
 cube('Rounded end lip',(0,side*.445,.104),(.85,.07,.15),cream,.028)
for y in [-.076,.076]:cube('Low compartment divider',(0,y,.099),(.75,.012,.034),cream,.006)
cube('Small brass rim inset',(0,-.455,.184),(.20,.027,.007),brass,.006)
bpy.ops.object.select_all(action='SELECT');bpy.ops.export_scene.fbx(filepath=out+'/CompartmentTray.fbx',use_selection=True,axis_forward='-Z',axis_up='Y',bake_anim=False)
bpy.ops.object.select_all(action='SELECT');bpy.ops.object.delete(use_global=False)
# Spring switch puller: a broad rounded U grip with two inward bent jaws.
pts=[]
for i in range(33):
 a=i/32*math.pi;pts.append((math.cos(a)*.065,math.sin(a)*.085,.015))
curve('Coated spring loop',pts,.021,teal)
for s in [-1,1]:curve('Stainless spring jaw',[(s*.065,0,.015),(s*.062,-.19,.015),(s*.048,-.24,.012),(s*.025,-.24,.006)],.008,steel)
bpy.ops.object.select_all(action='SELECT');bpy.ops.object.convert(target='MESH');bpy.ops.export_scene.fbx(filepath=out+'/SwitchPuller.fbx',use_selection=True,axis_forward='-Z',axis_up='Y',bake_anim=False)
