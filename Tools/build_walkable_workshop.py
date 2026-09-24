"""Original Little Switch environment kit. Blender 5.1; authored in metre units.
Run: blender --background --python Tools/build_walkable_workshop.py
Inputs use Unity axes; all bevels, joinery, recesses and UVs are exported to FBX.
"""
import bpy, math, os, random
from mathutils import Vector
random.seed(17)
ROOT=os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
OUT=os.path.join(ROOT,'LittleSwitch','Assets','WorkshopWalk','Models')
os.makedirs(OUT,exist_ok=True)
bpy.ops.wm.read_factory_settings(use_empty=True)
palette={'HoneyWood':(.48,.29,.14),'DarkWood':(.22,.13,.08),'Plaster':(.72,.69,.59),'Sage':(.30,.40,.34),'Metal':(.10,.14,.14),'Steel':(.36,.40,.38),'Cream':(.79,.76,.65),'Terracotta':(.54,.27,.17),'Paper':(.79,.72,.55),'Rubber':(.035,.047,.044),'Glass':(.43,.59,.55),'Glow':(1,.78,.44),'Teal':(.17,.34,.33),'Floor':(.39,.32,.24),'Screw':(.35,.31,.22),'Leaf':(.27,.38,.22)}
mats={}
for n,c in palette.items():
 m=bpy.data.materials.new(n);m.diffuse_color=(*c,1);mats[n]=m
def v(p):return (-p[0],-p[2],p[1])
def box(n,p,s,m='Metal',b=.015):
 bpy.ops.mesh.primitive_cube_add(size=1,location=v(p));o=bpy.context.object;o.name=n;o.dimensions=(s[0],s[2],s[1]);bpy.ops.object.transform_apply(location=False,rotation=False,scale=True);o.data.materials.append(mats[m])
 if b:
  be=o.modifiers.new('Soft manufactured edge','BEVEL');be.width=min(b,min(s)*.4);be.segments=3
  o.modifiers.new('Weighted corner normals','WEIGHTED_NORMAL')
 return o
def cyl(n,p,r,h,m='Metal',segments=24):
 bpy.ops.mesh.primitive_cylinder_add(vertices=segments,radius=r,depth=h,location=v(p));o=bpy.context.object;o.name=n;o.data.materials.append(mats[m]);be=o.modifiers.new('Rolled edge','BEVEL');be.width=min(.008,r*.15);be.segments=3;o.modifiers.new('Weighted normals','WEIGHTED_NORMAL');return o
def rod(n,a,b,r=.018,m='Metal'):
 aa,bb=Vector(v(a)),Vector(v(b));o=cyl(n,tuple((Vector(a)+Vector(b))*.5),r,(bb-aa).length,m);o.rotation_mode='QUATERNION';o.rotation_quaternion=(bb-aa).to_track_quat('Z','Y');return o
def tube(n,points,r=.012,m='Rubber'):
 curve=bpy.data.curves.new(n,'CURVE');curve.dimensions='3D';curve.resolution_u=12;curve.bevel_depth=r;curve.bevel_resolution=3
 spl=curve.splines.new('BEZIER');spl.bezier_points.add(len(points)-1)
 for bp,p in zip(spl.bezier_points,points):bp.co=Vector(v(p));bp.handle_left_type='AUTO';bp.handle_right_type='AUTO'
 o=bpy.data.objects.new(n,curve);bpy.context.collection.objects.link(o);o.data.materials.append(mats[m]);return o
def ring(n,p,r,t=.008,m='Steel',vertical=False):
 bpy.ops.mesh.primitive_torus_add(major_segments=32,minor_segments=8,location=v(p),major_radius=r,minor_radius=t);o=bpy.context.object;o.name=n;o.data.materials.append(mats[m]);
 if vertical:o.rotation_euler.x=math.pi/2
 return o
def export(n):
 objects=list(bpy.context.scene.objects)
 for o in objects:
  bpy.context.view_layer.objects.active=o;o.select_set(True)
  if o.type=='CURVE':bpy.ops.object.convert(target='MESH')
  if o.type=='MESH':
   for mod in list(o.modifiers):
    try:bpy.ops.object.modifier_apply(modifier=mod.name)
    except RuntimeError:pass
  o.select_set(False)
 # Combine by material so the detailed kit has few renderers / draw calls.
 groups={}
 for o in list(bpy.context.scene.objects):groups.setdefault(o.data.materials[0].name,[]).append(o)
 for name,obs in groups.items():
  bpy.ops.object.select_all(action='DESELECT')
  for o in obs:o.select_set(True)
  bpy.context.view_layer.objects.active=obs[0];bpy.ops.object.join();o=bpy.context.object;o.name=n+'_'+name
  bpy.ops.object.transform_apply(location=True,rotation=True,scale=True)
  bpy.ops.object.mode_set(mode='EDIT');bpy.ops.mesh.select_all(action='SELECT');bpy.ops.uv.smart_project(angle_limit=math.radians(66),island_margin=.02);bpy.ops.object.mode_set(mode='OBJECT')
 bpy.ops.export_scene.fbx(filepath=os.path.join(OUT,n+'.fbx'),object_types={'MESH'},use_mesh_modifiers=True,add_leaf_bones=False,bake_anim=False,axis_forward='-Z',axis_up='Y',bake_space_transform=True)
 bpy.ops.object.select_all(action='SELECT');bpy.ops.object.delete(use_global=False)
 print('EXPORTED '+n,flush=True)
def feet(w,d,h):
 for x in [-w/2+.08,w/2-.08]:
  for z in [-d/2+.08,d/2-.08]:
   box('Welded box section leg',(x,h/2,z),(.065,h,.065),'Metal',.008);box('Rubber leveling foot',(x,.025,z),(.10,.05,.10),'Rubber')
  box('End frame rail',(x,.20,0),(.065,.06,d-.10),'Metal')
 box('Back stretcher',(0,.22,d/2-.08),(w-.15,.055,.055),'Metal')
def top(w,d,h):
 box('Thick end grain worktop',(0,h-.035,0),(w,.07,d),'HoneyWood',.025)
 box('Front steel apron',(0,h-.15,-d/2+.10),(w-.17,.17,.035),'Metal')
 for x in [-w/2+.12,w/2-.12]:
  for z in [-d/2+.12,d/2-.12]:cyl('Countersunk worktop fixing',(x,h+.001,z),.014,.003,'Screw')
def drawer(x,y,z,w=.5):
 box('Drawer housing',(x,y,z),(w,.18,.50),'Metal');box('Folded drawer face',(x,y,z-.265),(w-.035,.15,.032),'Sage')
 rod('Drawer pull',(x-.105,y,z-.295),(x+.105,y,z-.295),.012,'Steel')
 box('Label pocket',(x,y-.037,z-.286),(.11,.022,.003),'Paper',.003)

# Architectural shell: large window recesses, entrance, recessed panels and beams.
for ix in range(16):
 x=-5.35+(ix+.5)*10.7/16
 for iz in range(5):
  z=-4.15+(iz+.5)*8.3/5
  box('Warm plank floor',(x,-.035,z),(10.7/16-.009,.07,8.3/5-.008),'Floor',.008)
box('North masonry',(0,1.8,4.34),(11.1,3.6,.28),'Plaster',.035)
box('East masonry',(5.54,1.8,0),(.28,3.6,8.4),'Plaster',.035)
# West window openings span z -1.6 .. 2.7 and y 1.12 .. 2.85.
box('West sill wall',(-5.54,.56,0),(.28,1.12,8.4),'Plaster')
box('West lintel',(-5.54,3.25,0),(.28,.78,8.4),'Plaster')
for z,depth in [(-2.91,2.58),(3.48,1.44),(.493,.30)]:box('Thick window pier',(-5.54,1.99,z),(.28,1.74,depth),'Plaster')
for z,d in [(-.64,1.97),(1.7,2.12)]:
 for yy in [1.12,2.87]:box('Deep timber window rail',(-5.48,yy,z),(.37,.11,d+.15),'DarkWood')
 for zz in [z-d/2,z+d/2]:box('Recessed timber jamb',(-5.48,1.995,zz),(.32,1.84,.11),'DarkWood')
 box('Rounded deep window sill',(-5.33,1.11,z),(.64,.10,d+.25),'HoneyWood',.025)
 box('Slender iron window mullion',(-5.58,2,z),(.055,1.69,.04),'Metal',.009)
 box('Slender iron window transom',(-5.58,2.4,z),(.055,.04,d),'Metal',.009)
# Entrance opening, closed modeled door in front wall.
for x,w in [(-4.80,1.4),(1.40,8.2)]:box('Front plaster',(x,1.8,-4.34),(w,3.6,.28),'Plaster')
box('Entrance lintel',(-3.4,2.95,-4.34),(1.4,1.3,.28),'Plaster')
for x in [-4.04,-2.76]:box('Door jamb',(x,1.15,-4.17),(.12,2.3,.22),'DarkWood')
box('Door head',(-3.4,2.3,-4.17),(1.39,.13,.22),'DarkWood')
box('Sage receiving door',(-3.4,1.12,-4.29),(1.15,2.2,.075),'Sage',.02)
for yy in [.48,1.25,1.84]:box('Raised door panel',(-3.4,yy,-4.24),(.89,.40,.035),'Sage',.018)
rod('Brass door handle',(-3.01,1,-4.17),(-2.89,1,-4.17),.02,'Screw')
box('Entry stone threshold',(-3.4,.025,-4.05),(1.35,.05,.42),'Steel')
# Low plinths, upper cornice, structural posts, visible joinery.
for x in [-5.34,5.34]:
 box('Wall baseboard',(x,.10,0),(.10,.2,8.3),'DarkWood')
 box('Low dado rail',(x,1.02,0),(.09,.085,8.3),'HoneyWood')
for z in [-4.15,4.15]:box('Back plinth',(0,.10,z),(10.7,.2,.11),'DarkWood')
for x in [-5.22,-2.7,1.0,5.22]:
 for z in [-4.06,4.06]:
  box('Timber structural upright',(x,1.73,z),(.18,3.46,.18),'DarkWood',.02)
  box('Iron post shoe',(x,.19,z),(.195,.30,.195),'Metal')
  for yy in [.16,.27,3.24]:rod('Visible iron coach bolt',(x-.06,yy,z-.10),(x-.06,yy,z-.12),.016,'Screw')
for z in [-3.8,0,3.8]:
 box('Ceiling cross beam',(0,3.43,z),(10.7,.23,.20),'DarkWood',.025)
 for x in [-5.1,5.1]:rod('Angled timber knee brace',(x,2.85,z),(x-math.copysign(.65,x),3.37,z),.065,'DarkWood')
for x in [-3.6,0,3.6]:box('Ceiling joist',(x,3.53,0),(.11,.12,8.5),'HoneyWood')
box('Plaster ceiling',(0,3.68,0),(11.1,.16,8.8),'Plaster')
# Services concentrated on electronics wall.
tube('Exposed tidy conduit',[(5.32,1.2,-2.6),(5.32,2.95,-2.6),(5.32,3.12,-2.4),(5.32,3.12,3.5)],.021,'Steel')
box('Electrical consumer unit',(5.29,1.62,-2.7),(.18,.48,.34),'Metal')
for z in [-2.80,-2.71,-2.62]:box('Fuse bank',(5.18,1.61,z),(.04,.10,.06),'Cream')
for y in [.55,1.5,2.6]:box('Conduit saddle',(5.29,y,-2.6),(.08,.025,.08),'Metal')
export('WorkshopShell')

feet(3.2,1.05,.84);top(3.2,1.05,.94)
for y in [.38,.58,.78]:drawer(-1.05,y,.05,.68)
box('Under bench shelf',(.43,.23,.08),(1.68,.045,.72),'HoneyWood')
for x in [-.3,1.40]:box('Bench vise mounting plate',(x,.95,.40),(.14,.012,.12),'Steel')
export('MainWorkbench')

feet(1.9,.72,.83);top(1.9,.72,.91)
for y in [.42,.62,.78]:drawer(.55,y,.04,.52)
box('Instrument upstand',(0,1.20,.36),(1.9,.12,.08),'Metal')
box('Instrument shelf',(0,1.33,.36),(1.94,.05,.34),'HoneyWood')
export('ElectronicsBench')

feet(1.6,.8,.69);top(1.6,.8,.77)
drawer(.46,.54,.03,.5);box('Notebook lower shelf',(-.43,.20,.08),(.65,.04,.55),'HoneyWood')
export('OrderDesk')

feet(1.75,.80,.83);top(1.75,.80,.91)
box('Packing roll spindle',(0,1.27,.31),(1.5,.025,.025),'Steel')
for x in [-.76,.76]:rod('Roll holder',(x,.92,.31),(x,1.27,.31),.022,'Metal')
rod('Kraft paper roll',(-.67,1.27,.31),(.67,1.27,.31),.09,'Paper')
box('Open packing shelf',(0,.25,.05),(1.57,.04,.6),'HoneyWood')
export('PackingBench')

# Detailed riveted storage rack, thin folded metal and wood liners.
for x in [-.72,.72]:
 for z in [-.27,.27]:
  box('Perforated angle upright',(x,1.06,z),(.045,2.12,.045),'Metal',.005)
  for y in [.18,.55,.92,1.29,1.66,2.03]:rod('Shelf bolt',(x-.026,y,z),(x+.026,y,z),.009,'Screw')
for y in [.16,.60,1.04,1.48,1.94]:
 box('Folded shelf tray',(0,y,0),(1.46,.045,.58),'Sage',.009)
 box('Shelf wood liner',(0,y+.027,0),(1.38,.012,.51),'HoneyWood',.004)
box('Tall rear brace',(0,1.10,.28),(1.38,.08,.025),'Metal')
rod('Diagonal rack brace',(-.70,.20,.29),(.70,2,.29),.018,'Metal')
export('StorageRack')

box('Organizer chassis',(0,.24,0),(.75,.48,.30),'Metal')
for row in range(3):
 for col in range(5):
  x=(col-2)*.142;y=.085+row*.15
  box('Component drawer',(x,y,-.018),(.134,.135,.29),'Sage',.01)
  box('Cream drawer face',(x,y,-.169),(.126,.117,.018),'Cream',.009)
  box('Finger pull',(x,y+.025,-.187),(.062,.023,.024),'Metal',.007)
  box('Contents index label',(x,y-.02,-.182),(.073,.029,.003),'Paper',.002)
export('ComponentDrawers')

# Tool board: real gaps between perforated strips, dark recessed hole disks.
box('Tool board oak frame',(0,.58,.025),(2.85,1.2,.08),'DarkWood')
box('Enamel perforated panel',(0,.58,-.027),(2.74,1.10,.022),'Teal',.008)
for row in range(12):
 for col in range(31):
  o=cyl('Recessed perforation',((col-15)*.084,.115+row*.084,-.041),.008,.002,'Rubber',12);o.rotation_euler.x=math.pi/2
for x in [-1.28,1.28]:
 for y in [.08,1.08]:rod('Pegboard mounting screw',(x,y,-.047),(x,y,-.054),.017,'Screw')
box('Lower tool ledge',(0,.03,-.12),(2.81,.05,.30),'HoneyWood')
for x in [-1,-.75,-.5,-.25,0,.25,.5,.75,1]:rod('Tool peg',(x,.68,-.045),(x,.68,-.115),.010,'Steel')
export('ToolWall')

# Open storage bin with sloping front lip, modeled thickness and label pocket.
box('Bin base',(0,.015,0),(.32,.03,.38),'Sage')
for x in [-.151,.151]:box('Bin side',(x,.105,0),(.018,.19,.38),'Sage',.006)
box('Bin back',(0,.105,.182),(.32,.19,.016),'Sage')
box('Sloping front lip',(0,.055,-.184),(.32,.09,.018),'Sage')
box('Index card',(0,.052,-.196),(.13,.034,.003),'Paper',.002)
export('OpenPartsBin')

# Clear organizer jar, separate lid and switches inside.
cyl('Transparent jar',(0,.09,0),.075,.16,'Glass',32);cyl('Jar screw lid',(0,.18,0),.080,.035,'Cream',32)
for i in range(10):
 a=i*2.4;r=.045*(i%3)/2
 box('Visible switch',(math.sin(a)*r,.04+(i//3)*.03,math.cos(a)*r),(.035,.023,.035),'Cream',.004)
 box('Switch stem',(math.sin(a)*r,.055+(i//3)*.03,math.cos(a)*r),(.012,.014,.012),'Terracotta',.003)
export('SwitchJar')

box('Solder station',(0,.10,0),(.32,.20,.25),'Sage',.035)
box('Control fascia',(0,.105,-.133),(.28,.145,.018),'Metal',.025)
box('Digital status display',(-.055,.135,-.147),(.12,.048,.008),'Glow',.008)
o=cyl('Temperature dial',(.075,.08,-.148),.030,.028,'Steel');o.rotation_euler.x=math.pi/2
for x in [-.12,-.09,-.06,-.03,0,.03,.06,.09,.12]:box('Station cooling vent',(x,.16,.128),(.012,.065,.01),'Rubber',.002)
tube('Iron silicone cable',[(.14,.07,0),(.26,.014,-.1),(.40,.017,-.25),(.48,.10,-.07)],.008)
rod('Solder iron handle',(.40,.11,.12),(.48,.10,-.07),.021,'Teal');rod('Solder iron tip',(.40,.11,.12),(.36,.11,.22),.006,'Steel')
box('Iron stand',(.42,.025,.12),(.18,.045,.23),'Metal')
for z in [.07,.1,.13,.16]:ring('Coiled heat guard',(.41,.10,z),.031,.006,'Steel',True)
export('SolderingStation')

box('Rubber meter bumper',(0,.024,0),(.13,.045,.22),'Terracotta',.025)
box('Meter face',(0,.05,0),(.106,.012,.19),'Cream',.012)
box('LCD screen',(0,.059,.053),(.079,.005,.048),'Sage',.007)
cyl('Range selector',(0,.068,-.035),.027,.015,'Metal')
for x in [-.03,.03]:cyl('Lead socket',(x,.059,-.08),.008,.009,'Rubber')
tube('Red test lead',[(-.03,.06,-.08),(-.10,.02,-.19),(-.2,.02,-.11),(-.20,.022,.14)],.004,'Terracotta')
tube('Black test lead',[(.03,.06,-.08),(.08,.02,-.18),(.19,.02,-.09),(.19,.022,.16)],.004,'Rubber')
export('Multimeter')

feet(1.6,.72,.83);top(1.6,.72,.91)
box('Spray booth rear',(0,1.32,.31),(1.28,.80,.045),'Sage')
for x in [-.65,.65]:box('Spray booth side',(x,1.32,.09),(.035,.80,.47),'Sage')
box('Extractor hood',(0,1.76,.12),(1.36,.11,.52),'Metal')
for y in [1.18+i*.055 for i in range(8)]:box('Filter grille',(0,y,.272),(.86,.017,.018),'Metal',.004)
rod('Ventilation duct',(0,1.80,.27),(0,2.35,.27),.105,'Steel')
for y in [1.88,2.07,2.26]:ring('Duct rolled seam',(0,y,.27),.107,.008,'Metal')
export('PaintStation')

for i in range(6):
 x=(i%3-1)*.082;z=(i//3-.5)*.09
 cyl('Paint bottle',(x,.055,z),.03,.10,['Sage','Terracotta','Cream','Teal','Steel','Paper'][i])
 cyl('Paint nozzle',(x,.118,z),.016,.034,'Cream');box('Bottle paper label',(x,.056,z-.029),(.036,.035,.003),'Paper',.003)
export('PaintBottles')

box('Drying rack foot',(0,.025,0),(.56,.05,.32),'Metal')
for x in [-.24,.24]:rod('Drying rack side',(x,.05,.10),(x,.61,.10),.016,'Steel')
for y in [.15,.30,.45,.60]:
 box('Drying grate',(0,y,0),(.53,.016,.29),'Steel',.004)
 box('Keyboard sample shell',(0,y+.025,0),(.38,.025,.13),['Cream','Teal','Terracotta','Sage'][int(round(y/.15))-1],.016)
export('DryingRack')

# Apron is an original shaped cloth mesh, not a rigid rectangle.
verts=[(-.17,0,.68),(.17,0,.68),(.21,0,.42),(.31,0,.08),(.29,0,0),(-.29,0,0),(-.31,0,.08),(-.21,0,.42)]
mesh=bpy.data.meshes.new('Tailored apron cloth');mesh.from_pydata([v((x,y,z)) for x,z,y in verts],[],[tuple(range(8))]);mesh.update();o=bpy.data.objects.new('Canvas work apron',mesh);bpy.context.collection.objects.link(o);o.data.materials.append(mats['Terracotta']);sol=o.modifiers.new('Heavy canvas thickness','SOLIDIFY');sol.thickness=.006
box('Apron front pocket',(0,.23,-.012),(.29,.18,.02),'Terracotta',.012)
tube('Apron neck loop',[(-.14,.67,0),(-.14,.84,0),(.14,.84,0),(.14,.67,0)],.012,'DarkWood')
export('WorkApron')

# Rolling island cart adds a middle-distance layer without blocking the route.
for x in [-.38,.38]:
 for z in [-.23,.23]:
  o=cyl('Rubber caster',(x,.065,z),.057,.035,'Rubber');o.rotation_euler.y=math.pi/2
  box('Cart upright',(x,.43,z),(.035,.68,.035),'Metal')
for y in [.17,.48,.80]:box('Lipped cart tray',(0,y,0),(.86,.055,.56),'Sage')
tube('Push handle',[(-.4,.80,-.22),(-.50,.91,-.22),(-.50,.91,.22),(-.4,.80,.22)],.018,'Steel')
export('ServiceCart')

# Original keyboard display with inset plate and actual beveled key geometry.
box('Keyboard case',(0,.021,0),(.44,.042,.16),'Cream',.018)
box('Recessed dark plate',(0,.044,0),(.418,.012,.143),'Metal',.01)
for row in range(5):
 for col in range(14):
  if row==0 and col in range(4,10):continue
  box('Sculpted keycap',((col-6.5)*.029,.058,(row-2)*.027),(.026,.019,.023),'Teal' if col==0 or row==4 else 'Cream',.004)
box('Space bar',(0,.058,-.054),(.172,.019,.023),'Cream',.004)
export('DisplayKeyboard')

# Physical fixture with spun metal shade, stem and inner diffuser.
rod('Pendant suspension',(0,.15,0),(0,.52,0),.008,'Rubber')
bpy.ops.mesh.primitive_cone_add(vertices=48,radius1=.23,radius2=.09,depth=.15,location=v((0,.075,0)));o=bpy.context.object;o.name='Spun enamel pendant';o.data.materials.append(mats['Sage']);be=o.modifiers.new('Rolled shade edge','BEVEL');be.width=.012;be.segments=3
cyl('Warm opal diffuser',(0,.008,0),.211,.009,'Glow',48);ring('Shade rolled lip',(0,.005,0),.23,.009,'Metal')
export('PendantLamp')

# Rounded exterior tree canopy, smoothed sufficiently for window-distance silhouettes.
rod('Tree trunk',(0,0,0),(0,2.5,0),.09,'DarkWood')
for i in range(7):
 a=i*2.4;p=(math.sin(a)*.54,2.15+(i%3)*.38,math.cos(a)*.54)
 rod('Tree branch',(0,1.7,0),p,.035,'DarkWood')
 bpy.ops.mesh.primitive_uv_sphere_add(segments=16,ring_count=8,location=v(p));o=bpy.context.object;o.name='Rounded stylized foliage';o.scale=(.74,.65,.74);o.data.materials.append(mats['Leaf'])
 for poly in o.data.polygons:poly.use_smooth=True
export('CourtyardTree')
print('WORKSHOP_KIT_COMPLETE',flush=True)

box('Solid oak display shelf',(0,-.025,0),(1.65,.05,.29),'HoneyWood')
for x in [-.65,.65]:
 rod('Shelf bracket',(x,-.20,.11),(x,-.05,-.11),.017,'Metal')
 box('Wall fixing plate',(x,-.13,.13),(.055,.25,.025),'Metal')
export('WallShelf')

for j in range(3):
 x=(j-1)*.12
 tube('Braided cable loop',[(x,.40,0),(x-.045,.26,-.005),(x-.055,.07,-.015),(x,.015,-.02),(x+.055,.07,-.015),(x+.045,.26,-.005),(x,.40,0)],.009,['Terracotta','Rubber','Teal'][j])
 box('Cable connector',(x+.015,.32,-.02),(.022,.05,.018),'Steel',.004)
export('CableLoops')

box('Brush rail',(0,.02,.025),(.39,.04,.05),'HoneyWood')
for i in range(5):
 x=(i-2)*.068
 rod('Brush handle',(x,.02,0),(x,.19+i%2*.035,0),.009,'DarkWood')
 box('Brush ferrule',(x,.20+i%2*.035,0),(.028,.045,.014),'Steel',.004)
 box('Soft brush bristles',(x,.24+i%2*.035,0),(.029,.04,.015),'Paper',.003)
export('BrushSet')

ring('Packing tape outer roll',(0,.027,0),.065,.024,'Terracotta')
ring('Cardboard tape core',(0,.027,0),.041,.005,'Paper')
box('Tape dispenser',(0,.023,-.12),(.10,.045,.17),'Metal')
box('Cutting serration',(0,.064,-.195),(.094,.015,.012),'Steel')
export('PackingTape')

box('Framed notice board',(0,.38,0),(.73,.76,.045),'DarkWood')
box('Cork inset',(0,.38,-.028),(.67,.70,.018),'Paper')
for i in range(4):
 x=(i%2-.5)*.31;y=.21+i//2*.33
 box('Pinned workshop print',(x,y,-.044),(.26,.27,.005),'Cream',.004)
 for j in range(4):box('Printed schematic trace',(x-.04+j*.026,y,-.049),(.009,.15-j*.02,.002),'Teal',.001)
 rod('Pin',(x,y+.10,-.048),(x,y+.10,-.059),.009,'Terracotta')
export('NoticeBoard')

box('Coat hook backplate',(0,.05,0),(.55,.10,.035),'HoneyWood')
for x in [-.2,0,.2]:tube('Coat hook',[(x,.05,-.02),(x,.02,-.08),(x,.08,-.09)],.012,'Steel')
export('CoatHooks')

box('Paper stack',(0,.012,0),(.27,.024,.19),'Cream',.004)
for i in range(5):box('Packing slip print',(-.055,.025,-.05+i*.021),(.14,.001,.004),'Teal',.001)
export('Paperwork')

box('Courtyard paving',(0,-.06,0),(23,.12,32),'Plaster',.03)
for z in range(-15,16,2):box('Paving joint',(-8,.003,z),(.012,.006,1.9),'Metal',.001)
box('Low planted border',(-9,.18,0),(.28,.36,30),'Sage',.04)
export('CourtyardGround')
