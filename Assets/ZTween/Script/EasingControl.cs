using UnityEngine;
using System.Collections;
using System;

public class EasingControl : MonoBehaviour {
	public event EventHandler updateEvent;
	public event EventHandler stateChangeEvent;
	public event EventHandler completedEvent;
	public event EventHandler loopedEvent;

	public enum TimeType
	{
		/// <summary>
		/// 代表普通更新循环，只在每帧的结束，根据Unity返回的Time.deltaTime增加速度（这意味着将time scale设置为0，就可以暂停所有动画）
		/// </summary>
		Normal,
		/// <summary>
		/// 在time scale为0时动画依然可以播放
		/// </summary>
		Real,
		/// <summary>
		/// 更新循环和物理引擎绑定
		/// </summary>
		Fixed,
	};
	
	/// <summary>
	/// 查询控制器的状态
	/// </summary>
	public enum PlayState
	{
		Stopped,
		Paused,
		Playing,
		Reversing,
	};
	
	/// <summary>
	/// 当动画完成时，我们可能想让物体维持原地的位置或者重置到起始位置
	/// </summary>
	public enum EndBehaviour
	{
		Constant,
		Reset,
	};
	
	/// <summary>
	/// 循环动画，可能会想要在每次循环时重置状态，或者回放到起始状态
	/// </summary>
	public enum LoopType
	{
		Repeat,
		PingPong,
	};

	public TimeType timeType = TimeType.Normal;
	public PlayState playState { get; private set; }
	public PlayState previousPlayState { get; private set; }
	public EndBehaviour endBehaviour = EndBehaviour.Constant;
	public LoopType loopType = LoopType.Repeat;
	public bool IsPlaying { get { return playState == PlayState.Playing || playState == PlayState.Reversing; }}

	public float startValue = 0.0f;
	public float endValue = 1.0f;
	public float duration = 1.0f;
	public int loopCount = 0;
	public Func<float, float, float, float> equation = EasingEquations.Linear;

	public float currentTime { get; private set; }
	public float currentValue { get; private set; }
	public float currentOffset { get; private set; }
	public int loops { get; private set; }

	void OnEnable ()
	{
		Resume();
	}
	
	void OnDisable ()
	{
		Pause();
	}

	void SetPlayState (PlayState target)
	{
		if (playState == target)
			return;
		
		previousPlayState = playState;
		playState = target;
		
		if (stateChangeEvent != null)
			stateChangeEvent(this, EventArgs.Empty);
		
		StopCoroutine("Ticker");
		if (IsPlaying)
			StartCoroutine("Ticker");
	}

	IEnumerator Ticker ()
	{
		while (true)
		{
			switch (timeType)
			{
			case TimeType.Normal:
				yield return new WaitForEndOfFrame();
				Tick(Time.deltaTime);
				break;
			case TimeType.Real:
				yield return new WaitForEndOfFrame();
				Tick(Time.unscaledDeltaTime);
				break;
			default: // Fixed
				yield return new WaitForFixedUpdate();
				Tick(Time.fixedDeltaTime);
				break;
			}
		}
	}

	void Tick (float time)
	{
		bool finished = false;
		if (playState == PlayState.Playing)
		{
			currentTime = Mathf.Clamp01( currentTime + (time / duration));
			finished = Mathf.Approximately(currentTime, 1.0f);
		}
		else // Reversing
		{
			currentTime = Mathf.Clamp01( currentTime - (time / duration));
			finished = Mathf.Approximately(currentTime, 0.0f);
		}
		
		float frameValue = (endValue - startValue) * equation (0.0f, 1.0f, currentTime) + startValue;
		currentOffset = frameValue - currentValue;
		currentValue = frameValue;
		
		if (updateEvent != null)
			updateEvent(this, EventArgs.Empty);
		
		if (finished)
		{
			++loops;
			if (loopCount < 0 || loopCount >= loops) 
			{
				if (loopType == LoopType.Repeat) 
					SeekToBeginning();
				else // PingPong
					SetPlayState( playState == PlayState.Playing ? PlayState.Reversing : PlayState.Playing );
				
				if (loopedEvent != null)
					loopedEvent(this, EventArgs.Empty);
			} 
			else
			{
				if (completedEvent != null)
					completedEvent(this, EventArgs.Empty);
				
				Stop ();
			}
		}
	}

	public void Play ()
	{
		SetPlayState(PlayState.Playing);
	}
	
	public void Reverse ()
	{
		SetPlayState(PlayState.Reversing);
	}
	
	public void Pause ()
	{
		SetPlayState(PlayState.Paused);
	}
	
	public void Resume ()
	{
		SetPlayState(previousPlayState);
	}
	
	public void Stop ()
	{
		SetPlayState(PlayState.Stopped);
		loops = 0;
		if (endBehaviour == EndBehaviour.Reset)
			SeekToBeginning ();
	}
	
	public void SeekToTime (float time)
	{
		currentTime = Mathf.Clamp01(time / duration);
		float newValue = (endValue - startValue) * currentTime + startValue;
		currentOffset = newValue - currentValue;
		currentValue = newValue;
		
		if (updateEvent != null)
			updateEvent(this, EventArgs.Empty);
	}
	
	public void SeekToBeginning ()
	{
		SeekToTime(0.0f);
	}
	
	public void SeekToEnd ()
	{
		SeekToTime(duration);
	}
}
