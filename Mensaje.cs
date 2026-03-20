namespace MiniCRM
{
    public class Mensaje
    {
        public int Id { get; set; }
        public int ConversacionId { get; set; }
        public string Texto { get; set; } = "";
        public string Fecha { get; set; } = "";
    }
}