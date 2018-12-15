using UnityEngine;
using System.Collections;

public class RenderQueueChanger : MonoBehaviour
{
    public Material _mat;
    public int _renderQueue;
    public MeshRenderer _meshRender;

	void Start()
	{
		if (_meshRender != null) _mat = _meshRender.materials[0];
		if (_mat != null)
		{
			_mat.renderQueue = _renderQueue;
		}
	}

    void Update()
    {
        /*
	    if (Time.timeSinceLevelLoad > 0)
	    {
	        Destroy(this);
	    }
         * */
    }
}
