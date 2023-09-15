using MicrobUy_API.Dtos.Enums;
using System.Reflection.Metadata;

namespace MicrobUy_API.Dtos
{
    public class CreateInstanceRequest
    {
        public string Nombre { get; set; }
        public string Url { get; set; }
        public string Logo { get; set; }
        public bool Activo { get; set; }
        public string Tematica { get; set; }
        public EsquemaColoresEnum EsquemaColores { get; set; }
        public PrivacidadEnum Privacidad { get; set; }
    }
}