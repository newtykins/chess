﻿using System;
using UnityEngine;

public class ArrowManager : MonoBehaviour
{
	public GameObject arrows;
	public Material arrowMaterial;
	public GameObject arrowHeadPrefab;
	public GameObject gameManagerObject;

	private GameManager _gameManager;
	private Vector2 _arrowStart;
	private Vector2 _arrowEnd;

	public void Start()
	{
		_gameManager = gameManagerObject.GetComponent<GameManager>();
	}

	public void Update()
	{
		// When the button has been initially pressed, store the start position of the arrow
		if (Input.GetMouseButtonDown(1))
		{
			var mousePosition = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
			_arrowStart = new Vector2(Mathf.Round(mousePosition.x), Mathf.Round(mousePosition.y));
		}

		// While holding it down, keep updating the end of the arrow
		if (Input.GetMouseButton(1))
		{
			var mousePosition = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
			_arrowEnd = new Vector2(Mathf.Round(mousePosition.x), Mathf.Round(mousePosition.y));
		}

		// Once the button has been finally released, draw the arrow
		if (Input.GetMouseButtonUp(1))
		{
			// Ensure that the arrow is in bounds
			var startInBounds = (0 < _arrowStart.x && _arrowStart.x <= _gameManager.boardSize) && (0 < _arrowStart.y && _arrowStart.y <= _gameManager.boardSize);
			var endInBounds = (0 < _arrowEnd.x && _arrowEnd.x <= _gameManager.boardSize) && (0 < _arrowEnd.y && _arrowEnd.y <= _gameManager.boardSize);

			if (startInBounds && endInBounds && _arrowStart != _arrowEnd)
			{
				// Ensure that the arrow is not a duplicate
				var isDuplicate = false;

				foreach (Transform arrow in arrows.transform)
				{
					var head = arrow.GetChild(0);

					// If the arrow heads are touching
					if ((Vector2)head.transform.position == _arrowEnd)
					{
						isDuplicate = true;
						break;
					}
				}

				if (!isDuplicate)
				{
					// todo: make prettier colours
					// The default arrow colour is white
					var colour = Color.yellow;

					// Hold down ctrl for a red arrow
					if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
						colour = Color.red;
					// Hold down alt for a blue arrow
					else if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
						colour = Color.blue;
					// Hold down shift for a green arrow
					else if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
						colour = Color.green;

					DrawArrow(_arrowStart, _arrowEnd, colour);
				}
			}
		}

		// Clear all arrows on a left click
		if (Input.GetMouseButtonDown(0))
		{
			ClearArrows();
		}
	}

	private void DrawArrow(Vector2 start, Vector2 end, Color colour)
	{
		// Instantiate the arrow
		var arrowCount = arrows.transform.childCount + 1;

		var arrowGameObject = new GameObject($"Arrow #{arrowCount}");
		arrowGameObject.transform.parent = arrows.transform;

		// Draw the body of the arrow
		var lineRenderer = arrowGameObject.AddComponent<LineRenderer>();

		lineRenderer.startColor = colour;
		lineRenderer.endColor = colour;
		lineRenderer.startWidth = 0.2f;
		lineRenderer.endWidth = 0.2f;
		lineRenderer.material = arrowMaterial;
		lineRenderer.SetPosition(0, new Vector3(start.x, start.y, -1));
		lineRenderer.SetPosition(1, new Vector3(end.x, end.y, -1));

		// Draw the head of the arrow
		// todo: draw the arrow head always on the line, use ReduceEnd
		var arrowHead = Instantiate(arrowHeadPrefab, new Vector3(end.x, end.y, -1), Quaternion.identity);
		var arrowHeadSpriteRenderer = arrowHead.GetComponent<SpriteRenderer>();
		arrowHeadSpriteRenderer.color = colour;
		arrowHead.name = $"Arrow #{arrowCount} Head";
		arrowHead.transform.parent = arrowGameObject.transform;

		// Calculate the arrow head's angle
		var arrowHeadAngle = (Mathf.Atan2((end.y - start.y), (end.x - start.x)) * Mathf.Rad2Deg) - 90f;
		arrowHead.transform.rotation = Quaternion.Euler(0f, 0f, arrowHeadAngle);
	}

	private void ClearArrows()
	{
		foreach (Transform arrow in arrows.transform)
		{
			Destroy(arrow.gameObject);
		}
	}

	private Vector2 ReduceEnd(Vector2 p1, Vector2 p2, float r)
	{
		var dx = p2.x - p1.x;
		var dy = p2.y - p1.y;
		var mag = Mathf.Sqrt(dx * dx + dy * dy);

		var x = p2.x - r * dx / mag;
		var y = p2.y - r * dy / mag;

		return new Vector2(x, y);
	}
}