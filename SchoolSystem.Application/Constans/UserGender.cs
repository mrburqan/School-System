namespace SchoolSystem.Application.Constans
{
    public enum UserGender
    {
        Male = 1,
        Female = 2
    }

    public static class UserGenderDescriptions
    {
        public const string Male = "male";
        public const string Female = "female";
    }

    public static class UserGenderDesc
    {
        public const string Male = UserGenderDescriptions.Male;
        public const string Female = UserGenderDescriptions.Female;
    }
}
