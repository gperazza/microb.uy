namespace MicrobUy_API.Dtos.Enums
{
    /// <summary>
    /// Definen la forma de registro de los usuarios
    /// </summary>
    public enum PrivacidadEnum
    {
        /// <summary>
        /// Todos pueden registrarse a la instancia sin restricciones.
        /// </summary>
        Abierta = 0,

        /// <summary>
        /// Todos pueden registrarse pero debe ser aprobado por un moderador o un administrador de la instancia.
        /// </summary>
        Autorizacion = 1,

        /// <summary>
        /// Los usuarios de una instancia pueden generar un link de
        /// invitación a otras personas, las cuales deben ser aprobadas por un moderador o un administrador de la instancia.
        /// </summary>
        Invitacion = 2,

        /// <summary>
        /// No están abiertos los registros a la instancia.
        /// </summary>
        Cerrada = 3
    }
}