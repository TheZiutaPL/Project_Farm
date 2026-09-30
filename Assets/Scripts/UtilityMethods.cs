using UnityEngine;

public static class UtilityMethods
{
    public static bool IsInterfaceValid(object checkedInterface) => checkedInterface != null && checkedInterface is Object obj && obj != null;
    public static bool IsInterfaceValid(object checkedInterface, out Object obj)
    {
        obj = null;

        if (checkedInterface == null)
            return false;

        if (checkedInterface is not Object temp)
            return false;

        obj = temp;

        return obj != null;
    }

    public static bool IsLayerInMask(LayerMask mask, int layer) => IsInMask(mask, 1 << layer);
    public static bool IsInMask(int mask, int other) => (other & mask) > 0;
}
