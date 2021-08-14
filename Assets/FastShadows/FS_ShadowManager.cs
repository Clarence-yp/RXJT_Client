using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//[ExecuteInEditMode()] 
public class FS_ShadowManager : MonoBehaviour {
    private static FS_ShadowManager _manager;
	private Hashtable shadowMeshes = new Hashtable();
	private Hashtable shadowMeshesStatic = new Hashtable();
	
	void Start(){
		FS_ShadowManager[] ms = (FS_ShadowManager[]) FindObjectsOfType(typeof(FS_ShadowManager));
		if (ms.Length > 1){
			Debug.LogWarning("There should only be one FS_ShadowManger in the scene. Found " + ms.Length);	
		}
	}
	
	void OnApplicationQuit(){
		shadowMeshes.Clear();
		shadowMeshesStatic.Clear();
	}
	
	//Singleton, returns this manager.
    public static FS_ShadowManager Manager (){
        if (_manager == null) {
			FS_ShadowManager sm = (FS_ShadowManager) FindObjectOfType(typeof(FS_ShadowManager));
			if (sm == null){
            	GameObject go = new GameObject("FS_ShadowManager");
				_manager = go.AddComponent<FS_ShadowManager>();
			} else {
				_manager = sm;	
			}
        }
        return _manager;
    }
	
	public void registerGeometry(FS_ShadowSimple s, FS_MeshKey meshKey){
		FS_ShadowManagerMesh m;
		if (meshKey.isStatic){
			if (!shadowMeshesStatic.ContainsKey(meshKey)){
				GameObject g = new GameObject("ShadowMeshStatic_" + meshKey.mat.name);
				g.transform.parent = transform;
				m = g.AddComponent<FS_ShadowManagerMesh>();
				m.shadowMaterial = s.shadowMaterial;
				m.isStatic = true;
				shadowMeshesStatic.Add(meshKey,m);				
			} else {
				m = (FS_ShadowManagerMesh) shadowMeshesStatic[meshKey];	
			}
		} else {
			if (!shadowMeshes.ContainsKey(meshKey)){
				GameObject g = new GameObject("ShadowMesh_" + meshKey.mat.name);
				g.transform.parent = transform;
				m = g.AddComponent<FS_ShadowManagerMesh>();
				m.shadowMaterial = s.shadowMaterial;
				m.isStatic = false;
				shadowMeshes.Add(meshKey,m);
			} else {
				m = (FS_ShadowManagerMesh) shadowMeshes[meshKey];	
			}
		}
		m.registerGeometry(s);		
	}
	
	int frameCalcedFustrum = 0;
	Plane[] fustrumPlanes;
	public Plane[] getCameraFustrumPlanes(){
		if (Time.frameCount != frameCalcedFustrum || fustrumPlanes == null){
			Camera mc = Camera.main;
			if (mc == null){
				Debug.LogWarning("No main camera could be found for visibility culling.");	
				fustrumPlanes = null;
			} else {
				fustrumPlanes = GeometryUtility.CalculateFrustumPlanes(mc);
				frameCalcedFustrum = Time.frameCount;
			}
		}
		return fustrumPlanes;
	}
}
