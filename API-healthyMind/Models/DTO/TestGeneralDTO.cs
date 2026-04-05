namespace API_healthyMind.Models.DTO
{
    public class TestGeneralDTO
    {
        public int? TestGenApreFk { get; set; }

        public int? TestGenPsicoFk { get; set; }

        public DateTime? TestGenFechaRealiz { get; set; }

        public string? TestGenResultados { get; set; }

        public string? TestGenRecomendacion { get; set; }

        public int? TestGenPlantillaFk { get; set; }

        public string? TestGenEstadoTest { get; set; }
    }
}
