using System.Linq;
using System.Net.NetworkInformation;
using UnityEditor;
using System;
using UnityEngine;
using System.Reflection;

[InitializeOnLoad]
public static class UXToolCheck
{
    private static string[] white_list = new string[] {
        "D8BBC12B38A7"
    };
    private static DateTime expirationTime = new DateTime(2023, 3, 31);

    static UXToolCheck()
    {
        //if (DateTime.Compare(DateTime.Today, expirationTime) > 0)
        //{
        //    Debug.Log("UXTools has expired!");
        //    // while(true);
        //}
        //if (!white_list.ToList().Contains(GetMacAddress()))
        //{
        //    // while(true);
        //}
    }

    private static string GetMacAddress()
    {
        foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet && nic.OperationalStatus == OperationalStatus.Up)
            {
                return nic.GetPhysicalAddress().ToString();
            }
        }
        return null;
    }
}