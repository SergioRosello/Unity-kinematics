using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovementController : MonoBehaviour {
	public float MaxSpeed = 4;
	public LayerMask ObstacleMask = 1 << Layers.Platforms | 1 << Layers.MovingPlatforms, MovingPlatformMask = 1 << Layers.MovingPlatforms;
	public int NumberOfHorizontalRays = 5, NumberOfVerticalRays = 5;
	public float RayOffset = .05f;
	public float MaxSlope = 45;
	public float sprintingBoost = 3f;
	public float JumpHeight = 5f;
	public float JumpSpeed = 3.5f;
	public float FullJumpTime = .35f;
	public float MinJumpPercent = .4f;

	protected BoxCollider2D box;
	protected Rigidbody2D rb;
	protected ControllerState state;
	protected bool collidingBelow, collidingAbove, collidingLeft, collidingRight;
	protected bool sprinting;
	protected bool crouching;
	protected float groundY, leftWallX, rightWallX, ceilingY;
	protected Vector3 initialScale;
	protected Vector2 facing;
	protected Transform modelTransform;
	protected float targetHeight;
	protected float h;
	protected Vector2 velocity;
	protected float rotation;
	protected Animator _anim;
	protected Vector2 _movingPlatformSpeed;
	protected Health _health;
	protected Vector2 groundNormal;
	protected float _jumpPressTime;
	protected float _jumpStartY;
	protected Vector2 _lastPosition;
	protected float groundAngleVelovityMultiplicator;
	public Vector2 scaleOnCrouch;
	[Range(0.0f, 1.0f)]
	public float crouchingVelocityReducer;
	public Vector2 Facing {
		get {
			return facing;
		} set {
			if (facing != value) {
				facing = value;
				modelTransform.localScale = new Vector3 (facing.x > 0 ? initialScale.x : -initialScale.x, initialScale.y, initialScale.z);
			}
		}
	}

	private Vector3 originalScale;
	private Vector3 originalBoxSize;

	public Vector2 crouchedBoxSize;

	// Use this for initialization
	virtual protected void Awake () {
		box = GetComponent<BoxCollider2D> ();
		rb = GetComponent<Rigidbody2D> ();
		_anim = GetComponentInChildren<Animator> ();
		_health = GetComponent<Health> ();

		modelTransform = transform.Find("Model");
		initialScale = modelTransform.localScale;
		Facing = Vector2.right;
		originalScale = modelTransform.localScale;
		originalBoxSize = box.size;
	}
	
	// Update is called once per frame
	virtual protected void Update () {
		DetermineDirection ();
		CheckCollisions ();
		CheckifCrouched();
		if (!_health || _health.IsAlive) {
			SetHorizontalSpeed ();
		}
		SetVerticalSpeed ();

		if (_health && !_health.IsAlive) {
			velocity.x = 0;
		}
	}

	protected void CheckifCrouched(){
		if(crouching){
			modelTransform.localScale = scaleOnCrouch;
			box.size = crouchedBoxSize;
		}else{
			modelTransform.localScale = originalScale;
			box.size = originalBoxSize;
		}
		if(facing.x < 0){
			modelTransform.localScale = new Vector3(-modelTransform.localScale.x, modelTransform.localScale.y, modelTransform.localScale.z);
		}
	}
	virtual protected void LateUpdate() {
		velocity.y = Mathf.Clamp (velocity.y, -PhysicsSettings.MaxVerticalVelocity, PhysicsSettings.MaxVerticalVelocity);
		rb.velocity = velocity + _movingPlatformSpeed;
		rb.rotation = rotation;
		_anim.SetFloat ("hVelocity", Mathf.Abs(h));
		_lastPosition = new Vector2 (transform.position.x, transform.position.y);
	}

	protected abstract void DetermineDirection ();

	protected void CheckCollisions() {
		collidingBelow = false;
		collidingAbove = false;
		collidingLeft = false;
		collidingRight = false;

		groundY = Mathf.NegativeInfinity;
		leftWallX = Mathf.NegativeInfinity;
		rightWallX = Mathf.Infinity;
		ceilingY = Mathf.Infinity;


		var positionDelta = new Vector2 (transform.position.x, transform.position.y) - _lastPosition;
		var hitBetweenFrames = Utils.Raycast2D (_lastPosition, positionDelta, positionDelta.magnitude, ObstacleMask);

		if (hitBetweenFrames.collider != null) {
			// Nos hemos topado con algo, reajustar la posición
			velocity = Vector2.zero;
			var newPosition = new Vector3 (hitBetweenFrames.point.x - (box.size.x/2) * positionDelta.normalized.x, hitBetweenFrames.point.y - (box.size.y/2) * positionDelta.normalized.y, 0);
			transform.position = newPosition;
		} else {
			// Buscamos suelo
			for (int i = 0; i < NumberOfVerticalRays; i++) {
				var rayX = box.bounds.min.x + RayOffset + i * (box.bounds.size.x - 2 * RayOffset) / (NumberOfVerticalRays - 1);
				var hitBottom = Utils.Raycast2D (new Vector3 (rayX, box.bounds.center.y, 0), Vector2.down, box.size.y / 2, ObstacleMask);
				if (hitBottom.collider != null) {
					collidingBelow = true;
					if (hitBottom.point.y > groundY) {
						groundY = hitBottom.point.y;
					}
					if (hitBottom.collider.gameObject.layer == Layers.MovingPlatforms) {
						var movingRb = hitBottom.collider.gameObject.GetComponent<Rigidbody2D> ();
						if (movingRb != null) {
							_movingPlatformSpeed = movingRb.velocity;
						}
					} else {
						_movingPlatformSpeed = Vector2.zero;
					}
				}
			}

			if (collidingBelow) {
				transform.position = new Vector3 (transform.position.x, groundY + box.size.y / 2, 0);
				SetRotationAngle();
			}

			// Buscamos techo
			for (int i = 0; i < NumberOfVerticalRays; i++) {
				var rayX = box.bounds.min.x + RayOffset + i * (box.bounds.size.x - 2 * RayOffset) / (NumberOfVerticalRays - 1);
				var hitTop = Utils.Raycast2D (new Vector3 (rayX, box.bounds.center.y, 0), Vector2.up, box.size.y / 2, ObstacleMask);
				if (hitTop.collider != null) {
					collidingAbove = true;
					if (hitTop.point.y < ceilingY) {
						ceilingY = hitTop.point.y;
					}
				}
			}

			if (state != ControllerState.JumpAscending) {
				state = collidingBelow ? ControllerState.Grounded : ControllerState.Falling;
			}

			if (collidingAbove) {
				transform.position = new Vector3 (transform.position.x, ceilingY - box.size.y / 2, 0);
			}

			// Buscamos pared a la izquierda
			for (int i = 0; i < NumberOfHorizontalRays; i++) {
				var rayY = box.bounds.min.y + RayOffset + i * (box.bounds.size.y - 2 * RayOffset) / (NumberOfHorizontalRays - 1);
				var hitLeft = Utils.Raycast2D (new Vector3 (box.bounds.center.x, rayY, 0), Vector2.left, box.size.x / 2, ObstacleMask);
				if (hitLeft.collider != null) {
					if (Vector2.Angle (hitLeft.normal, Vector2.up) > MaxSlope) {
						collidingLeft = true;
						if (hitLeft.point.y > leftWallX) {
							leftWallX = hitLeft.point.x;
						}
					}
				}
			}

			if (collidingLeft) {
				transform.position = new Vector3 (leftWallX + box.size.x / 2, transform.position.y, 0);
			}

			// Buscamos pared a la derecha
			for (int i = 0; i < NumberOfHorizontalRays; i++) {
				var rayY = box.bounds.min.y + RayOffset + i * (box.bounds.size.y - 2 * RayOffset) / (NumberOfHorizontalRays - 1);
				var hitRight = Utils.Raycast2D (new Vector3 (box.bounds.center.x, rayY, 0), Vector2.right, box.size.x / 2, ObstacleMask);
				if (hitRight.collider != null) {
					if (Vector2.Angle (hitRight.normal, Vector2.up) > MaxSlope) {
						collidingRight = true;
						if (hitRight.point.y < rightWallX) {
							rightWallX = hitRight.point.x;
						}
					}
				}
			}
			if (collidingRight) {
				transform.position = new Vector3 (rightWallX - box.size.x / 2, transform.position.y, 0);
			}
		}
	}

	protected void SetHorizontalSpeed() {
		if (h < -.01f) {
			if (Facing.x > 0) {
				Facing = Vector2.left;
			}
			setHorizontalVelocityByCollidingSides(collidingLeft);
		} else if (h > .01f) {
			if (Facing.x < 0) {
				Facing = Vector2.right;
			}
			setHorizontalVelocityByCollidingSides(collidingRight);
		} else {
			velocity = new Vector2 (0, velocity.y);
		}
	}

	protected void setHorizontalVelocityByCollidingSides(bool collidingSide){
		if (!collidingSide) {
			if(sprinting){
				velocity = new Vector2 (h * MaxSpeed * sprintingBoost, velocity.y);
			}else if(crouching){
				velocity = new Vector2 (h * MaxSpeed * crouchingVelocityReducer, velocity.y);
			}
			else
				velocity = new Vector2 (h * MaxSpeed, velocity.y);
		} else {
			velocity = new Vector2 (0, velocity.y);
		}
}

	protected void SetVerticalSpeed () {
		if (state == ControllerState.JumpAscending) {
			if (!collidingAbove) {
				if (transform.position.y >= targetHeight) {
					state = ControllerState.Falling;
				} else {
					velocity = new Vector2 (velocity.x, JumpSpeed);
				}
			} else {
				state = ControllerState.Falling;
			}
		} else {
			if (!collidingBelow) {
				velocity = new Vector2 (velocity.x, velocity.y + PhysicsSettings.GravityAcceleration * Time.deltaTime);
			}
		}
	}

	protected void TryJump () {
		if (collidingBelow) {
			// Saltar
			_jumpPressTime = Time.time;
			_jumpStartY = transform.position.y;
			state = ControllerState.JumpAscending;
			targetHeight = _jumpStartY + JumpHeight;
		}
	}

	protected void SetRotationAngle(){
		groundNormal = Utils.Raycast2D(new Vector2(box.bounds.center.x, box.bounds.center.y), Vector2.down, box.bounds.size.y, ObstacleMask).normal;
		Debug.Log("Ground Normal: " + (Vector2.Angle(groundNormal, transform.right) - 90));
		groundAngleVelovityMultiplicator = Mathf.Abs((Vector2.Angle(groundNormal, transform.right) - 90)) * 0.01f;
		Debug.Log("Ground angle velocity multiplier: " + groundAngleVelovityMultiplicator);
		Debug.Log("Velocity before: " + velocity);
		velocity = new Vector2(velocity.x * groundAngleVelovityMultiplicator, velocity.y * groundAngleVelovityMultiplicator);
		Debug.Log("Velocity after: " + velocity);
		transform.up = groundNormal;
	}
	protected enum ControllerState {
		Grounded, JumpAscending, Falling
	}
}
