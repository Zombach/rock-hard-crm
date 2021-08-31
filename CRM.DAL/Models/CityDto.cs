namespace CRM.DAL.Models
{
    public class CityDto
    {
        public int Id;
        public string Name;

        public override bool Equals(object obj)
        {
            return obj is CityDto dto &&
                   Id == dto.Id |
                   Name == dto.Name;
        }
    }


}