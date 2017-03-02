using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

/* I am not using this */

public class SceneLoader : MonoBehaviour 
{
	[SerializeField] private string [] Scenes;

	private WaitForEndOfFrame mWaitForEndOfFrame;

	public bool Loading { get; private set; }

	void Awake()
	{
		mWaitForEndOfFrame = new WaitForEndOfFrame();
        Loading = true;
        StartCoroutine(LoadScenes());
    }

	private IEnumerator LoadScenes()
	{
		for( int count = 0; count < Scenes.Length; count++ )
		{
            AsyncOperation ao = SceneManager.LoadSceneAsync(Scenes[count], LoadSceneMode.Additive);
			if( ao != null )
			{
				while( !ao.isDone )
				{
					yield return mWaitForEndOfFrame;
				}
			}
		}

		yield return mWaitForEndOfFrame;

		Loading = false;
	}
}
