namespace NeverFoundry.Wiki.Samples.Complete.Services
{
    public class EmailAddress
    {
        public string Address { get; set; }

        private string? _name;
        public string Name
        {
            get => string.IsNullOrEmpty(_name) ? Address : _name;
            set => _name = value;
        }

        public EmailAddress(string address, string? name = null)
        {
            Address = address;
            _name = name;
        }
    }
}
