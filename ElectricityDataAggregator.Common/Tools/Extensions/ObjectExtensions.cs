using ElectricityDataAggregator.Common.Exceptions;

namespace ElectricityDataAggregator.Common.Tools.Extensions
{
    public static class ObjectExtensions
    {
        public static void EnsureNotNull<T>(this T @object, object objectId = null)
        {
            if (@object.IsNull())
            {
                throw new ObjectNotFoundException(typeof(T).Name, objectId);
            }
        }

        public static bool IsNull<T>(this T @object)
        => @object == null;

        public static bool IsNull<T>(this List<T> list)
        => list == null;

        public static bool IsNotNull<T>(this T @object)
        => @object != null;

        public static bool IsNotNull<T>(this List<T> list)
        => list != null;
    }
}
