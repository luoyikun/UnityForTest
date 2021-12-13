#define HideInInspector

#if UNITY_EDITOR
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;


namespace EModules {
class DescriptionHelper : MonoBehaviour, IHashProperty {
#if HideInInspector
      [HideInInspector]   [SerializeField] List<GoGuidPair> HashFreezeHashKeys = new List<GoGuidPair>();
        [HideInInspector]   [SerializeField] List<bool> HashFreezeHashValues = new List<bool>();


        [HideInInspector] [SerializeField] List<GameObject> Hash1 = new List<GameObject>();
    [HideInInspector] [SerializeField] List<GoGuidPair> Hash1_Fix2_0 = new List<GoGuidPair>();
    [HideInInspector] [SerializeField] List<string> Hash2 = new List<string>();
    
    [HideInInspector] [SerializeField] List<Int32List> Hash3 = new List<Int32List>();
    [HideInInspector] [SerializeField] List<Int32List> Hash4 = new List<Int32List>();
    
    [HideInInspector] [SerializeField] List<GameObject> Hash5 = new List<GameObject>();
    [HideInInspector] [SerializeField] List<GoGuidPair> Hash5_Fix2_0 = new List<GoGuidPair>();
    [HideInInspector] [SerializeField] List<SingleList> Hash6 = new List<SingleList>();
    
    [HideInInspector] [SerializeField] List<GameObject> Hash7 = new List<GameObject>();
    [HideInInspector] [SerializeField] List<GoGuidPair> Hash7_Fix2_0 = new List<GoGuidPair>();
    [HideInInspector] [SerializeField] List<SingleList> Hash8 = new List<SingleList>();
    
    [HideInInspector] [SerializeField] List<GameObject> Hash9 = new List<GameObject>();
    [HideInInspector] [SerializeField] List<SingleList> Hash10 = new List<SingleList>();
    
    [HideInInspector] [SerializeField] private bool _enableRegistrator;
    [HideInInspector] [SerializeField] private string _saveInFolder;
    
    
    [HideInInspector] [SerializeField] List<HierarchySnapShotArray> Hash_HierarchyCache = new List<HierarchySnapShotArray>();
    [HideInInspector] [SerializeField] List<Int32ListArray> Hash_BookMarks = new List<Int32ListArray>();
    
    
    [HideInInspector] [SerializeField] int _mFavoritCategorySelected = 0;
    
    
    [HideInInspector] [SerializeField] List<GoGuidPair> m_GetHash_IconImageKey = new List<GoGuidPair>();
    [HideInInspector] [SerializeField] List<Int32List> m_GetHash_IconImageValue = new List<Int32List>();
#else

        [SerializeField] List<GoGuidPair> HashFreezeHashKeys = new List<GoGuidPair>();
          [SerializeField] List<bool> HashFreezeHashValues = new List<bool>();


    [SerializeField] List<GameObject> Hash1 = new List<GameObject>();
    [SerializeField] List<GoGuidPair> Hash1_Fix2_0 = new List<GoGuidPair>();
    [SerializeField] List<string> Hash2 = new List<string>();
    
    [SerializeField] List<Int32List> Hash3 = new List<Int32List>();
    [SerializeField] List<Int32List> Hash4 = new List<Int32List>();
    
    [SerializeField] List<GameObject> Hash5 = new List<GameObject>();
    [SerializeField] List<GoGuidPair> Hash5_Fix2_0 = new List<GoGuidPair>();
    [SerializeField] List<SingleList> Hash6 = new List<SingleList>();
    
    [SerializeField] List<GameObject> Hash7 = new List<GameObject>();
    [SerializeField] List<GoGuidPair> Hash7_Fix2_0 = new List<GoGuidPair>();
    [SerializeField] List<SingleList> Hash8 = new List<SingleList>();
    
    [SerializeField] List<GameObject> Hash9 = new List<GameObject>();
    [SerializeField] List<SingleList> Hash10 = new List<SingleList>();
    
    [SerializeField] private bool _enableRegistrator;
    [SerializeField] private string _saveInFolder;
    
    
    [SerializeField] List<HierarchySnapShotArray> Hash_HierarchyCache = new List<HierarchySnapShotArray>();
    [SerializeField] List<Int32ListArray> Hash_BookMarks = new List<Int32ListArray>();
    
    
    [SerializeField] int _mFavoritCategorySelected = 0;
    
    
    [SerializeField] List<GoGuidPair> m_GetHash_IconImageKey = new List<GoGuidPair>();
    [SerializeField] List<Int32List> m_GetHash_IconImageValue = new List<Int32List>();
#endif





        /*public List<GameObject> GetHash1() { return Hash1; }
        public void SetHash1(List<GameObject> hash) { Hash1 = hash; }*/
        public List<GoGuidPair> GetHash1_Fix2_0()
    {   if (Hash1.Count != 0)
        {   Hash1_Fix2_0 = Hash1.Select( h => (GoGuidPair)h ).ToList();
            Hash1.Clear();
        }
        return Hash1_Fix2_0;
    }
    public void SetHash1_Fix2_0(List<GoGuidPair> hash) { Hash1_Fix2_0 = hash; }
    public List<string> GetHash2() { return Hash2; }
    public void SetHash2(List<string> hash) { Hash2 = hash; }
    
    public List<Int32List> GetHash3() { return Hash3; }
    public void SetHash3(List<Int32List> hash) { Hash3 = hash; }
    public List<Int32List> GetHash4() { return Hash4; }
    public void SetHash4(List<Int32List> hash) { Hash4 = hash; }
    
    /*public List<GameObject> GetHash5() { return Hash5 ?? (Hash5 = new List<GameObject>()); }
    public void SetHash5(List<GameObject> hash) { Hash5 = hash; }*/
    public List<GoGuidPair> GetHash5_Fix2_0()
    {   if (Hash5.Count != 0)
        {   Hash5_Fix2_0 = Hash5.Select( h => (GoGuidPair)h ).ToList();
            Hash5.Clear();
        }
        return Hash5_Fix2_0;
    }
    public void SetHash5_Fix2_0(List<GoGuidPair> hash) { Hash5_Fix2_0 = hash; }
    public List<SingleList> GetHash6() { return Hash6 ?? (Hash6 = new List<SingleList>()); }
    public void SetHash6(List<SingleList> hash) { Hash6 = hash; }
    
    /*public List<GameObject> GetHash7() { return Hash7 ?? (Hash7 = new List<GameObject>()); }
    public void SetHash7(List<GameObject> hash) { Hash7 = hash; }*/
    public List<GoGuidPair> GetHash7_Fix2_0()
    {   if (Hash7.Count != 0)
        {   Hash7_Fix2_0 = Hash7.Select( h => (GoGuidPair)h ).ToList();
            Hash7.Clear();
        }
        return Hash7_Fix2_0;
    }
    public void SetHash7_Fix2_0(List<GoGuidPair> hash) { Hash7_Fix2_0 = hash; }
    public List<SingleList> GetHash8() { return Hash8 ?? (Hash8 = new List<SingleList>()); }
    public void SetHash8(List<SingleList> hash) { Hash8 = hash; }
    
    public List<GameObject> GetHash9() { return Hash9 ?? (Hash9 = new List<GameObject>()); }
    public void SetHash9(List<GameObject> hash) { Hash9 = hash; }
    public List<SingleList> GetHash10() { return Hash10 ?? (Hash10 = new List<SingleList>()); }
    public void SetHash10(List<SingleList> hash) { Hash10 = hash; }


        public List<GoGuidPair> GetFreezeHashKeys() { return HashFreezeHashKeys; }
        public void SetFreezeHashKeys( List<GoGuidPair> hash ) { HashFreezeHashKeys = hash; }
        public List<bool> GetFreezeHashValues() { return HashFreezeHashValues; }
        public void SetFreezeHashValues( List<bool> hash ) { HashFreezeHashValues = hash; }


        public List<HierarchySnapShotArray> HierarchyCache() { return Hash_HierarchyCache; }
    public void HierarchyCache(List<HierarchySnapShotArray> hash) { Hash_HierarchyCache = hash; }
    public List<Int32ListArray> GetBookMarks() { return Hash_BookMarks; }
    public void SetBookMarks(List<Int32ListArray> hash)
    {   Hash_BookMarks = hash;
        if (hash.Count > 0 && hash[0].array != null) SetHash4( hash[0].array );
    }
    
    
    public bool EnableRegistrator
    {   get { return _enableRegistrator; }
        set { _enableRegistrator = value; }
    }
    
    public string SaveInFolder
    {   get { return _saveInFolder; }
        set { _saveInFolder = value; }
    }
    
    public Component component
    {   get { return this; }
    }
    
    public UnityEngine.Object unityobject
    {   get { return null; }
    }
    
    public int FavoritCategorySelected { get { return _mFavoritCategorySelected; } set { _mFavoritCategorySelected = value; } }
    
    
    
    
    public List<GoGuidPair> GetHash_IconImageKey() { return m_GetHash_IconImageKey ?? (m_GetHash_IconImageKey = new List<GoGuidPair>()); }
    public void SetHash_IconImageKey(List<GoGuidPair> hash) { m_GetHash_IconImageKey = hash; }
    public List<Int32List> GetHash_IconImageValue() { return m_GetHash_IconImageValue ?? (m_GetHash_IconImageValue = new List<Int32List>()); }
    public void SetHash_IconImageValue(List<Int32List> hash) { m_GetHash_IconImageValue = hash; }
    
}
}


#endif