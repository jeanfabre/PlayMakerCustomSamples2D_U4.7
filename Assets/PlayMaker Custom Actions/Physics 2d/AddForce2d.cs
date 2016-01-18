// (c) Copyright HutongGames, LLC 2010-2015. All rights reserved.
/*--- __ECO__ __PLAYMAKER__ __ACTION__
EcoMetaStart
{
"script dependancies":[
						"Assets/PlayMaker Custom Actions/Physics 2d/Internal/RigidBody2dActionBase.cs"
					]
}
EcoMetaEnd
---*/

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Physics 2d")]
	[Tooltip("Adds a 2d force to a Game Object. Use Vector2 variable and/or Float variables for each axis.")]
	public class AddForce2d : RigidBody2dActionBase
	{
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[Tooltip("The GameObject to apply the force to.")]
		public FsmOwnerDefault gameObject;

#if UNITY_4_3 || UNITY_4_4
		// ForceMod2d and Space only came with 4.5
#else
		[Tooltip("Option for how to apply a force using AddForce")]
		public ForceMode2D forceMode;

		[Tooltip("Apply the force in world or local space.")]
		public Space space;

#endif


		[UIHint(UIHint.Variable)]
		[Tooltip("Optionally apply the force at a position on the object. This will also add some torque. The position is often returned from MousePick or GetCollision2dInfo actions.")]
		public FsmVector2 atPosition;


		[UIHint(UIHint.Variable)]
		[Tooltip("A Vector2 force to add. Optionally override any axis with the X, Y parameters.")]
		public FsmVector2 vector;
		
		[Tooltip("Force along the X axis. To leave unchanged, set to 'None'.")]
		public FsmFloat x;
		
		[Tooltip("Force along the Y axis. To leave unchanged, set to 'None'.")]
		public FsmFloat y;

		[Tooltip("A Vector3 force to add. z is ignored")]
		public FsmVector3 vector3;



		[Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;


		public override void Reset()
		{
			gameObject = null;
			atPosition = new FsmVector2 { UseVariable = true };
			vector = null;
			vector3 = new FsmVector3 {UseVariable = true};

			#if UNITY_4_3 || UNITY_4_4
			#else
			forceMode = ForceMode2D.Force;
			space = Space.Self;
			#endif

			// default axis to variable dropdown with None selected.
			x = new FsmFloat { UseVariable = true };
			y = new FsmFloat { UseVariable = true };

			everyFrame = false;
		}

		
		public override void Awake()
		{
			Fsm.HandleFixedUpdate = true;
		}

		public override void OnEnter()
		{
			CacheRigidBody2d(Fsm.GetOwnerDefaultTarget(gameObject));

			DoAddForce();
			
			if (!everyFrame)
			{
				Finish();
			}		
		}

		public override void OnFixedUpdate()
		{
			DoAddForce();
		}
		
		void DoAddForce()
		{
		
			if (!rb2d)
			{
				return;
			} 

			Vector2 force = vector.IsNone ? new Vector2(x.Value, y.Value) : vector.Value;

			if (!vector3.IsNone)
			{
				force.x = vector3.Value.x;
				force.y = vector3.Value.y;
			}

			// override any axis
			
			if (!x.IsNone) force.x = x.Value;
			if (!y.IsNone) force.y = y.Value;
			
			// apply force	
			#if UNITY_4_3 || UNITY_4_4
				if (!atPosition.IsNone)
				{
					rb2d.AddForceAtPosition(force, atPosition.Value);
				}
				else
				{
					rb2d.AddForce(force);
				}
			#else

				if (space == Space.World)
				{
					if (!atPosition.IsNone)
					{
						rb2d.AddForceAtPosition(force, atPosition.Value,forceMode);
					}
					else
					{
						rb2d.AddForce(force,forceMode);
					}
				}
				else
				{
					rb2d.AddRelativeForce(force,forceMode);
				}
			#endif



		}
		
		
	}
}
