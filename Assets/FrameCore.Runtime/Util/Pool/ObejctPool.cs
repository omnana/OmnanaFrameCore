using System.Collections.Generic;

public class ObejctPool<T>where T: new()
{
    private readonly Stack<T> _stack = new Stack<T>(5000);
    
    public T Get()
    {
        if (_stack.Count > 0)
            return _stack.Pop();
        
        return new T();
    }

    public void Recycle(T obj)
    {
        _stack.Push(obj);
    }

    public void Clear()
    {
        _stack.Clear();
    }
}
