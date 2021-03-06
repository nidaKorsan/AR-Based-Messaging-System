using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NativeCamera;

public class ImagePrefabManager : MonoBehaviour
{
    private void Awake()
    {
		if (NativeCamera.IsCameraBusy())
			return;
		TakePicture(512);

	}
    // Start is called before the first frame update
    void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		gameObject.transform.LookAt(2 * gameObject.transform.position - Camera.main.transform.position, Vector3.up);
	}
	//   /Dahili depolama/DCIM/Screenshots
	private void TakePicture(int maxSize)
	{
		NativeCamera.Permission permission = NativeCamera.TakePicture((path) =>
		{
			textManager.current.write("Image path: " + path);
			if (path != null)
			{
				// Create a Texture2D from the captured image
				Texture2D texture = NativeCamera.LoadImageAtPath(path, maxSize);
				if (texture == null)
				{
					Debug.Log("Couldn't load texture from " + path);
					return;
				}

				gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x, gameObject.transform.localScale.y * texture.height / (float)texture.width, gameObject.transform.localScale.z);
				gameObject.GetComponent<Renderer>().material.mainTexture = texture;
			
			}
		}, maxSize);

		textManager.current.write("Permission result: " + permission);
	}
}
