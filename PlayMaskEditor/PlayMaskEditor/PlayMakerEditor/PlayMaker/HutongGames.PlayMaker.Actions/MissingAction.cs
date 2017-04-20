using System;
namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Debug), Tooltip("Placeholder for a missing action.\n\nUsually generated when the editor can't load an action, e.g., if the script was deleted, but can also be used as a TODO note.")]
	public class MissingAction : SkillStateAction
	{
		[Tooltip("The name of the missing action.")]
		public string actionName;
	}
}
