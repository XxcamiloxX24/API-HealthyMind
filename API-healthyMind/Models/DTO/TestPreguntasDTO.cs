using System.Text.Json.Serialization;

namespace API_healthyMind.Models.DTO
{
    public class TestPreguntasDTO
    {
        public int TesRegistroFk { get; set; }

        public int TesPregFk { get; set; }

        public int TesRespFk { get; set; }
    }
}
