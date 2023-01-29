using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

namespace UI.ThreeDimensional.Examples
{
    public class ExampleController : MonoBehaviour
    {
        public Canvas Canvas;
        public List<GameObject> Examples = new List<GameObject>();

        public GameObject WordSpaceText;

        public List<UIObject3D> gridItems = new List<UIObject3D>();        

        public void SelectExample(int number)
        {
            Examples.ForEach(e => e.SetActive(false));

            Examples[number].SetActive(true);
        }

        public void SetCanvasMode(int mode)
        {
            Canvas.renderMode = (RenderMode)mode;

            Canvas.transform.position = Vector3.zero;
            Canvas.transform.localScale = Vector3.one;
            Canvas.transform.rotation = Quaternion.identity;

            Camera.main.transform.position = new Vector3(0, 0, -500f);
            Camera.main.transform.rotation = Quaternion.identity;

            WordSpaceText.SetActive(Canvas.renderMode == RenderMode.WorldSpace);
        }

        private void EnsureGridItemsCollectionIsPopulated()
        {
            // ensure that the grid items collection is populated
            if (!gridItems.Any())
            {
                gridItems = Examples[0].GetComponentsInChildren<UIObject3D>().ToList();
            }            
        }

        public void ToggleGridItemOutlines(bool toggle)
        {
            EnsureGridItemsCollectionIsPopulated();

            foreach (var item in gridItems)
            {
                var outlineComponent = item.GetComponent<Outline>() ?? item.gameObject.AddComponent<Outline>();

                outlineComponent.enabled = toggle;
            }
        }       

        public void ToggleGridItemRotation(bool toggle)
        {
            EnsureGridItemsCollectionIsPopulated();

            foreach (var item in gridItems)
            {
                var rotationComponent = item.GetComponent<RotateUIObject3D>() ?? item.gameObject.AddComponent<RotateUIObject3D>();

                rotationComponent.enabled = toggle;
            }
        }

        public void ToggleImageColor(bool toggle)
        {
            EnsureGridItemsCollectionIsPopulated();

            foreach (var item in gridItems)
            {
                var imageComponent = item.GetComponent<Image>();

                imageComponent.color = toggle ? Color.green : Color.white;
            }
        }

    }
}
