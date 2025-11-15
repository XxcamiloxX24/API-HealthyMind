namespace API_healthyMind.Models
{
    public class VerificationCode
    {
        public int id {  get; set; }
        public int AprendizId { get; set; }
        public Aprendiz Aprendiz { get; set; }
        public string Codigo { get; set; }
        public DateTime Expiration {  get; set; }

    }
}
