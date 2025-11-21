namespace API_healthyMind.Models.DTO
{
    public class EmocionesDTO
    {
        public string? EmoNombre { get; set; }

        /// <summary>
        /// descripcion de la emocion
        /// </summary>
        public string? EmoDescripcion { get; set; }

        /// <summary>
        /// url de la imagen de la emocion
        /// </summary>
        public string? EmoImage { get; set; }
    }
}
