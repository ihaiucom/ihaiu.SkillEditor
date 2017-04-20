using HutongGames.Editor;
using HutongGames.Extensions;
using HutongGames.PlayMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
namespace HutongGames.PlayMakerEditor
{
	internal class FsmGraphView
	{
		private enum DraggingMode
		{
			None,
			State,
			Transition,
			Selection,
			Minimap
		}
		private enum DragConstraint
		{
			None,
			X,
			Y
		}
		private const float MaxPossibleCanvasSize = 200000f;
		private const float ExpandGraphAmount = 100000f;
		private const float GraphSizePadding = 100f;
		private const int LeftMouseButton = 0;
		private const int RightMouseButton = 1;
		private const int MiddleMouseButton = 2;
		private const float DragThreshold = 3f;
		private static bool editingDisabled;
		private static string editingDisabledReason;
		private static bool updateVisibility;
		private static List<SkillState> visibleStates = new List<SkillState>();
		private static List<Link> visibleLinks = new List<Link>();
		private static List<SkillTransition> highlightedTransitions = new List<SkillTransition>();
		private static List<SkillState> highlightedStates = new List<SkillState>();
		private readonly CanvasView canvasView;
		private Vector2 graphSize;
		private Rect view;
		private float zoom = 1f;
		private Event currentEvent;
		private EventType eventType;
		private Vector2 currentMousePos;
		private Vector2 contextMenuPos;
		private bool mouseOverGraphView;
		private SkillState mouseOverState;
		private SkillTransition mouseOverTransition;
		private SkillTransition mouseOverGlobalTransition;
		private bool mouseOverStartBox;
		private Rect startBox;
		private static SkillTransition selectedGlobalTransition;
		private FsmGraphView.DraggingMode draggingMode;
		private readonly SkillState dummyDraggingState;
		private Vector2 dragStartPos;
		private List<Vector2> dragStartPositions;
		private bool dragStarted;
		private bool frameState;
		private bool graphExpanded;
		private FsmGraphView.DragConstraint dragConstraint;
		public static bool EditingDisable
		{
			get
			{
				return FsmGraphView.editingDisabled;
			}
		}
		public bool IsDragging
		{
			get
			{
				return this.draggingMode != FsmGraphView.DraggingMode.None;
			}
		}
		public FsmGraphView()
		{
			this.dummyDraggingState = new SkillState(null);
			this.canvasView = new CanvasView
			{
				ContentMargin = 300f
			};
		}
		public void Init()
		{
			this.canvasView.Init(SkillEditor.Window);
			this.RefreshView();
		}
		public void RefreshView()
		{
			this.UpdateGraphSize();
			this.zoom = this.canvasView.SetZoom(SkillEditor.Selection.Zoom);
			this.InitScale(this.zoom);
			this.canvasView.SetContentScrollPosition(SkillEditor.Selection.ScrollPosition);
			this.UpdateVisibility();
			SkillEditor.Repaint(true);
		}
		[Localizable(false)]
		public void TakeScreenshot()
		{
			string fsmSavePath = Files.GetFsmSavePath(SkillEditor.SelectedFsm);
			this.canvasView.TakeScreenshot("../" + FsmEditorSettings.ScreenshotsPath + "/" + fsmSavePath);
		}
		public bool IsStateOffscreen(SkillState state)
		{
			return state != null && this.view.get_width() != 0f && this.view.get_height() != 0f && (state.get_Position().get_x() < SkillEditor.Selection.ScrollPosition.x + 0f || state.get_Position().get_y() < SkillEditor.Selection.ScrollPosition.y + 0f || state.get_Position().get_x() + state.get_Position().get_width() + 0f > SkillEditor.Selection.ScrollPosition.x + this.view.get_width() || state.get_Position().get_y() + state.get_Position().get_height() + 0f > SkillEditor.Selection.ScrollPosition.y + this.view.get_height());
		}
		public void SanityCheckGraphBounds()
		{
			if (this.graphExpanded || this.graphSize.x > 100000f)
			{
				Debug.Log(Strings.get_Fixed_Graph_View_Bounds());
				this.UpdateGraphBounds();
			}
		}
		public void UpdateGraphBounds()
		{
			FsmGraphView.MoveAllStatesToOrigin(SkillEditor.SelectedFsm, 100f);
			this.UpdateGraphSize();
		}
		public void UpdateGraphSize()
		{
			if (SkillEditor.SelectedFsm == null || this.graphExpanded)
			{
				return;
			}
			this.graphSize = default(Vector2);
			SkillState[] states = SkillEditor.SelectedFsm.get_States();
			for (int i = 0; i < states.Length; i++)
			{
				SkillState fsmState = states[i];
				this.graphSize.x = Mathf.Max(this.graphSize.x, fsmState.get_Position().get_xMax());
				this.graphSize.y = Mathf.Max(this.graphSize.y, fsmState.get_Position().get_yMax());
			}
			this.graphSize.x = Mathf.Min(this.graphSize.x + 100f, 200000f);
			this.graphSize.y = Mathf.Min(this.graphSize.y + 200f, 200000f);
			this.canvasView.ContentSize = this.graphSize;
		}
		private void UpdateGraphBounds(Vector2 viewPos, float padding = 100f)
		{
			if (SkillEditor.SelectedFsm == null)
			{
				return;
			}
			Vector2 vector = this.canvasView.ContentToViewCoordinates(viewPos);
			Vector2 vector2 = this.canvasView.LocalToWorldCoordinates(viewPos + this.canvasView.ContentOrigin);
			SkillState state = SkillEditor.SelectedFsm.GetState(SkillEditor.SelectedFsm.get_StartState());
			Vector2 localPos = new Vector2(state.get_Position().get_x(), state.get_Position().get_y()) * this.zoom;
			Vector2 vector3 = vector2 - this.canvasView.LocalToWorldCoordinates(localPos);
			this.UpdateGraphBounds();
			localPos = new Vector2(state.get_Position().get_x(), state.get_Position().get_y()) * this.zoom;
			vector2 = this.canvasView.LocalToWorldCoordinates(localPos) + vector3;
			this.canvasView.SetScrollPosition(this.canvasView.WorldToLocalCoordinates(vector2 - vector / this.zoom));
			this.graphExpanded = false;
			this.UpdateGraphSize();
		}
		private void ExpandGraphBounds()
		{
			if (this.graphExpanded)
			{
				return;
			}
			Vector2 vector = new Vector2(100000f, 100000f);
			this.graphSize += vector * 2f;
			this.canvasView.ContentSize = this.graphSize;
			this.canvasView.SetScrollPosition(this.canvasView.ScrollPosition + vector * this.zoom);
			FsmGraphView.MoveAllStates(SkillEditor.SelectedFsm, vector);
			this.currentMousePos += vector * this.zoom;
			this.dragStartPos += vector * this.zoom;
			for (int i = 0; i < this.dragStartPositions.get_Count(); i++)
			{
				List<Vector2> list;
				int num;
				(list = this.dragStartPositions).set_Item(num = i, list.get_Item(num) + vector);
			}
			this.graphExpanded = true;
		}
		[Localizable(false)]
		public void OnGUI(Rect area)
		{
			if (SkillEditor.SelectedFsmIsLocked)
			{
				GUILayout.Label("LOCKED", new GUILayoutOption[0]);
			}
			EditorGUI.BeginDisabledGroup(SkillEditor.SelectedFsmIsLocked);
			this.view = area;
			this.currentEvent = Event.get_current();
			this.eventType = this.currentEvent.get_type();
			this.DrawBackground();
			this.DoCanvasView();
			this.DoMinimap();
			this.DoGameStateIcon();
			this.DrawFrame();
			this.DrawHintBox();
			this.DoDebugText();
			EditorGUI.EndDisabledGroup();
		}
		private void DoCanvasView()
		{
			if (FsmGraphView.updateVisibility && this.eventType == 8)
			{
				this.DoUpdateVisibility();
			}
			this.mouseOverGraphView = this.canvasView.ActiveView.Contains(this.currentEvent.get_mousePosition());
			SkillEditor.Selection.ScrollPosition = this.canvasView.BeginGUILayout(this.view, FsmEditorSettings.ShowScrollBars);
			this.HandleMouseEvents();
			this.HandleKeyEvents();
			this.DoFsmGraph(this.canvasView.ContentArea);
			this.HandleDragAndDropObjects();
			this.DoContextMenu();
			this.canvasView.EndGUILayout();
			this.SetScale(this.canvasView.Scale);
			if (this.canvasView.ViewChanged)
			{
				this.UpdateVisibility();
				this.canvasView.ViewChanged = false;
			}
		}
		[Localizable(false)]
		private void DoDebugText()
		{
		}
		public void Update()
		{
			this.canvasView.Update();
			if (FsmGraphView.updateVisibility)
			{
				this.DoUpdateVisibility();
			}
		}
		public void FrameState(SkillState state)
		{
			if (state != null)
			{
				this.FrameState(state, true);
			}
		}
		public void FrameState(SkillState state, bool smooth)
		{
			if (state == null)
			{
				return;
			}
			Vector2 vector = new Vector2
			{
				x = state.get_Position().get_x() + state.get_Position().get_width() * 0.5f,
				y = state.get_Position().get_y() + state.get_Position().get_height()
			};
			if (smooth)
			{
				if (this.draggingMode == FsmGraphView.DraggingMode.None)
				{
					this.canvasView.StartPanToPosition(vector);
					return;
				}
			}
			else
			{
				this.canvasView.CancelAutoPan();
				this.canvasView.SetContentScrollPosition(vector);
			}
		}
		private bool IsMouseEvent()
		{
			return this.currentEvent.get_isMouse() || this.canvasView.IsEdgeScrolling || this.eventType == 9;
		}
		private void HandleMouseEvents()
		{
			if (SkillEditor.MouseUp)
			{
				this.HandleMouseUp();
				return;
			}
			if (!this.mouseOverGraphView)
			{
				return;
			}
			if (this.IsMouseEvent())
			{
				this.currentMousePos = this.currentEvent.get_mousePosition() - this.canvasView.ContentOrigin;
			}
			this.UpdateMouseOverInfo(this.currentMousePos);
			if (SkillEditor.SelectedFsm != null)
			{
				EventType eventType = this.eventType;
				if (eventType != 0)
				{
					if (eventType == 3)
					{
						this.HandleMouseDrag();
					}
				}
				else
				{
					this.HandleMouseDown();
				}
				if (this.dragStarted && (this.draggingMode == FsmGraphView.DraggingMode.State || this.draggingMode == FsmGraphView.DraggingMode.Transition))
				{
					this.HandleMouseDrag();
					return;
				}
			}
			else
			{
				if (this.eventType == null && EditorGUI.get_actionKey())
				{
					SkillBuilder.AddFsmToSelected();
					GUIUtility.ExitGUI();
				}
			}
		}
		private void HandleKeyEvents()
		{
			if ((this.eventType == 5 && this.currentEvent.get_keyCode() == 96 && GUIUtility.get_keyboardControl() == 0) || (this.currentEvent.get_keyCode() == 96 && Keyboard.Control()))
			{
				SkillEditor.OpenActionWindow();
				return;
			}
			if (this.eventType == 4)
			{
				KeyCode keyCode = this.currentEvent.get_keyCode();
				if (keyCode == 282)
				{
					EditorCommands.ToggleShowHints();
				}
				if (keyCode == 292 && SkillEditor.SelectedFsm != null)
				{
					this.TakeScreenshot();
				}
				if (GUIUtility.get_hotControl() != 0)
				{
					return;
				}
				KeyCode keyCode2 = keyCode;
				if (keyCode2 != 102)
				{
					if (keyCode2 == 278)
					{
						if (SkillEditor.SelectedFsm != null)
						{
							this.FrameState(SkillEditor.SelectStateByName(SkillEditor.SelectedFsm.get_StartState(), true));
						}
					}
				}
				else
				{
					if (Application.get_isPlaying() && SkillEditor.SelectedFsm != null && SkillEditor.SelectedFsm.get_ActiveState() != null)
					{
						SkillEditor.SelectState(SkillEditor.SelectedFsm.get_ActiveState(), true);
					}
					else
					{
						this.FrameState(SkillEditor.SelectedState);
					}
				}
				if (EditorGUI.get_actionKey())
				{
					if (Event.get_current().get_alt())
					{
						switch (keyCode)
						{
						case 273:
							FsmGraphView.ToggleLinkStyle(SkillEditor.SelectedTransition);
							return;
						case 274:
							FsmGraphView.SetLinkConstraint(SkillEditor.SelectedTransition, 0);
							return;
						default:
							return;
						}
					}
					else
					{
						switch (keyCode)
						{
						case 273:
							EditorCommands.MoveTransitionUp(SkillEditor.SelectedTransition);
							break;
						case 274:
							EditorCommands.MoveTransitionDown(SkillEditor.SelectedTransition);
							return;
						case 275:
							FsmGraphView.SetLinkConstraint(SkillEditor.SelectedTransition, 2);
							return;
						case 276:
							FsmGraphView.SetLinkConstraint(SkillEditor.SelectedTransition, 1);
							return;
						default:
							return;
						}
					}
				}
			}
		}
		private void UpdateMouseOverInfo(Vector2 mousePos)
		{
			this.mouseOverState = null;
			this.mouseOverTransition = null;
			this.mouseOverGlobalTransition = null;
			if (SkillEditor.SelectedFsm == null)
			{
				return;
			}
			this.mouseOverStartBox = this.startBox.Contains(mousePos);
			using (List<SkillState>.Enumerator enumerator = FsmGraphView.visibleStates.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillState current = enumerator.get_Current();
					Rect rect = this.ScaleRect(current.get_Position());
					if (rect.Contains(mousePos))
					{
						this.mouseOverState = current;
						int num = (int)Mathf.Floor((mousePos.y - rect.get_y()) / SkillEditorStyles.StateRowHeight);
						if (num > 0)
						{
							this.mouseOverTransition = current.get_Transitions()[num - 1];
						}
					}
					else
					{
						List<SkillTransition> globalTransitions = SkillEditor.Builder.GetGlobalTransitions(current);
						if (globalTransitions.get_Count() != 0)
						{
							Rect rect2 = new Rect(rect);
							rect2.set_height((float)globalTransitions.get_Count() * SkillEditorStyles.StateRowHeight);
							Rect rect3 = rect2;
							rect3.set_y(rect3.get_y() - (rect3.get_height() + 32f * this.zoom));
							if (rect3.Contains(mousePos))
							{
								int num2 = (int)Mathf.Floor((mousePos.y - rect3.get_y()) / SkillEditorStyles.StateRowHeight);
								this.mouseOverGlobalTransition = globalTransitions.get_Item(num2);
							}
						}
					}
				}
			}
		}
		private void HandleMouseDown()
		{
			switch (this.currentEvent.get_button())
			{
			case 0:
			case 1:
				if (this.mouseOverStartBox)
				{
					SkillState state = SkillEditor.SelectedFsm.GetState(SkillEditor.SelectedFsm.get_StartState());
					SkillEditor.Selection.SelectState(state, this.currentEvent.get_shift(), false);
				}
				else
				{
					if (this.mouseOverGlobalTransition != null)
					{
						if (this.currentEvent.get_clickCount() > 1)
						{
							SkillEditor.Inspector.SetMode(InspectorMode.EventManager);
						}
						SkillEditor.EventManager.SelectEvent(this.mouseOverGlobalTransition.get_FsmEvent(), true);
						SkillState state2 = SkillEditor.SelectedFsm.GetState(this.mouseOverGlobalTransition.get_ToState());
						SkillEditor.Selection.SelectState(state2, this.currentEvent.get_shift(), false);
						if (this.currentEvent.get_alt() && Application.get_isPlaying())
						{
							SkillEditor.SelectedFsm.Event(this.mouseOverGlobalTransition.get_FsmEvent());
						}
					}
					else
					{
						if (this.mouseOverTransition != null)
						{
							if (this.currentEvent.get_alt())
							{
								SkillEditor.SelectState(SkillEditor.SelectStateByName(this.mouseOverTransition.get_ToState(), true), true);
								SkillEditor.Inspector.SetMode(InspectorMode.StateInspector);
							}
							else
							{
								SkillEditor.Selection.SelectState(this.mouseOverState, this.currentEvent.get_shift(), false);
							}
							if (SkillEditor.SelectedState == this.mouseOverState)
							{
								SkillEditor.Selection.SelectTransition(this.mouseOverTransition);
								if (this.currentEvent.get_clickCount() > 1)
								{
									SkillEditor.Inspector.SetMode(InspectorMode.EventManager);
								}
								SkillEditor.EventManager.SelectEvent(this.mouseOverTransition.get_FsmEvent(), true);
							}
							if (this.currentEvent.get_button() == 0)
							{
								if (this.draggingMode != FsmGraphView.DraggingMode.Transition)
								{
									this.MouseDragStart();
								}
								this.draggingMode = FsmGraphView.DraggingMode.Transition;
								if (this.currentEvent.get_alt() && Application.get_isPlaying())
								{
									SkillEditor.SelectedFsm.Event(this.mouseOverTransition.get_FsmEvent());
								}
							}
						}
						else
						{
							if (this.mouseOverState != null)
							{
								SkillEditor.Selection.SelectState(this.mouseOverState, this.currentEvent.get_shift(), this.currentEvent.get_alt());
								SkillEditor.Inspector.SetMode(InspectorMode.StateInspector);
								this.frameState = true;
								if (this.currentEvent.get_button() == 0)
								{
									if (this.draggingMode != FsmGraphView.DraggingMode.State)
									{
										this.MouseDragStart();
									}
									this.draggingMode = FsmGraphView.DraggingMode.State;
									if (this.currentEvent.get_alt() && Application.get_isPlaying())
									{
										SkillTransition fsmTransition = SkillEditor.Builder.FindTransitionToState(this.mouseOverState);
										if (fsmTransition != null)
										{
											SkillEditor.SelectedFsm.Event(fsmTransition.get_FsmEvent());
											SkillEditor.Selection.SelectTransition(null);
										}
										else
										{
											if (EditorGUI.get_actionKey())
											{
												SkillEditor.SelectedFsm.SetState(this.mouseOverState.get_Name());
											}
											else
											{
												Debug.Log(string.Format(Strings.get_Log_No_Transition_To_State(), this.mouseOverState.get_Name()));
												EditorApplication.Beep();
											}
										}
									}
								}
							}
							else
							{
								SkillEditor.Selection.SelectState(null, this.currentEvent.get_shift(), this.currentEvent.get_alt());
								if (this.currentEvent.get_button() == 0 && !Keyboard.AltAction())
								{
									if (this.draggingMode != FsmGraphView.DraggingMode.Selection)
									{
										this.MouseDragStart();
									}
									this.draggingMode = FsmGraphView.DraggingMode.Selection;
								}
							}
						}
					}
				}
				break;
			}
			SkillEditor.RepaintAll();
		}
		private void MouseDragStart()
		{
			this.dragStartPos = this.currentMousePos;
			this.dragStarted = false;
			this.dragStartPositions = new List<Vector2>();
			using (List<SkillState>.Enumerator enumerator = SkillEditor.Selection.States.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillState current = enumerator.get_Current();
					this.dragStartPositions.Add(new Vector2(current.get_Position().get_x(), current.get_Position().get_y()));
				}
			}
		}
		private bool OverDragThreshold()
		{
			float magnitude = (this.currentMousePos - this.dragStartPos).get_magnitude();
			return magnitude > 3f;
		}
		private void HandleMouseDrag()
		{
			SkillEditor.Repaint(false);
			switch (this.draggingMode)
			{
			case FsmGraphView.DraggingMode.State:
				if (!this.dragStarted)
				{
					this.dragStarted = this.OverDragThreshold();
					if (!FsmGraphView.editingDisabled && this.dragStarted)
					{
						SkillEditor.RegisterUndo(Strings.get_Command_Move_State());
						this.ExpandGraphBounds();
						return;
					}
				}
				else
				{
					if (!FsmGraphView.editingDisabled)
					{
						this.DoDragStates();
						return;
					}
				}
				break;
			case FsmGraphView.DraggingMode.Transition:
				if (this.dragStarted)
				{
					this.canvasView.DoEdgeScroll();
					return;
				}
				if (this.mouseOverState != SkillEditor.SelectedState)
				{
					this.dragStarted = true;
					this.ExpandGraphBounds();
					return;
				}
				break;
			case FsmGraphView.DraggingMode.Selection:
				if (!this.dragStarted)
				{
					this.dragStarted = this.OverDragThreshold();
				}
				break;
			default:
				return;
			}
		}
		private void DoDragStates()
		{
			Vector2 vector = this.canvasView.DoEdgeScroll();
			int num = 0;
			Vector2 vector2 = this.currentMousePos - this.dragStartPos + vector;
			using (List<SkillState>.Enumerator enumerator = SkillEditor.SelectedStates.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillState current = enumerator.get_Current();
					FsmGraphView.UpdateStatePosition(current, this.dragStartPositions.get_Item(num) + vector2 / this.zoom);
					if (this.currentEvent.get_shift())
					{
						this.ConstrainDragToAxis();
					}
					else
					{
						this.dragConstraint = FsmGraphView.DragConstraint.None;
					}
					num++;
				}
			}
			if ((!FsmEditorSettings.SnapToGrid && EditorGUI.get_actionKey()) || (FsmEditorSettings.SnapToGrid && !EditorGUI.get_actionKey()))
			{
				FsmGraphView.SnapSelectedStatesToGrid();
			}
		}
		private void ConstrainDragToAxis()
		{
			if (this.dragConstraint != FsmGraphView.DragConstraint.None)
			{
				if (this.dragConstraint == FsmGraphView.DragConstraint.X)
				{
					int num = 0;
					using (List<SkillState>.Enumerator enumerator = SkillEditor.SelectedStates.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							SkillState current = enumerator.get_Current();
							current.set_Position(new Rect(current.get_Position().get_x(), this.dragStartPositions.get_Item(num).y, current.get_Position().get_width(), current.get_Position().get_height()));
							num++;
						}
						return;
					}
				}
				int num2 = 0;
				using (List<SkillState>.Enumerator enumerator2 = SkillEditor.SelectedStates.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						SkillState current2 = enumerator2.get_Current();
						current2.set_Position(new Rect(this.dragStartPositions.get_Item(num2).x, current2.get_Position().get_y(), current2.get_Position().get_width(), current2.get_Position().get_height()));
						num2++;
					}
				}
				return;
			}
			float num3 = this.currentMousePos.x - this.dragStartPos.x;
			float num4 = this.currentMousePos.y - this.dragStartPos.y;
			if (Math.Abs(num3) < 3f && Math.Abs(num3) < 3f)
			{
				return;
			}
			this.dragConstraint = ((Mathf.Abs(num3) > Mathf.Abs(num4)) ? FsmGraphView.DragConstraint.X : FsmGraphView.DragConstraint.Y);
		}
		private static void SnapSelectedStatesToGrid()
		{
			if (SkillEditor.SelectedState == null)
			{
				return;
			}
			int snapGridSize = FsmEditorSettings.SnapGridSize;
			Vector2 vector = new Vector2(SkillEditor.SelectedState.get_Position().get_x(), SkillEditor.SelectedState.get_Position().get_y());
			Vector2 vector2 = new Vector2(FsmGraphView.SnapFloatToGrid(vector.x, (float)snapGridSize), FsmGraphView.SnapFloatToGrid(vector.y, (float)snapGridSize));
			Vector2 offset = vector2 - vector;
			using (List<SkillState>.Enumerator enumerator = SkillEditor.SelectedStates.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillState current = enumerator.get_Current();
					FsmGraphView.TranslateState(current, offset);
				}
			}
		}
		private static float SnapFloatToGrid(float x, float gridSize)
		{
			return Mathf.Floor(x / gridSize) * gridSize;
		}
		private void HandleMouseUp()
		{
			SkillEditor.MouseUpHandled();
			SkillEditor.Repaint(false);
			switch (this.draggingMode)
			{
			case FsmGraphView.DraggingMode.State:
				if (FsmGraphView.editingDisabled)
				{
					this.DisplayEditingDisabledNotification();
				}
				else
				{
					if (this.dragStarted)
					{
						this.UpdateGraphBounds(this.currentMousePos, 100f);
						SkillEditor.SetFsmDirty(false, false);
					}
					else
					{
						if (SkillEditor.SelectedState != null && EditorGUI.get_actionKey() && !this.currentEvent.get_alt())
						{
							if (this.currentEvent.get_shift())
							{
								EditorCommands.DeleteSelectedState();
							}
							else
							{
								EditorCommands.AddTransitionToSelectedState();
								FsmGraphView.ContextMenuSelectEvent(SkillEvent.get_Finished());
								this.UpdateGraphBounds(this.currentMousePos, 100f);
							}
						}
					}
				}
				break;
			case FsmGraphView.DraggingMode.Transition:
				if (FsmGraphView.editingDisabled)
				{
					this.DisplayEditingDisabledNotification();
				}
				else
				{
					if (this.dragStarted)
					{
						this.UpdateGraphBounds(this.currentMousePos, 100f);
						if (SkillEditor.SelectedTransition != null)
						{
							if (this.mouseOverState != null)
							{
								FsmGraphView.SetSelectedTransitionTarget(this.mouseOverState);
							}
							else
							{
								if (EditorGUI.get_actionKey())
								{
									SkillTransition selectedTransition = SkillEditor.SelectedTransition;
									SkillState fsmState = this.AddState(new Vector2(this.dummyDraggingState.get_Position().get_x() - 100000f, this.dummyDraggingState.get_Position().get_y() - 100000f));
									EditorCommands.SetTransitionTarget(selectedTransition, fsmState.get_Name());
								}
							}
						}
					}
					else
					{
						if (EditorGUI.get_actionKey() && this.currentEvent.get_shift())
						{
							EditorCommands.DeleteSelectedTransition();
						}
					}
				}
				break;
			case FsmGraphView.DraggingMode.Selection:
				if (this.dragStarted)
				{
					if (SkillEditor.SelectedFsm != null)
					{
						bool flag = this.dragStartPos.x > this.currentMousePos.x;
						float num = Mathf.Abs(this.currentMousePos.x - this.dragStartPos.x);
						float num2 = Mathf.Abs(this.currentMousePos.y - this.dragStartPos.y);
						Rect rect = new Rect(Mathf.Min(this.dragStartPos.x, this.currentMousePos.x), Mathf.Min(this.dragStartPos.y, this.currentMousePos.y), num, num2);
						List<SkillState> list = new List<SkillState>();
						SkillState[] states = SkillEditor.SelectedFsm.get_States();
						for (int i = 0; i < states.Length; i++)
						{
							SkillState fsmState2 = states[i];
							Rect rect2 = this.ScaleRect(fsmState2.get_Position());
							if (flag)
							{
								if (RectExtensions.IntersectsWith(rect, rect2))
								{
									list.Add(fsmState2);
								}
							}
							else
							{
								if (RectExtensions.Contains(rect, rect2))
								{
									list.Add(fsmState2);
								}
							}
						}
						SkillEditor.Selection.SelectStates(list, this.currentEvent.get_shift(), this.currentEvent.get_alt());
					}
				}
				else
				{
					if (EditorGUI.get_actionKey() && !this.currentEvent.get_shift())
					{
						this.AddState(this.currentMousePos / this.zoom);
					}
				}
				break;
			}
			this.draggingMode = FsmGraphView.DraggingMode.None;
			if (this.frameState && this.currentEvent.get_button() == 0 && FsmEditorSettings.FrameSelectedState && !this.dragStarted && SkillEditor.GraphView.IsStateOffscreen(SkillEditor.SelectedState))
			{
				SkillEditor.GraphView.FrameState(SkillEditor.SelectedState);
				this.frameState = false;
			}
			SkillEditor.RepaintAll();
		}
		public void EnableEditing()
		{
			FsmGraphView.editingDisabled = false;
		}
		public void DisableEditing(string reason)
		{
			FsmGraphView.editingDisabled = true;
			FsmGraphView.editingDisabledReason = reason;
		}
		private void DisplayEditingDisabledNotification()
		{
			SkillEditor.Window.ShowNotification(new GUIContent(FsmGraphView.editingDisabledReason));
		}
		private void DoContextMenu()
		{
			if (this.eventType == 16 && this.mouseOverGraphView)
			{
				this.contextMenuPos = this.currentMousePos;
				if (FsmGraphView.editingDisabled)
				{
					this.DisplayEditingDisabledNotification();
				}
				else
				{
					this.contextMenuPos = this.currentMousePos;
					if (SkillEditor.SelectedFsm == null)
					{
						if (Selection.get_activeGameObject() != null)
						{
							FsmGraphView.GenerateAddFsmContextMenu().ShowAsContext();
						}
					}
					else
					{
						this.GenerateContextMenu().ShowAsContext();
					}
				}
				this.currentEvent.Use();
			}
		}
		private static GenericMenu GenerateAddFsmContextMenu()
		{
			GenericMenu genericMenu = new GenericMenu();
			genericMenu.AddItem(new GUIContent(Strings.get_Menu_Add_FSM()), false, new GenericMenu.MenuFunction(SkillBuilder.AddFsmToSelected));
			if (SkillEditor.Builder.CanPaste())
			{
				genericMenu.AddItem(new GUIContent(Strings.get_Menu_Paste_FSM()), false, new GenericMenu.MenuFunction(EditorCommands.PasteFsm));
			}
			else
			{
				genericMenu.AddDisabledItem(new GUIContent(Strings.get_Menu_Paste_FSM()));
			}
			Templates.InitList();
			using (List<string>.Enumerator enumerator = Templates.Categories.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string current = enumerator.get_Current();
					using (Dictionary<SkillTemplate, string>.Enumerator enumerator2 = Templates.CategoryLookup.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							KeyValuePair<SkillTemplate, string> current2 = enumerator2.get_Current();
							if (current2.get_Value() == current)
							{
								SkillTemplate key = current2.get_Key();
								genericMenu.AddItem(new GUIContent(string.Format(Strings.get_Menu_Paste_Template(), current, key.get_name())), false, new GenericMenu.MenuFunction2(EditorCommands.AddTemplateToSelected), key);
								genericMenu.AddItem(new GUIContent(string.Format(Strings.get_Menu_Use_Template(), current, key.get_name())), false, new GenericMenu.MenuFunction2(EditorCommands.AddFsmAndUseTemplateWithSelected), key);
							}
						}
					}
				}
			}
			if (Templates.List.get_Count() == 0)
			{
				genericMenu.AddDisabledItem(new GUIContent(string.Format(Strings.get_Menu_Paste_Template(), Strings.get_Menu_None(), Strings.get_Menu_None())));
				genericMenu.AddDisabledItem(new GUIContent(string.Format(Strings.get_Menu_Use_Template(), Strings.get_Menu_None(), Strings.get_Menu_None())));
			}
			return genericMenu;
		}
		[Localizable(false)]
		private GenericMenu GenerateContextMenu()
		{
			GenericMenu genericMenu = new GenericMenu();
			string text = Menus.MakeMenuRoot(Strings.get_Menu_Transition_Event());
			if (this.mouseOverGlobalTransition != null)
			{
				SkillEditor.SelectStateByName(this.mouseOverGlobalTransition.get_ToState(), false);
				FsmGraphView.selectedGlobalTransition = this.mouseOverGlobalTransition;
				List<SkillInfo> list = SkillInfo.FindActionsTargetingGlobalTransition(SkillEditor.SelectedFsm, FsmGraphView.selectedGlobalTransition.get_EventName());
				if (list.get_Count() == 0)
				{
					genericMenu.AddDisabledItem(new GUIContent(Strings.get_Menu_Sent_By()));
				}
				else
				{
					using (List<SkillInfo>.Enumerator enumerator = list.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							SkillInfo current = enumerator.get_Current();
							if (current.fsm == SkillEditor.SelectedFsm)
							{
								genericMenu.AddItem(new GUIContent(string.Format("{0}/{1} : {2}", Strings.get_Menu_Sent_By(), current.state.get_Name(), Labels.GetActionLabel(current.action))), false, new GenericMenu.MenuFunction2(SkillInfo.SelectFsmInfo), current);
							}
						}
					}
					using (List<SkillInfo>.Enumerator enumerator2 = list.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							SkillInfo current2 = enumerator2.get_Current();
							if (current2.fsm != SkillEditor.SelectedFsm)
							{
								string fullStateLabel = Labels.GetFullStateLabel(current2.state);
								genericMenu.AddItem(new GUIContent(string.Format("{0}/{1} : {2}", Strings.get_Menu_Sent_By(), fullStateLabel, Labels.GetActionLabel(current2.action))), false, new GenericMenu.MenuFunction2(SkillInfo.SelectFsmInfo), current2);
							}
						}
					}
				}
				genericMenu.AddSeparator("");
				SkillEvent[] events = SkillEditor.SelectedFsm.get_Events();
				for (int i = 0; i < events.Length; i++)
				{
					SkillEvent fsmEvent = events[i];
					genericMenu.AddItem(new GUIContent(text + fsmEvent.get_Name()), fsmEvent == FsmGraphView.selectedGlobalTransition.get_FsmEvent(), new GenericMenu.MenuFunction2(FsmGraphView.ContextMenuSelectGlobalEvent), fsmEvent);
				}
				SkillEvent fsmEvent2 = SkillEvent.GetFsmEvent("FINISHED");
				genericMenu.AddItem(new GUIContent(text + "FINISHED"), FsmGraphView.selectedGlobalTransition.get_FsmEvent() == fsmEvent2, new GenericMenu.MenuFunction2(FsmGraphView.ContextMenuSelectGlobalEvent), fsmEvent2);
				genericMenu = ActionEditor.AddCommonEventMenus(genericMenu, text, FsmGraphView.selectedGlobalTransition.get_FsmEvent(), new GenericMenu.MenuFunction2(FsmGraphView.ContextMenuSelectGlobalEvent));
				genericMenu.AddSeparator("");
				genericMenu.AddItem(new GUIContent(Strings.get_Menu_Clear_Transition_Event()), false, new GenericMenu.MenuFunction2(FsmGraphView.ContextMenuSelectGlobalEvent), null);
				genericMenu.AddItem(new GUIContent(Strings.get_Menu_Delete_Transition()), false, new GenericMenu.MenuFunction2(this.DeleteGlobalTransition), this.mouseOverGlobalTransition);
			}
			else
			{
				if (SkillEditor.SelectedTransition != null)
				{
					List<SkillInfo> list2 = SkillInfo.FindActionsTargetingTransition(SkillEditor.SelectedState, SkillEditor.SelectedTransition.get_EventName());
					if (list2.get_Count() == 0)
					{
						genericMenu.AddDisabledItem(new GUIContent(Strings.get_Menu_Sent_By()));
					}
					else
					{
						string text2 = Menus.MakeMenuRoot(Strings.get_Menu_GraphView_Sent_By());
						using (List<SkillInfo>.Enumerator enumerator3 = list2.GetEnumerator())
						{
							while (enumerator3.MoveNext())
							{
								SkillInfo current3 = enumerator3.get_Current();
								if (current3.fsm == SkillEditor.SelectedFsm)
								{
									genericMenu.AddItem(new GUIContent(text2 + current3.state.get_Name() + " : " + Labels.GetActionLabel(current3.action)), false, new GenericMenu.MenuFunction2(SkillInfo.SelectFsmInfo), current3);
								}
							}
						}
						using (List<SkillInfo>.Enumerator enumerator4 = list2.GetEnumerator())
						{
							while (enumerator4.MoveNext())
							{
								SkillInfo current4 = enumerator4.get_Current();
								if (current4.fsm != SkillEditor.SelectedFsm)
								{
									string fullStateLabel2 = Labels.GetFullStateLabel(current4.state);
									genericMenu.AddItem(new GUIContent(text2 + fullStateLabel2 + " : " + Labels.GetActionLabel(current4.action)), false, new GenericMenu.MenuFunction2(SkillInfo.SelectFsmInfo), current4);
								}
							}
						}
					}
					genericMenu.AddSeparator("");
					SkillEvent fsmEvent3 = SkillEditor.SelectedTransition.get_FsmEvent();
					SkillEvent[] events2 = SkillEditor.SelectedFsm.get_Events();
					for (int j = 0; j < events2.Length; j++)
					{
						SkillEvent fsmEvent4 = events2[j];
						genericMenu.AddItem(new GUIContent(text + fsmEvent4.get_Name()), fsmEvent4 == fsmEvent3, new GenericMenu.MenuFunction2(FsmGraphView.ContextMenuSelectEvent), fsmEvent4);
					}
					SkillEvent fsmEvent5 = SkillEvent.GetFsmEvent("FINISHED");
					genericMenu.AddItem(new GUIContent(text + "FINISHED"), fsmEvent3 == fsmEvent5, new GenericMenu.MenuFunction2(FsmGraphView.ContextMenuSelectEvent), fsmEvent5);
					genericMenu = ActionEditor.AddCommonEventMenus(genericMenu, text, SkillEditor.SelectedTransition.get_FsmEvent(), new GenericMenu.MenuFunction2(FsmGraphView.ContextMenuSelectEvent));
					string toState = SkillEditor.SelectedTransition.get_ToState();
					string text3 = Menus.MakeMenuRoot(Strings.get_Menu_GraphView_Transition_Target());
					SkillState[] states = SkillEditor.SelectedFsm.get_States();
					for (int k = 0; k < states.Length; k++)
					{
						SkillState fsmState = states[k];
						genericMenu.AddItem(new GUIContent(text3 + fsmState.get_Name()), toState == fsmState.get_Name(), new GenericMenu.MenuFunction2(FsmGraphView.ContextMenuSelectTransitionTarget), fsmState.get_Name());
					}
					genericMenu.AddSeparator("");
					genericMenu.AddItem(new GUIContent(Strings.get_Menu_GraphView_Link_Style_Default()), SkillEditor.SelectedTransition.get_LinkStyle() == 0, new GenericMenu.MenuFunction2(EditorCommands.SetTransitionLinkStyle), 0);
					genericMenu.AddItem(new GUIContent(Strings.get_Menu_GraphView_Link_Style_Bezier()), SkillEditor.SelectedTransition.get_LinkStyle() == 1, new GenericMenu.MenuFunction2(EditorCommands.SetTransitionLinkStyle), 1);
					genericMenu.AddItem(new GUIContent(Strings.get_Menu_GraphView_Link_Style_Circuit()), SkillEditor.SelectedTransition.get_LinkStyle() == 2, new GenericMenu.MenuFunction2(EditorCommands.SetTransitionLinkStyle), 2);
					genericMenu.AddItem(new GUIContent(Strings.get_Menu_GraphView_Link_Direction_Auto()), SkillEditor.SelectedTransition.get_LinkConstraint() == 0, new GenericMenu.MenuFunction2(EditorCommands.SetTransitionLinkConstraint), 0);
					genericMenu.AddItem(new GUIContent(Strings.get_Menu_GraphView_Link_Direction_Left()), SkillEditor.SelectedTransition.get_LinkConstraint() == 1, new GenericMenu.MenuFunction2(EditorCommands.SetTransitionLinkConstraint), 1);
					genericMenu.AddItem(new GUIContent(Strings.get_Menu_GraphView_Link_Direction_Right()), SkillEditor.SelectedTransition.get_LinkConstraint() == 2, new GenericMenu.MenuFunction2(EditorCommands.SetTransitionLinkConstraint), 2);
					genericMenu = Menus.AddColorMenu(genericMenu, Strings.get_Menu_GraphView_Link_Color(), SkillEditor.SelectedTransition.get_ColorIndex(), new GenericMenu.MenuFunction2(EditorCommands.SetTransitionColorIndex));
					genericMenu.AddSeparator("");
					genericMenu.AddItem(new GUIContent(Strings.get_Menu_Move_Transition_Up()), false, new GenericMenu.MenuFunction2(EditorCommands.MoveTransitionUp), SkillEditor.SelectedTransition);
					genericMenu.AddItem(new GUIContent(Strings.get_Menu_Move_Transition_Down()), false, new GenericMenu.MenuFunction2(EditorCommands.MoveTransitionDown), SkillEditor.SelectedTransition);
					genericMenu.AddSeparator("");
					genericMenu.AddItem(new GUIContent(Strings.get_Menu_Clear_Transition_Event()), false, new GenericMenu.MenuFunction2(FsmGraphView.ContextMenuSelectEvent), null);
					genericMenu.AddItem(new GUIContent(Strings.get_Menu_Clear_Transition_Target()), false, new GenericMenu.MenuFunction2(FsmGraphView.ContextMenuSelectTransitionTarget), string.Empty);
					genericMenu.AddItem(new GUIContent(Strings.get_Menu_Delete_Transition()), false, new GenericMenu.MenuFunction(EditorCommands.DeleteSelectedTransition));
				}
				else
				{
					if (SkillEditor.SelectedState != null)
					{
						text = Menus.MakeMenuRoot(Strings.get_Menu_GraphView_Add_Transition());
						SkillEvent[] events3 = SkillEditor.SelectedFsm.get_Events();
						for (int l = 0; l < events3.Length; l++)
						{
							SkillEvent fsmEvent6 = events3[l];
							genericMenu.AddItem(new GUIContent(text + fsmEvent6.get_Name()), false, new GenericMenu.MenuFunction2(FsmGraphView.ContextMenuAddTransition), fsmEvent6);
						}
						SkillEvent fsmEvent7 = SkillEvent.GetFsmEvent("FINISHED");
						genericMenu.AddItem(new GUIContent(text + "FINISHED"), false, new GenericMenu.MenuFunction2(FsmGraphView.ContextMenuAddTransition), fsmEvent7);
						genericMenu = ActionEditor.AddCommonEventMenus(genericMenu, text, null, new GenericMenu.MenuFunction2(FsmGraphView.ContextMenuAddTransition));
						text = Menus.MakeMenuRoot(Strings.get_Menu_GraphView_Add_Global_Transition());
						SkillEvent[] events4 = SkillEditor.SelectedFsm.get_Events();
						for (int m = 0; m < events4.Length; m++)
						{
							SkillEvent fsmEvent8 = events4[m];
							genericMenu.AddItem(new GUIContent(text + fsmEvent8.get_Name()), false, new GenericMenu.MenuFunction2(FsmGraphView.ContextMenuAddGlobalTransition), fsmEvent8);
						}
						genericMenu.AddItem(new GUIContent(text + "FINISHED"), false, new GenericMenu.MenuFunction2(FsmGraphView.ContextMenuAddGlobalTransition), fsmEvent7);
						genericMenu = ActionEditor.AddCommonEventMenus(genericMenu, text, null, new GenericMenu.MenuFunction2(FsmGraphView.ContextMenuAddGlobalTransition));
						genericMenu.AddSeparator("");
						if (SkillEditor.Builder.IsStartState(SkillEditor.SelectedState))
						{
							genericMenu.AddDisabledItem(new GUIContent(Strings.get_Menu_GraphView_Set_Start_State()));
						}
						else
						{
							genericMenu.AddItem(new GUIContent(Strings.get_Menu_GraphView_Set_Start_State()), false, new GenericMenu.MenuFunction(EditorCommands.SetSelectedStateAsStartState));
						}
						genericMenu = Menus.AddColorMenu(genericMenu, Strings.get_Menu_GraphView_Set_Color(), SkillEditor.SelectedState.get_ColorIndex(), new GenericMenu.MenuFunction2(EditorCommands.SetSelectedStatesColorIndex));
						genericMenu.AddSeparator("");
						if (SkillEditor.SelectedStates.get_Count() > 0)
						{
							genericMenu.AddItem(SkillEditorContent.MenuGraphViewCopyStates, false, new GenericMenu.MenuFunction(EditorCommands.CopyStateSelection));
						}
						else
						{
							genericMenu.AddDisabledItem(SkillEditorContent.MenuGraphViewCopyStates);
						}
						genericMenu.AddItem(new GUIContent(Strings.get_Menu_GraphView_Save_Template()), false, new GenericMenu.MenuFunction(EditorCommands.SaveSelectionAsTemplate));
						genericMenu.AddItem(new GUIContent(Strings.get_Menu_GraphView_Delete_States()), false, new GenericMenu.MenuFunction(EditorCommands.DeleteMultiSelection));
						genericMenu.AddSeparator("");
						genericMenu.AddItem(new GUIContent(Strings.get_Menu_GraphView_Toggle_Breakpoint()), false, new GenericMenu.MenuFunction(EditorCommands.ToggleBreakpointOnSelectedState));
					}
					else
					{
						genericMenu.AddItem(new GUIContent(Strings.get_Menu_GraphView_Add_State()), false, new GenericMenu.MenuFunction(this.AddState));
						if (SkillEditor.Builder.CanPaste())
						{
							genericMenu.AddItem(SkillEditorContent.MenuGraphViewPasteStates, false, new GenericMenu.MenuFunction(this.PasteStates));
						}
						else
						{
							genericMenu.AddDisabledItem(SkillEditorContent.MenuGraphViewPasteStates);
						}
						Templates.InitList();
						using (List<string>.Enumerator enumerator5 = Templates.Categories.GetEnumerator())
						{
							while (enumerator5.MoveNext())
							{
								string current5 = enumerator5.get_Current();
								using (Dictionary<SkillTemplate, string>.Enumerator enumerator6 = Templates.CategoryLookup.GetEnumerator())
								{
									while (enumerator6.MoveNext())
									{
										KeyValuePair<SkillTemplate, string> current6 = enumerator6.get_Current();
										if (current6.get_Value() == current5)
										{
											SkillTemplate key = current6.get_Key();
											genericMenu.AddItem(new GUIContent(string.Format(Strings.get_Menu_Paste_Template(), current5, key.get_name())), false, new GenericMenu.MenuFunction2(this.PasteTemplate), key);
										}
									}
								}
							}
						}
						if (Templates.List.get_Count() == 0)
						{
							genericMenu.AddDisabledItem(new GUIContent(Strings.get_Menu_GraphView_Paste_Template_None()));
						}
						genericMenu.AddSeparator("");
						genericMenu.AddItem(new GUIContent(Strings.get_Menu_GraphView_Copy_FSM()), false, new GenericMenu.MenuFunction(EditorCommands.CopyFsm));
						genericMenu.AddItem(new GUIContent(Strings.get_Menu_GraphView_Save_Template()), false, new GenericMenu.MenuFunction(EditorCommands.SaveFsmAsTemplate));
						genericMenu.AddSeparator("");
						genericMenu.AddItem(new GUIContent(Strings.get_Menu_GraphView_Set_Watermark()), false, new GenericMenu.MenuFunction(EditorCommands.ChooseWatermark));
						genericMenu.AddItem(new GUIContent(Strings.get_Menu_GraphView_Save_Screenshot()), false, new GenericMenu.MenuFunction(this.TakeScreenshot));
						genericMenu.AddSeparator("");
						if (Selection.get_activeGameObject() != null)
						{
							if (SkillEditor.SelectedTemplate == null)
							{
								genericMenu.AddItem(new GUIContent(Strings.get_Menu_GraphView_Add_FSM_Component_New_FSM()), false, new GenericMenu.MenuFunction(SkillBuilder.AddFsmToSelected));
								if (SkillEditor.Builder.CanPaste())
								{
									genericMenu.AddItem(new GUIContent(Strings.get_Menu_GraphView_Add_FSM_Component_Paste_FSM()), false, new GenericMenu.MenuFunction(EditorCommands.PasteFsm));
								}
								else
								{
									genericMenu.AddDisabledItem(new GUIContent(Strings.get_Menu_GraphView_Add_FSM_Component_Paste_FSM()));
								}
								Templates.InitList();
								using (List<string>.Enumerator enumerator7 = Templates.Categories.GetEnumerator())
								{
									while (enumerator7.MoveNext())
									{
										string current7 = enumerator7.get_Current();
										using (Dictionary<SkillTemplate, string>.Enumerator enumerator8 = Templates.CategoryLookup.GetEnumerator())
										{
											while (enumerator8.MoveNext())
											{
												KeyValuePair<SkillTemplate, string> current8 = enumerator8.get_Current();
												if (current8.get_Value() == current7)
												{
													SkillTemplate key2 = current8.get_Key();
													genericMenu.AddItem(new GUIContent(string.Format(Strings.get_Menu_GraphView_Add_FSM_Component_Use_Template(), current7, key2.get_name())), false, new GenericMenu.MenuFunction2(EditorCommands.AddTemplate), key2);
												}
											}
										}
									}
								}
								if (Templates.List.get_Count() == 0)
								{
									genericMenu.AddDisabledItem(new GUIContent(Strings.get_Menu_GraphView_Add_FSM_Component_Add_Template()));
								}
							}
							else
							{
								genericMenu.AddItem(new GUIContent(Strings.get_Menu_GraphView_Add_To_Selected()), false, new GenericMenu.MenuFunction2(EditorCommands.AddTemplateToSelected), SkillEditor.SelectedTemplate);
							}
						}
						else
						{
							genericMenu.AddDisabledItem((SkillEditor.SelectedTemplate == null) ? new GUIContent(Strings.get_Menu_GraphView_Add_FSM_Component()) : new GUIContent(Strings.get_Menu_GraphView_Add_To_Selected()));
						}
						if (SkillEditor.SelectedTemplate == null)
						{
							genericMenu.AddItem(new GUIContent(Strings.get_Menu_GraphView_Remove_FSM_Component()), false, new GenericMenu.MenuFunction(EditorCommands.RemoveFsmComponent));
						}
						else
						{
							genericMenu.AddItem(new GUIContent(Strings.get_Menu_GraphView_Delete_Template()), false, new GenericMenu.MenuFunction(EditorCommands.DeleteTemplate));
						}
					}
				}
			}
			return genericMenu;
		}
		private static void ContextMenuAddTransition(object userdata)
		{
			EditorCommands.AddTransitionToSelectedState();
			FsmGraphView.ContextMenuSelectEvent(userdata);
		}
		private static void ContextMenuAddGlobalTransition(object userdata)
		{
			FsmGraphView.selectedGlobalTransition = EditorCommands.AddGlobalTransitionToSelectedState();
			FsmGraphView.ContextMenuSelectGlobalEvent(userdata);
		}
		private static void ContextMenuSelectEvent(object userdata)
		{
			SkillEvent fsmEvent = (SkillEvent)userdata;
			SkillEditor.Builder.AddEvent(SkillEditor.SelectedFsm, fsmEvent);
			EditorCommands.SetTransitionEvent(SkillEditor.SelectedTransition, fsmEvent);
			SkillEditor.EventManager.Reset();
			SkillEditor.EventManager.SelectEvent(fsmEvent, true);
			FsmGraphView.selectedGlobalTransition = null;
		}
		public static void ContextMenuSelectGlobalEvent(object userdata)
		{
			SkillEvent fsmEvent = (SkillEvent)userdata;
			SkillEditor.Builder.AddEvent(SkillEditor.SelectedFsm, fsmEvent);
			EditorCommands.SetTransitionEvent(FsmGraphView.selectedGlobalTransition, fsmEvent);
			SkillEditor.EventManager.Reset();
			SkillEditor.EventManager.SelectEvent(fsmEvent, true);
			FsmGraphView.selectedGlobalTransition = null;
		}
		private static void ContextMenuSelectTransitionTarget(object userdata)
		{
			string toState = userdata as string;
			EditorCommands.SetTransitionTarget(SkillEditor.SelectedTransition, toState);
		}
		private void HandleDragAndDropObjects()
		{
			if (this.mouseOverGraphView && (this.eventType == 9 || this.eventType == 10))
			{
				if (DragAndDropManager.mode == DragAndDropManager.DragMode.AddAction)
				{
					this.UpdateDragNewAction();
					return;
				}
				this.UpdateGenericDrag();
			}
		}
		private void UpdateGenericDrag()
		{
			Object[] objectReferences = DragAndDrop.get_objectReferences();
			if (objectReferences == null || objectReferences.Length == 0)
			{
				return;
			}
			Object @object = objectReferences[0];
			if (@object == null)
			{
				return;
			}
			if ((SkillEditor.SelectedFsm == null && @object is GameObject) || @object is AnimationClip || @object is Animation)
			{
				DragAndDrop.set_visualMode(1);
				if (this.eventType == 10)
				{
					if (FsmGraphView.editingDisabled)
					{
						this.DisplayEditingDisabledNotification();
						return;
					}
					SkillEditorMacros.DropObjectOnGraphView(SkillEditor.SelectedFsm, @object, this.currentMousePos);
				}
				return;
			}
		}
		private void UpdateDragNewAction()
		{
			if (Selection.get_activeObject() == null)
			{
				return;
			}
			DragAndDrop.set_visualMode(1);
			if (this.eventType == 10)
			{
				if (FsmGraphView.editingDisabled)
				{
					this.DisplayEditingDisabledNotification();
				}
				else
				{
					DragAndDrop.AcceptDrag();
					if (DragAndDropManager.AddAction != null)
					{
						this.DropNewAction(DragAndDropManager.AddAction);
					}
				}
			}
			this.currentEvent.Use();
		}
		private void DropNewAction(Type actionType)
		{
			if (SkillEditor.SelectedFsm == null && Selection.get_activeGameObject() != null)
			{
				SkillBuilder.AddFsmToSelected();
				if (SkillEditor.SelectedFsm != null)
				{
					SkillEditor.SelectStateByName(SkillEditor.SelectedFsm.get_StartState(), true);
					EditorCommands.AddAction(SkillEditor.SelectedState, actionType);
					GUIUtility.ExitGUI();
				}
			}
			else
			{
				if (this.mouseOverState != null)
				{
					EditorCommands.AddAction(this.mouseOverState, actionType);
					SkillEditor.SelectState(this.mouseOverState, false);
				}
				else
				{
					SkillState state = this.AddState(this.currentMousePos);
					EditorCommands.SetStateName(state, Labels.GetActionLabel(actionType));
					EditorCommands.AddAction(state, actionType);
				}
			}
			SkillEditor.SetFsmDirty(true, false);
		}
		private void DoLargeWatermarkText(string text)
		{
			GUI.Box(new Rect(this.view.get_x() + 5f, this.view.get_y() + 5f, this.view.get_width() - 5f, 100f), text, SkillEditorStyles.LargeWatermarkText);
		}
		private void DoSelectionHints()
		{
			if (Selection.get_activeGameObject() == null)
			{
				this.DoLargeWatermarkText(Strings.get_Hint_Select_a_GameObject());
				return;
			}
			this.DoLargeWatermarkText(Strings.get_Hint_Right_Click_to_Add_FSM());
		}
		public void UpdateVisibility()
		{
			FsmGraphView.updateVisibility = true;
		}
		private void DoUpdateVisibility()
		{
			FsmGraphView.visibleStates.Clear();
			FsmGraphView.visibleLinks.Clear();
			if (SkillEditor.SelectedFsm != null)
			{
				SkillState[] states = SkillEditor.SelectedFsm.get_States();
				for (int i = 0; i < states.Length; i++)
				{
					SkillState fsmState = states[i];
					if (this.canvasView.IsVisible(fsmState.get_Position()))
					{
						FsmGraphView.visibleStates.Add(fsmState);
					}
				}
				SkillState[] states2 = SkillEditor.SelectedFsm.get_States();
				for (int j = 0; j < states2.Length; j++)
				{
					SkillState fsmState2 = states2[j];
					if (!FsmGraphView.visibleStates.Contains(fsmState2))
					{
						for (int k = 0; k < fsmState2.get_Transitions().Length; k++)
						{
							SkillTransition fsmTransition = fsmState2.get_Transitions()[k];
							if (fsmTransition != null)
							{
								SkillState state = SkillEditor.SelectedFsm.GetState(fsmTransition.get_ToState());
								if (state != null && (FsmGraphView.visibleStates.Contains(state) || RectExtensions.IntersectsWith(RectExtensions.Union(fsmState2.get_Position(), state.get_Position()), this.canvasView.ViewRectInCanvas)))
								{
									FsmGraphView.visibleLinks.Add(new Link
									{
										FromState = fsmState2,
										TransitionIndex = k
									});
								}
							}
						}
					}
				}
			}
			FsmGraphView.updateVisibility = false;
		}
		private bool DrawTransitionEffects()
		{
			return !DebugFlow.Active && GameStateTracker.CurrentState == GameState.Running && SkillEditor.SelectedFsm.get_Active() && FsmEditorSettings.EnableTransitionEffects;
		}
		private void DoFsmGraph(Rect area)
		{
			if (SkillEditor.SelectedFsm == null || (!Application.get_isPlaying() && SkillEditor.SelectedFsmUsesTemplate))
			{
				return;
			}
			GUILayout.BeginArea(area);
			this.DrawCanvas();
			this.DrawStartArrow();
			if (this.DrawTransitionEffects())
			{
				this.DrawStateHistory(SkillTime.get_RealtimeSinceStartup(), 0.5f);
			}
			if (FsmEditorSettings.ShowCommentsInGraphView)
			{
				using (List<SkillState>.Enumerator enumerator = FsmGraphView.visibleStates.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						SkillState current = enumerator.get_Current();
						this.DrawStateDescription(current);
					}
				}
			}
			if (!FsmEditorSettings.DrawLinksBehindStates)
			{
				using (List<SkillState>.Enumerator enumerator2 = FsmGraphView.visibleStates.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						SkillState current2 = enumerator2.get_Current();
						this.DoFsmStateNode(current2);
					}
				}
			}
			IEnumerator enumerator3 = Enum.GetValues(typeof(DrawState)).GetEnumerator();
			try
			{
				while (enumerator3.MoveNext())
				{
					DrawState drawStateFilter = (DrawState)enumerator3.get_Current();
					using (List<SkillState>.Enumerator enumerator4 = FsmGraphView.visibleStates.GetEnumerator())
					{
						while (enumerator4.MoveNext())
						{
							SkillState current3 = enumerator4.get_Current();
							for (int i = 0; i < current3.get_Transitions().Length; i++)
							{
								this.DoLinkGUI(current3, i, drawStateFilter);
							}
						}
					}
					using (List<Link>.Enumerator enumerator5 = FsmGraphView.visibleLinks.GetEnumerator())
					{
						while (enumerator5.MoveNext())
						{
							Link current4 = enumerator5.get_Current();
							this.DoLinkGUI(current4.FromState, current4.TransitionIndex, drawStateFilter);
						}
					}
				}
			}
			finally
			{
				IDisposable disposable = enumerator3 as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			using (List<SkillState>.Enumerator enumerator6 = FsmGraphView.visibleStates.GetEnumerator())
			{
				while (enumerator6.MoveNext())
				{
					SkillState current5 = enumerator6.get_Current();
					this.DoGlobalTransitionGUI(current5);
				}
			}
			if (FsmEditorSettings.DrawLinksBehindStates)
			{
				using (List<SkillState>.Enumerator enumerator7 = FsmGraphView.visibleStates.GetEnumerator())
				{
					while (enumerator7.MoveNext())
					{
						SkillState current6 = enumerator7.get_Current();
						this.DoFsmStateNode(current6);
					}
				}
			}
			if (GameStateTracker.CurrentState != GameState.Stopped)
			{
				if (DebugFlow.Active || !FsmEditorSettings.EnableTransitionEffects)
				{
					this.DrawLastTransition();
				}
				else
				{
					if (SkillEditor.SelectedFsm.get_Active())
					{
						this.DrawTransitionHistory(SkillTime.get_RealtimeSinceStartup(), 0.5f);
					}
				}
			}
			if (this.draggingMode == FsmGraphView.DraggingMode.Selection && this.eventType == 7 && this.dragStarted)
			{
				this.DrawSelectionRect();
			}
			this.DrawFsmHelUrlButton(SkillEditor.SelectedFsm);
			GUILayout.EndArea();
		}
		public Rect ScaleRect(Rect rect)
		{
			return RectExtensions.Scale(rect, this.zoom);
		}
		public static Rect ScaleRect(Rect rect, float scale, Vector2 minSize)
		{
			return RectExtensions.MinSize(RectExtensions.Scale(rect, scale), minSize);
		}
		private void DoFsmStateNode(SkillState state)
		{
			if (this.eventType != 7)
			{
				return;
			}
			if (state.get_Position().get_width().Equals(0f))
			{
				this.UpdateStateSize(state);
			}
			Rect rect = this.ScaleRect(state.get_Position());
			if (this.zoom > 0.5f)
			{
				FsmGraphView.DrawShadow(rect);
			}
			bool selected = SkillEditor.Selection.Contains(state);
			DrawState drawState = FsmDrawState.GetFsmStateDrawState(SkillEditor.SelectedFsm, state, selected);
			if (DebugFlow.Active)
			{
				if (drawState == DrawState.Paused)
				{
					drawState = DrawState.Normal;
				}
				if (state == DebugFlow.DebugState)
				{
					if (DebugFlow.SelectedLogEntry != null && DebugFlow.SelectedLogEntry.get_LogType() == 7)
					{
						drawState = DrawState.Breakpoint;
					}
					else
					{
						drawState = DrawState.Paused;
					}
				}
			}
			if (drawState != DrawState.Normal)
			{
				FsmGraphView.DrawStateHighlight(rect, SkillEditorStyles.HighlightColors[(int)drawState]);
			}
			SkillEditorStyles.StateBox.Draw(rect, false, false, false, false);
			GUI.set_backgroundColor(PlayMakerPrefs.get_Colors()[state.get_ColorIndex()]);
			GUIContent arg_F4_0 = SkillEditorContent.StateTitleBox;
			string name;
			SkillEditorContent.StateTitleBox.set_tooltip(name = state.get_Name());
			arg_F4_0.set_text(name);
			SkillEditorStyles.StateTitleBox.Draw(rect, SkillEditorContent.StateTitleBox, false, false, false, false);
			for (int i = 0; i < state.get_Transitions().Length; i++)
			{
				SkillTransition fsmTransition = state.get_Transitions()[i];
				Rect rect2 = new Rect(rect);
				rect2.set_y(rect2.get_y() + SkillEditorStyles.StateRowHeight * (float)(i + 1));
				if (fsmTransition == SkillEditor.SelectedTransition)
				{
					GUI.set_backgroundColor(Color.get_white());
					GUI.set_contentColor(Color.get_white());
					SkillEditorStyles.TransitionBoxSelected.Draw(rect2, Labels.GetEventLabel(fsmTransition), false, false, false, false);
				}
				else
				{
					GUI.set_backgroundColor(FsmGraphView.GetTransitionBoxColor(state, fsmTransition));
					GUI.set_contentColor(FsmGraphView.GetTransitionContentColor(state, fsmTransition));
					SkillEditorStyles.TransitionBox.Draw(rect2, Labels.GetEventLabel(fsmTransition), false, false, false, false);
				}
			}
			GUI.set_backgroundColor(Color.get_white());
			GUI.set_contentColor(Color.get_white());
			if (state.get_IsBreakpoint())
			{
				this.DrawBreakpoint(state);
			}
			if (FsmEditorSettings.ShowStateLoopCounts && EditorApplication.get_isPlaying())
			{
				rect.Set(rect.get_x(), rect.get_y() - 20f, 100f, 20f);
				GUI.Label(rect, state.get_maxLoopCount().ToString());
			}
			if (FsmErrorChecker.StateHasErrors(state))
			{
				this.DrawStateError(state);
			}
		}
		private void DoLinkGUI(SkillState fromState, int i, DrawState drawStateFilter)
		{
			SkillTransition fsmTransition = fromState.get_Transitions()[i];
			DrawState fsmTransitionDrawState = FsmDrawState.GetFsmTransitionDrawState(SkillEditor.SelectedFsm, fsmTransition, SkillEditor.SelectedTransition == fsmTransition);
			if (fsmTransitionDrawState != drawStateFilter)
			{
				return;
			}
			SkillState state = SkillEditor.SelectedFsm.GetState(fsmTransition.get_ToState());
			if (SkillEditor.Selection.Contains(fsmTransition) && this.draggingMode == FsmGraphView.DraggingMode.Transition && this.dragStarted)
			{
				state = this.dummyDraggingState;
				if (this.mouseOverState != null)
				{
					this.dummyDraggingState.set_Position(this.mouseOverState.get_Position());
				}
				else
				{
					FsmGraphView.UpdateStatePosition(this.dummyDraggingState, this.currentMousePos / this.zoom);
					Rect position = new Rect(this.dummyDraggingState.get_Position());
					position.set_y(position.get_y() - SkillEditorStyles.StateRowHeight * 0.5f);
					position.set_width(0f);
					this.dummyDraggingState.set_Position(position);
				}
			}
			if (this.eventType == 7)
			{
				Color linkColor = FsmGraphView.GetLinkColor(fromState, fsmTransition, fsmTransitionDrawState);
				float width = SkillEditorStyles.LinkWidths[(int)fsmTransitionDrawState];
				this.DrawLink(fromState, state, i, linkColor, width);
			}
		}
		private static Color GetLinkColor(SkillState fromState, SkillTransition transition, DrawState drawState)
		{
			if (drawState == DrawState.Normal)
			{
				if (transition.get_ColorIndex() > 0)
				{
					return PlayMakerPrefs.get_Colors()[transition.get_ColorIndex()];
				}
				if (FsmEditorSettings.ColorLinks && fromState.get_ColorIndex() > 0)
				{
					return PlayMakerPrefs.get_Colors()[fromState.get_ColorIndex()];
				}
			}
			return SkillEditorStyles.LinkColors[(int)drawState];
		}
		private static Color GetTransitionBoxColor(SkillState state, SkillTransition transition)
		{
			if (transition.get_ColorIndex() > 0)
			{
				return Color.get_white() * 0.5f + PlayMakerPrefs.get_Colors()[transition.get_ColorIndex()] * 0.5f;
			}
			return Color.get_white() * 0.5f + PlayMakerPrefs.get_Colors()[state.get_ColorIndex()] * 0.5f;
		}
		private static Color GetTransitionContentColor(SkillState state, SkillTransition transition)
		{
			return Color.get_black();
		}
		[Localizable(false)]
		private void DrawStartArrow()
		{
			SkillState startState = SkillEditor.Builder.GetStartState();
			if (startState == null)
			{
				return;
			}
			Rect rect = this.ScaleRect(startState.get_Position());
			List<SkillTransition> globalTransitions = SkillEditor.Builder.GetGlobalTransitions(startState);
			float stateRowHeight = SkillEditorStyles.StateRowHeight;
			Rect rect2 = new Rect(rect);
			rect2.set_height(stateRowHeight);
			Rect rect3 = rect2;
			rect3.set_y(rect3.get_y() - ((float)globalTransitions.get_Count() * stateRowHeight + stateRowHeight + 31f * this.zoom));
			GUI.set_backgroundColor(Color.get_white());
			GUI.Box(rect3, "START", SkillEditorStyles.StartTransitionBox);
			this.startBox = rect3;
			if (globalTransitions.get_Count() == 0)
			{
				this.DrawGlobalArrow(startState, Color.get_black());
			}
		}
		private void DoGlobalTransitionGUI(SkillState state)
		{
			List<SkillTransition> globalTransitions = SkillEditor.Builder.GetGlobalTransitions(state);
			if (globalTransitions.get_Count() == 0)
			{
				return;
			}
			Rect rect = new Rect(this.ScaleRect(state.get_Position()));
			rect.set_height((float)globalTransitions.get_Count() * SkillEditorStyles.StateRowHeight);
			Rect rect2 = rect;
			rect2.set_y(rect2.get_y() - (rect2.get_height() + 32f * this.zoom));
			GUILayout.BeginArea(rect2);
			GUI.set_backgroundColor(Color.get_white());
			using (List<SkillTransition>.Enumerator enumerator = globalTransitions.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillTransition current = enumerator.get_Current();
					GUILayout.Box(Labels.GetEventLabel(current), SkillEditorStyles.GlobalTransitionBox, new GUILayoutOption[0]);
				}
			}
			GUILayout.EndArea();
			Handles.set_color(Color.get_black());
			Handles.DrawLine(new Vector3(rect2.get_x(), rect2.get_y() + 1f), new Vector3(rect2.get_x() + rect2.get_width(), rect2.get_y() + 1f));
			this.DrawGlobalArrow(state, Color.get_black());
		}
		private void DrawStateDescription(SkillState state)
		{
			if (string.IsNullOrEmpty(state.get_Description()))
			{
				return;
			}
			Rect rect = this.ScaleRect(state.get_Position());
			SkillEditorContent.StateDescription.set_text(state.get_Description());
			Rect rect2 = GUILayoutUtility.GetRect(SkillEditorContent.StateDescription, SkillEditorStyles.CommentBox, new GUILayoutOption[]
			{
				GUILayout.MaxWidth(rect.get_width())
			});
			rect2.set_x(rect.get_x());
			rect2.set_y(rect.get_yMax() + 5f);
			GUI.Box(rect2, state.get_Description(), SkillEditorStyles.CommentBox);
		}
		private void DrawFsmHelUrlButton(Skill fsm)
		{
			if (!string.IsNullOrEmpty(fsm.get_DocUrl()))
			{
				Rect rect = new Rect(this.canvasView.ScrollPosition.x + this.view.get_width() - 32f, this.canvasView.ScrollPosition.y + 10f, 18f, 18f);
				SkillEditorContent.HelpButton.set_tooltip(Strings.get_Tooltip_Fsm_Docs());
				if (GUI.Button(rect, SkillEditorContent.HelpButton, GUIStyle.get_none()))
				{
					Application.OpenURL(fsm.get_DocUrl());
				}
			}
		}
		private void DrawBackground()
		{
			if (this.eventType == 7)
			{
				SkillEditorStyles.Background.Draw(this.view, false, false, false, false);
				if (this.canvasView.TakingScreenshot && !this.canvasView.ScreenshotFirstTile)
				{
					return;
				}
				if (FsmEditorSettings.EnableWatermarks)
				{
					this.DrawWatermark();
				}
				if (SkillEditor.SelectedFsm == null)
				{
					this.DoSelectionHints();
				}
				else
				{
					string text = (SkillEditor.SelectedTemplate != null) ? string.Format(Strings.get_Label_Selected_Template(), SkillEditor.SelectedTemplate.get_name()) : Labels.GetFullFsmLabel(SkillEditor.SelectedFsm);
					this.DoLargeWatermarkText(text);
					if (FsmEditorSettings.ShowFsmDescriptionInGraphView)
					{
						this.DrawFsmDescription(SkillEditor.SelectedFsm);
					}
				}
			}
			if (!Application.get_isPlaying() && SkillEditor.SelectedFsmComponent != null && SkillEditor.SelectedFsmComponent.get_FsmTemplate() != null && GUI.Button(this.view, Strings.get_FsmGraphView_Click_to_Edit_Template(), SkillEditorStyles.CenteredLabel))
			{
				SkillEditor.SelectFsm(SkillEditor.SelectedFsmComponent.get_FsmTemplate().fsm);
			}
		}
		private void DrawFsmDescription(Skill fsm)
		{
			if (this.canvasView.TakingScreenshot && !this.canvasView.ScreenshotFirstTile)
			{
				return;
			}
			Rect rect = default(Rect);
			if (!string.IsNullOrEmpty(fsm.get_Description()))
			{
				SkillEditorContent.FsmDescription.set_text(fsm.get_Description());
				rect.set_height(SkillEditorStyles.SmallWatermarkText.CalcHeight(SkillEditorContent.FsmDescription, 200f));
				GUI.Box(new Rect(this.view.get_x() + 5f, this.view.get_y() + 40f, 200f, rect.get_height()), fsm.get_Description(), SkillEditorStyles.SmallWatermarkText);
				rect.set_height(rect.get_height() + (this.view.get_y() + 40f));
			}
		}
		private void DrawWatermark()
		{
			Texture texture = SkillEditorStyles.DefaultWatermark;
			if (this.canvasView.TakingScreenshot && !this.canvasView.ScreenshotFirstTile)
			{
				return;
			}
			if (SkillEditor.SelectedFsm != null)
			{
				texture = Watermarks.Get(SkillEditor.SelectedFsm);
			}
			if (texture == null || this.canvasView.TakingScreenshot)
			{
				return;
			}
			Color color = GUI.get_color();
			GUI.set_color(SkillEditorStyles.WatermarkTint);
			GUI.Box(new Rect(this.view.get_x(), this.view.get_y(), this.view.get_width(), this.view.get_height() - EditorStyles.get_toolbar().get_fixedHeight()), texture, SkillEditorStyles.Watermark);
			GUI.set_color(color);
		}
		private void DrawStateHistory(float currentTime, float timeWindow)
		{
			if (!EditorApplication.get_isPlaying())
			{
				return;
			}
			float num = currentTime - timeWindow;
			FsmGraphView.highlightedStates.Clear();
			using (List<SkillLogEntry>.Enumerator enumerator = SkillEditor.SelectedFsm.get_MyLog().get_Entries().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillLogEntry current = enumerator.get_Current();
					if (current.get_Time() > currentTime)
					{
						break;
					}
					if (current.get_Time() >= num && (current.get_LogType() == 6 || current.get_LogType() == 5) && current.get_State() != null && !FsmGraphView.highlightedStates.Contains(current.get_State()))
					{
						SkillEditor.Repaint(false);
						if (this.eventType == 7)
						{
							Color activeHighlightColor = SkillEditorStyles.ActiveHighlightColor;
							activeHighlightColor.a = (current.get_Time() - num) / timeWindow;
							FsmGraphView.DrawStateHighlight(this.ScaleRect(current.get_State().get_Position()), activeHighlightColor);
							FsmGraphView.highlightedStates.Add(current.get_State());
						}
					}
				}
			}
		}
		private void DrawTransitionHistory(float currentTime, float timeWindow)
		{
			if (!EditorApplication.get_isPlaying() || this.eventType != 7)
			{
				return;
			}
			float num = currentTime - timeWindow;
			Color color = (!EditorApplication.get_isPaused()) ? SkillEditorStyles.ActiveHighlightColor : SkillEditorStyles.PausedHighlightColor;
			float width = SkillEditorStyles.LinkWidths[2];
			FsmGraphView.highlightedTransitions.Clear();
			using (List<SkillLogEntry>.Enumerator enumerator = SkillEditor.SelectedFsm.get_MyLog().get_Entries().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SkillLogEntry current = enumerator.get_Current();
					if (current.get_LogType() == 4 && current.get_Time() >= num)
					{
						if (current.get_Time() > currentTime)
						{
							break;
						}
						SkillTransition transition = current.get_Transition();
						if (transition != null && !FsmGraphView.highlightedTransitions.Contains(transition))
						{
							SkillState state = current.get_State();
							SkillState state2 = SkillEditor.SelectedFsm.GetState(transition.get_ToState());
							color.a = (current.get_Time() - num) / timeWindow;
							if (state != null)
							{
								int transitionIndex = SkillEditor.Builder.GetTransitionIndex(state, transition);
								this.DrawLink(state, state2, transitionIndex, color, width);
							}
							else
							{
								this.DrawGlobalArrow(state2, color);
							}
							FsmGraphView.highlightedTransitions.Add(transition);
						}
					}
				}
			}
		}
		private void DrawLastTransition()
		{
			SkillLogEntry lastTransition = DebugFlow.GetLastTransition();
			if (lastTransition == null)
			{
				return;
			}
			if (lastTransition.get_Fsm() != SkillEditor.SelectedFsm)
			{
				return;
			}
			SkillTransition transition = lastTransition.get_Transition();
			if (transition == null)
			{
				return;
			}
			SkillState state = lastTransition.get_State();
			SkillState state2 = SkillEditor.SelectedFsm.GetState(transition.get_ToState());
			if (state2 == null)
			{
				return;
			}
			Color color = DebugFlow.Active ? SkillEditorStyles.PausedHighlightColor : SkillEditorStyles.ActiveHighlightColor;
			float width = SkillEditorStyles.LinkWidths[2];
			if (state != null)
			{
				int transitionIndex = SkillEditor.Builder.GetTransitionIndex(state, transition);
				this.DrawLink(state, state2, transitionIndex, color, width);
				return;
			}
			this.DrawGlobalArrow(state2, color);
		}
		private void DrawFrame()
		{
			if (this.eventType == 7)
			{
				Color color = GUI.get_color();
				DrawState drawState = FsmDrawState.GetDrawState(SkillEditor.SelectedFsm);
				GUI.set_color(SkillEditorStyles.HighlightColors[(int)drawState]);
				SkillEditorStyles.InnerGlowBox.Draw(this.view, false, false, false, false);
				GUI.set_color(color);
			}
		}
		private void DrawCanvas()
		{
			if (SkillEditor.SelectedFsm == null)
			{
				return;
			}
			Color color = GUI.get_color();
			GUI.set_color(SkillEditorStyles.MinimapFrameColor);
			if (FsmEditorSettings.DrawFrameAroundGraph)
			{
				Rect rect = new Rect(0f, 0f, this.graphSize.x * this.zoom, this.graphSize.y * this.zoom);
				GUI.Box(rect, GUIContent.none, SkillEditorStyles.SinglePixelFrame);
			}
			if (this.canvasView.TakingScreenshot)
			{
				Texture texture = Watermarks.Get(SkillEditor.SelectedFsm);
				if (texture != null)
				{
					GUI.set_color(SkillEditorStyles.WatermarkTint);
					GUI.Box(new Rect(0f, 0f, this.canvasView.ContentArea.get_width(), this.canvasView.ContentArea.get_height()), texture, SkillEditorStyles.Watermark);
				}
			}
			GUI.set_color(color);
		}
		private void DrawHintBox()
		{
			if (this.eventType == 7 && FsmEditorSettings.ShowHints)
			{
				if (SkillEditor.SelectedFsm != null)
				{
					Rect rect = default(Rect);
					rect.set_x(this.view.get_xMax() - (SkillEditorContent.HintGraphShortcutsSize.x + 20f));
					rect.set_y(this.view.get_yMax() - (SkillEditorContent.HintGraphShortcutsSize.y + 20f));
					rect.set_width(SkillEditorContent.HintGraphShortcutsSize.x);
					rect.set_height(SkillEditorContent.HintGraphShortcutsSize.y);
					Rect rect2 = rect;
					Rect rect3 = default(Rect);
					rect3.set_x(this.view.get_xMax() - (SkillEditorContent.HintGraphCommandsSize.x + 20f));
					rect3.set_y(this.view.get_yMax() - (SkillEditorContent.HintGraphCommandsSize.y + 20f));
					rect3.set_width(SkillEditorContent.HintGraphCommandsSize.x);
					rect3.set_height(SkillEditorContent.HintGraphCommandsSize.y);
					Rect rect4 = rect3;
					rect4.set_x(rect4.get_x() - rect2.get_width());
					rect4.set_width(rect4.get_width() + rect2.get_width());
					GUI.Box(rect4, SkillEditorContent.HintGraphCommands, SkillEditorStyles.HintBox);
					GUI.Box(rect2, SkillEditorContent.HintGraphShortcuts, SkillEditorStyles.HintBoxTextOnly);
					return;
				}
				Rect rect5 = default(Rect);
				rect5.set_x(this.view.get_xMax() - (SkillEditorContent.HintGettingStartedSize.x + 20f));
				rect5.set_y(this.view.get_yMax() - (SkillEditorContent.HintGettingStartedSize.y + 20f));
				rect5.set_width(SkillEditorContent.HintGettingStartedSize.x);
				rect5.set_height(SkillEditorContent.HintGettingStartedSize.y);
				Rect rect6 = rect5;
				GUI.Box(rect6, SkillEditorContent.HintGettingStarted, SkillEditorStyles.HintBox);
			}
		}
		private static void DrawShadow(Rect rect)
		{
			Color color = GUI.get_color();
			GUI.set_color(Color.get_black());
			SkillEditorStyles.DropShadowBox.Draw(rect, false, false, false, false);
			GUI.set_color(color);
		}
		private static void DrawStateHighlight(Rect rect, Color color)
		{
			Color color2 = GUI.get_color();
			GUI.set_color(color);
			SkillEditorStyles.SelectionBox.Draw(rect, false, false, false, false);
			GUI.set_color(color2);
		}
		private void DrawSelectionRect()
		{
			float num = Mathf.Min(this.dragStartPos.x, this.currentMousePos.x);
			float num2 = Mathf.Min(this.dragStartPos.y, this.currentMousePos.y);
			float num3 = Mathf.Abs(this.currentMousePos.x - this.dragStartPos.x);
			float num4 = Mathf.Abs(this.currentMousePos.y - this.dragStartPos.y);
			Rect rect = new Rect(num, num2, num3, num4);
			SkillEditorStyles.SelectionRect.Draw(rect, false, false, false, false);
		}
		private void DrawLink(SkillState fromState, SkillState toState, int i, Color color, float width)
		{
			if (toState == null)
			{
				return;
			}
			SkillTransition fsmTransition = fromState.get_Transitions()[i];
			GraphViewLinkStyle graphViewLinkStyle = FsmEditorSettings.GraphViewLinkStyle;
			switch (fsmTransition.get_LinkStyle())
			{
			case 1:
				graphViewLinkStyle = GraphViewLinkStyle.BezierLinks;
				break;
			case 2:
				graphViewLinkStyle = GraphViewLinkStyle.CircuitLinks;
				break;
			}
			switch (graphViewLinkStyle)
			{
			case GraphViewLinkStyle.BezierLinks:
				BezierLink.Draw(fromState, toState, i, color, width, SkillEditorStyles.LeftArrow, SkillEditorStyles.RightArrow, this.zoom);
				return;
			case GraphViewLinkStyle.CircuitLinks:
				CircuitLink.Draw(fromState, toState, i, color, width, SkillEditorStyles.LeftArrow, SkillEditorStyles.RightArrow, this.zoom);
				return;
			default:
				return;
			}
		}
		private void DrawBreakpoint(SkillState state)
		{
			Rect rect = this.ScaleRect(state.get_Position());
			rect.set_width((float)SkillEditorStyles.BreakpointOff.get_normal().get_background().get_width());
			rect.set_height((float)SkillEditorStyles.BreakpointOff.get_normal().get_background().get_height() * this.zoom);
			if (FsmEditorSettings.BreakpointsEnabled && SkillEditor.SelectedFsm.EnableBreakpoints)
			{
				SkillEditorStyles.BreakpointOn.Draw(rect, false, false, false, false);
				return;
			}
			SkillEditorStyles.BreakpointOff.Draw(rect, false, false, false, false);
		}
		private void DrawStateError(SkillState state)
		{
			Rect rect = this.ScaleRect(state.get_Position());
			rect.set_y(rect.get_y() - 6f);
			rect.set_x(rect.get_x() - 5f);
			float width;
			rect.set_height(width = 14f);
			rect.set_width(width);
			GUI.DrawTexture(rect, SkillEditorStyles.StateErrorIcon);
		}
		[Localizable(false)]
		private void DoGameStateIcon()
		{
			if (SkillEditor.SelectedFsm == null || !EditorApplication.get_isPlaying())
			{
				return;
			}
			float num = (float)FsmEditorSettings.GameStateIconSize;
			Rect rect = new Rect(10f, this.view.get_height() - num, num, num + 20f);
			if (SkillEditor.SelectedFsm.get_Active() && !SkillEditor.SelectedFsm.get_Finished())
			{
				Texture2D texture2D = SkillEditorStyles.GetGameStateIcons()[(int)GameStateTracker.CurrentState];
				if (texture2D != null && GUI.Button(rect, texture2D, GUIStyle.get_none()))
				{
					switch (GameStateTracker.CurrentState)
					{
					case GameState.Running:
						EditorApplication.set_isPaused(true);
						break;
					case GameState.Break:
					case GameState.Error:
						SkillEditor.GotoBreakpoint();
						break;
					case GameState.Paused:
						EditorApplication.set_isPaused(false);
						break;
					}
				}
			}
			Color color = GUI.get_color();
			DrawState drawState = FsmDrawState.GetDrawState(SkillEditor.SelectedFsm);
			GUI.set_color(SkillEditorStyles.HighlightColors[(int)drawState]);
			rect.set_y(rect.get_y() - 3f);
			rect.set_width(this.view.get_width() - rect.get_width());
			string text;
			if (!SkillEditor.SelectedFsm.get_Active())
			{
				rect.set_x(5f);
				text = Strings.get_Label_DISABLED();
				GUI.set_color(SkillEditorStyles.LargeWatermarkText.get_normal().get_textColor());
			}
			else
			{
				if (SkillEditor.SelectedFsm.get_Finished())
				{
					rect.set_x(5f);
					text = Strings.get_Label_FINISHED();
					GUI.set_color(SkillEditorStyles.LargeWatermarkText.get_normal().get_textColor());
				}
				else
				{
					rect.set_x(45f);
					if (DebugFlow.ActiveAndScrubbing)
					{
						text = ((DebugFlow.DebugState != null) ? DebugFlow.DebugState.get_Name() : (" " + Strings.get_Label_None_In_Table()));
					}
					else
					{
						text = SkillEditor.SelectedFsm.get_ActiveStateName();
					}
				}
			}
			GUI.Box(rect, text, SkillEditorStyles.LargeText);
			GUI.set_color(color);
		}
		private void DrawGlobalArrow(SkillState state, Color color)
		{
			if (state == null)
			{
				return;
			}
			Rect rect = this.ScaleRect(state.get_Position());
			Rect rect2 = new Rect(rect);
			rect2.set_height((float)SkillEditorStyles.GlobalArrow.get_height() * this.zoom);
			rect2.set_width((float)SkillEditorStyles.GlobalArrow.get_width() * this.zoom);
			Rect position = rect2;
			position.set_y(position.get_y() - position.get_height());
			position.set_x(position.get_x() + rect.get_width() * 0.5f - position.get_width() * 0.5f);
			GUIHelpers.DrawTexture(position, SkillEditorStyles.GlobalArrow, color, 0);
		}
		private void DoMinimap()
		{
			if (!FsmEditorSettings.GraphViewShowMinimap || SkillEditor.SelectedFsm == null || this.canvasView.TakingScreenshot)
			{
				return;
			}
			float graphViewMinimapSize = FsmEditorSettings.GraphViewMinimapSize;
			Color color = GUI.get_color();
			float num = this.graphExpanded ? 100000f : 0f;
			float num2 = this.graphSize.x - num * 2f;
			float num3 = this.graphSize.y - num * 2f;
			float num4 = Mathf.Min(new float[]
			{
				graphViewMinimapSize / num2,
				graphViewMinimapSize / num3,
				0.15f
			});
			float num5 = num4 / this.zoom;
			Rect rect = new Rect(this.view.get_width() - num2 * num4 - 20f, this.view.get_y() + 20f, num2 * num4, num3 * num4);
			Vector2 vector = (this.canvasView.ScrollPosition - this.canvasView.ContentOrigin - new Vector2(num, num)) * num5;
			GUI.set_color(SkillEditorStyles.MinimapFrameColor);
			GUI.BeginGroup(rect, SkillEditorStyles.SinglePixelFrame);
			GUI.set_color(SkillEditorStyles.MinimapViewRectColor);
			GUI.Box(new Rect(vector.x, vector.y, this.view.get_width() * num5, this.view.get_height() * num5), GUIContent.none, SkillEditorStyles.SelectionRect);
			SkillState[] states = SkillEditor.SelectedFsm.get_States();
			for (int i = 0; i < states.Length; i++)
			{
				SkillState fsmState = states[i];
				if (Application.get_isPlaying() && fsmState == SkillEditor.SelectedFsm.get_ActiveState())
				{
					DrawState fsmStateDrawState = FsmDrawState.GetFsmStateDrawState(SkillEditor.SelectedFsm, fsmState, false);
					GUI.set_color(SkillEditorStyles.HighlightColors[(int)fsmStateDrawState]);
				}
				else
				{
					GUI.set_color(PlayMakerPrefs.get_MinimapColors()[fsmState.get_ColorIndex()]);
				}
				GUI.DrawTexture(FsmGraphView.ScaleRect(new Rect(fsmState.get_Position().get_x() - num, fsmState.get_Position().get_y() - num, fsmState.get_Position().get_width(), fsmState.get_Position().get_height()), num4, new Vector2(10f, 4f)), EditorGUIUtility.get_whiteTexture());
			}
			GUI.set_color(color);
			if (this.draggingMode == FsmGraphView.DraggingMode.Minimap || (this.eventType == null && this.currentEvent.get_button() == 0 && new Rect(0f, 0f, rect.get_width(), rect.get_height()).Contains(this.currentEvent.get_mousePosition())))
			{
				this.canvasView.CancelAutoPan();
				this.canvasView.SetScrollPosition(this.canvasView.ContentOrigin + this.currentEvent.get_mousePosition() / num5 - this.canvasView.ViewCenter);
				this.draggingMode = FsmGraphView.DraggingMode.Minimap;
				this.UpdateVisibility();
			}
			GUI.EndGroup();
		}
		public void AddState()
		{
			this.AddState(this.contextMenuPos / this.zoom);
		}
		public SkillState AddState(Vector2 position)
		{
			SkillEditor.RegisterUndo(Strings.get_Command_Add_State());
			SkillState fsmState = SkillEditor.Builder.AddState(position / this.zoom);
			this.UpdateStateSize(fsmState);
			FsmGraphView.UpdateStatePosition(fsmState, position);
			SkillEditor.SelectState(fsmState, false);
			if (FsmEditorSettings.SnapToGrid)
			{
				FsmGraphView.SnapSelectedStatesToGrid();
			}
			this.UpdateGraphBounds(this.contextMenuPos, 100f);
			SkillEditor.SetFsmDirty(true, false);
			return fsmState;
		}
		public void PasteStates()
		{
			EditorCommands.PasteStates(this.contextMenuPos / this.zoom);
			if (FsmEditorSettings.SnapToGrid)
			{
				FsmGraphView.SnapSelectedStatesToGrid();
			}
			this.UpdateGraphBounds(this.contextMenuPos, 100f);
		}
		public void PasteTemplate(object userdata)
		{
			SkillTemplate template = userdata as SkillTemplate;
			EditorCommands.PasteTemplate(template, this.contextMenuPos / this.zoom);
			if (FsmEditorSettings.SnapToGrid)
			{
				FsmGraphView.SnapSelectedStatesToGrid();
			}
			this.UpdateGraphBounds(this.contextMenuPos, 100f);
		}
		private static void SetSelectedTransitionTarget(SkillState toState)
		{
			EditorCommands.SetTransitionTarget(SkillEditor.SelectedTransition, toState.get_Name());
		}
		public void DeleteGlobalTransition(object userdata)
		{
			SkillTransition transition = userdata as SkillTransition;
			EditorCommands.DeleteGlobalTransition(transition);
		}
		public static Vector2 GetViewCenter()
		{
			Rect rect = SkillEditor.GraphView.view;
			Vector2 vector = new Vector2(SkillEditor.Selection.ScrollPosition.x + rect.get_width() * 0.5f, SkillEditor.Selection.ScrollPosition.y + rect.get_height() * 0.5f);
			return vector / SkillEditor.GraphView.zoom;
		}
		public static void TranslateState(SkillState state, Vector2 offset)
		{
			Vector2 position = new Vector2(state.get_Position().get_x(), state.get_Position().get_y()) + offset;
			FsmGraphView.UpdateStatePosition(state, position);
		}
		public static void UpdateStatePosition(SkillState state, Vector2 position)
		{
			Rect position2 = new Rect(state.get_Position());
			position2.set_x(position.x);
			position2.set_y(position.y);
			state.set_Position(position2);
		}
		public static Vector2 MoveAllStatesToOrigin(Skill fsm, float padding)
		{
			if (fsm == null)
			{
				return Vector2.get_zero();
			}
			Vector2 vector = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
			SkillState[] states = fsm.get_States();
			for (int i = 0; i < states.Length; i++)
			{
				SkillState fsmState = states[i];
				if (fsmState.get_Position().get_x() < vector.x)
				{
					vector.x = fsmState.get_Position().get_x();
				}
				if (fsmState.get_Position().get_y() < vector.y)
				{
					vector.y = fsmState.get_Position().get_y();
				}
			}
			vector.x -= padding * 0.5f;
			vector.y -= padding;
			vector.x = Mathf.Floor(vector.x / (float)FsmEditorSettings.SnapGridSize) * (float)FsmEditorSettings.SnapGridSize;
			vector.y = Mathf.Floor(vector.y / (float)FsmEditorSettings.SnapGridSize) * (float)FsmEditorSettings.SnapGridSize;
			FsmGraphView.MoveAllStates(fsm, -vector);
			return vector;
		}
		public static void MoveAllStates(Skill fsm, Vector2 delta)
		{
			SkillState[] states = fsm.get_States();
			for (int i = 0; i < states.Length; i++)
			{
				SkillState fsmState = states[i];
				SkillState arg_5A_0 = fsmState;
				Rect position = new Rect(fsmState.get_Position());
				position.set_x(fsmState.get_Position().get_x() + delta.x);
				position.set_y(fsmState.get_Position().get_y() + delta.y);
				arg_5A_0.set_Position(position);
			}
			SkillEditor.SetFsmDirty(false, false);
		}
		public void UpdateStateSize(SkillState state)
		{
			if (state == null)
			{
				return;
			}
			Rect rect = new Rect(state.get_Position());
			rect.set_width(FsmGraphView.CalculateStateWidth(state));
			Rect position = rect;
			int num = state.get_Transitions().Length + 1;
			position.set_height((float)num * 16f);
			state.set_Position(position);
			FsmGraphView.UpdateStatePosition(state, new Vector2(state.get_Position().get_x(), state.get_Position().get_y()));
			this.UpdateGraphSize();
		}
		private static float CalculateStateWidth(SkillState state)
		{
			float num = SkillEditorStyles.DefaultStateBoxStyle.CalcSize(new GUIContent(state.get_Name())).x;
			SkillTransition[] transitions = state.get_Transitions();
			for (int i = 0; i < transitions.Length; i++)
			{
				SkillTransition fsmTransition = transitions[i];
				float x = SkillEditorStyles.DefaultStateBoxStyle.CalcSize(new GUIContent(fsmTransition.get_EventName())).x;
				if (x > num)
				{
					num = x;
				}
			}
			SkillTransition[] globalTransitions = state.get_Fsm().get_GlobalTransitions();
			for (int j = 0; j < globalTransitions.Length; j++)
			{
				SkillTransition fsmTransition2 = globalTransitions[j];
				if (fsmTransition2.get_ToState() == state.get_Name())
				{
					float x2 = SkillEditorStyles.DefaultStateBoxStyle.CalcSize(new GUIContent(fsmTransition2.get_EventName())).x;
					if (x2 > num)
					{
						num = x2;
					}
				}
			}
			if (FsmEditorSettings.ShowCommentsInGraphView && !string.IsNullOrEmpty(state.get_Description()))
			{
				float x3 = SkillEditorStyles.DefaultStateBoxStyle.CalcSize(new GUIContent(state.get_Description())).x;
				if (x3 > num)
				{
					num = (num + x3) * 0.5f;
				}
			}
			return Mathf.Clamp(num + 20f, 100f, (float)FsmEditorSettings.StateMaxWidth);
		}
		public void UpdateAllStateSizes()
		{
			using (List<Skill>.Enumerator enumerator = SkillEditor.FsmList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Skill current = enumerator.get_Current();
					this.UpdateStateSizes(current);
				}
			}
			SkillEditor.Repaint(false);
		}
		public void UpdateStateSizes(Skill fsm)
		{
			if (fsm != null)
			{
				SkillState[] states = fsm.get_States();
				for (int i = 0; i < states.Length; i++)
				{
					SkillState fsmState = states[i];
					fsmState.set_Fsm(fsm);
					this.UpdateStateSize(fsmState);
				}
			}
		}
		public static void ToggleLinkStyle(SkillTransition transition)
		{
			if (transition != null)
			{
				SkillEditor.RegisterUndo(Strings.get_Command_Set_Link_Style());
				switch (transition.get_LinkStyle())
				{
				case 0:
					transition.set_LinkStyle((FsmEditorSettings.GraphViewLinkStyle == GraphViewLinkStyle.BezierLinks) ? 2 : 1);
					break;
				case 1:
					transition.set_LinkStyle(2);
					break;
				case 2:
					transition.set_LinkStyle(1);
					break;
				}
				SkillEditor.SetFsmDirty(false, false);
			}
		}
		public static void SetLinkStyle(SkillTransition transition, SkillTransition.CustomLinkStyle linkStyle)
		{
			if (transition != null)
			{
				transition.set_LinkStyle(linkStyle);
			}
		}
		public static void SetLinkConstraint(SkillTransition transition, SkillTransition.CustomLinkConstraint linkConstraint)
		{
			if (transition != null)
			{
				SkillEditor.RegisterUndo(Strings.get_Command_Set_Link_Constraint());
				transition.set_LinkConstraint(linkConstraint);
				SkillEditor.SetFsmDirty(false, false);
			}
		}
		private void InitScale(float scale)
		{
			SkillEditorStyles.InitScale(scale);
			this.zoom = scale;
		}
		private void SetScale(float scale)
		{
			SkillEditor.Selection.Zoom = scale;
			if (Math.Abs(scale - this.zoom) > Mathf.Epsilon)
			{
				this.zoom = Mathf.Clamp(scale, 0.3f, 1f);
				SkillEditorStyles.SetScale(this.zoom);
				SkillEditor.Repaint(true);
				this.UpdateVisibility();
			}
		}
		public void ApplySettings()
		{
			this.canvasView.MouseWheelZoomsView = !FsmEditorSettings.MouseWheelScrollsGraphView;
			this.canvasView.MinScale = 0.3f;
			this.canvasView.MaxScale = 1f;
			this.canvasView.ZoomSpeed = FsmEditorSettings.GraphViewZoomSpeed;
			this.canvasView.EdgeScrollSpeed = FsmEditorSettings.EdgeScrollSpeed;
			this.canvasView.EdgeScrollZone = FsmEditorSettings.EdgeScrollZone;
			SkillEditor.Repaint(true);
		}
	}
}
