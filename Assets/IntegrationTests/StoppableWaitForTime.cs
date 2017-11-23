using UnityEngine;
using System.Collections;

namespace Shopify.Unity.Tests {
	public class StoppableWaitForTime : CustomYieldInstruction {
		public override bool keepWaiting {
			get {
				return !_IsStopped && Time.time - StartTime < _MaxTime;
			}
		}

		public float DurationToStop {
			get {
				return StopTime - StartTime;
			}
		}

		public bool IsStopped {
			get {
				return _IsStopped;
			}
		}

		float _MaxTime;
		bool _IsStopped;

		float StopTime;
		float StartTime;

		public StoppableWaitForTime (float maxTime) {
			_MaxTime = maxTime;
			StartTime = Time.time;
		}

		public void Stop() {
			_IsStopped = true;
			StopTime = Time.time;
		}
	}
}

