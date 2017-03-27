﻿/**
Graph Tool

Copyright (c) 2017 Sokuhatiku

This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
*/

using UnityEngine;
using UnityEngine.UI;

namespace GraphTool
{

	public abstract class GraphPartsBase : MaskableGraphic,
		ILayoutElement,
		ICanvasRaycastFilter
	{
		[SerializeField]protected GraphHandler handler;
		protected Vector2 transration;
		protected Vector2 scale;
		protected Vector2 offset;

#if UNITY_EDITOR
		protected override void Reset()
		{
			base.Reset();
			handler = GetComponentInParent<GraphHandler>();
		}
#endif

		protected override void OnEnable()
		{
			base.OnEnable();
			handler = GetComponentInParent<GraphHandler>();
			if (handler != null)
			{
				handler.OnUpdateGraph += UpdateGraph;
				RecalculateScale();
			}
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			if (handler != null)
				handler.OnUpdateGraph -= UpdateGraph;
		}

		protected void UpdateGraph()
		{
			if (handler == null) return;
			RecalculateScale();
			SetVerticesDirty();
			OnUpdateGraph();
		}

		protected virtual void OnUpdateGraph() { }

		protected virtual void RecalculateScale()
		{
			var scopeRect = handler.ScopeRect;
			var tfRect = rectTransform.rect;
			var pivot = rectTransform.pivot;

			transration = -scopeRect.position;
			scale = new Vector2(
				tfRect.width / scopeRect.width,
				tfRect.height / scopeRect.height);
			offset = new Vector2(
				-pivot.x * tfRect.width,
				-pivot.y * tfRect.height);
		}

		protected Vector2 ScopeToRect(Vector2 point)
		{
			point += transration;
			point.Scale(scale);
			point += offset;
			return point;
		}

		protected float ScopeToRectX(float x)
		{
			return (x + transration.x) * scale.x + offset.x;
		}

		protected float ScopeToRectY(float y)
		{
			return (y + transration.y) * scale.y + offset.y;
		}

		protected Vector2 RectToScope(Vector2 point)
		{
			point -= offset;
			point = new Vector2(point.x / scale.x, point.y / scale.y);
			point -= transration;
			return point;
		}

		protected float RectToScopeX(float x)
		{
			return (x - offset.x) / scale.x - transration.x;
		}

		protected float RectToScopeY(float y)
		{
			return (y - offset.y) / scale.y - transration.y;
		}


		protected virtual void AddDot(VertexHelper vh, Vector3 pt, float radius)
		{
			AddDot(vh, pt, radius, color);
		}

		protected virtual void AddDot(VertexHelper vh, Vector3 pt, float radius, Color32 color)
		{
			if (radius <= 0) return;
			vh.AddVert(new Vector3(pt.x, pt.y - radius, -pt.z), color, Vector2.zero);
			vh.AddVert(new Vector3(pt.x - radius, pt.y, -pt.z), color, Vector2.zero);
			vh.AddVert(new Vector3(pt.x, pt.y + radius, -pt.z), color, Vector2.zero);
			vh.AddVert(new Vector3(pt.x + radius, pt.y, -pt.z), color, Vector2.zero);

			var vertId = vh.currentVertCount - 1;
			vh.AddTriangle(vertId - 3, vertId - 2, vertId - 1);
			vh.AddTriangle(vertId - 1, vertId, vertId - 3);
		}

		protected virtual void AddLine(VertexHelper vh, Vector3 from, Vector3 to, float radius)
		{
			AddLine(vh, from, to, radius, color);
		}

		protected virtual void AddLine(VertexHelper vh, Vector3 from, Vector3 to, float radius, Color32 color)
		{
			AddLine(vh, from, color, to, color, radius);
		}

		protected virtual void AddLine(VertexHelper vh,
			Vector3 from,Color32 fromColor,
			Vector3 to, Color32 toColor, float radius)
		{
			if (radius <= 0) return;

			var dir = (to - from).normalized;
			var lSide = new Vector3(-dir.y, dir.x) * radius;
			var rSide = new Vector3(dir.y, -dir.x) * radius;

			vh.AddVert(from + lSide, fromColor, Vector2.zero);
			vh.AddVert(from + rSide, fromColor, Vector2.zero);
			vh.AddVert(to + rSide, toColor, Vector2.zero);
			vh.AddVert(to + lSide, toColor, Vector2.zero);

			var vertId = vh.currentVertCount - 1;
			vh.AddTriangle(vertId - 1, vertId - 2, vertId - 3);
			vh.AddTriangle(vertId - 3, vertId, vertId - 1);
		}


		public void CalculateLayoutInputHorizontal() { }
		public void CalculateLayoutInputVertical() { }

		public float minWidth { get { return 0; } }
		public float preferredWidth { get { return 0; } }
		public float flexibleWidth { get { return -1; } }

		public float minHeight { get { return 0; } }
		public float preferredHeight { get { return 0; } }
		public float flexibleHeight { get { return -1; } }

		public int layoutPriority { get { return 0; } }


		public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
		{
			return false;
		}
	}
}