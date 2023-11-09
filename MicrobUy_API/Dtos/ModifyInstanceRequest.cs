using MicrobUy_API.Dtos.Enums;

namespace MicrobUy_API.Dtos
{
    public class ModifyInstanceRequest
    {
        public string Nombre { get; set; }
        public string Logo { get; set; }
        public CreateTematicaRequestDto Tematica { get; set; }
        public string Description { get; set; }
        public EsquemaColoresEnum EsquemaColores { get; set; }
        public PrivacidadEnum Privacidad { get; set; }
    }
}
