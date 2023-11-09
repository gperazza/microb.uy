using MicrobUy_API.Dtos.Enums;
using MicrobUy_API.Models;

namespace MicrobUy_API.Dtos
{
    public class ModifyInstanceRequest
    {
        public string Nombre { get; set; }
        public string Logo { get; set; }
        public TematicaModel Tematica { get; set; }
        public string Description { get; set; }
        public EsquemaColoresEnum EsquemaColores { get; set; }
        public PrivacidadEnum Privacidad { get; set; }
    }
}
