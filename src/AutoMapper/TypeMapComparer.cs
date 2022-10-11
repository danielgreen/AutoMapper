namespace AutoMapper;

/// <summary>
/// Orders TypeMaps by examining which ones are closest to the supplied specimen TypeMap.
/// To determine the closest, the comparer looks at how close each TypeMap's source and destination types are
/// to the specimen's source and destination types in the respective type hierarchies.
/// If any of the types encountered are interfaces, since they are not part of a hierarchy they are sorted to the end.
/// </summary>
public class TypeMapComparer : IComparer<TypeMap>
{
    private readonly TypeMap _specimen;

    public TypeMapComparer(TypeMap specimen) => this._specimen = specimen;

    public int Compare(TypeMap x, TypeMap y)
    {
        int hopsToSourceForX = GetHops(_specimen.SourceType, x.SourceType);
        int hopsToDestinationForX = GetHops(_specimen.DestinationType, x.DestinationType);

        int hopsToSourceForY = GetHops(_specimen.SourceType, y.SourceType);
        int hopsToDestinationForY = GetHops(_specimen.DestinationType, y.DestinationType);

        int totalHopsForX = hopsToSourceForX + hopsToDestinationForX;
        int totalHopsForY = hopsToSourceForY + hopsToDestinationForY;

        bool isXCloserToRoot = totalHopsForX < totalHopsForY || (totalHopsForX == totalHopsForY && hopsToSourceForX < hopsToSourceForY);

        return isXCloserToRoot ? -1 : 1;
    }

    private int GetHops(Type from, Type to)
    {
        if (from.IsInterface || to.IsInterface)
            return 1000;

        int hops = 0;
        var current = from;

        while (current is not null && !current.Equals(to))
        {
            current = current.BaseType;
            ++hops;
        }

        return current is null ? 1000 : hops;
    }
}
