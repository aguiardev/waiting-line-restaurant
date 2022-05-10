namespace WaitingLineRestaurant.API.Entities
{
    public class Customer
    {
        public int Id { get; set; }
        public string Phone { get; set; }
        public string Name { get; set; }
        public int Position { get; set; }

        public Customer()
        {
            Position = 1;
        }
    }
}