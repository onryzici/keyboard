using System.Collections.Generic; using UnityEngine; using TMPro;
namespace LittleSwitch {
public static class SoftShapes {
 static Dictionary<string,Material> mats=new Dictionary<string,Material>();
 public static Color C(string hex){ColorUtility.TryParseHtmlString("#"+hex,out var c);return c;}
 public static Material Mat(string hex){if(mats.TryGetValue(hex,out var m)&&m)return m;m=new Material(Shader.Find("Universal Render Pipeline/Lit")){name="Mat_"+hex,color=C(hex)};m.SetFloat("_Smoothness",.18f);mats[hex]=m;return m;}
 public static Mesh Rounded(Vector3 size,float r){
  var v=new List<Vector3>();var n=new List<Vector3>();var uv=new List<Vector2>();var tris=new List<int>();int steps=6;r=Mathf.Min(r,Mathf.Min(size.x,Mathf.Min(size.y,size.z))*.49f);Vector3 inner=size*.5f-Vector3.one*r;
  Vector3[] normals={Vector3.right,Vector3.left,Vector3.up,Vector3.down,Vector3.forward,Vector3.back};
  foreach(var normal in normals){Vector3 a=new Vector3(normal.y,normal.z,normal.x),b=Vector3.Cross(normal,a);int start=v.Count;
   for(int y=0;y<=steps;y++)for(int x=0;x<=steps;x++){Vector3 p=normal*.5f+a*((float)x/steps-.5f)+b*((float)y/steps-.5f);p=Vector3.Scale(p,size);Vector3 q=new Vector3(Mathf.Clamp(p.x,-inner.x,inner.x),Mathf.Clamp(p.y,-inner.y,inner.y),Mathf.Clamp(p.z,-inner.z,inner.z));var dir=(p-q).normalized;v.Add(q+dir*r);n.Add(dir);uv.Add(new Vector2((float)x/steps,(float)y/steps));}
   for(int y=0;y<steps;y++)for(int x=0;x<steps;x++){int i=start+y*(steps+1)+x;tris.Add(i);tris.Add(i+1);tris.Add(i+steps+1);tris.Add(i+1);tris.Add(i+steps+2);tris.Add(i+steps+1);}
  }var mesh=new Mesh{name="SoftBox"};mesh.SetVertices(v);mesh.SetNormals(n);mesh.SetUVs(0,uv);mesh.SetTriangles(tris,0);mesh.RecalculateBounds();return mesh;
 }
 public static GameObject Box(string name,Transform parent,Vector3 pos,Vector3 size,string color,float radius=.06f,bool collider=false){var go=new GameObject(name);go.transform.SetParent(parent,false);go.transform.localPosition=pos;go.AddComponent<MeshFilter>().sharedMesh=Rounded(size,radius);go.AddComponent<MeshRenderer>().sharedMaterial=Mat(color);if(collider)go.AddComponent<BoxCollider>().size=size;return go;}
 public static GameObject Sphere(string name,Transform parent,Vector3 pos,Vector3 size,string color){var g=GameObject.CreatePrimitive(PrimitiveType.Sphere);g.name=name;g.transform.SetParent(parent,false);g.transform.localPosition=pos;g.transform.localScale=size;g.GetComponent<Renderer>().sharedMaterial=Mat(color);Object.DestroyImmediate(g.GetComponent<Collider>());return g;}
 public static GameObject Cyl(string name,Transform parent,Vector3 pos,Vector3 scale,string color){var g=GameObject.CreatePrimitive(PrimitiveType.Cylinder);g.name=name;g.transform.SetParent(parent,false);g.transform.localPosition=pos;g.transform.localScale=scale;g.GetComponent<Renderer>().sharedMaterial=Mat(color);Object.DestroyImmediate(g.GetComponent<Collider>());return g;}
 public static void Rod(Transform p,Vector3 a,Vector3 b,float width,string col){var g=Cyl("Stem",p,(a+b)*.5f,new Vector3(width,(b-a).magnitude*.5f,width),col);g.transform.up=b-a;}
 public static TextMeshPro Label(string text,Transform p,Vector3 pos,float size,string col,Vector3 rotation){var g=new GameObject("Label "+text);g.transform.SetParent(p,false);g.transform.localPosition=pos;g.transform.localEulerAngles=rotation;var t=g.AddComponent<TextMeshPro>();t.text=text;t.fontSize=size*10;t.color=C(col);t.alignment=TextAlignmentOptions.Center;t.rectTransform.sizeDelta=new Vector2(6,1);return t;}
}
}
