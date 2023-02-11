using MauiGeo.Model;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace MauiGeo.Services;

public class ApiService
{
    //https://www.niceonecode.com/blog/74/how-to-read-response-cookies-using-system.net.http.httpclient
    CookieContainer cookies;
    HttpClientHandler handler;
    HttpClient client;
    Uri uri;

    public ApiService()
    {

        cookies = new CookieContainer();
        handler = new HttpClientHandler();
        handler.CookieContainer = cookies;

        client = new HttpClient(handler);
        //uri = new Uri("https://localhost:7072");
        //uri = DeviceInfo.Platform == DevicePlatform.Android ? new Uri("https://10.0.2.2:7072") : new Uri("https://localhost:7072");
        //endast http funkar i emulatorn utan att behöva installera certs.
        //uri = DeviceInfo.Platform == DevicePlatform.Android ? new Uri("http://10.0.2.2:5072") : new Uri("http://localhost:5072"); //Dev
        uri = new Uri("https://www.sellden.dev/findmeapi"); //Production
    }


    public async Task<bool> Register(Credentials credentials)
    {
        try
        {
            var response = await client.PostAsJsonAsync<Credentials>(uri.OriginalString + "/register", credentials);

            System.Diagnostics.Debug.WriteLine($"Response: {response.StatusCode} {response.ReasonPhrase}");

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("Unable to Register:" + ex.Message);
            return false;
        }
    }

    public async Task<bool> Login(Credentials credentials)
    {
        try
        {
            var response = await client.PostAsJsonAsync<Credentials>(uri.OriginalString + "/login", credentials);

            System.Diagnostics.Debug.WriteLine($"Response: {response.StatusCode} {response.ReasonPhrase}");
            var headers = response.Headers;
            
            foreach (var header in headers)
            {
                System.Diagnostics.Debug.WriteLine($"Key: {header.Key} Value: {header.Value}");
            }

            PrintCookies();

            return true;
            //return await response.Content.ReadFromJsonAsync<T>();

        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("Unable to Post:" + ex.Message);
            return false;
        }
    }

    public async Task<bool> Logout()
    {

        try
        {
            EnsureLogin();
            var response = await client.PostAsync(uri.OriginalString + "/logout", null);
            PrintCookies();

            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("Unable to Get:" + ex.Message);
            return false;
        }
    }

    public void PrintCookies()
    {
        try
        {
            EnsureLogin();
            List<Cookie> responseCookies = cookies.GetCookies(uri).Cast<Cookie>().ToList();

            if (responseCookies.Count > 0)
            {
                foreach (Cookie cookie in responseCookies)
                    System.Diagnostics.Debug.WriteLine(cookie.Name + ": " + cookie.Value);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("No cookies found.");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("Unable to print cookies:" + ex.Message);
        }


    }

    public async Task<bool> SendLocation(Location location)
    {
        try
        {
            EnsureLogin();

            //TODO Check connectivity. If no internet, dont send.

            var response = await client.PostAsJsonAsync<Location>(uri.OriginalString + "/location", location);

            System.Diagnostics.Debug.WriteLine($"Sending location: {response.StatusCode} {response.ReasonPhrase}");

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("Unable to Send location:" + ex.Message);
            throw new Exception("User not logged in");
        }

    }

    //add follower
    public async Task<bool> AddFollower(UserName userName)
    {
        try
        {
            EnsureLogin();
            var response = await client.PostAsJsonAsync<UserName>(uri.OriginalString + "/addfollower", userName);

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            System.Diagnostics.Debug.WriteLine($"Response: {response.StatusCode} {response.ReasonPhrase}");
            var headers = response.Headers;

            foreach (var header in headers)
            {
                System.Diagnostics.Debug.WriteLine($"Key: {header.Key} Value: {header.Value}");
            }

            return true;
            //return await response.Content.ReadFromJsonAsync<T>();

        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("Unable to Post:" + ex.Message);
            return false;
        }
    }

    //get followers
    public async Task<List<UserName>> Followers()
    {
        try
        {
            EnsureLogin();
            var response = await client.GetFromJsonAsync<List<UserName>>(uri.OriginalString + "/followers");

            return response;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("Unable to get followers:" + ex.Message);
            return null;
        }
    }

    //remove follower
    public async Task<bool> RemoveFollower(UserName userName)
    {
        try
        {
            EnsureLogin();
            var response = await client.PostAsJsonAsync<UserName>(uri.OriginalString + "/removefollower", userName);

            System.Diagnostics.Debug.WriteLine($"Response: {response.StatusCode} {response.ReasonPhrase}");
            //TODO: hantera om anropet misslyckas
            var headers = response.Headers;

            foreach (var header in headers)
            {
                System.Diagnostics.Debug.WriteLine($"Key: {header.Key} Value: {header.Value}");
            }

            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("Unable to remove follower:" + ex.Message);
            return false;
        }
    }
    //get following
    public async Task<List<UserName>> Following()
    {
        try
        {
            EnsureLogin();
            var response = await client.GetFromJsonAsync<List<UserName>>(uri.OriginalString + "/following");

            return response;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("Unable to get following:" + ex.Message);
            return null;
        }
    }

    //my locations
    public async Task<List<Location>> MyLocations()
    {
        try
        {
            EnsureLogin();
            var response = await client.GetFromJsonAsync<List<Location>>(uri.OriginalString + "/mylocations");

            return response;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("Unable to get my locations:" + ex.Message);
            return null;
        }
    }

    //follow location / user
    public async Task<List<LocationDto>> FriendLocations(UserName userName)
    {

        try
        {
            EnsureLogin();
            //var response = await client.GetAsync(uri.OriginalString + "/friendlocation/" + userName.Email);

            //JsonSerializerOptions options = new JsonSerializerOptions();
            //options.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault;

            //var json = await JsonSerializer.DeserializeAsync<List<Location>>(await response.Content.ReadAsStreamAsync(), options);

            var response = await client.GetFromJsonAsync<List<LocationDto>>(uri.OriginalString + "/friendlocation/" + userName.Email);

            return response;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("Unable to get my locations:" + ex.Message);
            return null;
        }
    }


    public void EnsureLogin()
    {
        if (!CheckLogin())
        {
            throw new NotLoggedInException();
        }
    }

    public bool CheckLogin()
    {
        //Checking if there is a cookie in the cookie container.
        List<Cookie> responseCookies = cookies.GetCookies(uri).Cast<Cookie>().ToList();

        if (responseCookies.Count > 0)
        {
            System.Diagnostics.Debug.WriteLine($"Logged in");
            return true;
        }
        else
        {
            System.Diagnostics.Debug.WriteLine($"Logged out");
            return false;
        }
    }
}
