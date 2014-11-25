using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Lib
{
    public class Hotel
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public string SecurityToken { get { return m_securityToken; } }

        private string m_securityToken;

        public Hotel() {}

        public Hotel(string id, string name, string securityToken)
        {
            Id = id;
            Name = name;
            m_securityToken = securityToken;
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

    [DataContract]
    public class PriceDTO
    {
        [DataMember(Name = "currency")]
        public string Currency { get; private set; }

        [DataMember(Name = "amount")]
        public double Amount { get; private set; }
    }

    [DataContract]
    public class RoomDTO
    {
        [DataMember(Name = "name")]
        public string Name { get; private set; }

        [DataMember(Name = "shortDesc")]
        public string Description { get; private set; }

        [DataMember(Name = "price")]
        public PriceDTO Price { get; private set; }

        [DataMember(Name = "headerImage")]
        public string ImageUrl { get; private set; }
    }

    [DataContract]
    public class HotelLookup
    {
        [DataMember]
        public string HotelName { get; set; }

        [DataMember]
        public string Country { get; set; }

        [DataMember]
        public string SecurityToken { get; set; }
    }

    public class HotelsAPI
    {
        private ToggleManager m_toggleManager;
        private HttpClient m_hotelsClient;
        private HttpClient m_roomsClient;

        private JsonSerializer serializer = new JsonSerializer();
        private DataContractJsonSerializer roomSerializer = new DataContractJsonSerializer(typeof(RoomDTO[]));

        public HotelsAPI(ToggleManager toggleManager)
        {
            m_toggleManager = toggleManager;

            m_hotelsClient = Utils.MakeRestClient("http://hotels-world.fanta.wixpress.com/api/");
            m_roomsClient = Utils.MakeRestClient("http://hotels.fanta.wixpress.com/api/");

            //serializer.Deserialize()
        }

        public List<Hotel> GetHotels()
        {
            //return UnwrapResult(GetHotelsAsync());
            return GetFakeHotels();
        }

        public async Task<List<Hotel>> GetHotelsAsync()
        {
            var resp = await m_hotelsClient.GetAsync("search?lan=50&lot=25&distance=30");

            if (resp.IsSuccessStatusCode)
                return (await resp.Content.ReadAsAsync<Hotel[]>()).ToList();
            else
                throw new InvalidOperationException("Request failed");
        }

        private List<Hotel> GetFakeHotels()
        {
            return m_toggleManager.AddLatency(() =>
                new List<Hotel>() {
                    new Hotel("hotel-1", "Hotel 1",
                        "Ct8l4ZzadI7YwqtHvun8xB482Nl81FzsDeOGThl2Wtc.eyJpbnN0YW5jZUlkIjoiMTM4ZTAwZWEtMzI4NC04ODY5LWViYmMtMDMxNGQ4ODU0NTQ5Iiwic2lnbkRhdGUiOiIyMDE0LTExLTI1VDA2OjM4OjAxLjI1MloiLCJ1aWQiOiJmZGUwMTUxMi04ZWVkLTRmNDItODc4Zi1iODkxYTdhMWJlNjYiLCJwZXJtaXNzaW9ucyI6Ik9XTkVSIiwiaXBBbmRQb3J0IjoiODguMTE5LjE1MC4xOTYvMzQ4MDEiLCJ2ZW5kb3JQcm9kdWN0SWQiOm51bGwsImRlbW9Nb2RlIjpmYWxzZX0"),
                    //new Hotel("hotel-2", "Hotel 2", null)
                });
        }
        
        public List<Room> GetRooms(Hotel hotel)
        {
            return Utils.UnwrapResult(GetRoomsAsync(hotel));
        }
        
        public async Task<List<Room>> GetRoomsAsync(Hotel hotel)
        {
            var resp = await m_roomsClient.GetAsync("rooms?instance=" + hotel.SecurityToken);

            if (resp.IsSuccessStatusCode)
            {
                var dtos = (RoomDTO[])roomSerializer.ReadObject(await resp.Content.ReadAsStreamAsync());

                return dtos.Select(dto =>
                    new Room(dto.Name,
                             dto.Description,
                             string.Format("{1}{0}", dto.Price.Currency, dto.Price.Amount),
                             FormatImageUrl(dto.ImageUrl))).ToList();
            }
            else
                throw new InvalidOperationException("Request failed");
        }

        public List<Room> GetFakeRooms(Hotel hotel)
        {
            return m_toggleManager.AddLatency(() =>
                new List<Room>() {
                    new Room("Room 1", "Awesome", "$1", "http://www.americanlayout.com/wp/wp-content/uploads/2012/08/C-To-Go-300x300.png"),
                    new Room("Room 2", "Awesome", "$2", "https://encrypted-tbn2.gstatic.com/images?q=tbn:ANd9GcSodMApp3rQxC0HGW2xhppbDz9AC5XAvxiU7MkSxhYlkX1A1NrAPf-sLg")
                });
        }

        private string FormatImageUrl(string relativePath)
        {
            if (relativePath == null)
                return "http://static.wix.com/media/fde015_5727dda09b554a2881fdd3e7dcf522d0.png_650_srz_240_170_75_22_0.5_1.2_75_png_650_srz";
            else
                return string.Format("{0}{1}", "http://static.fanta.wixpress.com/media/", relativePath);
        }
    }
}
