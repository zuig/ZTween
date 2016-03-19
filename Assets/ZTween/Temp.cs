using UnityEngine;
using System.Collections;
using System;

public class Temp : MonoBehaviour {
	void Start () {
		ZTweener ZTweener = transform.MoveTo( new Vector3(5, 0, 0), 3f, EasingEquations.EaseInOutQuad );
		ZTweener.easingControl.loopCount = -1;
		ZTweener.easingControl.loopType = EasingControl.LoopType.PingPong;
	}
}
