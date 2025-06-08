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
    "SENSORING_API": "https://api.example.com/api"
}
```
And replace `<IPADRESS>`, `<PORT>`, `<DB_NAME>`, `<DB_USER>` and `<DB_PASSWORD>` inside your connection string. Also change the JWT key and Sensoring API URL. Make sure that the API URL does not end with a `/`, these will be added by our own logic.