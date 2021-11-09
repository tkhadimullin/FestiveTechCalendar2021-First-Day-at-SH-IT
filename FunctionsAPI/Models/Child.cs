namespace test_santa_api
{
    [GenerateDto]
    public class Child 
    {
        public int Id {get;set;}
        public string FirstName {get;set;}
        public string LastName {get;set;}
        public string City {get;set;}
        public string Motto {get;set;}
        public bool IsNaughty {get;set;}
    }
}