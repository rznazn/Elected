
using System;
using System.Collections.Generic;
using System.Json;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Elected
{
    public class JsonParser
    {
        public List<Representative> representatives = new List<Representative>();
        public List<Election> elections = new List<Election>();
        public IPhotosLoadedListner listener;

        public JsonParser(IPhotosLoadedListner listener)
        {
            this.listener = listener;
        }
        public List<Representative> ParseRepresentativeJson(JsonValue data)
        {
            JsonArray JsonOffices =  data.ContainsKey("offices") ? (JsonArray)data["offices"] : new JsonArray(0);
            JsonArray JsonReps = data.ContainsKey("officials") ? (JsonArray)data["officials"] : new JsonArray(0);

            representatives = new List<Representative>();

            for (int i = 0; i < JsonReps.Count; i ++)
            {
                string addressString = "";
                List<string> urls = new List<string> { "no url listed" };
                List<string> phoneNumbers = new List<string> { "no phone number listed" };
                List<string> emails = new List<string> { "no email listed" };
                List<Channel> channels = new List<Channel> { new Channel("no data found", "no data found")};
                JsonValue repJson = JsonReps[i];
                if (repJson.ContainsKey("address"))
                {
                    JsonArray addresses = (JsonArray) repJson["address"];
                    JsonValue address = addresses[0];
                    
                    addressString = address.ContainsKey("line1") ? addressString + address["line1"] : addressString;
                    addressString = address.ContainsKey("line2") ? addressString + "\n" + address["line2"] : addressString;
                    addressString = address.ContainsKey("line3") ? addressString + "\n" + address["line3"] : addressString;
                    addressString = address.ContainsKey("city") ? addressString + "\n" + address["city"] : addressString;
                    addressString = address.ContainsKey("state") ? addressString + " " + address["state"] : addressString;
                    addressString = address.ContainsKey("zip") ? addressString + ", " + address["zip"] : addressString;

                }

                if (repJson.ContainsKey("phones"))
                {
                    JsonArray phoneNumbersJson = (JsonArray)repJson["phones"];
                    phoneNumbers = new List<string>();
                    for (int i2 = 0; i2 < phoneNumbersJson.Count; i2 ++)
                    {
                        phoneNumbers.Add( phoneNumbersJson[i2]);
                    }
                }

                if (repJson.ContainsKey("emails"))
                {
                    JsonArray emailsJson = (JsonArray)repJson["emails"];
                    emails = new List<string>();
                    for (int i2 = 0; i2 < emailsJson.Count; i2++)
                    {
                        emails.Add(emailsJson[i2]);
                    }
                }

                if (repJson.ContainsKey("urls"))
                {
                    JsonArray urlsJson = (JsonArray)repJson["urls"];
                    urls = new List<string>();
                    for (int i6 = 0; i6< urlsJson.Count; i6 ++)
                    {
                        urls.Add(urlsJson[i6]);
                    }
                }

                if (repJson.ContainsKey("channels"))
                {
                    JsonArray channelsJson = (JsonArray)repJson["channels"];
                    channels = new List<Channel>();
                    for (int i7 = 0; i7 < channelsJson.Count; i7 ++)
                    {
                        channels.Add(new Channel(channelsJson[i7]["type"], channelsJson[i7]["id"]));
                    }
                }
                string name = "no name given";
                if (repJson.ContainsKey("name"))
                { 
                 name =  repJson["name"] ;
                }
                string party = "unknown";
                if (repJson.ContainsKey("party"))
                {
                    party = repJson["party"];
                }
                representatives.Add( new Representative {
                    Name = name,
                    Party = party,
                    Address = addressString,
                    PhoneNumbers = phoneNumbers,
                    Urls = urls,
                    Channels = channels,
                    Emails = emails});
            }
            for (int i3 = 0; i3 < JsonOffices.Count; i3++)
            {
                JsonValue office = JsonOffices[i3];
                if (office.ContainsKey("officialIndices"))
                {
                    JsonArray officials = (JsonArray)office["officialIndices"];
                    for (int i4 = 0; i4 < officials.Count; i4 ++)
                    {
                        int index = officials[i4];
                        representatives[index].OfficeName = office["name"];
                    }
                }
            }

            GetImageBitmapFromUrl(JsonReps);
            return representatives;
        }
        private async void GetImageBitmapFromUrl(JsonArray data)
        {
            for (int i = 0; i < data.Count; i++)
            {
                JsonValue repJson = data[i];
                if (repJson.ContainsKey("photoUrl"))
                {
                    string url = repJson["photoUrl"];
                    if (!url.Contains(".png"))
                    {
                        using (var webClient = new HttpClient())
                        {
                            var imageBytes = await webClient.GetByteArrayAsync(url);
                            representatives[i].PhotoBytes = imageBytes;
                        }
                    }
                }
            }
            listener.OnPhotosLoaded(representatives);
        }

        public interface IPhotosLoadedListner
        {
            void OnPhotosLoaded(List<Representative> data);
        }

        public List<Election> ParseElectionJson(JsonValue data)
        {
            elections = new List<Election>();
            if (!data.ContainsKey("error"))
            {
                JsonValue firstElection = data["election"];
                elections.Add(new Election { id = firstElection["id"], name = firstElection["name"], date = firstElection["electionDay"] });


                if (data.ContainsKey("otherElections"))
                { 
                JsonArray upcomingElections = (JsonArray)data["otherElections"];

                for (int i = 0; i < upcomingElections.Count; i++)
                {
                    var current = upcomingElections[i];
                    elections.Add(new Election { id = current["id"], name = current["name"], date = current["electionDay"] });
                }
                }
            }

            return elections;
        }

        public string ParseElectionInfoJson(JsonValue data)
        {
            string response= "";
            if (data.ContainsKey("election"))
            {
                JsonValue election = data["election"];
                response = response + election["name"] + "\n" + election["electionDay"] + "\n";
            }
            if (data.ContainsKey("pollingLocations"))
            {
                JsonArray pollingLocations= (JsonArray)data["pollingLocations"];
                for (int i = 0; i < pollingLocations.Count; i++)
                {
                    JsonValue current = pollingLocations[i];
                    JsonValue address = current["address"];
                    response = address.ContainsKey("locationName") ? response + address["locationName"] + "\n" : response;
                    response = address.ContainsKey("line1") ? response + address["line1"] : response;
                    response = address.ContainsKey("line2") ? response + "\n" + address["line2"] : response;
                    response = address.ContainsKey("line3") ? response + "\n" + address["line3"] : response;
                    response = address.ContainsKey("city") ? response + "\n" + address["city"] : response;
                    response = address.ContainsKey("state") ? response + " " + address["state"] : response;
                    response = address.ContainsKey("zip") ? response + ", " + address["zip"] + "\n" : response;

                    response = current.ContainsKey("notes") ? response + current["notes"] + "\n": response;
                    response = current.ContainsKey("pollingHours") ? response + "Polling Hours:\n" + current["pollingHours"] + "\n\n" : response;
                }
                
            }
            if (data.ContainsKey("contests"))
            {
                JsonArray contests = (JsonArray)data["contests"];
                for (int i =0; i < contests.Count; i++)
                {
                    JsonValue current = contests[i];
                    response = current.ContainsKey("office") ? response + "Contested Positions\n\n" + current["office"] + "\n\n" : response;
                   if (current.ContainsKey("candidates"))
                    {
                        response = response + "candidates\n\n";
                        JsonArray candidates = (JsonArray)current["candidates"];
                        for (int i2 = 0; i2 < candidates.Count; i2 ++)
                        {
                            JsonValue currentCand = candidates[i2];
                            response = currentCand.ContainsKey("name") ? response + currentCand["name"] + "\n" : response;
                            response = currentCand.ContainsKey("party") ? response + currentCand["party"] + "\n" : response;
                            response = currentCand.ContainsKey("phone") ? response + "phone #: " + currentCand["phone"] + "\n" : response;
                            response = currentCand.ContainsKey("email") ? response + currentCand["email"] + "\n\n" : response;


                        }
                    }
                }
            }

            return response;
        }
    }
}
