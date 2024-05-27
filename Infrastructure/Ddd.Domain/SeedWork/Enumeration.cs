using System.Reflection;

namespace RecAll.Infrastructure.Ddd.Domain.SeedWork;

public abstract class Enumeration : IComparable {
    public int Id { get; private set; }

    public string Name { get; private set; }

    public string DisplayName { get; set; }

    protected Enumeration(int id, string name, string displayName) =>
        (Id, Name, DisplayName) = (id, name, displayName);

    public override string ToString() => Name;

    public static IEnumerable<TEnumeration> GetAll<TEnumeration>()
        where TEnumeration : Enumeration =>
        typeof(TEnumeration)
            .GetFields(BindingFlags.Public | BindingFlags.Static |
                BindingFlags.DeclaredOnly)
            .Where(p => p.FieldType == typeof(TEnumeration))
            .Select(p => p.GetValue(null)).Cast<TEnumeration>();

    public override bool Equals(object obj) {
        if (obj is not Enumeration enumeration) {
            return false;
        }

        var typeMatches = GetType() == obj.GetType();
        var valueMatches = Id.Equals(enumeration.Id);

        return typeMatches && valueMatches;
    }

    public override int GetHashCode() => Id.GetHashCode();

    public static int AbsoluteDifference(Enumeration left, Enumeration right) =>
        Math.Abs(left.Id - right.Id);

    public static TEnumeration FromValue<TEnumeration>(int value)
        where TEnumeration : Enumeration =>
        Parse<TEnumeration, int>(value, "value", p => p.Id == value);

    public static TEnumeration FromDisplayName<TEnumeration>(string displayName)
        where TEnumeration : Enumeration =>
        Parse<TEnumeration, string>(displayName, "display name",
            p => p.Name == displayName);

    private static TEnumeration Parse<TEnumeration, KValue>(KValue value,
        string description, Func<TEnumeration, bool> predicate)
        where TEnumeration : Enumeration =>
        GetAll<TEnumeration>().FirstOrDefault(predicate) ??
        throw new InvalidOperationException(
            $"'{value}' is not a valid {description} in {typeof(TEnumeration)}");

    public static bool IsValidValue<TEnumeration>(int value)
        where TEnumeration : Enumeration =>
        GetAll<TEnumeration>().FirstOrDefault(p => p.Id == value) is not null;

    public int CompareTo(object obj) => Id.CompareTo(((Enumeration)obj).Id);
}