using System;

namespace DOTSTemplate
{
    public static class CollectionExtensions
    {
        public static T[] Insert<T>(this T[] array, int index, T value)
        {
            if (array == null) array = new T[index + 1];
            else if (array.Length <= index) Array.Resize(ref array, index + 1);
            else Array.Resize(ref array, array.Length + 1);

            if (array.Length > index + 1)
            {
                Array.Copy(array, index, array, index + 1, array.Length - index - 1);
            }

            array[index] = value;
            return array;
        }

        public static T[] RemoveAt<T>(this T[] array, int index)
        {
            if (array == null) return array;
            if (index < 0) return array;
            if (index < array.Length - 1)
            {
                Array.Copy(array, index + 1, array, index, array.Length - (index + 1));
            }
            Array.Resize(ref array, array.Length - 1);
            return array;
        }
    }
}