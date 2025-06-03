# Setup

## Add user secrets

In Rider, right click on the `.sln`
```
Tools -> .NET User Secrets
```

A `secrets.json` should open, add the following:
```json
{
    "CONN_STRING":"Server=<IPADRESS>:Port=<PORT>;Database=<DB_NAME>;User=<DB_USER>;Password=<DB_PASSWORD>;",
    "JWT_KEY": "a string of 35+ ascii chars"
}
```
And replace `<IPADRESS>`, `<PORT>`, `<DB_NAME>`, `<DB_USER>` and `<DB_PASSWORD>`.