using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Linq;

namespace LinqLesson
{
  
    public class Parsing
    {
        public static void Main(string[] args)
        {

            string fileName = @"C:\Akateria\Projects\Lesson19\Lesson19\data.json";
            string jsonString = File.ReadAllText(fileName);
            var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            options.Converters.Add(new DateTimeConverterUsingDateTimeParse());

            Person[] persons = JsonSerializer.Deserialize<Person[]>(jsonString, options)!;
            Friend[] friends = new Friend[persons.Length];

            //find out who is located farthest north / south / west / east using latitude/ longitude data
            var selectedPeople = persons.OrderBy(p => p.Latitude);
            var south = selectedPeople.FirstOrDefault();
            Console.WriteLine($"{south.Name} located farthest south {south.Latitude}");
            selectedPeople = persons.OrderByDescending(p => p.Latitude);
            var north = selectedPeople.FirstOrDefault();
            Console.WriteLine($"{north.Name} located farthest north {north.Latitude}");
            selectedPeople = persons.OrderBy(p => p.Longitude);
            var west = selectedPeople.FirstOrDefault();
            Console.WriteLine($"{west.Name} located farthest west {west.Longitude}");
            selectedPeople = persons.OrderByDescending(p => p.Longitude);
            var east = selectedPeople.FirstOrDefault();
            Console.WriteLine($"{east.Name} located farthest east {east.Longitude}");


            //find max and min distance between 2 persons

            double GetDiff(Person p1, Person p2)
            {
                    double diff = Math.Acos(Math.Sin(p1.Latitude * Math.PI / 180) * Math.Sin(p2.Latitude * Math.PI / 180) + Math.Cos(p1.Latitude * Math.PI / 180) * Math.Cos(p2.Latitude * Math.PI / 180) * Math.Cos(p2.Longitude * Math.PI / 180 - p1.Longitude * Math.PI / 180));
                    return diff;               
            }


            var selectedPeople1 = from p1 in persons
                                  from p2 in persons
                                  where p1.Latitude != p2.Latitude && p1.Longitude != p2.Longitude
                                  let distance = GetDiff(p1, p2)                                  
                                  select new { distance };
            

            Console.WriteLine($"Max distance between 2 persons is {selectedPeople1.Max(p => p.distance)} km");
            Console.WriteLine($"Min distance between 2 persons is {selectedPeople1.Min(p => p.distance)} km");


            //find 2 persons whos ‘about’ have the most same words

            int SetArray(string str)
            {
                string[] subs = str.Split(' ');

                return subs.Distinct().Count();
            }

            var selectedPeople2 = from p in persons
                              let about = SetArray(p.About)
                              orderby about descending
                                  select new
                              { p.Name, about };

            var dict = selectedPeople2.ToDictionary(x => x.Name, x => x.about).Take(2);
            foreach (var p in dict)
            {
                Console.WriteLine($"The most same words in ‘about’ {p.Key + "\t" + p.Value}");
            }

            //find persons with same friends(compare by friend’s name
            
            var querry =  persons
                .SelectMany(persons => persons.Friends, (person, friend) => new { person, friend })
                .Select(result =>
                  new
                  {
                      person = result.person.Name,
                      friend = result.friend.Name
                  }
          );

            var selectedPeople3 = from p1 in querry
                                  from p2 in querry
                                  where p1.friend == p2.friend
                                  select new { p1, p2 };


            foreach (var obj in selectedPeople3)
            {
                Console.WriteLine(obj);
            }


        }
        public class DateTimeConverterUsingDateTimeParse : JsonConverter<DateTime>
        {
            public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                Debug.Assert(typeToConvert == typeof(DateTime));
                return DateTime.Parse(reader.GetString());
            }
            public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.ToString());
            }
        }
    }

    

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Gender
    {
        Male,     
        Female
    }

    public class Friend
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Person
    {
        public string Id { get; set; }
        public int Index { get; set; }
        public Guid Guid { get; set; }
        public bool IsActive { get; set; }
        public string Balance { get; set; }
        public int Age { get; set; }
        public string EyeColor { get; set; }
        public string Name { get; set; }
        public Gender Gender { get; set; }
        public string Company { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string About { get; set; }
        public DateTime Registered { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string[] Tags { get; set; }
        public Friend[] Friends { get; set; }
    }
}

//checked
