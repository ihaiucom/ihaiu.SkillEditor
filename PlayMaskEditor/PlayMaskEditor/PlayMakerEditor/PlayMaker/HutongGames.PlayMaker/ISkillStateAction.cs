using System;
using UnityEngine;
namespace HutongGames.PlayMaker
{
	public interface ISkillStateAction
	{
		bool Enabled
		{
			get;
			set;
		}
		void Init(SkillState state);
		void Reset();
		void OnEnter();
		void OnUpdate();
		void OnGUI();
		void OnFixedUpdate();
		void OnLateUpdate();
		void OnExit();
		bool Event(SkillEvent fsmEvent);
		void DoControllerColliderHit(ControllerColliderHit collider);
		void DoCollisionEnter(Collision collisionInfo);
		void DoCollisionStay(Collision collisionInfo);
		void DoCollisionExit(Collision collisionInfo);
		void DoTriggerEnter(Collider other);
		void DoTriggerStay(Collider other);
		void DoTriggerExit(Collider other);
		void Log(string text);
		void LogWarning(string text);
		void LogError(string text);
		string ErrorCheck();
	}
}
