using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace EpicXRCrossPlatformInput{

    public class XR_UI_Raycaster : MonoBehaviour
    {
        // Public Vars
        [Header("Raycast Settings")]
        public float RaycastDistance = Mathf.Infinity;
        public LayerMask UILayer;

        [Header("Input Settings")]
        public ButtonTypes UIInteractionButton = ButtonTypes.Trigger;

        [Header("Pointer")]
        public GameObject PointerPrefab;


        // Private Vars
        private GameObject pointer;
        private bool inputPressed = false;
        private bool inputUp = false;
        private Controller controller;
        private bool wasPointingAtLocation = false;
        private Transform hitElement;

        // UI element state  related stuff

        private bool isButtonSelected = false;
        private bool isToggleSelected = false;
        private bool isSliderSelected = false;
        private bool isScrollBarSelected = false;


        private void Start()
        {
            controller = GetComponent<Controller>();
            if(controller == null)
            {
                Debug.LogError("No controller script found please attatch one");
            }
            pointer = GameObject.Instantiate(PointerPrefab, this.transform);
            if (pointer == null)
            {
                Debug.LogError("No pointer prefab assigned on XR_UI_Raycaster please assign one to the gameobejct :" + this.transform.name);
            }
            else
            {
                pointer.transform.localPosition = Vector3.zero;
                pointer.transform.localRotation = Quaternion.identity;
                pointer.transform.localPosition = new Vector3(0, 0, pointer.transform.localScale.z / 2);
                pointer.SetActive(false);
            }
        }
        private void Update()
        {
            GetInput();
            CastRay();
        }


        private void GetInput()
        {
            inputPressed = XRCrossPlatformInputManager.Instance.GetInputByButton(UIInteractionButton,controller.Hand,true);
            inputUp = XRCrossPlatformInputManager.Instance.GetInputUp(UIInteractionButton, controller.Hand);
        }

        private void CastRay()
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, RaycastDistance, UILayer))
            {
                // pointer stuff
                pointer.SetActive(true);
                pointer.transform.localScale = new Vector3(pointer.transform.localScale.x, pointer.transform.localScale.y, hit.distance);
                pointer.transform.localPosition = new Vector3(0, 0, pointer.transform.localScale.z / 2);

                hitElement = hit.transform;
                HandleButtonRaycast(hit);
                wasPointingAtLocation = true;
            }
            else
            {
                pointer.SetActive(false);
                TurnOffBeam();
                UnselectButton();
            }
        }

        /// <summary>
        /// Handles raycasting specifically buttons
        /// </summary>
        private void HandleButtonRaycast(RaycastHit hit )
        {
            Transform hitUI = hit.transform;
            if (hitUI.GetComponent<Button>())
            {
                Button button = hitUI.GetComponent<Button>();

                if (inputPressed)
                {
                    EventSystem.current.SetSelectedGameObject(null);
                    isButtonSelected = false;
                }
                else if (!isButtonSelected)
                {
                    button.Select();
                    isButtonSelected = true;
                }

                if (inputUp)
                {
                    button.onClick.Invoke(); // Boom we clicked the button
                }
            }
            else if(hitUI.GetComponent<Toggle>())
            {
                Toggle toggle = hitUI.GetComponent<Toggle>();
                if (inputPressed)
                {
                    EventSystem.current.SetSelectedGameObject(null);
                    isToggleSelected = false;
                }
                else if (!isButtonSelected)
                {
                    toggle.Select();
                    isToggleSelected = true;
                }

                if (inputUp)
                {
                    toggle.isOn = !toggle.isOn;
                }
            }
            else if (hitUI.GetComponent<UnityEngine.UI.Slider>())
            {
                UnityEngine.UI.Slider slider = hitUI.GetComponent<UnityEngine.UI.Slider>();
                if (inputPressed)
                {
                    RectTransform rectTransform = slider.transform.GetComponent<RectTransform>();
                    float width = rectTransform.rect.width;
                    EventSystem.current.SetSelectedGameObject(null);
                    isSliderSelected = false;
                    Vector3 newSliderValue = slider.transform.InverseTransformPoint(hit.point);
                    float maxValue = (width / 2);
                    float minValue = (maxValue) * -1;
                    float value = Mathf.InverseLerp(minValue,maxValue,newSliderValue.x);
                    slider.value = value;
                }
                else if (!isButtonSelected)
                {
                    slider.Select();
                    isSliderSelected = true;
                }
            }
            else if (hitUI.GetComponent<Scrollbar>())
            {
                Scrollbar scrollbar = hitUI.GetComponent<Scrollbar>();
                if (inputPressed)
                {
                    RectTransform rectTransform = scrollbar.transform.GetComponent<RectTransform>();
                    EventSystem.current.SetSelectedGameObject(null);
                    isScrollBarSelected = false;
                    Vector3 newScrollbarValue = scrollbar.transform.InverseTransformPoint(hit.point);

                    float value = 0;
                    if (scrollbar.direction == Scrollbar.Direction.LeftToRight )
                    {
                        float width = rectTransform.rect.width;
                        float maxValue = (width / 2);
                        float minValue = (maxValue) * -1;
                        value = Mathf.InverseLerp(minValue, maxValue, newScrollbarValue.x);
                    }
                    else if(scrollbar.direction == Scrollbar.Direction.RightToLeft)
                    {
                        float width = rectTransform.rect.width;
                        float maxValue = (width / 2);
                        float minValue = (maxValue) * -1;
                        value = Mathf.InverseLerp(maxValue,minValue , newScrollbarValue.x);
                    }
                    else if (scrollbar.direction == Scrollbar.Direction.TopToBottom)
                    {
                        float height = rectTransform.rect.height;
                        float maxValue = (height / 2);
                        float minValue = (maxValue) * -1;
                        value = Mathf.InverseLerp(maxValue, minValue, newScrollbarValue.y);

                    }
                    else if (scrollbar.direction == Scrollbar.Direction.BottomToTop)
                    {
                        float height = rectTransform.rect.height;
                        float maxValue = (height / 2);
                        float minValue = (maxValue) * -1;
                        value = Mathf.InverseLerp(minValue, maxValue, newScrollbarValue.y);

                    }

                    if (hitUI.GetComponentInParent<ScrollRect>())
                    {
                        float height = rectTransform.rect.height;
                        float maxValue = 0;
                        float minValue = (height) * -1;
                        value = Mathf.InverseLerp(minValue, maxValue, newScrollbarValue.y);
                    }
               
                    scrollbar.value = value;
                }
                else if (!isButtonSelected)
                {
                    scrollbar.Select();
                    isScrollBarSelected = true;
                }

            }
        }
        private void TurnOffBeam()
        {
            if (wasPointingAtLocation)
            {
                wasPointingAtLocation = false;
            }
        }

        private void UnselectButton()
        {
            if (hitElement != null && isButtonSelected)
            {
                EventSystem.current.SetSelectedGameObject(null);
                isButtonSelected = false;
            }
        }

    }
}
