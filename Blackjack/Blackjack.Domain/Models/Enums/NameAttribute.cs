namespace Blackjack.Domain.Models.Enums
{
    public class NameAttribute : Attribute
    {
        public string Name { get; private set; }
        public string Description { get; private set; }

        public NameAttribute(string name, string description)
        {
            this.Name = name;
            this.Description = description;
        }
    }
}
