using UnityEngine;
using System;

public static class TransformExtensions
{
	public static ZTweener MoveTo (this Transform t, Vector3 position)
	{
		return MoveTo (t, position, ZTweener.DefaultDuration);
	}
	public static ZTweener MoveTo (this Transform t, Vector3 position, float duration)
	{
		return MoveTo (t, position, duration, ZTweener.DefaultEquation);
	}
	public static ZTweener MoveTo (this Transform t, Vector3 position, float duration, Func<float, float, float, float> equation)
	{
		TransformPositionZTweener ZTweener = t.gameObject.AddComponent<TransformPositionZTweener> ();
		ZTweener.startValue = t.position;
		ZTweener.endValue = position;
		ZTweener.easingControl.duration = duration;
		ZTweener.easingControl.equation = equation;
		ZTweener.easingControl.Play ();
		return ZTweener;
	}
	public static ZTweener MoveToLocal (this Transform t, Vector3 position)
	{
		return MoveToLocal (t, position, ZTweener.DefaultDuration);
	}
	public static ZTweener MoveToLocal (this Transform t, Vector3 position, float duration)
	{
		return MoveToLocal (t, position, duration, ZTweener.DefaultEquation);
	}
	public static ZTweener MoveToLocal (this Transform t, Vector3 position, float duration, Func<float, float, float, float> equation)
	{
		TransformLocalPositionZTweener ZTweener = t.gameObject.AddComponent<TransformLocalPositionZTweener> ();
		ZTweener.startValue = t.localPosition;
		ZTweener.endValue = position;
		ZTweener.easingControl.duration = duration;
		ZTweener.easingControl.equation = equation;
		ZTweener.easingControl.Play ();
		return ZTweener;
	}
	public static ZTweener ScaleTo (this Transform t, Vector3 scale)
	{
		return ScaleTo (t, scale, ZTweener.DefaultDuration);
	}
	public static ZTweener ScaleTo (this Transform t, Vector3 scale, float duration)
	{
		return ScaleTo (t, scale, duration, ZTweener.DefaultEquation);
	}
	public static ZTweener ScaleTo (this Transform t, Vector3 scale, float duration, Func<float, float, float, float> equation)
	{
		TransformScaleZTweener ZTweener = t.gameObject.AddComponent<TransformScaleZTweener> ();
		ZTweener.startValue = t.localScale;
		ZTweener.endValue = scale;
		ZTweener.easingControl.duration = duration;
		ZTweener.easingControl.equation = equation;
		ZTweener.easingControl.Play ();
		return ZTweener;
	}
}
