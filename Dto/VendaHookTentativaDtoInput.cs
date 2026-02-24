namespace API_RNB.Dto
{
    public class VendaHookTentativaDtoInput
    {
        public int Id { get; set; }
        public int Tentativas { get; set; }
        public string Erro { get; set; }
    }
}