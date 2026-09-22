using UnityEngine;using UnityEditor;using UnityEditor.SceneManagement;using System.Collections.Generic;using System.Linq;
public static class WorkshopRoomFinish {
 public static void Apply(){
 var root=GameObject.Find("Spacious textured workshop").transform;var material=GameObject.Find("Rear wall below window").GetComponent<Renderer>().sharedMaterial;
 foreach(var n in new[]{"Rear wall below window","Rear wall above window","Rear left pier","Rear right pier"})GameObject.Find(n).SetActive(false);
 var vertices=new List<Vector3>();var normals=new List<Vector3>();var uv=new List<Vector2>();var triangles=new List<int>();
 System.Action<Vector3,Vector3,Vector3,Vector3,Vector3> quad=(a,b,c,d,n)=>{int i=vertices.Count;vertices.AddRange(new[]{a,b,c,d});normals.AddRange(new[]{n,n,n,n});foreach(var v in new[]{a,b,c,d})uv.Add(new Vector2(v.x,v.y)/3f);triangles.AddRange(new[]{i,i+1,i+2,i,i+2,i+3});};
 float[] xx={-10.2f,-5.33f,1.83f,10.2f};float[] yy={0,4.86f,10.2f,13.5f};float front=6.24f,back=6.66f;
 for(int x=0;x<3;x++)for(int y=0;y<3;y++){if(x==1&&y==1)continue;quad(new Vector3(xx[x],yy[y],front),new Vector3(xx[x],yy[y+1],front),new Vector3(xx[x+1],yy[y+1],front),new Vector3(xx[x+1],yy[y],front),Vector3.back);quad(new Vector3(xx[x+1],yy[y],back),new Vector3(xx[x+1],yy[y+1],back),new Vector3(xx[x],yy[y+1],back),new Vector3(xx[x],yy[y],back),Vector3.forward);}
 quad(new Vector3(-5.33f,4.86f,front),new Vector3(-5.33f,4.86f,back),new Vector3(-5.33f,10.2f,back),new Vector3(-5.33f,10.2f,front),Vector3.right);
 quad(new Vector3(1.83f,10.2f,front),new Vector3(1.83f,10.2f,back),new Vector3(1.83f,4.86f,back),new Vector3(1.83f,4.86f,front),Vector3.left);
 var mesh=new Mesh{name="Continuous masonry with open window"};mesh.SetVertices(vertices);mesh.SetNormals(normals);mesh.SetUVs(0,uv);mesh.SetTriangles(triangles,0);mesh.RecalculateBounds();mesh.RecalculateTangents();AssetDatabase.CreateAsset(mesh,"Assets/LittleSwitch/Art/ContinuousWorkshopWall.asset");var wall=new GameObject("Continuous plaster wall",typeof(MeshFilter),typeof(MeshRenderer));wall.transform.SetParent(root);wall.GetComponent<MeshFilter>().sharedMesh=mesh;wall.GetComponent<Renderer>().sharedMaterial=material;
 foreach(string name in new[]{"Left plaster return","Right plaster return"}){var g=GameObject.Find(name);g.transform.localScale=new Vector3(1,13.5f/9.2f,1);var p=g.transform.position;g.transform.position=new Vector3(Mathf.Sign(p.x)*10.2f,6.75f,p.z);}
 GameObject.Find("Left baseboard").transform.position=new Vector3(-9.96f,.22f,.2f);GameObject.Find("Right baseboard").transform.position=new Vector3(9.96f,.22f,.2f);GameObject.Find("Rear baseboard").transform.localScale=new Vector3(20.0f/16.55f,1,1);
 GameObject.Find("Oak floor").transform.localScale=new Vector3(20.5f/17.2f,1,1);
 GameObject.Find("Upper timber cornice").transform.position=new Vector3(0,13.33f,6.12f);GameObject.Find("Upper timber cornice").transform.localScale=new Vector3(20.2f/16.7f,1,1);
 foreach(Transform t in root){if(t.name=="Window jamb"){t.localScale=new Vector3(1,5.47f/3.38f,1);t.position=new Vector3(t.position.x,7.53f,t.position.z);}if(t.name=="Window header")t.position=new Vector3(t.position.x,10.2f,t.position.z);if(t.name=="Window center mullion"){t.localScale=new Vector3(1,5.15f/3.05f,1);t.position=new Vector3(t.position.x,7.53f,t.position.z);}if(t.name=="Window transom")t.position=new Vector3(t.position.x,8.65f,t.position.z);if(t.name.StartsWith("Exterior ·"))t.position+=new Vector3(0,0,14f);}
 // A real ceiling closes the room instead of exposing the background above the walls.
 var ceiling=GameObject.CreatePrimitive(PrimitiveType.Cube);ceiling.name="Plaster ceiling";ceiling.transform.SetParent(root);ceiling.transform.position=new Vector3(0,13.62f,.2f);ceiling.transform.localScale=new Vector3(20.8f,.25f,13.4f);ceiling.GetComponent<Renderer>().sharedMaterial=material;
 EditorSceneManager.MarkSceneDirty(wall.scene);EditorSceneManager.SaveScene(wall.scene);AssetDatabase.SaveAssets();
 }
}
