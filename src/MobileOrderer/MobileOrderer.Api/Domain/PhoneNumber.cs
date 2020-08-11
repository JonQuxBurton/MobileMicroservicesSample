namespace MobileOrderer.Api.Domain
{
    public class PhoneNumber
    {
        private readonly string value;

        public PhoneNumber(string value)
        {
            this.value = value;
        }

        public override string ToString()
        {
            return value;
        }
    }
}
