namespace ApisWebConsult.Modelo
{
    public class Menu
    {
        public int Id { get; set; }
        public string? DescripcionMenu { get; set; }
        public int? IdPadre { get; set; }
        public string? Attach { get; set; }
        public string? RolesUsers { get; set; }

    }
}
