"""Create shallow bevelled brick relief as one mesh per existing wall panel.
Input is extracted from WorkshopShell_Plaster; openings and room dimensions stay intact.
"""
import json, math, struct
from pathlib import Path
import numpy as np

panels=json.load(open(Path(__file__).with_name('workshop-wall-panels.json')))
out=Path('/tmp/workshop-brick-meshes');out.mkdir(exist_ok=True)
count=0
for k,p in enumerate(panels):
    if not (abs(p['plane']-5.427)<.02 or abs(p['plane']-21.93)<.02 or abs(p['plane']+19.35)<.02 or abs(p['plane']+26.823)<.02):continue
    lo=np.array(p['lo']);hi=np.array(p['hi']);lo[1]=max(lo[1],4.2)
    if hi[1]<=lo[1]:continue
    axis=p['axis'];uaxis=2 if axis==0 else 0;normal=np.zeros(3);normal[axis]=p['sign']
    verts=[];norms=[];uv=[];tri=[];bw=4.3/4;bh=.30;bricks=0
    def point(u,v,d):
        q=np.zeros(3);q[uaxis]=u;q[1]=v;q[axis]=p['plane']+p['sign']*d;return q
    def quad(points):
        a,b,c,d=points;n=np.cross(b-a,c-a);n/=max(np.linalg.norm(n),1e-8)
        ix=len(verts)
        for q in points:verts.append(q);norms.append(n);uv.append([atlasL+(q[uaxis]-left)/(right-left)*(atlasR-atlasL),atlasB+(q[1]-bottom)/(top-bottom)*(atlasT-atlasB)])
        tri.extend([ix,ix+1,ix+2,ix,ix+2,ix+3])
    for row in range(math.floor(lo[1]/bh),math.ceil(hi[1]/bh)):
      offset=(row%2)*bw*.5
      for col in range(math.floor((lo[0]+offset)/bw),math.ceil((hi[0]+offset)/bw)):
        left=max(lo[0],col*bw-offset+.020);right=min(hi[0],(col+1)*bw-offset-.020)
        bottom=max(lo[1],row*bh+.020);top=min(hi[1],(row+1)*bh-.020)
        if right-left<.055 or top-bottom<.055:continue
        rng=np.random.default_rng((row*73856093+col*19349663+k*83492791)&0xffffffff)
        ar=int(rng.integers(0,20));ac=int(rng.integers(0,5));px=(34 if ar%2==0 else 119)+ac*170.5
        atlasL=(px+7)/1024;atlasR=(px+159)/1024;atlasB=1-(ar*51.2+43)/1024;atlasT=1-(ar*51.2+8)/1024
        depth=.049+rng.uniform(-.012,.012);bevel=min(.018,(right-left)/6,(top-bottom)/6)
        corners=np.array([[left,bottom],[right,bottom],[right,top],[left,top]])
        corners+=rng.uniform(-.003,.003,(4,2))
        center=corners.mean(0);front=corners+np.sign(center-corners)*bevel
        frontpts=[point(*c,depth+rng.uniform(-.003,.003)) for c in front]
        outerpts=[point(*c,depth-bevel) for c in corners];basepts=[point(*c,.001) for c in corners]
        # Keep the front winding toward the room on all four walls.
        if np.cross(frontpts[1]-frontpts[0],frontpts[2]-frontpts[0])@normal<0:
            frontpts.reverse();outerpts.reverse();basepts.reverse()
        quad(frontpts)
        for j in range(4):
            z=(j+1)%4
            quad([frontpts[z],frontpts[j],outerpts[j],outerpts[z]])
            quad([outerpts[z],outerpts[j],basepts[j],basepts[z]])
        bricks+=1
    if not verts:continue
    with (out/f'WallRelief{k:02}.bin').open('wb') as f:
        f.write(struct.pack('<i',len(verts)));f.write(np.asarray(verts,dtype='<f4').tobytes());f.write(np.asarray(norms,dtype='<f4').tobytes());f.write(np.asarray(uv,dtype='<f4').tobytes());f.write(struct.pack('<i',len(tri)));f.write(np.asarray(tri,dtype='<i4').tobytes())
    count+=bricks
    print(k,'bricks',bricks,'vertices',len(verts),'triangles',len(tri)//3)
print('total bricks',count)
