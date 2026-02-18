using System;
using CodeExecutor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EditorUIAdaptor.Behaviours
{
    public class ResumeButton : MonoBehaviour
    {
        public EditorWindowHandler editor;
        public Button thisButton;
        public ColorBlock colorRunning;
        public ColorBlock colorWaiting;
        public Image icon;
        public Sprite runningImage;
        public Sprite pausedImage;
        public Sprite stoppedImage;

        private CodeService.WorkingState _runningState = CodeService.WorkingState.Stopped;
        public CodeService.WorkingState RunningState
        {
            get => _runningState;
            set { 
                thisButton.colors = value switch
                {
                    CodeService.WorkingState.Running => colorWaiting,
                    CodeService.WorkingState.Stepping => colorWaiting,
                    CodeService.WorkingState.Paused => colorWaiting,
                    CodeService.WorkingState.Stopped => colorRunning,
                    _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
                };
                //thisButton.targetGraphic.Rebuild(CanvasUpdate.MaxUpdateValue);
                _runningState = value;
                icon.sprite = _runningState switch
                {
                    CodeService.WorkingState.Running => runningImage,
                    CodeService.WorkingState.Stepping => runningImage,
                    CodeService.WorkingState.Paused => pausedImage,
                    CodeService.WorkingState.Stopped => stoppedImage,
                    _ => throw new ArgumentOutOfRangeException(nameof(_runningState), _runningState, null)
                };
                editor.GetTextEditor().disableInput = _runningState is not CodeService.WorkingState.Stopped;
            }
        }
        public void OnClickThis()
        {
            switch (RunningState)
            {
                case CodeService.WorkingState.Running:
                case CodeService.WorkingState.Stepping:
                    CodeService.Instance.StopExecute(); break;
                case CodeService.WorkingState.Paused:
                    CodeService.Instance.ResumeExecute(); break;
                case CodeService.WorkingState.Stopped:
                    CodeService.Instance.StartExecute(editor.GetWindowName()); break;
                default: throw new AccessViolationException("This should never happen!");
            }
            EventSystem.current.SetSelectedGameObject(UIBGMouseListener.Instance.gameObject);
        }
    }
}
