namespace Condominio.Utils.Authorization
{
    /// <summary>
    /// Constantes para los nombres de roles del sistema
    /// </summary>
    public static class AppRoles
    {
        /// <summary>
        /// Rol por defecto para operaciones básicas (health check, login)
        /// </summary>
        public const string Default = "Defecto";

        /// <summary>
        /// Rol para habitantes - Pueden ver su información personal y propiedades asignadas
        /// </summary>
        public const string Habitante = "Habitante";

        /// <summary>
        /// Rol de administrador - Puede realizar operaciones CRUD en Usuarios y Propiedades
        /// </summary>
        public const string Administrador = "Administrador";

        /// <summary>
        /// Rol de administrador de roles - Puede actualizar roles de usuarios
        /// </summary>
        public const string RoleAdmin = "RoleAdmin";

        /// <summary>
        /// Obtiene todos los roles del sistema
        /// </summary>
        public static string[] AllRoles => new[]
        {
            Default,
            Habitante,
            Administrador,
            RoleAdmin
        };

        /// <summary>
        /// Roles con permisos administrativos
        /// </summary>
        public static string[] AdminRoles => new[]
        {
            Administrador,
            RoleAdmin
        };
    }
}
