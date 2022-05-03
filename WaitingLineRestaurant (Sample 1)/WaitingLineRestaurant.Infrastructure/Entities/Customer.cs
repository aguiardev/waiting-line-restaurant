namespace WaitingLineRestaurant.Infrastructure.Entities
{
    public class Customer
    {
        public int Id { get; set; }
        public string Phone { get; set; }
        public string Name { get; set; }
        public int Position { get; set; }
        public string PeopleQuantity { get; set; }
    }
}