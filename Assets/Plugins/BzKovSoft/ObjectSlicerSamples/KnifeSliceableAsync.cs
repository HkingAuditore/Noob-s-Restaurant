﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BzKovSoft.ObjectSlicer;
using System.Diagnostics;

namespace BzKovSoft.ObjectSlicerSamples
{
	/// <summary>
	/// This script will invoke slice method of IBzSliceableAsync interface if knife slices this GameObject.
	/// The script must be attached to a GameObject that have rigidbody on it and
	/// IBzSliceable implementation in one of its parent.
	/// </summary>
	[DisallowMultipleComponent]
	public class KnifeSliceableAsync : MonoBehaviour
	{
		IBzSliceableAsync _sliceableAsync;
		
		void Start()
		{
			_sliceableAsync = GetComponentInParent<IBzSliceableAsync>();
		}

		void OnTriggerEnter(Collider other)
		{
			var knife = other.gameObject.GetComponent<BzKnife>();
			if (knife == null)
				return;

			StartCoroutine(Slice(knife));
		}

		private IEnumerator Slice(BzKnife knife)
		{
			// The call from OnTriggerEnter, so some object positions are wrong.
			// We have to wait for next frame to work with correct values
			yield return null;

			Vector3 point = GetCollisionPoint(knife);
			Vector3 normal = Vector3.Cross(knife.MoveDirection, knife.BladeDirection);
			Plane plane = new Plane(normal, point);

			if (_sliceableAsync != null)
			{
				_sliceableAsync.Slice(plane, knife.SliceID, null);
			}
		}

		private Vector3 GetCollisionPoint(BzKnife knife)
		{
			Vector3 distToObject = transform.position - knife.Origin;
			Vector3 proj = Vector3.Project(distToObject, knife.BladeDirection);

			Vector3 collisionPoint = knife.Origin + proj;
			return collisionPoint;
		}
	}
}