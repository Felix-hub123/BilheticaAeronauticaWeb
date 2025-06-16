namespace BilheticaAeronauticaWeb.Data.Entities
{
    public class Administrador
    {
       public int Id { get; set; }
        public string Nome { get; set; }
        public string Apelido { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public User User { get; set; }
    }
}
