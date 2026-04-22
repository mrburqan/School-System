namespace SchoolSystem.Application.Constans
{
    public enum UserType
    {
        Admin = 1,
        Teacher = 2,
        Student = 3
    }

    public static class UserTypeDescriptions
    {
        public const string Admin = "admin";
        public const string Teacher = "teacher";
        public const string Student = "student";
    }

    public static class UserDescType
    {
        public const string Admin = UserTypeDescriptions.Admin;
        public const string Teacher = UserTypeDescriptions.Teacher;
        public const string Student = UserTypeDescriptions.Student;
    }
}
