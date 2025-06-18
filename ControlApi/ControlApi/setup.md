# Setup

## Add user secrets

In Rider, right click on the `.sln`
```
Tools -> .NET User Secrets
```

A `secrets.json` should open, add the following:
```json
{
    "CONN_STRING":"Server=<IPADRESS>;Port=<PORT>;Database=<DB_NAME>;User=<DB_USER>;Password=<DB_PASSWORD>;",
    "JWT_KEY": "a string of 35+ ascii chars",
    "SENSORING_API": "https://api.example.com/api",
    "TESTING_SENSORING_API": "https://api.example.com/api",
    "SENSORING_API_AUTH": "a base64 string?"
}
```
And replace `<IPADRESS>`, `<PORT>`, `<DB_NAME>`, `<DB_USER>` and `<DB_PASSWORD>` inside your connection string. 
Also change the JWT key and **Sensoring API** URL. Make sure that the API URL does not end with a `/`, these will be added by our own logic.

Replace `SENSORING_API_AUTH` with the api key from teh **Sensoring api**.

`TESTING_SENSORING_API` is the api that will be used when `const bool TESTING` is set to `true` in program.cs. 
Keep in mind that the testing api will also add items to the db which might be fake data. 
This test api is built against [oldmartijntje's api](https://api.oldmartijntje.nl/) endpoint instead of a real **sensoring api**. 

## TEST MODE

TEST MODE is there as an backup for if the real **sensoring api** decides to fail at a critical moment. 
TEST MODE connects to [oldmartijntje's api](https://api.oldmartijntje.nl/) debug api insted of the **Sensoring api**. TEST MODE is toggled in program.cs on line 10:
```cs
const bool TESTING = true;
```

You should set this to `false` to connect to the real **Sensoring API**.

The URL of testing mode and normal mode are defined in the user secrets. 
No it is not enough to change the url from the testing api to the real **sensoring api** because the return response from their endpoints is handled differently.

## Change POI

We are working with an API that gets us points of interest. If you would want to change the kinds of POIs that we are gathering, go to [their website](https://overpass-turbo.eu/#) to test your API query.