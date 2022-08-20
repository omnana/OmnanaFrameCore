using System.Collections.Generic;

namespace FrameCore.Util.Pool
{
    public static class ListPool<T>
    {
        private static readonly Stack<List<T>> Stack = new Stack<List<T>>();

        public static List<T> Pop(int capacity = 0)
        {
            return Stack.Count == 0 ? new List<T>(capacity) : Stack.Pop();
        }

        public static void Recycle(List<T> list)
        {
            list.Clear();
            Stack.Push(list);
        }

        public static void Clear()
        {
            Stack.Clear();
        }
    }
    
    public static class ArrayPool<T> where T : new()
    {
        private static readonly Stack<T[]> Stack = new Stack<T[]>();

        public static T[] Pop(int count)
        {
            return Stack.Count == 0 ? new T[count] : Stack.Pop();
        }

        public static void Recycle(T[] array)
        {
            Stack.Push(array);
        }

        public static void Clear()
        {
            Stack.Clear();
        }
    }
}
