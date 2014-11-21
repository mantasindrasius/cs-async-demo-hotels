using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib
{
    public class Hotel
    {
        public string Id { get; private set; }
        public string Name { get; private set; }

        public Hotel(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    public class Room
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Price { get; private set; }
        public string ImageUrl { get; private set; }

        public Room(string name, string description, string price, string imageUrl)
        {
            Name = name;
            Description = description;
            Price = price;
            ImageUrl = imageUrl;
        }
    }

    public class HotelsAPI
    {
        public List<Hotel> GetHotels()
        {
            return new List<Hotel>() {
                new Hotel("hotel-1", "Hotel 1"),
                new Hotel("hotel-2", "Hotel 2")
            };
        }

        public List<Room> GetRooms(String hotelId)
        {
            return new List<Room>() {
                new Room("Room 1", "Awesome", "$1", "http://www.americanlayout.com/wp/wp-content/uploads/2012/08/C-To-Go-300x300.png"),
                new Room("Room 2", "Awesome", "$2", "https://encrypted-tbn2.gstatic.com/images?q=tbn:ANd9GcSodMApp3rQxC0HGW2xhppbDz9AC5XAvxiU7MkSxhYlkX1A1NrAPf-sLg")
            };
        }
    }
}
