namespace Condominio.Utils.Authorization
{
    public static class AppRoles
    {
        public const string Super = "super";
        public const string Administrador = "admin";
        public const string Director = "director";
        public const string Habitante = "habitante";
        public const string Auxiliar = "auxiliar";
        public const string Seguridad = "seguridad";

        public static string[] AllRoles => new[]
        {
            Super,
            Administrador,
            Director,
            Habitante,
            Auxiliar,
            Seguridad
        };

        public static string[] AdminRoles => new[]
        {
            Super,
            Administrador,
        };
    }
}
