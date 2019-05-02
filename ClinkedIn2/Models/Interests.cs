namespace ClinkedIn2.Models
{
    public class Interests
    {
        public Interests(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; }
        public string Name { get; set; }
    }
}
