"""Original beveled workshop accessories and dished keycap mesh; no external inputs."""
import os
exec(open(os.path.join(os.path.dirname(__file__),'build_walkable_workshop.py'),encoding='utf-8').read().split('# Architectural shell:')[0])
OUT=os.path.join(ROOT,'LittleSwitch','Assets','WorkshopRevision','Models')
# A compact routed timber tray with actual recessed compartments and brass feet.
box('Tray base',(0,.012,0),(.36,.024,.19),'HoneyWood',.008)
for x in [-.175,.175]:box('Raised rim',(x,.031,0),(.012,.04,.19),'HoneyWood',.005)
for z in [-.09,.09]:box('Raised rim',(0,.031,z),(.35,.04,.012),'HoneyWood',.005)
for x in [-.055,.055]:box('Compartment divider',(x,.026,0),(.008,.028,.17),'HoneyWood',.003)
for i in range(7):
 x=-.125+(i%2)*.035;z=-.05+(i//2)*.027
 cyl('Spare screw',(x,.031,z),.008,.013,'Steel',16)
 rod('Screw slot',(x-.005,.039,z),(x+.005,.039,z),.0015,'Rubber')
for x in [.02,.10]:ring('Stabilizer wire loop',(x,.028,0),.025,.002,'Steel')
export('RoutedPartsTray')
# Coiled braided USB lead, shaped strain reliefs and metal connector ends.
points=[]
for i in range(161):
 t=i/160*math.pi*2*5
 points.append((math.cos(t)*.065,.008+i*.00006,math.sin(t)*.052))
tube('Coiled USB cable',points,.004,'Teal')
tube('Cable tail',[points[-1],(.10,.01,0),(.135,.01,.035)],.004,'Teal')
box('USB plug body',(.15,.01,.035),(.035,.018,.018),'Rubber',.004)
box('USB metal tip',(.175,.01,.035),(.025,.013,.013),'Steel',.002)
export('CoiledKeyboardCable')
# Sculpted, dished cap in keyboard local units, with rounded skirt and shoulder.
OUT=os.path.join(ROOT,'LittleSwitch','Assets','Resources','Keyboard');os.makedirs(OUT,exist_ok=True)
verts=[];faces=[];N=32
for y,w,d in [(0,.145,.141),(.022,.158,.153),(.155,.15,.146),(.207,.134,.13),(.219,.122,.118),(.210,.080,.076),(.201,.028,.025)]:
 for i in range(N):
  a=2*math.pi*i/N;c=math.cos(a);s=math.sin(a)
  # Rounded superellipse perimeter; real shape rather than a box primitive.
  x=w*math.copysign(abs(c)**.38,c);z=d*math.copysign(abs(s)**.38,s)
  verts.append(v((x,y,z)))
for ringid in range(6):
 for i in range(N):a=ringid*N+i;b=ringid*N+(i+1)%N;faces.append((a,b,b+N,a+N))
verts.append(v((0,.199,0)))
for i in range(N):faces.append((6*N+i,6*N+(i+1)%N,len(verts)-1))
faces.append(tuple(reversed(range(N))))
mesh=bpy.data.meshes.new('Dished keycap mesh');mesh.from_pydata(verts,[],faces);mesh.update()
o=bpy.data.objects.new('Dished keycap',mesh);bpy.context.collection.objects.link(o);o.data.materials.append(mats['Cream'])
for p in mesh.polygons:p.use_smooth=True
export('SculptedKeycap')
