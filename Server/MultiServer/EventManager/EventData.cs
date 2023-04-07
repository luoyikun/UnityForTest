using System.Collections;
using System.Collections.Generic;


public class EventData
{
}

public class EventDataEx<T> : EventData
{
    private T mData;

    public EventDataEx(T varData)
    {
        mData = varData;
    }

    public T GetData()
    {
        return mData;
    }
}