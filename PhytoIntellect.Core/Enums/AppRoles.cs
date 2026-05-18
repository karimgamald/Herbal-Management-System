namespace PhytoIntellect.Core.Constants;

public static class AppRoles
{
    public const string Patient = "Patient";
    public const string Herbalist = "Herbalist";
    public const string Admin = "Admin";
    public static bool IsValidRole(string role)
    {
        return role == Patient || role == Herbalist || role == Admin;
    }
}