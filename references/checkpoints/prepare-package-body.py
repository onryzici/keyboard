exec(open('/tmp/refine-bench-props.py').read().split("load('/tmp/clock2.glb')")[0])
load('/tmp/kenney-furniture/Models/GLTF format/cardboardBoxOpen.glb')
for o in bpy.context.scene.objects:
 if o.type!='MESH':continue
 bm=bmesh.new();bm.from_mesh(o.data);bmesh.ops.remove_doubles(bm,verts=list(bm.verts),dist=.00001);bm.verts.ensure_lookup_table();bm.verts.index_update()
 seen=set();remove=[]
 for f in bm.faces:
  key=tuple(sorted(v.index for v in f.verts))
  if key in seen or any(v.co.z>.222 for v in f.verts):remove.append(f)
  seen.add(key)
 bmesh.ops.delete(bm,geom=remove,context='FACES');bmesh.ops.recalc_face_normals(bm,faces=list(bm.faces));bm.to_mesh(o.data);bm.free()
 bpy.context.view_layer.objects.active=o;mod=o.modifiers.new('Thick folded cardboard','SOLIDIFY');mod.thickness=.004;mod.offset=0;bpy.ops.object.modifier_apply(modifier=mod.name)
export('ComponentPackageBody',.0015)
