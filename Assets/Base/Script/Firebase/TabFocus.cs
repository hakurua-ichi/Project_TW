using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class TabFocus : MonoBehaviour
{
    private Selectable[] _selectables;
    private int _currentIndex = -1;

    public void Initialize(params Selectable[] uiElements)
    {
        _selectables = uiElements.Where(s => s != null && s.gameObject.activeInHierarchy && s.interactable).ToArray();
        _currentIndex = -1;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && _selectables != null && _selectables.Length > 0)
        {
            var activeAndInteractableSelectables = _selectables
                .Where(s => s != null && s.gameObject.activeInHierarchy && s.interactable)
                .ToList();

            if (activeAndInteractableSelectables.Count == 0) return;

            _currentIndex = -1;
            GameObject currentSelectedGO = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
            if (currentSelectedGO != null)
            {
                for (int i = 0; i < activeAndInteractableSelectables.Count; i++)
                {
                    if (activeAndInteractableSelectables[i].gameObject == currentSelectedGO)
                    {
                        _currentIndex = i;
                        break;
                    }
                }
            }

            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                if (_currentIndex == -1)
                {
                    _currentIndex = activeAndInteractableSelectables.Count - 1;
                }
                else
                {
                    _currentIndex--;
                    if (_currentIndex < 0)
                    {
                        _currentIndex = activeAndInteractableSelectables.Count - 1;
                    }
                }
            }
            else
            {
                _currentIndex++;
                if (_currentIndex >= activeAndInteractableSelectables.Count)
                {
                    _currentIndex = 0;
                }
            }

            if (_currentIndex >= 0 && _currentIndex < activeAndInteractableSelectables.Count)
            {
                activeAndInteractableSelectables[_currentIndex].Select();
            }
        }
    }
}