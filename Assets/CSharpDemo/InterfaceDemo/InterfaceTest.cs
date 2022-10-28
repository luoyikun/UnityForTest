using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace InterfaceDemo
{
    public class InterfaceTest : MonoBehaviour
    {
        public InterfaceScripts m_inSc;
        // Start is called before the first frame update
        void Start()
        {
            m_inSc.MethodToImplement();

            IMyInterface imyI = gameObject.GetComponent<IMyInterface>();
            imyI.MethodToImplement();
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}
