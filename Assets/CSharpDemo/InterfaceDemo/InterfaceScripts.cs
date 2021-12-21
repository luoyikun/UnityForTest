using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InterfaceDemo
{
    public class InterfaceScripts : MonoBehaviour, IMyInterface
    {
        public void MethodToImplement()
        {
            Debug.Log("InterfaceScripts");
        }

        // Start is called before the first frame update
        void Start()
        {
            AbChild child = new AbChild();
            child.TestA();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

    interface IMyInterface
    {
        // �ӿڳ�Ա
        void MethodToImplement();
    }

    public abstract class AbBase  //������
    {
        public int NumberA = 100;
        public int NumberB = 200;

        public void TestA()
        {
            Debug.Log("TestA");
        }
        public abstract void Swap();    //���󷽷�

        public abstract int getNumberA { get; } //���󷽷�
        public abstract int getNumberB { get; } //���󷽷�

    }

    public class AbChild : AbBase
    {
        public override int getNumberA
        {
            get {
                return NumberA;
            }
            
        }

        public override int getNumberB
        {
            get
            {
                return NumberB;
            }
        }

        public override void Swap()
        {
            int temp = NumberA;
            NumberA = NumberB;
            NumberB = temp;
        }

        public void TestA()
        {
            base.TestA();
        }
    }

}


