using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LoadingBar : MonoBehaviour {
	private int angle = 0;
	private bool isLoading = false;

    public string[] strText;

    public GameObject loading;
    public Text guideText;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update() {
	}

	void OnEnable(){
		isLoading = true;
        StartCoroutine("StartAni");
    }

	void OnDisable(){
		isLoading = false;
	}

	void FixedUpdate () {
		if (isLoading) {
			angle += 3;
			if (angle >= 360) {
				angle = 0;
			}
			loading.transform.localRotation = Quaternion.Euler (0, 0, -angle);
		} else {
			angle = 0;
		}
	}

    IEnumerator StartAni()
    {
        int index = 0;
        while (true)
        {
            if (guideText != null)
            {
                guideText.text = strText[index % 3];
                index++;
            }
            yield return new WaitForSeconds(1.0f);
        }
    }
}
