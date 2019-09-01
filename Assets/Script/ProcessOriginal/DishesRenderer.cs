using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[ExecuteInEditMode]
public class DishesRenderer : MonoBehaviour {

	public enum ProcessType {none=0,fry,stir,boil}
	private List<ProcessType> _ProcessTags = new List<ProcessType>();
    public Material[] _ProcessMaterials = new Material[4];


    private Material[] _UsedMaterials = new Material[10];
    //private int _NumofProcess = 0;

	public void AddProcess(ProcessType _processtype){
		this._ProcessTags.Add(_processtype);
        _UsedMaterials[(int)_ProcessTags.Count-1] = _ProcessMaterials[(int)_processtype];
        this.GetComponent<Renderer>().materials = _UsedMaterials;
	}

    public void RemoveProcess()
    {
        this._ProcessTags.RemoveAt(_ProcessTags.Count-1);
        _UsedMaterials[(int)_ProcessTags.Count] = null;
        this.GetComponent<Renderer>().materials = _UsedMaterials;
    }
}
